using RandomizerCore.Logic;
using RandomizerMod.RC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoreLocations.Rando.Costs
{
    public interface IWeighted
    {
        double GetWeight();
    }

    public class WeightedRandomCostChooser : ICostProvider
    {
        private record WeightedItem(Func<double> GetWeight, ICostProvider Item);

        private List<WeightedItem> children = new();

        public bool PreferNonEmptyProviders { get; set; } = true;
        public bool ManagesChildFinalization { get; set; } = false;

        public bool HasNonFreeCostsAvailable => children.Any(x => x.Item.HasNonFreeCostsAvailable);

        public void AddFixedWeight(double weight, ICostProvider costProvider)
        {
            children.Add(new(() => weight, costProvider));
        }

        public void AddDynamicWeight<T>(T provider) where T : ICostProvider, IWeighted
        {
            children.Add(new(provider.GetWeight, provider));
        }

        public LogicCost Next(LogicManager lm, Random rng, RandoModItem item)
        {
            ICostProvider? provider = null;
            if (PreferNonEmptyProviders)
            {
                provider = SelectWeighted(rng, children.Where(x => x.Item.HasNonFreeCostsAvailable));
                if (provider != null)
                {
                    return provider.Next(lm, rng, item);
                }
            }

            provider = SelectWeighted(rng, children);
            if (provider != null)
            {
                return provider.Next(lm, rng, item);
            }

            throw new InvalidOperationException("Cannot select from a chooser with no children");
        }

        private ICostProvider? SelectWeighted(Random rng, IEnumerable<WeightedItem> items)
        {
            double totalWeight = items.Sum(x => x.GetWeight());
            double accumulatedWeight = 0.0;
            foreach (WeightedItem wi in items)
            {
                accumulatedWeight += wi.GetWeight();
                if (rng.NextDouble() * totalWeight <= accumulatedWeight)
                {
                    return wi.Item;
                }
            }
            // this can only happen when there are no entries
            return null;
        }

        public void FinishConstruction(Random rng)
        {
            if (!ManagesChildFinalization)
            {
                return;
            }

            foreach (WeightedItem wi in children)
            {
                wi.Item.FinishConstruction(rng);
            }
        }
    }
}
