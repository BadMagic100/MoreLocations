using ItemChanger;
using ItemChanger.Modules;

namespace MoreLocations.ItemChanger.CostIconSupport
{
    public class EggCostSupport : IMixedCostSupport
    {
        public CostDisplayer GetDisplayer(Cost c)
        {
            return new EggCostDisplayer();
        }

        public bool MatchesCost(Cost c)
        {
            return c is CumulativeRancidEggCost;
        }
    }
}
