namespace Content.Client.Maps.Components;

/// <summary>
/// Stores core information about a station, namely its config and associated grids.
/// All station entities will have this component.
/// </summary>
[RegisterComponent]
public sealed class StationDataComponent : Component
{
    /// <summary>
    /// The game map prototype, if any, associated with this station.
    /// </summary>
    [DataField("stationConfig")]
    public StationConfig? StationConfig = null;

    /// <summary>
    /// List of all grids this station is part of.
    /// </summary>
    [DataField("grids")]
    public readonly HashSet<EntityUid> Grids = new();
}
