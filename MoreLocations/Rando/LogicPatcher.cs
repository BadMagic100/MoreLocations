using ItemChanger;
using RandomizerCore.Json;
using RandomizerCore.Logic;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System.IO;

namespace MoreLocations.Rando
{
    internal static class LogicPatcher
    {
        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(-1000f, DefineTermsAndLocations);
            RCData.RuntimeLogicOverride.Subscribe(15f, PatchLogic);
        }

        private static void DefineTermsAndLocations(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.Enabled)
            {
                return;
            }

            AddLogicObjects(lmb);
        }

        private static void PatchLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.Enabled)
            {
                return;
            }

            OverrideLemmUsages(lmb);
        }

        private static void AddLogicObjects(LogicManagerBuilder lmb)
        {
            JsonLogicFormat fmt = new();

            using Stream t = typeof(LogicPatcher).Assembly.GetManifestResourceStream("MoreLocations.Resources.Logic.terms.json");
            lmb.DeserializeFile(LogicFileType.Terms, fmt, t);

            using Stream i = typeof(LogicPatcher).Assembly.GetManifestResourceStream("MoreLocations.Resources.Logic.items.json");
            lmb.DeserializeFile(LogicFileType.Items, fmt, i);

            using Stream l = typeof(LogicPatcher).Assembly.GetManifestResourceStream("MoreLocations.Resources.Logic.locations.json");
            lmb.DeserializeFile(LogicFileType.Locations, fmt, l);
        }

        private static void OverrideLemmUsages(LogicManagerBuilder lmb)
        {
            // even when lemm shop is not enabled, this should happen - we just need to define vanilla lemm placements in RB instead
            string[] expensiveLocations = [
                LocationNames.Unbreakable_Greed,
                LocationNames.Unbreakable_Heart,
                LocationNames.Unbreakable_Strength,
                LocationNames.Dash_Slash,
                LocationNames.Vessel_Fragment_Basin
            ];
            foreach (string expensiveLocation in expensiveLocations)
            {
                lmb.DoSubst(new RawSubstDef(expensiveLocation, "Can_Visit_Lemm", TermNames.LARGEGEODEPOSIT));
            }
        }
    }
}
