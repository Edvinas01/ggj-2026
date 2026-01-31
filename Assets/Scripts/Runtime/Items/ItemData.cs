using RIEVES.GGJ2026.Core.Constants;
using UnityEngine;

namespace RIEVES.GGJ2026.Runtime.Items
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + "Data_Item",
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Item Data",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    internal sealed class ItemData : ScriptableObject
    {
        [SerializeField]
        private string itemName;

        [Min(0)]
        [SerializeField]
        private int value = 5;

        public string ItemName => itemName;

        public int Value => value;
    }
}
