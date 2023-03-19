using ItemChanger;
using MoreLocations.Rando.Costs;
using RandomizerCore.Logic;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System;

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

            RequestBuilder.OnUpdate.Subscribe(-1000f, CapRelicGeoCosts);

            RequestBuilder.OnUpdate.Subscribe(-500f, SetupRefs);
            RequestBuilder.OnUpdate.Subscribe(-100f, ApplyShopCostRandomization);

            RequestBuilder.OnUpdate.Subscribe(0f, ApplyMiscLocationSettings);
            RequestBuilder.OnUpdate.Subscribe(0f, ApplyLemmShopSettings);
            RequestBuilder.OnUpdate.Subscribe(0f, ApplyJunkShopSettings);

            RequestBuilder.OnUpdate.Subscribe(100f, DerangeLemmShop);
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
            anyCostProvider?.FinishConstruction(rb.rng);

            relicCostProvider = null;
            anyCostProvider = null;
        }

        private static void SetupRefs(RequestBuilder rb)
        {
            if (!RandoInterop.Enabled)
            {
                return;
            }

            rb.EditLocationRequest(LocationNames.Lemm, info =>
            {
                info.getLocationDef = () => new LocationDef()
                {
                    Name = LocationNames.Lemm,
                    SceneName = SceneNames.Ruins1_05,
                    FlexibleCount = true,
                    AdditionalProgressionPenalty = true,
                };
            });
        }

        private static void CapRelicGeoCosts(RequestBuilder rb)
        {
            if (!RandoInterop.Enabled)
            {
                return;
            }

            rb.EditItemRequest(MoreItemNames.Wanderers_Journal_Sale, info =>
            {
                info.getItemDef = () => new()
                {
                    Name = MoreItemNames.Wanderers_Journal_Sale,
                    Pool = "Geo",
                    PriceCap = 1,
                    MajorItem = false,
                };
            });
            rb.EditItemRequest(MoreItemNames.Hallownest_Seal_Sale, info =>
            {
                info.getItemDef = () => new()
                {
                    Name = MoreItemNames.Hallownest_Seal_Sale,
                    Pool = "Geo",
                    PriceCap = 1,
                    MajorItem = false,
                };
            });
            rb.EditItemRequest(MoreItemNames.Kings_Idol_Sale, info =>
            {
                info.getItemDef = () => new()
                {
                    Name = MoreItemNames.Kings_Idol_Sale,
                    Pool = "Geo",
                    PriceCap = 1,
                    MajorItem = false,
                };
            });
            rb.EditItemRequest(MoreItemNames.Arcane_Egg_Sale, info =>
            {
                info.getItemDef = () => new()
                {
                    Name = MoreItemNames.Arcane_Egg_Sale,
                    Pool = "Geo",
                    PriceCap = 1,
                    MajorItem = false,
                };
            });

            if (!rb.TryGetItemDef(ItemNames.Wanderers_Journal, out ItemDef wjDef)
                || !rb.TryGetItemDef(ItemNames.Hallownest_Seal, out ItemDef hsDef)
                || !rb.TryGetItemDef(ItemNames.Kings_Idol, out ItemDef kiDef)
                || !rb.TryGetItemDef(ItemNames.Arcane_Egg, out ItemDef aeDef))
            {
                throw new ArgumentException("Could not find existing item defs for all relics");
            }
            rb.EditItemRequest(ItemNames.Wanderers_Journal, info =>
            {
                info.getItemDef = () => wjDef with { PriceCap = 500 };
            });
            rb.EditItemRequest(ItemNames.Hallownest_Seal, info =>
            {
                info.getItemDef = () => hsDef with { PriceCap = 500 };
            });
            rb.EditItemRequest(ItemNames.Kings_Idol, info =>
            {
                info.getItemDef = () => kiDef with { PriceCap = 500 };
            });
            rb.EditItemRequest(ItemNames.Arcane_Egg, info =>
            {
                info.getItemDef = () => aeDef with { PriceCap = 500 };
            });
        }

        private static void ApplyShopCostRandomization(RequestBuilder rb)
        {
            rb.CostConverters.Subscribe(0f, ConvertDeferredCosts!);
            rb.rm.OnSendEvent += (eventType, _) =>
            {
                if (eventType == RandomizerCore.RandoEventType.Initializing)
                {
                    TearDownCostManagement(rb);
                }
            };

            if (!RandoInterop.Enabled)
            {
                return;
            }

            rb.EditLocationRequest(LocationNames.Lemm, info =>
            {
                info.onRandoLocationCreation += (factory, rl) =>
                {
                    if (relicCostProvider == null)
                    {
                        return;
                    }
                    rl.AddCost(relicCostProvider.Next(factory.lm, factory.rng, null!));
                };
            });
        }

        private static bool ConvertDeferredCosts(LogicCost lc, out Cost? cost)
        {
            if (lc is CappedIntCost cic)
            {
                cost = cic.GetIcCost();
                return true;
            }
            cost = default;
            return false;
        }

        private static void ApplyMiscLocationSettings(RequestBuilder rb)
        {

        }

        private static void ApplyLemmShopSettings(RequestBuilder rb)
        {
            if (!RandoInterop.Enabled)
            {
                return;
            }

            // todo - make these numbers settings
            for (int i = 0; i < 14; i++)
            {
                rb.AddItemByName(MoreItemNames.Wanderers_Journal_Sale);
                rb.AddLocationByName(LocationNames.Lemm);
            }
            for (int i = 0; i < 17; i++)
            {
                rb.AddItemByName(MoreItemNames.Hallownest_Seal_Sale);
                rb.AddLocationByName(LocationNames.Lemm);
            }
            for (int i = 0; i < 8; i++)
            {
                rb.AddItemByName(MoreItemNames.Kings_Idol_Sale);
                rb.AddLocationByName(LocationNames.Lemm);
            }
            for (int i = 0; i < 4; i++)
            {
                rb.AddItemByName(MoreItemNames.Arcane_Egg_Sale);
                rb.AddLocationByName(LocationNames.Lemm);
            }
        }

        private static void ApplyJunkShopSettings(RequestBuilder rb)
        {

        }

        private static void DerangeLemmShop(RequestBuilder rb)
        {

        }

        private static void AddRelicCostTolerances(LogicManager lm, GenerationSettings gs, ProgressionInitializer pi)
        {

        }
    }
}
