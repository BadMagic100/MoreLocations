using Newtonsoft.Json;
using RandomizerCore.Extensions;
using RandomizerCore.Logic;
using RandomizerMod.RC;
using System;

namespace MoreLocations.Rando.Costs
{
    public class ProvidedGeoCost : LogicGeoCost
    {
        [JsonConstructor]
        public ProvidedGeoCost()
        {
        }

        public ProvidedGeoCost(LogicManager lm) : base(lm, -1)
        {
        }

        public void PostRandomize(Random rng, RandoModItem item)
        {
            if (GeoAmount >= 0)
            {
                return;
            }

            // borrowed logic from RandomizerMod under LGPL v2.1
            // https://github.com/homothetyhk/RandomizerMod/blob/d7f9c3a54b7139e814d620d65ed59085dbe2638d/RandomizerMod/RC/Requests/BuiltinRequests.cs#L520
            const double pow = 1.2;
            int amount;

            int cap = item.ItemDef is not null ? item.ItemDef.PriceCap : 500;
            if (cap <= 100)
            {
                amount = cap;
            }
            else if (item.Required)
            {
                amount = rng.PowerLaw(pow, 100, Math.Min(cap, 500)).ClampToMultipleOf(5);
            }
            else
            {
                amount = rng.PowerLaw(pow, 100, cap).ClampToMultipleOf(5);
            }

            GeoAmount = amount;
        }
    }

    public class GeoCostProvider : ICostProvider
    {
        public bool HasNonFreeCostsAvailable => true;

        public LogicCost Next(LogicManager lm, Random rng)
        {
            return new ProvidedGeoCost(lm);
        }

        public void PreRandomize(Random rng) { }
    }
}
