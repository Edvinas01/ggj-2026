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
    }
}
