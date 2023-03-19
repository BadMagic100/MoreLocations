using System.Collections.Generic;

namespace MoreLocations.Rando.Settings.Presets
{
    public static class RelicGeoPresets
    {
        public static RelicGeoSettings None => new()
        {
            JournalSales = 0,
            SealSales = 0,
            IdolSales = 0,
            EggSales = 0,
        };
        public static RelicGeoSettings All => new()
        {
            JournalSales = 14,
            SealSales = 17,
            IdolSales = 8,
            EggSales = 4,
        };
        public static RelicGeoSettings Reduced => new()
        {
            JournalSales = 7,
            SealSales = 9,
            IdolSales = 4,
            EggSales = 2,
        };

        public static readonly Dictionary<string, RelicGeoSettings> Presets = new()
        {
            [nameof(None)] = None,
            [nameof(All)] = All,
            [nameof(Reduced)] = Reduced,
        };
    }

    public static class RelicCostPresets
    {
        public static RelicCostSettings Standard => new()
        {
            MinimumJournalCost = 1,
            MaximumJournalCost = 10,
            JournalTolerance = 2,

            MinimumSealCost = 1,
            MaximumSealCost = 12,
            SealTolerance = 2,

            MinimumIdolCost = 1,
            MaximumIdolCost = 4,
            IdolTolerance = 2,

            MinimumEggCost = 1,
            MaximumEggCost = 1,
            EggTolerance = 2,
        };
        public static RelicCostSettings More => new()
        {
            MinimumJournalCost = 1,
            MaximumJournalCost = 14,
            JournalTolerance = 0,

            MinimumSealCost = 1,
            MaximumSealCost = 17,
            SealTolerance = 0,

            MinimumIdolCost = 1,
            MaximumIdolCost = 8,
            IdolTolerance = 0,

            MinimumEggCost = 1,
            MaximumEggCost = 4,
            EggTolerance = 0,
        };

        public static RelicCostSettings Expert => new()
        {
            MinimumJournalCost = 4,
            MaximumJournalCost = 14,
            JournalTolerance = 0,

            MinimumSealCost = 5,
            MaximumSealCost = 17,
            SealTolerance = 0,

            MinimumIdolCost = 2,
            MaximumIdolCost = 8,
            IdolTolerance = 0,

            MinimumEggCost = 2,
            MaximumEggCost = 4,
            EggTolerance = 0,
        };

        public static readonly Dictionary<string, RelicCostSettings> Presets = new()
        {
            [nameof(Standard)] = Standard,
            [nameof(More)] = More,
            [nameof(Expert)] = Expert,
        };
    }
}
