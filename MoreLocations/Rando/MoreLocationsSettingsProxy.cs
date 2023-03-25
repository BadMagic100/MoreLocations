using MoreLocations.Rando.Settings;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;
using RandoSettingsManager.SettingsManagement.Versioning.Comparators;
using System.IO;
using System.Reflection;

namespace MoreLocations.Rando
{
    internal static class SettingsManagement
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new MoreLocationsSettingsProxy());
        }
    }

    internal class MoreLocationsSettingsProxy : RandoSettingsProxy<RandomizerSettings, (string, string, string, string)>
    {
        public override string ModKey => "MoreLocations";

        public override VersioningPolicy<(string, string, string, string)> VersioningPolicy { get; }

        public MoreLocationsSettingsProxy()
        {
            Assembly a = typeof(MoreLocationsMod).Assembly;
            using Stream items = a.GetManifestResourceStream("MoreLocations.Resources.Logic.items.json");
            using Stream locations = a.GetManifestResourceStream("MoreLocations.Resources.Logic.locations.json");
            using Stream terms = a.GetManifestResourceStream("MoreLocations.Resources.Logic.terms.json");

            VersioningPolicy = CompoundVersioningPolicy.Of(
                new EqualityVersioningPolicy<string>(MoreLocationsMod.Instance.GetVersion(), new SemVerComparator(places: 2)),
                new ContentHashVersioningPolicy(items),
                new ContentHashVersioningPolicy(locations),
                new ContentHashVersioningPolicy(terms)
            );
        }

        public override void ReceiveSettings(RandomizerSettings? settings)
        {
            if (settings != null)
            {
                ConnectionMenu.Instance!.Apply(settings);
            }
            else
            {
                ConnectionMenu.Instance!.Disable();
            }
        }

        public override bool TryProvideSettings(out RandomizerSettings? settings)
        {
            settings = RandoInterop.Settings;
            return settings.Enabled;
        }
    }
}
