using Modding;
using System;

namespace MoreLocations
{
    public class MoreLocationsMod : Mod
    {
        private static MoreLocationsMod? _instance;

        internal static MoreLocationsMod Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(MoreLocationsMod)} was never constructed");
                }
                return _instance;
            }
        }

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public MoreLocationsMod() : base("MoreLocations")
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            // put additional initialization logic here

            Log("Initialized");
        }
    }
}
