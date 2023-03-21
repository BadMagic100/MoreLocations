namespace MoreLocations.Rando.Settings
{
    public class RandomizerSettings
    {
        public bool Enabled { get; set; } = false;

        public MiscLocationSettings MiscLocationSettings { get; set; } = new();

        public LemmShopSettings LemmShopSettings { get; set; } = new();

        public JunkShopSettings JunkShopSettings { get; set; } = new();
    }
}
