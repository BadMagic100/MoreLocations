using ItemChanger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreLocations.ItemChanger
{
    public enum RelicType
    {
        Journal = 1,
        Seal = 2,
        Idol = 3,
        Egg = 4
    }

    public static class RelicTypeExtensions
    {
        public static string Name(this RelicType type) => type switch
        {
            RelicType.Journal => "Wanderer's Journal",
            RelicType.Seal => "Hallownest Seal",
            RelicType.Idol => "King's Idol",
            RelicType.Egg => "Arcane Egg",
            _ => throw new NotImplementedException()
        };

        public static string IntName(this RelicType type) => $"trinket{(int)type}";
        public static string SoldIntName(this RelicType type) => $"soldTrinket{(int)type}";
    }

    public record RelicCost : Cost
    {
        public readonly RelicType Type;
        public readonly ConsumablePDIntCost baseCost;

        public RelicCost(int amount, RelicType type)
        {
            this.Type = type;
            baseCost = new ConsumablePDIntCost(amount, type.IntName(), "");
        }

        [JsonConstructor]
        private RelicCost(ConsumablePDIntCost baseCost, RelicType type)
        {
            this.Type = type;
            this.baseCost = baseCost;
        }

        public override Cost GetBaseCost() => baseCost;

        public override bool HasPayEffects() => true;
        public override bool CanPay() => baseCost.CanPay();

        public override string GetCostText()
        {
            int amount = baseCost.amount;
            string suffix = amount == 1 ? "" : "s";
            return $"Trade {amount} {Type.Name()}{suffix}";
        }

        public override void OnPay()
        {
            baseCost.Pay();
            PlayerData.instance.IntAdd(Type.SoldIntName(), baseCost.amount);
        }
    }
}
