using GlobalEnums;
using ItemChanger;
using ItemChangerTesting;
using MoreLocations;
using System.Collections.Generic;

namespace MoreLocationsICTesting
{
    public class StagNestEggTest : Test
    {
        public override int Priority => -1;

        public override StartDef StartDef => new StartDef()
        {
            SceneName = SceneNames.Cliffs_03,
            X = 85.8f,
            Y = 46.4f,
            MapZone = (int)MapZone.CLIFFS,
            RespawnFacingRight = true,
            SpecialEffects = SpecialStartEffects.Default | SpecialStartEffects.SlowSoulRefill
        };

        public override IEnumerable<AbstractPlacement> GetPlacements(TestArgs args)
        {
            yield return Finder.GetLocation(MoreLocationNames.Stag_Nest_Egg)!.Wrap().Add(Finder.GetItem(ItemNames.Grub)!);
        }
    }
}
