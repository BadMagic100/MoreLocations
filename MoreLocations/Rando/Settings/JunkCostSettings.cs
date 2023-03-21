using MenuChanger.Attributes;

namespace MoreLocations.Rando.Settings
{
    public class JunkCostSettings
    {
        [MenuRange(1, int.MaxValue)]
        [DynamicBound(nameof(MaximumCostsPerItem), true)]
        public int MinimumCostsPerItem;
        [MenuRange(1, int.MaxValue)]
        [DynamicBound(nameof(MinimumCostsPerItem), false)]
        public int MaximumCostsPerItem;
    }
}
