using RandomizerMod.RC;
using System;

namespace MoreLocations.Rando.Costs
{
    internal interface IPostRandomizedCost
    {
        void PostRandomize(Random rng, RandoModItem item);
    }
}
