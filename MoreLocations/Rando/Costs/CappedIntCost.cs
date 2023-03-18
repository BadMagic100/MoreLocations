using ItemChanger;
using Newtonsoft.Json;
using RandomizerCore.Logic;
using System;
using System.Collections.Generic;

namespace MoreLocations.Rando.Costs
{
    public class CappedIntCost : LogicCost
    {
        [JsonIgnore]
        private Func<int, Cost>? converter;

        public Term? term;
        public int logicCost;
        public int realCost;
        
        [JsonConstructor]
        public CappedIntCost() { }

        public CappedIntCost(Term term, int initialAmount, int initialCapacity, Func<int, Cost> icConverter)
        {
            this.term = term;
            this.realCost = initialAmount;
            this.logicCost = initialCapacity;
            this.converter = icConverter;
        }


        public override bool CanGet(ProgressionManager pm) => pm.Has(term!, logicCost);

        public override IEnumerable<Term> GetTerms()
        {
            if (term == null)
            {
                throw new InvalidOperationException("Term is undefined");
            }
            yield return term;
        }

        public void FinishConstruction(int finalCapacity, int additionalAmount)
        {
            logicCost = finalCapacity;
            realCost += additionalAmount;

            if (realCost == 0)
            {
                logicCost = 0;
            }
        }

        public Cost GetIcCost()
        {
            return converter?.Invoke(realCost) ?? throw new InvalidOperationException("Cost converter is undefined");
        }

        public override string ToString()
        {
            if (realCost == logicCost)
            {
                return $"{{{term?.Name} >= {logicCost}}}";
            }
            return $"{{Actual: {term?.Name} >= {realCost}, Worst-case: {term?.Name} >= {logicCost}}}";
        }
    }
}
