using System.Diagnostics;
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



}
