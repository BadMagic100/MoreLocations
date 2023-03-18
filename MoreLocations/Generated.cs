using RandoConstantGenerators;

namespace MoreLocations
{
    [GenerateJsonConsts("$[*].Name", "Logic/items.json")]
    public static partial class MoreItemNames { }

    [GenerateLogicDefNames("Logic/locations.json")]
    public static partial class MoreLocationNames { }

    [GenerateTypedTerms("Logic/terms.json")]
    public static partial class TermNames { }
}
