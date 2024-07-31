using ItemChanger;
using ItemChanger.Locations;
using MoreLocations.ItemChanger;
using MoreLocations.ItemChanger.CostIconSupport;
using MoreLocations.Rando.Costs;
using Newtonsoft.Json;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.Randomization;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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

            RequestBuilder.OnUpdate.Subscribe(-500f, SetupMiscLocationRefs);
            RequestBuilder.OnUpdate.Subscribe(-500f, SetupLemmRefs);
            RequestBuilder.OnUpdate.Subscribe(-500f, SetupJunkShopRefs);
            RequestBuilder.OnUpdate.Subscribe(-100f, ApplyShopCostRandomization);

            RequestBuilder.OnUpdate.Subscribe(0f, ApplyMiscLocationSettings);
            RequestBuilder.OnUpdate.Subscribe(0f, ApplyLemmShopSettings);
            RequestBuilder.OnUpdate.Subscribe(0f, ApplyJunkShopSettings);

            RequestBuilder.OnUpdate.Subscribe(20f, HandleJunkItemRemove);

            RequestBuilder.OnUpdate.Subscribe(100f, DerangeLemmShop);
        }

        private static void SetupCostManagement(RequestBuilder rb)
        {
            relicCostProvider = new WeightedRandomCostChooser()
            {
                ManagesChildLifecycle = true,
                PreferNonEmptyProviders = true,
            };
            relicCostProvider.AddDynamicWeight(new CappedIntCostProvider("WANDERERSJOURNALS", 
                RandoInterop.Settings.LemmShopSettings.CostSettings.MinimumJournalCost, 
                RandoInterop.Settings.LemmShopSettings.CostSettings.MaximumJournalCost, 
                amount => new RelicCost(amount, RelicType.Journal)));
            relicCostProvider.AddDynamicWeight(new CappedIntCostProvider("HALLOWNESTSEALS",
                RandoInterop.Settings.LemmShopSettings.CostSettings.MinimumSealCost,
                RandoInterop.Settings.LemmShopSettings.CostSettings.MaximumSealCost,
                amount => new RelicCost(amount, RelicType.Seal)));
            relicCostProvider.AddDynamicWeight(new CappedIntCostProvider("KINGSIDOLS", 
                RandoInterop.Settings.LemmShopSettings.CostSettings.MinimumIdolCost,
                RandoInterop.Settings.LemmShopSettings.CostSettings.MaximumIdolCost,
                amount => new RelicCost(amount, RelicType.Idol)));
            relicCostProvider.AddDynamicWeight(new CappedIntCostProvider("ARCANEEGGS", 
                RandoInterop.Settings.LemmShopSettings.CostSettings.MinimumEggCost, 
                RandoInterop.Settings.LemmShopSettings.CostSettings.MaximumEggCost,
                amount => new RelicCost(amount, RelicType.Egg)));

            anyCostProvider = new WeightedRandomCostChooser()
            {
                ManagesChildLifecycle = false,
                PreferNonEmptyProviders = true,
            };
            anyCostProvider.AddFixedWeight(1.0, new GeoCostProvider());
            anyCostProvider.AddFixedWeight(1.0, 
                new SimpleCostProvider("GRUBS", rb.gs.CostSettings.MinimumGrubCost, rb.gs.CostSettings.MaximumGrubCost));
            anyCostProvider.AddFixedWeight(1.0, 
                new SimpleCostProvider("ESSENCE", rb.gs.CostSettings.MinimumEssenceCost, rb.gs.CostSettings.MaximumEssenceCost));
            anyCostProvider.AddFixedWeight(1.0,
                new SimpleCostProvider("CHARMS", rb.gs.CostSettings.MinimumCharmCost, rb.gs.CostSettings.MaximumCharmCost));

            // only include lemm and jiji costs if their respective shops are randomized
            if (RandoInterop.Settings.LemmShopSettings.Enabled)
            {
                anyCostProvider.AddFixedWeight(1.0, relicCostProvider);
            }
            if (rb.gs.NoveltySettings.EggShop)
            {
                anyCostProvider.AddFixedWeight(1.0,
                    new SimpleCostProvider("RANCIDEGGS", rb.gs.CostSettings.MinimumEggCost, rb.gs.CostSettings.MaximumEggCost));
            }

            IEnumerable<ICostProvider> connectionAddedProviders = ConnectionInterop.costProviders
                .Where(p => p.include())
                .Select(p => p.providerFactory());
            foreach (ICostProvider provider in connectionAddedProviders)
            {
                anyCostProvider.AddFixedWeight(1, provider);
            }
        }

        private static void PreRandomizeCostsAndCleanUp(RequestBuilder rb)
        {
            relicCostProvider?.PreRandomize(rb.rng);
            anyCostProvider?.PreRandomize(rb.rng);

            relicCostProvider = null;
            anyCostProvider = null;
        }

        private static void CapRelicGeoCosts(RequestBuilder rb)
        {
            if (!RandoInterop.Enabled || !RandoInterop.Settings.LemmShopSettings.Enabled)
            {
                return;
            }

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

        private static void SetupMiscLocationRefs(RequestBuilder rb)
        {
            if (!RandoInterop.Enabled)
            {
                return;
            }

            rb.EditLocationRequest(MoreLocationNames.Swim, info =>
            {
                info.getLocationDef = () => new()
                {
                    Name = MoreLocationNames.Swim,
                    SceneName = SceneNames.Crossroads_50,
                    FlexibleCount = false,
                    AdditionalProgressionPenalty = false,
                };
            });
            rb.EditLocationRequest(MoreLocationNames.Stag_Nest_Egg, info =>
            {
                info.getLocationDef = () => new()
                {
                    Name = MoreLocationNames.Stag_Nest_Egg,
                    SceneName = SceneNames.Cliffs_03,
                    FlexibleCount = false,
                    AdditionalProgressionPenalty = false,
                };
            });
            rb.EditLocationRequest(MoreLocationNames.Geo_Chest_Above_Baldur_Shell, info =>
            {
                info.getLocationDef = () => new()
                {
                    Name = MoreLocationNames.Geo_Chest_Above_Baldur_Shell,
                    SceneName = SceneNames.Fungus1_28,
                    FlexibleCount = false,
                    AdditionalProgressionPenalty = false,
                };
            });
        }

        private static void SetupLemmRefs(RequestBuilder rb)
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

            if (rb.gs.SplitGroupSettings.RandomizeOnStart
                && RandoInterop.Settings.LemmShopSettings.RelicGeoGroup >= 0
                && RandoInterop.Settings.LemmShopSettings.RelicGeoGroup <= -2)
            {
                RandoInterop.Settings.LemmShopSettings.RelicGeoGroup = rb.rng.Next(3);
            }
            // 0 and -1 put us in the main item group by default, no need for special grouping
            if (RandoInterop.Settings.LemmShopSettings.RelicGeoGroup > 0)
            {
                ItemGroupBuilder? relicGeoGroup = null;
                string label = RBConsts.SplitGroupPrefix + RandoInterop.Settings.LemmShopSettings.RelicGeoGroup;
                foreach (ItemGroupBuilder igb in rb.EnumerateItemGroups())
                {
                    if (igb.label == label)
                    {
                        relicGeoGroup = igb;
                        break;
                    }
                }
                relicGeoGroup ??= rb.MainItemStage.AddItemGroup(label);

                rb.OnGetGroupFor.Subscribe(0.05f, ResolveGroup);
                bool ResolveGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb)
                {
                    switch (type)
                    {
                        case RequestBuilder.ElementType.Item:
                            if (item == MoreItemNames.Wanderers_Journal_Sale
                                || item == MoreItemNames.Hallownest_Seal_Sale
                                || item == MoreItemNames.Kings_Idol_Sale
                                || item == MoreItemNames.Arcane_Egg_Sale)
                            {
                                gb = relicGeoGroup;
                                return true;
                            }
                            break;
                        case RequestBuilder.ElementType.Location:
                            if (item == LocationNames.Lemm)
                            {
                                gb = relicGeoGroup;
                                return true;
                            }
                            break;
                        default:
                            break;
                    }
                    gb = default!;
                    return false;
                }
            }
        }

        private static void SetupJunkShopRefs(RequestBuilder rb)
        {
            if (!RandoInterop.Enabled)
            {
                return;
            }

            rb.EditLocationRequest(MoreLocationNames.Junk_Shop, info =>
            {
                info.getLocationDef = () => new LocationDef()
                {
                    Name = MoreLocationNames.Junk_Shop,
                    SceneName = SceneNames.Room_GG_Shortcut,
                    FlexibleCount = true,
                    AdditionalProgressionPenalty = true,
                };
            });
        }

        private static void ApplyShopCostRandomization(RequestBuilder rb)
        {
            rb.CostConverters.Subscribe(0f, ConvertDeferredCosts!);
            rb.rm.OnSendEvent += (eventType, _) =>
            {
                if (eventType == RandoEventType.Initializing)
                {
                    PreRandomizeCostsAndCleanUp(rb);
                }
            };

            if (!RandoInterop.Enabled)
            {
                return;
            }

            rb.EditLocationRequest(LocationNames.Lemm, info =>
            {
                info.customPlacementFetch += (factory, rp) =>
                {
                    if (factory.TryFetchPlacement(rp.Location.Name, out AbstractPlacement plt))
                    {
                        return plt;
                    }

                    ShopLocation loc = (ShopLocation)factory.MakeLocation(rp.Location.Name);
                    loc.costDisplayerSelectionStrategy = new MixedCostDisplayerSelectionStrategy()
                    {
                        Capabilities =
                        {
                            new RelicCostSupport()
                        }
                    };
                    plt = loc.Wrap();
                    factory.AddPlacement(plt);
                    return plt;
                };
                info.onRandoLocationCreation += (factory, rl) =>
                {
                    if (relicCostProvider == null)
                    {
                        return;
                    }
                    rl.AddCost(relicCostProvider.Next(factory.lm, factory.rng));
                };
            });
            rb.EditLocationRequest(MoreLocationNames.Junk_Shop, info =>
            {
                info.customPlacementFetch += (factory, rp) =>
                {
                    if (factory.TryFetchPlacement(rp.Location.Name, out AbstractPlacement plt))
                    {
                        return plt;
                    }

                    ShopLocation loc = (ShopLocation)factory.MakeLocation(rp.Location.Name);
                    MixedCostDisplayerSelectionStrategy cdss = (MixedCostDisplayerSelectionStrategy)loc.costDisplayerSelectionStrategy;
                    cdss.Capabilities.AddRange(ConnectionInterop.costSupportCapabilities);
                    plt = loc.Wrap();
                    factory.AddPlacement(plt);
                    return plt;
                };
                info.onRandoLocationCreation += (factory, rl) =>
                {
                    if (anyCostProvider == null)
                    {
                        return;
                    }
                    int numCosts = rb.rng.Next(RandoInterop.Settings.JunkShopSettings.CostSettings.MinimumCostsPerItem,
                        RandoInterop.Settings.JunkShopSettings.CostSettings.MaximumCostsPerItem + 1);
                    for (int i = 0; i < numCosts; i++)
                    {
                        try
                        {
                            rl.AddCost(anyCostProvider.SelectWithoutReplacement(factory.lm, factory.rng));
                        } 
                        catch (InvalidOperationException)
                        {
                            // this is totally fine - it just means that the user requested more costs than we could give them;
                            // we don't need to fail out but we can stop trying to add more.
                            MoreLocationsMod.Instance.LogWarn($"Exhausted all {i} available cost types selecting " +
                                $"costs for an item at {MoreLocationNames.Junk_Shop}.");
                            break;
                        }
                    }
                    anyCostProvider.ResetAvailablePool();
                };
                info.onRandomizerFinish += placement =>
                {
                    if (placement.Location is not RandoModLocation rl 
                        || placement.Item is not RandoModItem ri 
                        || rl.costs == null)
                    {
                        return;
                    }
                    foreach (IPostRandomizedCost prc in rl.costs.OfType<IPostRandomizedCost>())
                    {
                        prc.PostRandomize(rb.rng, ri);
                    }
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
            if (!RandoInterop.Enabled)
            {
                return;
            }

            if (RandoInterop.Settings.MiscLocationSettings.Swim)
            {
                rb.AddLocationByName(MoreLocationNames.Swim);
            }
            if (RandoInterop.Settings.MiscLocationSettings.StagNestEgg)
            {
                rb.AddLocationByName(MoreLocationNames.Stag_Nest_Egg);
            }
            if (RandoInterop.Settings.MiscLocationSettings.BaldurShellChest)
            {
                rb.AddLocationByName(MoreLocationNames.Geo_Chest_Above_Baldur_Shell);
            }
            if (RandoInterop.Settings.MiscLocationSettings.AdditionalLocations)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                JsonSerializer jsonSerializer = new() {TypeNameHandling = TypeNameHandling.Auto};
                
                using Stream stream = assembly.GetManifestResourceStream("MoreLocations.Resources.Data.customlocations.json");
                StreamReader streamReader = new(stream);
                List<AdditionalLocation> additionalLocations = jsonSerializer.Deserialize<List<AdditionalLocation>>(new JsonTextReader(streamReader));
            
                foreach (AdditionalLocation al in additionalLocations)
                {
                    rb.AddLocationByName(al.name);
                }
            }
        }

        private static void ApplyLemmShopSettings(RequestBuilder rb)
        {
            if (!RandoInterop.Enabled)
            {
                return;
            }

            if (RandoInterop.Settings.LemmShopSettings.Enabled)
            {
                rb.AddItemByName(MoreItemNames.Wanderers_Journal_Sale,
                    RandoInterop.Settings.LemmShopSettings.GeoSettings.JournalSales);
                rb.AddItemByName(MoreItemNames.Hallownest_Seal_Sale,
                    RandoInterop.Settings.LemmShopSettings.GeoSettings.SealSales);
                rb.AddItemByName(MoreItemNames.Kings_Idol_Sale,
                    RandoInterop.Settings.LemmShopSettings.GeoSettings.IdolSales);
                rb.AddItemByName(MoreItemNames.Arcane_Egg_Sale,
                    RandoInterop.Settings.LemmShopSettings.GeoSettings.EggSales);

                rb.AddLocationByName(LocationNames.Lemm, 43);
            }
            else
            {
                for (int i = 0; i < 14; i++)
                {
                    rb.AddToVanilla(new VanillaDef(MoreItemNames.Wanderers_Journal_Sale, LocationNames.Lemm,
                        new CostDef[] { new CostDef("WANDERERSJOURNALS", 1) }));
                }
                for (int i = 0; i < 17; i++)
                {
                    rb.AddToVanilla(new VanillaDef(MoreItemNames.Hallownest_Seal_Sale, LocationNames.Lemm,
                        new CostDef[] { new CostDef("HALLOWNESTSEALS", 1) }));
                }
                for (int i = 0; i < 8; i++)
                {
                    rb.AddToVanilla(new VanillaDef(MoreItemNames.Kings_Idol_Sale, LocationNames.Lemm,
                        new CostDef[] { new CostDef("KINGSIDOLS", 1) }));
                }
                for (int i = 0; i < 4; i++)
                {
                    rb.AddToVanilla(new VanillaDef(MoreItemNames.Arcane_Egg_Sale, LocationNames.Lemm,
                        new CostDef[] { new CostDef("ARCANEEGGS", 1) }));
                }
            }
        }

        private static void ApplyJunkShopSettings(RequestBuilder rb)
        {
            if (!RandoInterop.Enabled || !RandoInterop.Settings.JunkShopSettings.Enabled)
            {
                return;
            }

            rb.AddLocationByName(MoreLocationNames.Junk_Shop);
        }

        private static void HandleJunkItemRemove(RequestBuilder rb)
        {
            // if lemm shop isn't active, we don't care what base rando does. If junk isn't being removed
            // aren't randomized, we don't need to put relics back because they never left
            if (!RandoInterop.Enabled || !RandoInterop.Settings.LemmShopSettings.Enabled 
                || !rb.gs.CursedSettings.ReplaceJunkWithOneGeo)
            {
                return;
            }

            // also if relics aren't randomized they never got removed either
            if (rb.gs.PoolSettings.Relics)
            {
                rb.AddItemByName(ItemNames.Wanderers_Journal, 14);
                rb.AddItemByName(ItemNames.Hallownest_Seal, 17);
                rb.AddItemByName(ItemNames.Kings_Idol, 8);
                rb.AddItemByName(ItemNames.Arcane_Egg, 4);
            }

            rb.RemoveItemByName(MoreItemNames.Wanderers_Journal_Sale);
            rb.RemoveItemByName(MoreItemNames.Hallownest_Seal_Sale);
            rb.RemoveItemByName(MoreItemNames.Kings_Idol_Sale);
            rb.RemoveItemByName(MoreItemNames.Arcane_Egg_Sale);
        }

        private static void DerangeLemmShop(RequestBuilder rb)
        {
            if (!RandoInterop.Enabled || !RandoInterop.Settings.LemmShopSettings.Enabled || !rb.gs.CursedSettings.Deranged)
            {
                return;
            }

            HashSet<string> relicGeoNames = new HashSet<string>()
                {
                    MoreItemNames.Wanderers_Journal_Sale,
                    MoreItemNames.Hallownest_Seal_Sale,
                    MoreItemNames.Kings_Idol_Sale,
                    MoreItemNames.Arcane_Egg_Sale
                };
            foreach (ItemGroupBuilder igb in rb.EnumerateItemGroups())
            {
                if (igb.strategy is DefaultGroupPlacementStrategy dgps)
                {
                    dgps.ConstraintList.Add(new((i, l) => !(relicGeoNames.Contains(i.Name) && l.Name == LocationNames.Lemm)));
                }
            }
        }

        private static void AddRelicCostTolerances(LogicManager lm, GenerationSettings gs, ProgressionInitializer pi)
        {
            if (!RandoInterop.Enabled || !RandoInterop.Settings.LemmShopSettings.Enabled)
            {
                return;
            }

            pi.Setters.Add(new TermValue(lm.GetTermStrict("WANDERERSJOURNALS"),
                -RandoInterop.Settings.LemmShopSettings.CostSettings.JournalTolerance));
            pi.Setters.Add(new TermValue(lm.GetTermStrict("HALLOWNESTSEALS"),
                -RandoInterop.Settings.LemmShopSettings.CostSettings.SealTolerance));
            pi.Setters.Add(new TermValue(lm.GetTermStrict("KINGSIDOLS"),
                -RandoInterop.Settings.LemmShopSettings.CostSettings.IdolTolerance));
            pi.Setters.Add(new TermValue(lm.GetTermStrict("ARCANEEGGS"),
                -RandoInterop.Settings.LemmShopSettings.CostSettings.EggTolerance));
        }
    }
}
