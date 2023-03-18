using MoreLocations.Rando.Costs;
using RandomizerCore.Logic;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace MoreLocations.Rando
{
    internal static class RequestModifier
    {
        private static ICostProvider? relicCostProvider;
        private static ICostProvider? anyCostProvider;

        public static void Hook()
        {
            ProgressionInitializer.OnCreateProgressionInitializer += AddRelicCostTolerances;

            RequestBuilder.OnUpdate.Subscribe(float.NegativeInfinity, _ => SetupCostManagement());
            RequestBuilder.OnUpdate.Subscribe(float.PositiveInfinity, _ => TearDownCostManagement());
        }

        private static void SetupCostManagement()
        {

        }

        private static void TearDownCostManagement()
        {

        }

        private static void AddRelicCostTolerances(LogicManager lm, GenerationSettings gs, ProgressionInitializer pi)
        {

        }
    }
}
