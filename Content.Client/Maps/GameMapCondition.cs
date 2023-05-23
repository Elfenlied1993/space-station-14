using Content.Shared.Maps;

namespace Content.Client.Maps;

[ImplicitDataDefinitionForInheritors]
public abstract class GameMapCondition
{
    [DataField("inverted")]
    public bool Inverted { get; }
    public abstract bool Check(GameMapPrototype map);
}
