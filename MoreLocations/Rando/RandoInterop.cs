using Modding;
using MoreLocations.Rando.Settings;
using Newtonsoft.Json;
using RandomizerMod.Logging;

namespace MoreLocations.Rando
{
    internal static class RandoInterop
    {
        public static bool Enabled => MoreLocationsMod.Instance.GS.RS.Enabled;
        public static RandomizerSettings Settings => MoreLocationsMod.Instance.GS.RS;

        public static void Hook()
        {
            ConnectionMenu.Hook();
            LogicPatcher.Hook();
            RequestModifier.Hook();

            if (ModHooks.GetMod("RandoSettingsManager") is Mod)
            {
                SettingsManagement.Hook();
            }

            SettingsLog.AfterLogSettings += AddMoreLocationsSettings;
        }

        private static void AddMoreLocationsSettings(LogArguments args, System.IO.TextWriter tw)
        {
            tw.WriteLine("MoreLocations Settings:");
            using JsonTextWriter jtw = new(tw) { CloseOutput = false };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, Settings);
            tw.WriteLine();
        }
    }
}
