using MenuChanger.Attributes;
using MoreLocations.Rando.Settings.Presets;

namespace MoreLocations.Rando.Settings
{
    public class LemmShopSettings
    {
        public bool Enabled = true;
        [MenuRange(-1, 99)]
        public int RelicGeoGroup = -1;
        public RelicGeoSettings GeoSettings = RelicGeoPresets.Reduced;
        public RelicCostSettings CostSettings = RelicCostPresets.Standard;
    }
}
