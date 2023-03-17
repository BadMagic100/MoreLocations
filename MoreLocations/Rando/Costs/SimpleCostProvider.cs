using RandomizerCore.Logic;
using RandomizerMod.RC;
using System;

namespace MoreLocations.Rando.Costs
{
    public class SimpleCostProvider : ICostProvider
    {
        private readonly string term;
        private readonly int min, max;
        public SimpleCostProvider(string term, int min, int max)
        {
            this.term = term;
            this.min = min;
            this.max = max;
        }

        public bool HasNonFreeCostsAvailable => true;

        public LogicCost Next(LogicManager lm, Random rng, RandoModItem item)
        {
            return new SimpleCost(lm.GetTermStrict(term), rng.Next(min, max + 1));
        }

        public void FinishConstruction(Random rng) { }
    }
}
