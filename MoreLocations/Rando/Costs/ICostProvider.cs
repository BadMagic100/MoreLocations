using RandomizerCore.Logic;
using RandomizerMod.RC;
using System;

namespace MoreLocations.Rando.Costs
{
    public interface ICostProvider
    {
        bool HasNonFreeCostsAvailable { get; }

        LogicCost Next(LogicManager lm, Random rng, RandoModItem item);

        void FinishConstruction(Random rng);
    }
}
