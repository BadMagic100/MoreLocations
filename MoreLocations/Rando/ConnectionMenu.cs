using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MoreLocations.Rando.Settings;
using MoreLocations.Rando.Settings.Presets;
using RandomizerMod;
using RandomizerMod.Menu;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        // misc page
        private SmallButton jumpToMiscPage;
        private MenuElementFactory<MiscLocationSettings> miscLocationMef;

        // lemm page
        private SmallButton jumpToLemmPage;
        private MenuElementFactory<LemmShopSettings> lemmShopRootMef;
        private MenuElementFactory<RelicGeoSettings> relicGeoSettingsMef;
        private MenuElementFactory<RelicCostSettings> relicCostSettingsMef;

        // junk shop page
        private SmallButton jumpToJunkShopPage;
        private MenuElementFactory<JunkShopSettings> junkShopRootMef;
        private MenuElementFactory<JunkCostSettings> junkCostSettingsMef;

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
            rootPage = new MenuPage("MoreLocations", connectionPage);

            MenuLabel header = new(rootPage, "MoreLocations");
            header.MoveTo(SpaceParameters.TOP_CENTER);

            rootMef = new MenuElementFactory<RandomizerSettings>(rootPage, RandoInterop.Settings);
            enableToggle = (ToggleButton) rootMef.ElementLookup[nameof(RandomizerSettings.Enabled)];

            jumpToMiscPage = new(rootPage, "Miscellaneous Locations");
            jumpToMiscPage.AddHideAndShowEvent(CreateMiscPage());

            jumpToLemmPage = new(rootPage, "Lemm Shop");
            jumpToLemmPage.AddHideAndShowEvent(CreateLemmPage());

            jumpToJunkShopPage = new(rootPage, "Junk Shop");
            jumpToJunkShopPage.AddHideAndShowEvent(CreateJunkShopPage());

            VerticalItemPanel vip = new(rootPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_SMALL, true, 
                enableToggle,
                jumpToMiscPage,
                jumpToLemmPage,
                jumpToJunkShopPage);

            rootButton = new(connectionPage, Localization.Localize("MoreLocations"));
            rootButton.AddHideAndShowEvent(connectionPage, rootPage);
            BindTopLevelButtonColor(rootButton, () => RandoInterop.Settings.Enabled);

            Localization.Localize(header);
            Localization.Localize(rootMef);
            Localization.Localize(enableToggle);
            Localization.Localize(jumpToMiscPage);
            Localization.Localize(jumpToLemmPage);
            Localization.Localize(jumpToJunkShopPage);
            Localization.Localize(rootButton);
        }

        [MemberNotNull(nameof(miscLocationMef))]
        private MenuPage CreateMiscPage()
        {
            MenuPage miscPage = new("Miscellaneous Locations", rootPage);

            MenuLabel header = new(miscPage, "MoreLocations - Misc. Locations");
            header.MoveTo(SpaceParameters.TOP_CENTER);

            miscLocationMef = new(miscPage, RandoInterop.Settings.MiscLocationSettings);

            VerticalItemPanel vip = new(miscPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_SMALL,
                true, miscLocationMef.Elements);

            BindTopLevelButtonColor(jumpToMiscPage, () => RandoInterop.Settings.MiscLocationSettings.Any);

            Localization.Localize(header);
            Localization.Localize(miscLocationMef);

            return miscPage;
        }

        [MemberNotNull(nameof(lemmShopRootMef), nameof(relicGeoSettingsMef), nameof(relicCostSettingsMef))]
        private MenuPage CreateLemmPage()
        {
            MenuPage lemmPage = new("Lemm Shop", rootPage);

            MenuLabel header = new(lemmPage, "MoreLocations - Lemm");
            header.MoveTo(SpaceParameters.TOP_CENTER);

            lemmShopRootMef = new(lemmPage, RandoInterop.Settings.LemmShopSettings);

            relicGeoSettingsMef = new(lemmPage, RandoInterop.Settings.LemmShopSettings.GeoSettings);
            MenuPreset<RelicGeoSettings> geoSettingsPreset = new(lemmPage, "Relic Geo Items",
                RelicGeoPresets.Presets, RandoInterop.Settings.LemmShopSettings.GeoSettings,
                _ => "",
                relicGeoSettingsMef);
            GridItemPanel geoSettingsHorizontalGrid = new(lemmPage, Vector2.zero, 4, 
                0f, SpaceParameters.HSPACE_SMALL, false, 
                relicGeoSettingsMef.Elements);

            relicCostSettingsMef = new(lemmPage, RandoInterop.Settings.LemmShopSettings.CostSettings);
            MenuPreset<RelicCostSettings> costSettingsPreset = new(lemmPage, "Relic Costs",
                RelicCostPresets.Presets, RandoInterop.Settings.LemmShopSettings.CostSettings,
                _ => "",
                relicCostSettingsMef);
            GridItemPanel costSettingsHorizontalGrid = new(lemmPage, Vector2.zero, 3,
                SpaceParameters.VSPACE_MEDIUM, SpaceParameters.HSPACE_SMALL, false,
                relicCostSettingsMef.Elements);

            IMenuElement[] vipElements = lemmShopRootMef.Elements.OfType<IMenuElement>()
                .Append(geoSettingsPreset)
                .Append(geoSettingsHorizontalGrid)
                .Append(costSettingsPreset)
                .Append(costSettingsHorizontalGrid)
                .ToArray();

            VerticalItemPanel vip = new(lemmPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_MEDIUM,
                true, vipElements);

            BindTopLevelButtonColor(jumpToLemmPage, () => RandoInterop.Settings.LemmShopSettings.Enabled);

            Localization.Localize(header);
            Localization.Localize(lemmShopRootMef);
            Localization.Localize(relicGeoSettingsMef);
            Localization.Localize(geoSettingsPreset);
            Localization.Localize(relicCostSettingsMef);
            Localization.Localize(costSettingsPreset);

            return lemmPage;
        }

        [MemberNotNull(nameof(junkShopRootMef), nameof(junkCostSettingsMef))]
        private MenuPage CreateJunkShopPage()
        {
            MenuPage junkShopPage = new("Junk Shop", rootPage);

            MenuLabel header = new(junkShopPage, "MoreLocations - Junk Shop");
            header.MoveTo(SpaceParameters.TOP_CENTER);

            junkShopRootMef = new(junkShopPage, RandoInterop.Settings.JunkShopSettings);

            junkCostSettingsMef = new(junkShopPage, RandoInterop.Settings.JunkShopSettings.CostSettings);
            MenuPreset<JunkCostSettings> costSettingsPreset = new(junkShopPage, "Junk Costs",
                JunkShopCostPresets.Presets, RandoInterop.Settings.JunkShopSettings.CostSettings,
                _ => "",
                junkCostSettingsMef);
            GridItemPanel costSettingsHorizontalGrid = new(junkShopPage, Vector2.zero, 2,
                SpaceParameters.VSPACE_MEDIUM, SpaceParameters.HSPACE_SMALL, false,
                junkCostSettingsMef.Elements);

            IMenuElement[] vipElements = junkShopRootMef.Elements.OfType<IMenuElement>()
                .Append(costSettingsPreset)
                .Append(costSettingsHorizontalGrid)
                .ToArray();

            VerticalItemPanel vip = new(junkShopPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_MEDIUM,
                true, vipElements);

            BindTopLevelButtonColor(jumpToJunkShopPage, () => RandoInterop.Settings.JunkShopSettings.Enabled);

            Localization.Localize(header);
            Localization.Localize(junkShopRootMef);
            Localization.Localize(junkCostSettingsMef);
            Localization.Localize(costSettingsPreset);

            return junkShopPage;
        }

        private void BindTopLevelButtonColor(SmallButton target, Func<bool> condition)
        {
            target.Parent.BeforeShow += () =>
            {
                target.Text.color = condition() ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            };
        }

        public void Apply(RandomizerSettings settings)
        {
            rootMef.SetMenuValues(settings);

            miscLocationMef.SetMenuValues(settings.MiscLocationSettings);

            lemmShopRootMef.SetMenuValues(settings.LemmShopSettings);
            relicGeoSettingsMef.SetMenuValues(settings.LemmShopSettings.GeoSettings);
            relicCostSettingsMef.SetMenuValues(settings.LemmShopSettings.CostSettings);

            junkShopRootMef.SetMenuValues(settings.JunkShopSettings);
            junkCostSettingsMef.SetMenuValues(settings.JunkShopSettings.CostSettings);
        }

        public void Disable()
        {
            enableToggle.SetValue(false);
        }
    }
}
