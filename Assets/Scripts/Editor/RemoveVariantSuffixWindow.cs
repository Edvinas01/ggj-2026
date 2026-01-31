using RIEVES.GGJ2026.Core.Constants;
using UnityEngine;
using UnityEditor;

namespace RIEVES.GGJ2026.Editor
{
    public static class RemoveVariantSuffix
    {
        [MenuItem(MenuItemConstants.BaseWindowItemName + "/Remove Variant Suffix")]
        private static void RemoveSuffix()
        {
            var selectedObjects = Selection.objects;

            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("No objects selected.");
                return;
            }

            Undo.RecordObjects(selectedObjects, "Remove Variant Suffix");

            foreach (var obj in selectedObjects)
            {
                if (obj.name.EndsWith(" Variant"))
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(obj), obj.name.Replace(" Variant", " Variant"));
                }
            }
        }
    }
}
