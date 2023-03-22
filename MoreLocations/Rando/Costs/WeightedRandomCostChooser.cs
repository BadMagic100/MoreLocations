using RandomizerCore.Logic;
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
        private HashSet<WeightedItem> selected = new();

        public bool PreferNonEmptyProviders { get; set; } = true;
        public bool ManagesChildLifecycle { get; set; } = false;

        public bool HasNonFreeCostsAvailable => children.Any(x => x.Item.HasNonFreeCostsAvailable);

        public void AddFixedWeight(double weight, ICostProvider costProvider)
        {
            children.Add(new(() => weight, costProvider));
        }

        public void AddDynamicWeight<T>(T provider) where T : ICostProvider, IWeighted
        {
            children.Add(new(provider.GetWeight, provider));
        }

        public LogicCost Next(LogicManager lm, Random rng)
        {
            LogicCost lc = SelectWithoutReplacement(lm, rng);
            ResetAvailablePool();
            return lc;
        }

        public LogicCost SelectWithoutReplacement(LogicManager lm, Random rng)
        {
            ICostProvider? provider = null;
            if (PreferNonEmptyProviders)
            {
                provider = SelectWeighted(rng, children.Where(x => x.Item.HasNonFreeCostsAvailable));
                if (provider is WeightedRandomCostChooser wc)
                {
                    return wc.SelectWithoutReplacement(lm, rng);
                }
                else if (provider != null)
                {
                    return provider.Next(lm, rng);
                }
            }

            provider = SelectWeighted(rng, children);
            if (provider is WeightedRandomCostChooser wcc)
            {
                return wcc.SelectWithoutReplacement(lm, rng);
            }
            else if (provider != null)
            {
                return provider.Next(lm, rng);
            }
            throw new InvalidOperationException("Cannot select from a chooser with no unselected children");
        }

        public void ResetAvailablePool()
        {
            selected.Clear();
            foreach (WeightedItem wi in children)
            {
                if (wi.Item is WeightedRandomCostChooser wcc)
                {
                    wcc.ResetAvailablePool();
                }
            }
        }

        private ICostProvider? SelectWeighted(Random rng, IEnumerable<WeightedItem> items)
        {
            items = items.Where(x => !selected.Contains(x));
            double totalWeight = items.Sum(x => x.GetWeight());
            double accumulatedWeight = 0.0;
            foreach (WeightedItem wi in items)
            {
                accumulatedWeight += wi.GetWeight();
                if (rng.NextDouble() * totalWeight <= accumulatedWeight)
                {
                    selected.Add(wi);
                    return wi.Item;
                }
            }
            // this can only happen when there are no entries
            return null;
        }

        public void PreRandomize(Random rng)
        {
            if (!ManagesChildLifecycle)
            {
                return;
            }

            foreach (WeightedItem wi in children)
            {
                wi.Item.PreRandomize(rng);
            }
        }
    }
}
