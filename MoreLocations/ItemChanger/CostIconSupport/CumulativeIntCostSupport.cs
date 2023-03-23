using ItemChanger;

namespace MoreLocations.ItemChanger.CostIconSupport
{
    public record CumulativeIntCostSupport(string PdFieldName, string IconName) : IMixedCostSupport
    {
        public CostDisplayer GetDisplayer(Cost c)
        {
            return new PDIntCostDisplayer()
            {
                FieldName = PdFieldName,
                Cumulative = true,
                CustomCostSprite = new ItemChangerSprite(IconName)
            };
        }

        public bool MatchesCost(Cost c)
        {
            return c is PDIntCost pdi && pdi.fieldName == PdFieldName;
        }
    }
}
