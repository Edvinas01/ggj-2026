using UnityEngine;
using UnityEditor;
using System.IO;
using RIEVES.GGJ2026.Core.Constants;

namespace RIEVES.GGJ2026.Editor
{
    internal sealed class CreateMaterialsFromTexturesWindow : EditorWindow
    {
        [MenuItem(MenuItemConstants.BaseWindowItemName + "/Material From Textures Creator")]
        private static void OpenWindow()
        {
            GetWindow<CreateMaterialsFromTexturesWindow>("Create Materials");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create URP Unlit Materials", EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUILayout.HelpBox(
                "Select one or more textures in the Project window.\n" + "A material will be created for each texture in the same folder.",
                MessageType.Info
            );

            GUILayout.Space(10);

            if (GUILayout.Button("Create Materials"))
            {
                CreateMaterials();
            }
        }

        private void CreateMaterials()
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit");

            if (shader == null)
            {
                Debug.LogError("URP Unlit shader not found. Make sure URP is installed.");
                return;
            }

            var selection = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

            if (selection.Length == 0)
            {
                Debug.LogWarning("No textures selected.");
                return;
            }

            foreach (var obj in selection)
            {
                var texture = obj as Texture2D;
                if (texture == false)
                {
                    continue;
                }

                var texturePath = AssetDatabase.GetAssetPath(texture);
                var folderPath = Path.GetDirectoryName(texturePath);
                var materialPath = Path.Combine(folderPath, texture.name + ".mat");

                if (File.Exists(materialPath))
                {
                    Debug.LogWarning($"Material already exists: {materialPath}");
                    continue;
                }

                var mat = new Material(shader);
                mat.SetTexture("_BaseMap", texture);

                AssetDatabase.CreateAsset(mat, materialPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Materials created successfully.");
        }
    }
}
