using MenuChanger.Attributes;

namespace MoreLocations.Rando.Settings
{
    public class RelicCostSettings
    {
        [DynamicBound(nameof(MaximumJournalCost), true)]
        [MenuRange(0, 14)]
        public int MinimumJournalCost;
        [DynamicBound(nameof(MinimumJournalCost), false)]
        [MenuRange(0, 14)]
        public int MaximumJournalCost;
        [DynamicBound(nameof(JournalToleranceUB), true)]
        [MenuRange(0, 14)]
        public int JournalTolerance;
        private int JournalToleranceUB => 14 - MaximumJournalCost;

        [DynamicBound(nameof(MaximumSealCost), true)]
        [MenuRange(0, 17)]
        public int MinimumSealCost;
        [DynamicBound(nameof(MinimumSealCost), false)]
        [MenuRange(0, 17)]
        public int MaximumSealCost;
        [DynamicBound(nameof(SealToleranceUB), true)]
        [MenuRange(0, 17)]
        public int SealTolerance;
        private int SealToleranceUB => 17 - MaximumSealCost;

        [DynamicBound(nameof(MaximumIdolCost), true)]
        [MenuRange(0, 8)]
        public int MinimumIdolCost;
        [DynamicBound(nameof(MinimumIdolCost), false)]
        [MenuRange(0, 8)]
        public int MaximumIdolCost;
        [DynamicBound(nameof(IdolToleranceUB), true)]
        [MenuRange(0, 8)]
        public int IdolTolerance;
        private int IdolToleranceUB => 8 - MaximumIdolCost;

        [DynamicBound(nameof(MaximumEggCost), true)]
        [MenuRange(0, 4)]
        public int MinimumEggCost;
        [DynamicBound(nameof(MinimumEggCost), false)]
        [MenuRange(0, 4)]
        public int MaximumEggCost;
        [DynamicBound(nameof(EggToleranceUB), true)]
        [MenuRange(0, 4)]
        public int EggTolerance;
        private int EggToleranceUB => 4 - MaximumEggCost;
    }
}
