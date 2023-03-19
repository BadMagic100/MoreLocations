using MenuChanger.Attributes;

namespace MoreLocations.Rando.Settings
{
    public class RelicGeoSettings
    {
        [MenuRange(0, 14)]
        public int JournalSales;
        [MenuRange(0, 17)]
        public int SealSales;
        [MenuRange(0, 8)]
        public int IdolSales;
        [MenuRange(0, 4)]
        public int EggSales;
    }
}
