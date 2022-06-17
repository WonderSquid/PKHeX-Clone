﻿using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.ParseSettings;

namespace PKHeX.Core;

/// <summary>
/// Logic to verify the current <see cref="PKM.RelearnMoves"/>.
/// </summary>
public static class VerifyRelearnMoves
{
    internal static void DummyValid(CheckMoveResult p) => p.Set(MoveSource.Relearn, 0, Severity.Valid, L_AValid, CheckIdentifier.RelearnMove);

    public static CheckMoveResult[] VerifyRelearn(PKM pk, IEncounterTemplate enc, CheckMoveResult[] result)
    {
        if (ShouldNotHaveRelearnMoves(enc, pk))
            return VerifyRelearnNone(pk, result);

        return enc switch
        {
            IRelearn s when s.Relearn.Count != 0 => VerifyRelearnSpecifiedMoveset(pk, s.Relearn, result),
            EncounterEgg e => VerifyEggMoveset(e, result, pk.RelearnMoves),
            EncounterSlot6AO {CanDexNav:true} z when pk.RelearnMove1 != 0 => VerifyRelearnDexNav(pk, result, z),
            EncounterSlot8b {IsUnderground:true} u => VerifyRelearnUnderground(pk, result, u),
            _ => VerifyRelearnNone(pk, result),
        };
    }

    public static bool ShouldNotHaveRelearnMoves(IGeneration enc, PKM pk) => enc.Generation < 6 || pk.IsOriginalMovesetDeleted();

    private static CheckMoveResult[] VerifyRelearnSpecifiedMoveset(PKM pk, IReadOnlyList<int> required, CheckMoveResult[] result)
    {
        CheckResult(pk.RelearnMove4, required[3], result[3]);
        CheckResult(pk.RelearnMove3, required[2], result[2]);
        CheckResult(pk.RelearnMove2, required[1], result[1]);
        CheckResult(pk.RelearnMove1, required[0], result[0]);
        return result;

        static void CheckResult(int move, int require, CheckMoveResult p)
        {
            if (move == require)
            {
                DummyValid(p);
                return;
            }
            var c = string.Format(LMoveFExpect_0, MoveStrings[require]);
            p.Set(MoveSource.Relearn, 0, Severity.Invalid, c, CheckIdentifier.RelearnMove);
        }
    }

    private static void ParseExpectEmpty(CheckMoveResult p, int move)
    {
        if (move == 0)
            DummyValid(p);
        else
            p.Set(MoveSource.Relearn, 0, Severity.Invalid, LMoveRelearnNone, CheckIdentifier.RelearnMove);
    }

    private static CheckMoveResult[] VerifyRelearnDexNav(PKM pk, CheckMoveResult[] result, EncounterSlot6AO slot)
    {
        // All other relearn moves must be empty.
        ParseExpectEmpty(result[3], pk.RelearnMove4);
        ParseExpectEmpty(result[2], pk.RelearnMove3);
        ParseExpectEmpty(result[1], pk.RelearnMove2);

        // DexNav Pokémon can have 1 random egg move as a relearn move.
        var p = result[0];
        if (!slot.CanBeDexNavMove(pk.RelearnMove1)) // not found
            p.Set(MoveSource.Relearn, 6, Severity.Invalid, LMoveRelearnDexNav, CheckIdentifier.RelearnMove);
        else
            DummyValid(p);

        return result;
    }

    private static CheckMoveResult[] VerifyRelearnUnderground(PKM pk, CheckMoveResult[] result, EncounterSlot8b slot)
    {
        // All other relearn moves must be empty.
        ParseExpectEmpty(result[3], pk.RelearnMove4);
        ParseExpectEmpty(result[2], pk.RelearnMove3);
        ParseExpectEmpty(result[1], pk.RelearnMove2);

        // Underground Pokémon can have 1 random egg move as a relearn move.
        var p = result[0];
        if (!slot.CanBeUndergroundMove(pk.RelearnMove1)) // not found
            p.Set(MoveSource.Relearn, 0, Severity.Invalid, LMoveRelearnUnderground, CheckIdentifier.RelearnMove);
        else
            DummyValid(p);

        return result;
    }

    private static CheckMoveResult[] VerifyRelearnNone(PKM pk, CheckMoveResult[] result)
    {
        // No relearn moves should be present.
        ParseExpectEmpty(result[3], pk.RelearnMove4);
        ParseExpectEmpty(result[2], pk.RelearnMove3);
        ParseExpectEmpty(result[1], pk.RelearnMove2);
        ParseExpectEmpty(result[0], pk.RelearnMove1);
        return result;
    }

    internal static CheckMoveResult[] VerifyEggMoveset(EncounterEgg e, CheckMoveResult[] result, int[] moves, CheckIdentifier type = CheckIdentifier.RelearnMove)
    {
        int gen = e.Generation;
        var origins = MoveBreed.Process(gen, e.Species, e.Form, e.Version, moves, out var valid);
        if (valid)
        {
            for (int i = 0; i < result.Length; i++)
            {
                var msg = EggSourceUtil.GetSource(origins, gen, i);
                result[i].Set(MoveSource.EggMove, gen, Severity.Valid, msg, type);
            }
        }
        else
        {
            var expected = MoveBreed.GetExpectedMoves(moves, e);
            origins = MoveBreed.Process(gen, e.Species, e.Form, e.Version, expected, out _);
            for (int i = 0; i < moves.Length; i++)
            {
                var msg = EggSourceUtil.GetSource(origins, gen, i);
                var expect = expected[i];
                var p = result[i];
                if (moves[i] == expect)
                {
                    p.Set(MoveSource.EggMove, gen, Severity.Valid, msg, type);
                }
                else
                {
                    msg = string.Format(LMoveRelearnFExpect_0, MoveStrings[expect], msg);
                    p.Set(MoveSource.EggMove, gen, Severity.Invalid, msg, type);
                }
            }
        }

        var dupe = IsAnyMoveDuplicate(moves);
        if (dupe != NO_DUPE)
            result[dupe].Set(MoveSource.EggMove, gen, Severity.Invalid, LMoveSourceDuplicate, type);
        return result;
    }

    private const int NO_DUPE = -1;

    private static int IsAnyMoveDuplicate(ReadOnlySpan<int> move)
    {
        int m1 = move[0];
        int m2 = move[1];

        if (m1 != 0 && m1 == m2)
            return 1;
        int m3 = move[2];
        if (m3 != 0 && (m1 == m3 || m2 == m3))
            return 2;
        int m4 = move[3];
        if (m4 != 0 && (m1 == m4 || m2 == m4 || m3 == m4))
            return 3;
        return NO_DUPE;
    }
}
