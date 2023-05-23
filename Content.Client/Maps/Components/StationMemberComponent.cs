namespace Content.Client.Maps.Components;

/// <summary>
/// Indicates that a grid is a member of the given station.
/// </summary>
[RegisterComponent]
public sealed class StationMemberComponent : Component
{
    /// <summary>
    /// Station that this grid is a part of.
    /// </summary>
    [ViewVariables]
    public EntityUid Station = EntityUid.Invalid;
}
