using ItemChanger.Tags;

namespace MoreLocations.ItemChanger
{
    public static class InteropTagFactory
    {
        private static void SetProperty(this InteropTag t, string prop, object? value)
        {
            if (value != null)
            {
                t.Properties[prop] = value;
            }
        }

        private const string CmiModSourceProperty = "ModSource";
        private const string CmiPoolGroupProperty = "PoolGroup";
        private const string CmiPinSpriteKeyProperty = "PinSpriteKey";
        private const string CmiMapLocationsProperty = "MapLocations";

        public static InteropTag CmiSharedTag(string? poolGroup = null, string? pinSpriteKey = null)
        {
            InteropTag t = new()
            {
                Message = "RandoSupplementalMetadata",
                Properties =
                {
                    [CmiModSourceProperty] = "MoreLocations"
                }
            };
            t.SetProperty(CmiPoolGroupProperty, poolGroup);
            t.SetProperty(CmiPinSpriteKeyProperty, pinSpriteKey);
            return t;
        }

        public static InteropTag CmiLocationTag(string? poolGroup = null, string? pinSpriteKey = null,
            (string, float, float)[]? mapLocations = null)
        {
            InteropTag t = CmiSharedTag(poolGroup: poolGroup, pinSpriteKey: pinSpriteKey);
            t.SetProperty(CmiMapLocationsProperty, mapLocations);
            return t;
        }
    }
}
