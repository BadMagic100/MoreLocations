using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Locations;

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
                Test = new PDBool(nameof(PlayerData.quirrelEpilogueCompleted))
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
                flingType = FlingType.Everywhere
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
                flingType = FlingType.Everywhere
            });
        }

        private static void DefineRelicSaleItems()
        {
            SpawnGeoItem wj = SpawnGeoItem.MakeGeoItem(200);
            wj.name = MoreItemNames.Wanderers_Journal_Sale;
            SpawnGeoItem hs = SpawnGeoItem.MakeGeoItem(450);
            hs.name = MoreItemNames.Hallownest_Seal_Sale;
            SpawnGeoItem ki = SpawnGeoItem.MakeGeoItem(800);
            ki.name = MoreItemNames.Kings_Idol_Sale;
            SpawnGeoItem ae = SpawnGeoItem.MakeGeoItem(1200);
            ae.name = MoreItemNames.Arcane_Egg_Sale;

            Finder.DefineCustomItem(wj);
            Finder.DefineCustomItem(hs);
            Finder.DefineCustomItem(ki);
            Finder.DefineCustomItem(ae);
        }
    }
}
