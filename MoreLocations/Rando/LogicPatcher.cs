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
            RCData.RuntimeLogicOverride.Subscribe(15f, ApplyLogic);
        }

        private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.Enabled)
            {
                return;
            }

            AddTermsAndItems(lmb);
            AddLocationLogic(lmb);
            OverrideLemmUsages(lmb);
        }

        private static void AddTermsAndItems(LogicManagerBuilder lmb)
        {
            using Stream t = typeof(LogicPatcher).Assembly.GetManifestResourceStream("MoreLocations.Resources.Logic.terms.json");
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.Terms, t);

            using Stream i = typeof(LogicPatcher).Assembly.GetManifestResourceStream("MoreLocations.Resources.Logic.item.json");
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.Items, i);
        }

        private static void AddLocationLogic(LogicManagerBuilder lmb)
        {
            using Stream l = typeof(LogicPatcher).Assembly.GetManifestResourceStream("MoreLocations.Resource.Logic.locations.json");
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.Locations, l);
        }

        private static void OverrideLemmUsages(LogicManagerBuilder lmb)
        {
            // will cause issues until the correct changes are made RB-side
            //string[] expensiveLocations = new string[] {
            //    LocationNames.Unbreakable_Greed,
            //    LocationNames.Unbreakable_Heart,
            //    LocationNames.Unbreakable_Strength,
            //    LocationNames.Dash_Slash,
            //    LocationNames.Vessel_Fragment_Basin
            //};
            //foreach (string expensiveLocation in expensiveLocations)
            //{
            //    lmb.DoSubst(new RawSubstDef(expensiveLocation, "Can_Visit_Lemm", TermNames.LARGEGEODEPOSIT));
            //}
        }
    }
}
