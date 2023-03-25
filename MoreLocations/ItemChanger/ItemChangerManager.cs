using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Locations;
using MoreLocations.ItemChanger.CostIconSupport;

namespace MoreLocations.ItemChanger
{
    internal static class ItemChangerManager
    {
        public static void Hook()
        {
            DefineSwimLocation();
            DefineStagEggLocation();
            DefineBaldurShellChest();
            DefineRelicSaleItems();
            DefineJunkShopLocation();
        }

        private static void DefineSwimLocation()
        {
            AbstractLocation quirrelDrownedLocation = new CoordinateLocation()
            {
                sceneName = SceneNames.Crossroads_50,
                x = 231.0f,
                y = 25.4f,
                elevation = 0,
                flingType = FlingType.Everywhere
            };
            Finder.DefineCustomLocation(new DualLocation()
            {
                name = MoreLocationNames.Swim,
                sceneName = SceneNames.Crossroads_50,
                trueLocation = quirrelDrownedLocation,
                falseLocation = new EmptyLocation(),
                Test = new PDBool(nameof(PlayerData.quirrelEpilogueCompleted)),
                tags = new()
                {
                    InteropTagFactory.CmiLocationTag(poolGroup: "Skills", mapLocations: new[]
                    {
                        (SceneNames.Crossroads_50, 1.7f, -0.1f)
                    })
                }
            });
        }

        private static void DefineStagEggLocation()
        {
            Finder.DefineCustomLocation(new CoordinateLocation()
            {
                name = MoreLocationNames.Stag_Nest_Egg,
                sceneName = SceneNames.Cliffs_03,
                x = 96.3f,
                y = 46.4f,
                elevation = 0,
                flingType = FlingType.Everywhere,
                tags = new()
                {
                    InteropTagFactory.CmiLocationTag(poolGroup: "Lore Tablets", mapLocations: new[]
                    {
                        (SceneNames.Cliffs_02, -2.3f, 1.3f)
                    })
                }
            });
        }

        private static void DefineBaldurShellChest()
        {
            Finder.DefineCustomLocation(new ExistingFsmContainerLocation()
            {
                name = MoreLocationNames.Geo_Chest_Above_Baldur_Shell,
                sceneName = SceneNames.Fungus1_28,
                objectName = "Chest",
                fsmName = "Chest Control",
                containerType = Container.Chest,
                nonreplaceable = true,
                flingType = FlingType.Everywhere,
                tags = new()
                {
                    InteropTagFactory.CmiLocationTag(poolGroup: "Geo Chests", mapLocations: new[]
                    {
                        (SceneNames.Fungus1_28, 0.1f, -0.1f)
                    })
                }
            });
        }

        private static void DefineRelicSaleItems()
        {
            SpawnGeoItem wj = SpawnGeoItem.MakeGeoItem(200);
            wj.name = MoreItemNames.Wanderers_Journal_Sale;
            wj.tags = new()
            {
                InteropTagFactory.CmiSharedTag(poolGroup: "Relic Sales", pinSpriteKey: "Geo Chests")
            };
            SpawnGeoItem hs = SpawnGeoItem.MakeGeoItem(450);
            hs.name = MoreItemNames.Hallownest_Seal_Sale;
            hs.tags = new()
            {
                InteropTagFactory.CmiSharedTag(poolGroup: "Relic Sales", pinSpriteKey: "Geo Chests")
            };
            SpawnGeoItem ki = SpawnGeoItem.MakeGeoItem(800);
            ki.name = MoreItemNames.Kings_Idol_Sale;
            ki.tags = new()
            {
                InteropTagFactory.CmiSharedTag(poolGroup: "Relic Sales", pinSpriteKey: "Geo Chests")
            };
            SpawnGeoItem ae = SpawnGeoItem.MakeGeoItem(1200);
            ae.name = MoreItemNames.Arcane_Egg_Sale;
            ae.tags = new()
            {
                InteropTagFactory.CmiSharedTag(poolGroup: "Relic Sales", pinSpriteKey: "Geo Chests")
            };

            Finder.DefineCustomItem(wj);
            Finder.DefineCustomItem(hs);
            Finder.DefineCustomItem(ki);
            Finder.DefineCustomItem(ae);
        }

        private static void DefineJunkShopLocation()
        {
            CustomShopLocation junkShop = new()
            {
                name = MoreLocationNames.Junk_Shop,
                sceneName = SceneNames.Room_GG_Shortcut,
                objectName = "Fluke Hermit",
                fsmName = "npc_control",
                dungDiscount = false,
                facingDirection = FacingDirection.Left,
                flingType = FlingType.DirectDeposit,
                costDisplayerSelectionStrategy = new MixedCostDisplayerSelectionStrategy()
                {
                    Capabilities =
                    {
                        new RelicCostSupport(),
                        new CumulativeIntCostSupport(nameof(PlayerData.grubsCollected), "ShopIcons.Grub"),
                        new CumulativeIntCostSupport(nameof(PlayerData.dreamOrbs), "ShopIcons.Essence"),
                        new EggCostSupport()
                    }
                },
                tags = new()
                {
                    InteropTagFactory.CmiLocationTag(poolGroup: "Shops", mapLocations: new[]
                    {
                        (SceneNames.GG_Waterways, 0.7f, 0.6f)
                    })
                }
            };
            Finder.DefineCustomLocation(junkShop);
        }
    }
}
