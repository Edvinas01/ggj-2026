using RIEVES.GGJ2026.Runtime.Items;

namespace RIEVES.GGJ2026.Runtime.Stats
{
    internal readonly struct ItemUsedStat
    {
        public ItemData ItemData { get; }

        public ItemUsedStat(ItemData itemData)
        {
            ItemData = itemData;
        }
    }
}
