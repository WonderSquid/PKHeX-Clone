using System;

namespace PKHeX.Core;

/// <summary>
/// Moveset verifier for entities currently existing as an egg.
/// </summary>
internal static class LearnVerifierEgg
{
    public static void Verify(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc, PKM pk)
    {
        if (enc.Generation >= 6)
            VerifyFromRelearn(result, current, enc, pk);
        else // No relearn moves available.
            VerifyPre3DS(result, current, enc);
    }

    private static void VerifyPre3DS(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc)
    {
        if (enc is EncounterEgg e)
            VerifyRelearnMoves.VerifyEggMoveset(e, result, current);
        else
            VerifyFromEncounter(result, current, enc);
    }

    private static void VerifyFromEncounter(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc)
    {
        ReadOnlySpan<int> initial;
        if (enc is IMoveset { Moves.Count: not 0 } m)
            initial = (int[])m.Moves;
        else
            initial = GameData.GetLearnset(enc.Version, enc.Species, enc.Form).GetBaseEggMoves(enc.LevelMin);
        VerifyMovesInitial(result, current, initial);
    }

    private static void VerifyMovesInitial(Span<MoveResult> result, ReadOnlySpan<int> current, ReadOnlySpan<int> moves)
    {
        // Check that the sequence of current move matches the relearn move sequence.
        for (int i = 0; i < result.Length; i++)
            result[i] = GetMethodInitial(current[i], moves[i]);
    }

    private static void VerifyFromRelearn(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc,
        PKM pk)
    {
        if (enc is EncounterEgg)
            VerifyMatchesRelearn(result, current, pk);
        else if (enc is IMoveset { Moves.Count: not 0 } m)
            VerifyMovesInitial(result, current, (int[])m.Moves);
        else
            VerifyFromEncounter(result, current, enc);
    }

    private static void VerifyMatchesRelearn(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk)
    {
        // Check that the sequence of current move matches the relearn move sequence.
        for (int i = 0; i < result.Length; i++)
            result[i] = GetMethodRelearn(current[i], pk.GetRelearnMove(i));
    }

    private static MoveResult GetMethodInitial(int current, int relearn)
    {
        if (current != relearn)
            return MoveResult.Unobtainable(relearn);
        if (current == 0)
            return MoveResult.Empty;
        return MoveResult.Relearn;
    }

    private static MoveResult GetMethodRelearn(int current, int relearn)
    {
        if (current != relearn)
            return MoveResult.Unobtainable(relearn);
        if (current == 0)
            return MoveResult.Empty;
        return MoveResult.Relearn;
    }
}
