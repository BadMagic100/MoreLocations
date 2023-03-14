using ItemChanger;
using System;

namespace MoreLocations.ItemChanger
{
    public class NullLocation : AbstractLocation
    {
        public override AbstractPlacement Wrap()
        {
            throw new NotImplementedException();
        }

        protected override void OnLoad()
        {
        }

        protected override void OnUnload()
        {
        }
    }
}
