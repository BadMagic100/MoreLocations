using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MoreLocations.Rando.Settings;
using MoreLocations.Rando.Settings.Presets;
using RandomizerMod;
using RandomizerMod.Menu;
using UnityEngine;

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

            VerticalItemPanel vip = new(rootPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_SMALL, true, enableToggle);

            SmallButton jumpToLemmPage = new(rootPage, "Lemm Shop");
            jumpToLemmPage.AddHideAndShowEvent(CreateLemmPage());
            vip.Add(jumpToLemmPage);

            vip.ResetNavigation();

            rootButton = new(connectionPage, Localization.Localize("More Locations"));
            rootButton.AddHideAndShowEvent(connectionPage, rootPage);
            connectionPage.BeforeShow += SetTopLevelButtonColor;
        }

        private MenuPage CreateLemmPage()
        {
            MenuPage lemmPage = new("Lemm Shop", rootPage);

            // todo - enabled toggle

            MenuElementFactory<RelicGeoSettings> geoSettingsMef = new(lemmPage, RandoInterop.Settings.LemmShopSettings.GeoSettings);
            MenuPreset<RelicGeoSettings> geoSettingsPreset = new(lemmPage, "Relic Geo Items",
                RelicGeoPresets.Presets, RandoInterop.Settings.LemmShopSettings.GeoSettings,
                _ => "",
                geoSettingsMef);
            GridItemPanel geoSettingsHorizontalGrid = new(lemmPage, Vector2.zero, 4, 
                0f, SpaceParameters.HSPACE_SMALL, false, 
                geoSettingsMef.Elements);

            MenuElementFactory<RelicCostSettings> costSettingsMef = new(lemmPage, RandoInterop.Settings.LemmShopSettings.CostSettings);
            MenuPreset<RelicCostSettings> costSettingsPreset = new(lemmPage, "Relic Costs",
                RelicCostPresets.Presets, RandoInterop.Settings.LemmShopSettings.CostSettings,
                _ => "",
                costSettingsMef);
            GridItemPanel costSettingsHorizontalGrid = new(lemmPage, Vector2.zero, 3,
                SpaceParameters.VSPACE_MEDIUM, SpaceParameters.HSPACE_SMALL, false,
                costSettingsMef.Elements);

            VerticalItemPanel vip = new(lemmPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_MEDIUM, true,
                geoSettingsPreset, 
                geoSettingsHorizontalGrid,
                costSettingsPreset,
                costSettingsHorizontalGrid);

            return lemmPage;
        }

        private void SetTopLevelButtonColor()
        {
            rootButton.Text.color = RandoInterop.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
        }
    }
}
