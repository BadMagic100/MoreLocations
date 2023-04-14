using ItemChanger;
using RandomizerCore.Logic;
using System;
using System.Collections.Generic;

namespace MoreLocations.Rando.Costs
{
    public class CappedIntCostProvider : ICostProvider, IWeighted
    {
        private Func<int, Cost> converter;

        private readonly string term;
        private readonly int preferredMinCost;
        private readonly int initialCapacity;
        private int capacity;

        private List<CappedIntCost> createdCosts = new();

        double IWeighted.GetWeight()
        {
            // don't divide by 0
            if (preferredMinCost == 0)
            {
                return 1.0;
            }
            // uniformly weight when empty, otherwise weight according to how many costs of the preferred size are available
            return Math.Max(1.0, (double)capacity / preferredMinCost);
        }

        public CappedIntCostProvider(string term, int preferredMinCost, int initialCapacity, Func<int, Cost> converter)
        {
            this.term = term;
            this.preferredMinCost = preferredMinCost;
            this.initialCapacity = initialCapacity;
            this.capacity = initialCapacity;

            this.converter = converter;
        }

        public bool HasNonFreeCostsAvailable => capacity > 0;

        public LogicCost Next(LogicManager lm, Random rng)
        {
            int amount = Math.Min(preferredMinCost, capacity);
            capacity -= amount;
            CappedIntCost c = new(lm.GetTermStrict(term), amount, initialCapacity, converter);
            createdCosts.Add(c);
            return c;
        }

        public void PreRandomize(Random rng)
        {
            // only consume rng and do work if there's actually costs that need it (prevent unintended side effects)
            if (createdCosts.Count == 0)
            {
                return;
            }

            // pick a random amount of the remaining capacity (max total value) to distribute back down to the generated costs
            int capacityToDistribute = rng.Next(capacity + 1);
            capacity -= capacityToDistribute;
            int finalConsumedCapacity = initialCapacity - capacity;

            // using the approach outlined here: https://stackoverflow.com/a/48205426/9202129
            // we are generating N values which sum to capacityToDistribute by generating N - 1 sorted "sectioning points"
            // and using the generated interval widths to sum up to capacity
            int[] sectioningPoints = new int[createdCosts.Count + 1];
            sectioningPoints[0] = 0;
            sectioningPoints[createdCosts.Count] = capacityToDistribute;
            // small optimization - no need to randomize if we're not giving out any capacity
            if (capacityToDistribute > 0)
            {
                for (int i = 0; i < createdCosts.Count; i++)
                {
                    sectioningPoints[i] = rng.Next(capacityToDistribute + 1);
                }
            }
            Array.Sort(sectioningPoints);
            for (int i = 0; i < createdCosts.Count; i++)
            {
                int additionalCapacity = sectioningPoints[i + 1] - sectioningPoints[i];
                createdCosts[i].FinishConstruction(finalConsumedCapacity, additionalCapacity);
            }
        }
    }
}
