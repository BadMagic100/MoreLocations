using ItemChanger;
using ItemChanger.Locations;
using MoreLocations.Rando;

namespace MoreLocations.ItemChanger
{
    internal static class ItemChangerManager
    {
        public static void Hook()
        {
            DefineSwimLocation();
            DefineStagEggLocation();
            DefineBaldurShellChest();
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
                falseLocation = new NullLocation() { sceneName = SceneNames.Crossroads_50 },
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
    }
}
