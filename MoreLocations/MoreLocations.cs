using Modding;
using MoreLocations.ItemChanger;
using MoreLocations.Rando;
using System;

namespace MoreLocations
{
    public class MoreLocationsMod : Mod, IGlobalSettings<GlobalSettings>
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

        internal GlobalSettings GS { get; set; } = new();

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public MoreLocationsMod() : base("MoreLocations")
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            ItemChangerManager.Hook();

            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                RandoInterop.Hook();
            }

            Log("Initialized");
        }

        public void OnLoadGlobal(GlobalSettings s) => GS = s;

        public GlobalSettings OnSaveGlobal() => GS;
    }
}
