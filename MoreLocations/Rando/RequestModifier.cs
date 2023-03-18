using ItemChanger;
using MoreLocations.Rando.Costs;
using RandomizerCore.Logic;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace MoreLocations.Rando
{
    internal static class RequestModifier
    {
        private static WeightedRandomCostChooser? relicCostProvider = null;

        private static WeightedRandomCostChooser? anyCostProvider = null;

        public static void Hook()
        {
            ProgressionInitializer.OnCreateProgressionInitializer += AddRelicCostTolerances;

            RequestBuilder.OnUpdate.Subscribe(float.NegativeInfinity, SetupCostManagement);
            RequestBuilder.OnUpdate.Subscribe(float.PositiveInfinity, TearDownCostManagement);

            RequestBuilder.OnUpdate.Subscribe(-1000f, CapRelicGeoCosts);

            RequestBuilder.OnUpdate.Subscribe(-500f, SetupRefs);
            RequestBuilder.OnUpdate.Subscribe(-100f, ApplyShopCostRandomization);

            RequestBuilder.OnUpdate.Subscribe(0f, ApplyMiscLocationSettings);
            RequestBuilder.OnUpdate.Subscribe(0f, ApplyLemmShopSettings);
            RequestBuilder.OnUpdate.Subscribe(0f, ApplyJunkShopSettings);

            RequestBuilder.OnUpdate.Subscribe(100f, DerangeLemmShop);
            RequestBuilder.OnUpdate.Subscribe(100f, ApplyDuplicateShopConstraint);
        }

        private static void SetupCostManagement(RequestBuilder rb)
        {
            relicCostProvider = new WeightedRandomCostChooser()
            {
                ManagesChildFinalization = true,
                PreferNonEmptyProviders = true,
            };
            relicCostProvider.AddDynamicWeight(new CappedIntCostProvider("WANDERERSJOURNALS", 1, 14, amount =>
            {
                string suffix = amount == 1 ? "" : "s";
                return new ConsumablePDIntCost(amount, nameof(PlayerData.trinket1), $"{amount} Wanderer's Journal{suffix}");
            }));
            relicCostProvider.AddDynamicWeight(new CappedIntCostProvider("HALLOWNESTSEALS", 1, 17, amount =>
            {
                string suffix = amount == 1 ? "" : "s";
                return new ConsumablePDIntCost(amount, nameof(PlayerData.trinket2), $"{amount} Hallownest Seal{suffix}");
            }));
            relicCostProvider.AddDynamicWeight(new CappedIntCostProvider("KINGSIDOLS", 1, 8, amount =>
            {
                string suffix = amount == 1 ? "" : "s";
                return new ConsumablePDIntCost(amount, nameof(PlayerData.trinket3), $"{amount} King's Idol{suffix}");
            }));
            relicCostProvider.AddDynamicWeight(new CappedIntCostProvider("ARCANEEGGS", 1, 4, amount =>
            {
                string suffix = amount == 1 ? "" : "s";
                return new ConsumablePDIntCost(amount, nameof(PlayerData.trinket4), $"{amount} Arcane Egg{suffix}");
            }));

            anyCostProvider = new WeightedRandomCostChooser()
            {
                ManagesChildFinalization = false,
                PreferNonEmptyProviders = true,
            };
            anyCostProvider.AddFixedWeight(1.0, relicCostProvider);
            anyCostProvider.AddFixedWeight(1.0, new GeoCostProvider());
            anyCostProvider.AddFixedWeight(1.0, 
                new SimpleCostProvider("GRUBS", rb.gs.CostSettings.MinimumGrubCost, rb.gs.CostSettings.MaximumGrubCost));
            anyCostProvider.AddFixedWeight(1.0, 
                new SimpleCostProvider("ESSENCE", rb.gs.CostSettings.MinimumEssenceCost, rb.gs.CostSettings.MaximumEssenceCost));
            anyCostProvider.AddFixedWeight(1.0, 
                new SimpleCostProvider("RANCIDEGGS", rb.gs.CostSettings.MinimumEggCost, rb.gs.CostSettings.MaximumEggCost));
            anyCostProvider.AddFixedWeight(1.0,
                new SimpleCostProvider("CHARMS", rb.gs.CostSettings.MinimumCharmCost, rb.gs.CostSettings.MaximumCharmCost));
        }

        private static void TearDownCostManagement(RequestBuilder rb)
        {
            relicCostProvider?.FinishConstruction(rb.rng);
        }

        private static void SetupRefs(RequestBuilder rb)
        {

        }

        private static void CapRelicGeoCosts(RequestBuilder rb)
        {

        }

        private static void ApplyShopCostRandomization(RequestBuilder rb)
        {

        }

        private static void ApplyMiscLocationSettings(RequestBuilder rb)
        {

        }

        private static void ApplyLemmShopSettings(RequestBuilder rb)
        {

        }

        private static void ApplyJunkShopSettings(RequestBuilder rb)
        {

        }

        private static void DerangeLemmShop(RequestBuilder rb)
        {

        }

        private static void ApplyDuplicateShopConstraint(RequestBuilder rb)
        {

        }

        private static void AddRelicCostTolerances(LogicManager lm, GenerationSettings gs, ProgressionInitializer pi)
        {

        }
    }
}
