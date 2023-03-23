using MoreLocations.ItemChanger.CostIconSupport;
using MoreLocations.Rando.Costs;
using System;
using System.Collections.Generic;

namespace MoreLocations.Rando
{
    public static class ConnectionInterop
    {
        internal static readonly List<(Func<bool> include, Func<ICostProvider> providerFactory)> costProviders = new();
        internal static readonly List<IMixedCostSupport> costSupportCapabilities = new();

        /// <summary>
        /// Adds a cost provider that is conditionally included in the junk shop root provider
        /// </summary>
        /// <param name="shouldIncludeProviderInRequest">
        ///     Determines at request-time whether to include your provider. 
        ///     Usually checking your connection settings.
        /// </param>
        /// <param name="costProviderFactory">
        ///     A method to construct the provider to add if the condition is true (note that cost providers
        ///     are generally stateful and should be reconstructed each request).
        /// </param>
        /// <seealso cref="SimpleCostProvider"/>
        /// <seealso cref="CappedIntCostProvider"/>
        public static void AddRandoCostProviderToJunkShop(Func<bool> shouldIncludeProviderInRequest,
            Func<ICostProvider> costProviderFactory)
        {
            costProviders.Add((shouldIncludeProviderInRequest, costProviderFactory));
        }

        /// <summary>
        /// Adds IC CostDisplayer support for your custom costs in junk shop
        /// </summary>
        /// <param name="costSupport">
        ///     The cost support to add. See the classes in MoreLocations.ItemChanger.CostIconSupport
        ///     for example implementations and usable classes.
        /// </param>
        /// <seealso cref="CumulativeIntCostSupport"/>
        public static void AddIcCostDisplayerSupportToJunkShop(IMixedCostSupport costSupport)
        {
            costSupportCapabilities.Add(costSupport);
        }
    }
}
