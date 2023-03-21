using System.Collections.Generic;

namespace MoreLocations.Rando.Settings.Presets
{
    public static class JunkShopCostPresets
    {
        public static JunkCostSettings Basic => new()
        {
            MinimumCostsPerItem = 1,
            MaximumCostsPerItem = 1,
        };
        public static JunkCostSettings Eclectic => new()
        {
            MinimumCostsPerItem = 1,
            MaximumCostsPerItem = 3,
        };
        public static JunkCostSettings Costsanity => new()
        {
            MinimumCostsPerItem = 3,
            MaximumCostsPerItem = 5
        };

        public static readonly Dictionary<string, JunkCostSettings> Presets = new()
        {
            [nameof(Basic)] = Basic,
            [nameof(Eclectic)] = Eclectic,
            [nameof(Costsanity)] = Costsanity,
        };
    }
}
