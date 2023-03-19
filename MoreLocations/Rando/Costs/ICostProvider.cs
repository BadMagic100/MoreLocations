using RandomizerCore.Logic;
using System;

namespace MoreLocations.Rando.Costs
{
    public interface ICostProvider
    {
        bool HasNonFreeCostsAvailable { get; }

        LogicCost Next(LogicManager lm, Random rng);

        void FinishConstruction(Random rng);
    }
}
