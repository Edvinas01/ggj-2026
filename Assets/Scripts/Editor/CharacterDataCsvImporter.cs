using System;
using RIEVES.GGJ2026.Core.Constants;
using RIEVES.GGJ2026.Runtime.Characters;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RIEVES.GGJ2026.Editor
{
    internal static class CharacterDataCsvImporter
    {
        private const string DirectoryTextures = "Assets/Visuals/Characters/Textures";
        private const string DirectoryData = "Assets/Data/Characters";

        [MenuItem(MenuItemConstants.BaseWindowItemName + "/Character Data Importer")]
        private static void ImportFromCsv()
        {
            CharacterImporterWindow.ShowWindow();
        }

        internal static void PerformImport(CharacterData templateObject)
        {
            var path = EditorUtility.OpenFilePanel("Select Character CSV", Application.dataPath, "csv");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (!Directory.Exists(DirectoryData))
            {
                Directory.CreateDirectory(DirectoryData);
                AssetDatabase.Refresh();
            }

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(
                reader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                    TrimOptions = TrimOptions.Trim
                }
            );

            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord.ToList();

            var correctChoiceColumns = headers
                .Select((h, i) => new { h, i })
                .Where(x => x.h == "Choice Correct")
                .Select(x => x.i)
                .ToList();

            var incorrectChoiceColumns = headers
                .Select((h, i) => new { h, i })
                .Where(x => x.h == "Choice Incorrect")
                .Select(x => x.i)
                .ToList();

            var rows = new List<string[]>();

            while (csv.Read())
            {
                var row = new string[headers.Count];
                for (var i = 0; i < headers.Count; i++)
                {
                    row[i] = csv.GetField(i);
                }

                rows.Add(row);
            }

            var characterColumn = headers.IndexOf("Character");
            var frontTextureColumn = headers.IndexOf("Front");
            var backTextureColumn = headers.IndexOf("Back");
            var messageColumn = headers.IndexOf("Message");
            var typeColumn = headers.IndexOf("Type");
            var huntMessageColumn = headers.IndexOf("Hunt Message");

            var groupedByCharacter = rows.GroupBy(r => r[characterColumn]);
            var importedCharacters = 0;

            foreach (var characterGroup in groupedByCharacter)
            {
                var characterName = characterGroup.Key;
                var assetPath = $"{DirectoryData}/Data_Character_{characterName.Replace(" ", "_")}.asset";

                var characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                if (characterData == false)
                {
                    if (templateObject != null)
                    {
                        characterData = Object.Instantiate(templateObject);
                    }
                    else
                    {
                        characterData = ScriptableObject.CreateInstance<CharacterData>();
                    }

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
                Debug.Log($"Imported conversation {characterName} with {conversation.Messages.Count()} messages");

                SetField(characterData, "conversation", conversation);

                EditorUtility.SetDirty(characterData);

                importedCharacters++;
                continue;

                void SetField<TTarget, TValue>(TTarget target, string fieldName, TValue value)
                {
                    var field = typeof(TTarget).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        field.SetValue(target, value);
                    }
                    else
                    {
                        Debug.LogWarning($"Field is not set", characterData);
                    }
                }
            }

            Debug.Log($"Imported {importedCharacters} characters");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
    }

    internal class CharacterImporterWindow : EditorWindow
    {
        private CharacterData templateObject;

        internal static void ShowWindow()
        {
            var window = GetWindow<CharacterImporterWindow>("Character Data Importer");
            window.minSize = new Vector2(400, 150);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Character Data CSV Importer", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.Label("Template Scriptable Object (Optional):", EditorStyles.label);
            templateObject = (CharacterData)EditorGUILayout.ObjectField(
                templateObject,
                typeof(CharacterData),
                false
            );

            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "Select a template CharacterData object to use as a base for newly created assets. " + "If no template is selected, default ScriptableObjects will be created.",
                MessageType.Info
            );

            GUILayout.Space(20);

            GUI.enabled = true;
            if (GUILayout.Button("Import CSV", GUILayout.Height(30)))
            {
                Close();
                CharacterDataCsvImporter.PerformImport(templateObject);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }
    }
}
