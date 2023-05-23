using System.Diagnostics;
using Content.Shared.Maps;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Maps;

/// <summary>
/// Prototype data for a game map.
/// </summary>
/// <remarks>
/// Forks should not directly edit existing parts of this class.
/// Make a new partial for your fancy new feature, it'll save you time later.
/// </remarks>
[Prototype("gameMap"), PublicAPI]
[DebuggerDisplay("GameMapPrototype [{ID} - {MapName}]")]
public sealed partial class GameMapPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// Name of the map to use in generic messages, like the map vote.
    /// </summary>
    [DataField("mapName", required: true)]
    public string MapName { get; } = default!;

    /// <summary>
    /// Relative directory path to the given map, i.e. `/Maps/saltern.yml`
    /// </summary>
    [DataField("mapPath", required: true)]
    public ResPath MapPath { get; } = default!;

    [DataField("stations", required: true)]
    private Dictionary<string, StationConfig> _stations = new();

    /// <summary>
    /// The stations this map contains. The names should match with the BecomesStation components.
    /// </summary>
    public IReadOnlyDictionary<string, StationConfig> Stations => _stations;

    /// <summary>
    /// Controls if the map can be used as a fallback if no maps are eligible.
    /// </summary>
    [DataField("fallback")]
    public bool Fallback { get; }

    /// <summary>
    /// Minimum players for the given map.
    /// </summary>
    [DataField("minPlayers", required: true)]
    public uint MinPlayers { get; }

    /// <summary>
    /// Maximum players for the given map.
    /// </summary>
    [DataField("maxPlayers")]
    public uint MaxPlayers { get; } = uint.MaxValue;

    [DataField("conditions")] private readonly List<GameMapCondition> _conditions = new();

    /// <summary>
    /// The game map conditions that must be fulfilled for this map to be selectable.
    /// </summary>
    public IReadOnlyList<GameMapCondition> Conditions => _conditions;

}
