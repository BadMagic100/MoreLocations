using GlobalEnums;
using ItemChanger;
using ItemChanger.StartDefs;
using ItemChangerTesting;
using MoreLocations.Rando;
using System.Collections.Generic;

namespace MoreLocationsICTesting
{
    public class SwimTest : Test
    {
        public override int Priority => -1;

        public override StartDef StartDef => TransitionBasedStartDef.FromGate(SceneNames.Crossroads_50, "right1", (int)MapZone.BLUE_LAKE);

        public override string GetName() => "Swim Location Test";

        public override void Start(TestArgs args)
        {
            base.Start(args);
            AbstractPlacement start = ItemChanger.Internal.Ref.Settings.Placements["Start"];
            start.Add(Finder.GetItem(ItemNames.Monomon)!);
        }

        public override IEnumerable<AbstractPlacement> GetPlacements(TestArgs args)
        {
            yield return Finder.GetLocation(MoreLocationNames.Swim)!.Wrap().Add(Finder.GetItem(ItemNames.Grub)!);
        }
    }
}
