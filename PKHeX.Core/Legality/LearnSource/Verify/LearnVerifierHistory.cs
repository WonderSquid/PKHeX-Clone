using System;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Core;

internal static class LearnVerifierHistory
{
    public static void Verify(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc, PKM pk, EvolutionHistory history)
    {
        if (enc.Species is (int)Species.Smeargle)
        {
            VerifySmeargle(result, current, pk);
            return;
        }

        // Empty moves are valid (unless the game does not have a deleter -- handled later).
        MarkEmptySlots(result, current);

        if (!pk.IsOriginalMovesetDeleted())
        {
            // Knock off relearn moves if available.
            if (pk.Format >= 6)
                MarkRelearnMoves(result, current, pk);

            // Knock off initial moves if available.
            MarkSpecialMoves(result, current, enc, pk);
        }

        // Iterate games to identify move sources.
        if (Iterate(result, current, pk, history, enc))
            return;

        // Mark remaining as unknown source.
        for (int i = 0; i < result.Length; i++)
        {
            if (!result[i].IsParsed)
                result[i] = MoveResult.Unobtainable();
        }
    }

    private static void MarkSpecialMoves(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc, PKM pk)
    {
        if (enc is IMoveset { Moves: int[] {Length: not 0} moves})
            MarkInitialMoves(result, current, moves);
        else if (enc is EncounterSlot8GO g)
            MarkInitialMoves(result, current, g.GetInitialMoves(pk.Met_Level));
    }

    private static bool Iterate(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc)
    {
        // Basic gist of move source identification: check if the moves can be learned in the current format.
        // If moves are still unverified, we step backwards in time to the previous game environment, until all moves are checked.
        // If moves are STILL unverified, then they must not be legal.
        var game = LearnCheckerUtil.GetCurrentSource(pk);
        while (true)
        {
            bool finished = game.Check(result, current, pk, history, enc);
            if (finished)
                return true;

            var next = game.GetPrevious(result, pk, history, enc);
            if (next is null)
                return false;

            game = next;
        }
    }

    public static void MarkInitialMoves(Span<MoveResult> result, ReadOnlySpan<int> current, ReadOnlySpan<int> moves)
    {
        for (int i = current.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = moves[i];
            var index = current.IndexOf(move);
            if (index >= 0)
                result[i] = new MoveResult(LearnMethod.Initial);
        }
    }

    private static void MarkEmptySlots(Span<MoveResult> result, ReadOnlySpan<int> current)
    {
        for (int i = current.Length - 1; i >= 0; i--)
        {
            var move = current[i];
            if (move != 0)
                return;
            result[i] = MoveResult.Empty;
        }
    }

    private static void VerifySmeargle(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk)
    {
        for (int i = current.Length - 1; i >= 0; i--)
        {
            var move = current[i];
            if (move == 0)
                result[i] = MoveResult.Empty;
            else if (Legal.IsValidSketch(move, pk.Format))
                result[i] = MoveResult.Sketch;
            else
                result[i] = MoveResult.Unobtainable();
        }
    }

    private static void MarkRelearnMoves(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk)
    {
        // Check if any of the current moves can be relearned.
        for (int i = 0; i < current.Length; i++)
        {
            var move = pk.GetRelearnMove(i);
            if (move == 0) // No more relearn moves.
                return;

            var index = current.IndexOf(move);
            if (index == -1) // Not a relearn move.
                continue;

            result[i] = MoveResult.Relearn;
        }
    }
}

public static class LearnCheckerUtil
{
    public static ILearnGroup GetCurrentSource(PKM pk) => GetCurrentSource(pk.Context);

    public static ILearnGroup GetCurrentSource(EntityContext context) => context switch
    {
        Gen1  => LearnGroup1.Instance,
        Gen2  => LearnGroup2.Instance,
        Gen3  => LearnGroup3.Instance,
        Gen4  => LearnGroup4.Instance,
        Gen5  => LearnGroup5.Instance,
        Gen6  => LearnGroup6.Instance,
        Gen7  => LearnGroup7.Instance,
        Gen8  => LearnGroup8.Instance,

        Gen7b => LearnGroup7b.Instance,
        Gen8a => LearnGroup8a.Instance,
        Gen8b => LearnGroup8b.Instance,

        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };
}