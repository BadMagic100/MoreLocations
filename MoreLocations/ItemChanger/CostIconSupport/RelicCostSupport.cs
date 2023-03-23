using ItemChanger;
using System.Collections.Generic;

namespace MoreLocations.ItemChanger.CostIconSupport
{
    public class RelicCostSupport : IMixedCostSupport
    {
        private static readonly Dictionary<string, string> fieldSpriteLookup = new()
        {
            [nameof(PlayerData.trinket1)] = "WanderersJournal",
            [nameof(PlayerData.trinket2)] = "HallownestSeal",
            [nameof(PlayerData.trinket3)] = "KingsIdol",
            [nameof(PlayerData.trinket4)] = "ArcaneEgg",
        };

        public CostDisplayer GetDisplayer(Cost c)
        {
            ConsumablePDIntCost cpdi = (ConsumablePDIntCost)c;
            string field = cpdi.fieldName;
            return new PDIntCostDisplayer()
            {
                FieldName = field,
                Cumulative = false,
                CustomCostSprite = new ItemChangerSprite($"ShopIcons.{fieldSpriteLookup[field]}"),
            };
        }

        public bool MatchesCost(Cost c)
        {
            return c is ConsumablePDIntCost cpdi && fieldSpriteLookup.ContainsKey(cpdi.fieldName);
        }
    }
}
