using Newtonsoft.Json;

namespace MoreLocations.Rando.Settings
{
    public class MiscLocationSettings
    {
        [JsonIgnore]
        public bool Any => Swim || StagNestEgg || BaldurShellChest || AdditionalLocations;
        public bool Swim = true;
        public bool StagNestEgg = true;
        public bool BaldurShellChest = true;
        public bool AdditionalLocations = true;
    }
}
