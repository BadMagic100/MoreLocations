using MoreLocations.Rando.Settings.Presets;

namespace MoreLocations.Rando.Settings
{
    public class LemmShopSettings
    {
        public bool Enabled = true;
        public RelicGeoSettings GeoSettings = RelicGeoPresets.Reduced;
        public RelicCostSettings CostSettings = RelicCostPresets.Standard;
    }
}
