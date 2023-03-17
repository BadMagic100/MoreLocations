using RandomizerCore.Extensions;
using RandomizerCore.Logic;
using RandomizerMod.RC;
using System;

namespace MoreLocations.Rando.Costs
{
    public class GeoCostProvider : ICostProvider
    {
        public bool HasNonFreeCostsAvailable => true;

        public LogicCost Next(LogicManager lm, Random rng, RandoModItem item)
        {
            int amount;
            // borrowed logic from RandomizerMod
            const double pow = 1.2;

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

            return new LogicGeoCost(lm, amount);
        }

        public void FinishConstruction() { }
    }
}
