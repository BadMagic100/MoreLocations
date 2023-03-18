using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using RandomizerMod;
using RandomizerMod.Menu;

namespace MoreLocations.Rando
{
    internal class ConnectionMenu
    {
        internal static ConnectionMenu? Instance { get; private set; }

        private SmallButton rootButton;
        private MenuPage rootPage;
        private MenuElementFactory<RandomizerSettings> rootMef;
        private ToggleButton enableToggle;

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += () => Instance = null;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button)
        {
            button = Instance!.rootButton;
            return true;
        }

        private static void ConstructMenu(MenuPage connectionPage)
        {
            Instance = new ConnectionMenu(connectionPage);
        }

        public ConnectionMenu(MenuPage connectionPage)
        {
            rootPage = new MenuPage(Localization.Localize("More Locations"), connectionPage);

            rootMef = new MenuElementFactory<RandomizerSettings>(rootPage, RandoInterop.Settings);
            enableToggle = (ToggleButton) rootMef.ElementLookup[nameof(RandomizerSettings.Enabled)];

            rootButton = new(connectionPage, Localization.Localize("More Locations"));
            rootButton.AddHideAndShowEvent(connectionPage, rootPage);
            connectionPage.BeforeShow += SetTopLevelButtonColor;
        }

        private void SetTopLevelButtonColor()
        {
            rootButton.Text.color = RandoInterop.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
        }
    }
}
