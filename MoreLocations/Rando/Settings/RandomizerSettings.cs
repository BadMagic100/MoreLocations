namespace MoreLocations.Rando.Settings
{
    public class RandomizerSettings
    {
        public bool Enabled { get; set; } = false;

        public LemmShopSettings LemmShopSettings { get; set; } = new();
    }
}
