using ItemChangerTesting;
using MoreLocations.Rando;

namespace MoreLocationsICTesting
{
    public class BaldurShellChestTest : SimpleTest
    {
        public override int Priority => -1;

        public BaldurShellChestTest() : base(MoreLocationNames.Geo_Chest_Above_Baldur_Shell, "left1")
        {
        }
    }
}
