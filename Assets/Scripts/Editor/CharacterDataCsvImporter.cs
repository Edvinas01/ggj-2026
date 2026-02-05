using System;
using RIEVES.GGJ2026.Core.Constants;
using RIEVES.GGJ2026.Runtime.Characters;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace RIEVES.GGJ2026.Editor
{
    internal class CharacterImporterWindow : EditorWindow
    {
        private const string TemplateObjectPath = "Assets/Data/Characters/Dev/Data_Character_ImportTemplate.asset";
        private const string GoogleSheetsUrlPrefsKey = "CharacterImporterWindow_GoogleSheetsUrl";
        private const string DirectoryTextures = "Assets/Visuals/Characters/Textures";
        private const string DirectoryData = "Assets/Data/Characters";

        private CharacterData templateObject;

        [SerializeField]
        private string googleSheetsUrl = "";
        private bool isImporting;

        [MenuItem(MenuItemConstants.BaseWindowItemName + "/Character Data Importer")]
        internal static void ShowWindow()
        {
            var window = GetWindow<CharacterImporterWindow>("Character Data Importer");
            window.templateObject = AssetDatabase.LoadAssetAtPath<CharacterData>(TemplateObjectPath);
            window.googleSheetsUrl = EditorPrefs.GetString(GoogleSheetsUrlPrefsKey);
            window.minSize = new Vector2(200, 150);
            window.Show();
        }

        private void OnGUI()
        {
            using (new EditorGUI.DisabledScope(isImporting))
            {
                EditorGUI.BeginChangeCheck();

                templateObject = (CharacterData)EditorGUILayout.ObjectField(
                    "Template Object",
                    templateObject,
                    typeof(CharacterData),
                    false
                );

                googleSheetsUrl = EditorGUILayout.TextField(
                    "Google Sheets URL ",
                    googleSheetsUrl
                );

                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(GoogleSheetsUrlPrefsKey, googleSheetsUrl);
                }

                GUILayout.Space(10);

                if (GUILayout.Button("Import CSV", GUILayout.Height(30)))
                {
                    ImportCsvAsync().Forget();
                }
            }
        }

        private async UniTask ImportCsvAsync()
        {
            isImporting = true;

            try
            {
                if (string.IsNullOrWhiteSpace(googleSheetsUrl))
                {
                    var path = EditorUtility.OpenFilePanel("Select Character CSV", Application.dataPath, "csv");
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }

                    if (Directory.Exists(DirectoryData) == false)
                    {
                        Directory.CreateDirectory(DirectoryData);
                        AssetDatabase.Refresh();
                    }

                    using var reader = new StreamReader(path);
                    PerformImport(reader);
                }
                else
                {
                    var csvText = await DownloadGoogleCsvAsync(googleSheetsUrl);
                    using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(csvText ?? ""));
                    using var reader = new StreamReader(memoryStream);
                    PerformImport(reader);
                }
            }
            finally
            {
                isImporting = false;
            }
        }

        private void PerformImport(StreamReader streamReader)
        {
            using var csv = new CsvReader(
                streamReader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                    TrimOptions = TrimOptions.Trim,
                    ShouldSkipRecord = args =>
                    {
                        var readerRow = args.Row;
                        var parser = readerRow.Parser;
                        var record = parser.Record;

                        if (record == null)
                        {
                            return true;
                        }

                        return record.All(string.IsNullOrWhiteSpace);
                    },
                }
            );

            csv.Read();
            csv.ReadHeader();

            if (csv.HeaderRecord == null)
            {
                Debug.LogError("CSV header record is missing, cannot import");
                return;
            }

            var headers = csv.HeaderRecord.ToList();

            var incorrectChoiceColumns = GetColumns("Choice Incorrect", headers);
            var correctChoiceColumns = GetColumns("Choice Correct", headers);
            var characterColumn = headers.IndexOf("Character");
            var frontTextureColumn = headers.IndexOf("Front");
            var backTextureColumn = headers.IndexOf("Back");
            var messageColumn = headers.IndexOf("Message");
            var typeColumn = headers.IndexOf("Type");
            var huntMessageColumn = headers.IndexOf("Hunt Message");

            // Collect rows
            var rows = new List<string[]>();
            while (csv.Read())
            {
                var row = new string[headers.Count];
                for (var index = 0; index < headers.Count; index++)
                {
                    var field = csv.GetField(index);
                    if (field == null)
                    {
                        row[index] = string.Empty;
                    }
                    else
                    {
                        row[index] = field.Trim();
                    }
                }

                rows.Add(row);
            }

            // Group chars by name as - row = char convo, so we can have multiple rows per char
            var groupedByCharacter = rows.GroupBy(row => row[characterColumn]);
            var importedCharacters = 0;

            foreach (var characterGroup in groupedByCharacter)
            {
                var characterName = characterGroup.Key;
                var assetPath = $"{DirectoryData}/Data_Character_{characterName.Replace(" ", "_")}.asset";

                var characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                if (characterData == false)
                {
                    characterData = templateObject
                        ? CreateInstance<CharacterData>()
                        : Instantiate(templateObject);

                    AssetDatabase.CreateAsset(characterData, assetPath);
                }

                var firstRow = characterGroup.First();

                var frontTexture = LoadTexture(firstRow[frontTextureColumn]);
                var backTexture = LoadTexture(firstRow[backTextureColumn]);

                SetField(characterData, "characterName", characterName);
                SetField(characterData, "frontTexture", frontTexture);
                SetField(characterData, "backTexture", backTexture);

                var messages = new List<CharacterMessageData>();
                foreach (var row in characterGroup)
                {
                    var message = new CharacterMessageData();

                    var messageType = CharacterMessageType.CorrectIncorrect;
                    if (Enum.TryParse<CharacterMessageType>(row[typeColumn], out var parsedMessageType))
                    {
                        messageType = parsedMessageType;
                    }

                    var huntMessage = row[huntMessageColumn];
                    var content = row[messageColumn];

                    var correctChoices = new List<string>();
                    foreach (var col in correctChoiceColumns)
                    {
                        if (!string.IsNullOrWhiteSpace(row[col]))
                        {
                            correctChoices.Add(row[col]);
                        }
                    }

                    var incorrectChoices = new List<string>();
                    foreach (var col in incorrectChoiceColumns)
                    {
                        if (!string.IsNullOrWhiteSpace(row[col]))
                        {
                            incorrectChoices.Add(row[col]);
                        }
                    }

                    SetField(message, "messageType", messageType);
                    SetField(message, "content", content);
                    SetField(message, "huntMessage", huntMessage);
                    SetField(message, "correctChoices", correctChoices);
                    SetField(message, "incorrectChoices", incorrectChoices);

                    messages.Add(message);
                }

                var conversation = new CharacterConversationData();
                SetField(conversation, "messages", messages);
                Debug.Log($"Imported conversation \"{characterName}\" with {conversation.Messages.Count()} messages");

                SetField(characterData, "conversation", conversation);

                EditorUtility.SetDirty(characterData);

                importedCharacters++;
            }

            Debug.Log($"Imported {importedCharacters} characters");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void SetField<TTarget, TValue>(TTarget target, string fieldName, TValue value)
        {
            var targetObject = target as Object;
            var targetName = targetObject?.name ?? typeof(TTarget).Name;

            var field = typeof(TTarget).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                Debug.LogWarning($"Field \"{fieldName}\" does not exist on \"{targetName}\"", targetObject);
                return;
            }

            if (value == null)
            {
                Debug.LogWarning($"Value of field \"{fieldName}\" is not set on \"{targetName}\"", targetObject);
                return;
            }

            field.SetValue(target, value);
        }

        private static IReadOnlyList<int> GetColumns(string headerName, List<string> headers)
        {
            return headers
                .Select((header, index) => new { header, index })
                .Where(entry => string.Equals(entry.header, headerName, StringComparison.CurrentCultureIgnoreCase))
                .Select(entry => entry.index)
                .ToList();
        }

        private static Texture2D LoadTexture(string textureName)
        {
            if (string.IsNullOrWhiteSpace(textureName))
            {
                return null;
            }

            var path = $"{DirectoryTextures}/{textureName}";
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static string ToGoogleCsvExportUrl(string sheetsUrl, int gid = 0)
        {
            var uri = new Uri(sheetsUrl);

            var builder = new UriBuilder(uri)
            {
                Fragment = string.Empty,
                Query = $"tqx=out:csv&gid={gid}",
            };

            var path = builder.Path;

            var idx = path.IndexOf("/spreadsheets/d/", StringComparison.Ordinal);
            if (idx == -1)
            {
                throw new ArgumentException("Not a Google Sheets URL");
            }

            var parts = path.Substring(idx).Split('/');
            var sheetId = parts[3];

            builder.Path = $"/spreadsheets/d/{sheetId}/gviz/tq";

            return builder.Uri.ToString();
        }

        private static async UniTask<string> DownloadGoogleCsvAsync(string url)
        {
            var exportUrl = ToGoogleCsvExportUrl(url);

            using var req = UnityWebRequest.Get(exportUrl);
            await req.SendWebRequest().ToUniTask();

            if (req.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Error downloading Google CSV: {req.error}");
            }

            return req.downloadHandler.text;
        }
    }
}
