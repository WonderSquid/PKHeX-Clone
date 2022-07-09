using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in the games that it represents.
/// </summary>
public interface ILearnGroup
{
    /// <summary>
    /// Gets the next group to traverse to continue checking moves.
    /// </summary>
    ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc);

    /// <summary>
    /// Checks if it is plausible that the <see cref="pk"/> has visited this game group.
    /// </summary>
    bool HasVisited(PKM pk, EvolutionHistory history);

    bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc);
}
