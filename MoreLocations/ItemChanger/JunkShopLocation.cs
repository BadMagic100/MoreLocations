using ItemChanger.Locations;
using Modding;

namespace MoreLocations.ItemChanger
{
    public class JunkShopLocation : CustomShopLocation
    {
        protected override void OnLoad()
        {
            base.OnLoad();
            ModHooks.GetPlayerBoolHook += KeepFlukeHermitHome;
        }

        protected override void OnUnload()
        {
            ModHooks.GetPlayerBoolHook -= KeepFlukeHermitHome;
            base.OnUnload();
        }

        private bool KeepFlukeHermitHome(string name, bool orig)
        {
            if (name == nameof(PlayerData.scaredFlukeHermitReturned))
            {
                return true;
            }
            return orig;
        }
    }
}
