using ItemChanger;
using ItemChanger.Internal;

namespace MoreLocations.ItemChanger
{
    public class MoreLocationsSprite : EmbeddedSprite
    {
        private static SpriteManager spriteManager = new(typeof(MoreLocationsSprite).Assembly, "MoreLocations.Resources.Images.",
            new SpriteManager.Info()
            {
                defaultFilterMode = UnityEngine.FilterMode.Bilinear,
                defaultPixelsPerUnit = 100f,
                overridePPUs = new()
                {
                    ["FlukeShopFigurehead"] = 64f
                }
            });

        public override SpriteManager SpriteManager => spriteManager;
    }
}
