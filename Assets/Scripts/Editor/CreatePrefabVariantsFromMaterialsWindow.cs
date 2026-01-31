using UnityEngine;
using UnityEditor;
using RIEVES.GGJ2026.Core.Constants;

namespace RIEVES.GGJ2026.Editor
{
    internal sealed class CreatePrefabVariantsFromMaterialsWindow : EditorWindow
    {
        private GameObject prefab;

        [MenuItem(MenuItemConstants.BaseWindowItemName + "/Prefab From Materials Creator")]
        private static void OpenWindow()
        {
            GetWindow<CreatePrefabVariantsFromMaterialsWindow>("Prefab Variants");
        }

        private void OnGUI()
        {
            GUILayout.Label("Prefab Variant Creator", EditorStyles.boldLabel);
            GUILayout.Space(5);

            prefab = (GameObject)EditorGUILayout.ObjectField(
                "Prefab",
                prefab,
                typeof(GameObject),
                false
            );

            GUILayout.Space(10);

            var selectedMaterials = Selection.GetFiltered<Material>(SelectionMode.Assets);

            EditorGUILayout.HelpBox(
                $"Selected materials: {selectedMaterials.Length}\n" + "Select materials in the Project window.",
                MessageType.Info
            );

            GUILayout.Space(10);

            GUI.enabled = prefab != null && selectedMaterials.Length > 0;

            if (GUILayout.Button("Create"))
            {
                CreateVariants(selectedMaterials);
            }

            GUI.enabled = true;
        }

        private void CreateVariants(Material[] materials)
        {
            foreach (var mat in materials)
            {
                if (mat == null)
                    continue;

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.transform.position = Vector3.zero;

                var matName = mat.name.Replace(prefab.name, "");
                if (matName.StartsWith("_"))
                {
                    matName = matName.Substring(1);
                }

                instance.name = $"{prefab.name}_{matName}";

                ApplyMaterialToAllRenderers(instance, mat);

                Undo.RegisterCreatedObjectUndo(instance, "Create Prefab Variant");
            }
        }

        private static void ApplyMaterialToAllRenderers(GameObject root, Material mat)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);

            foreach (var r in renderers)
            {
                var mats = new Material[r.sharedMaterials.Length];
                for (var i = 0; i < mats.Length; i++)
                {
                    mats[i] = mat;
                }

                r.sharedMaterials = mats;
            }
        }
    }
}
