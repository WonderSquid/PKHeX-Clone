using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen1"/>.
/// </summary>
public sealed class LearnGroup1 : ILearnGroup
{
    public static readonly LearnGroup1 Instance = new();
    private const int Generation = 2;

    public ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc) => pk.Context switch
    {
        EntityContext.Gen1 when enc.Generation == 1 && pk is PK1 pk1 && HasDefinitelyVisitedGen2(pk1) => LearnGroup2.Instance,
        EntityContext.Gen2 => null,
        _ => enc.Generation == 1 ? LearnGroup2.Instance : null,
    };

    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen1.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option = LearnOption.Current)
    {
        if (enc.Generation == 1)
            CheckEncounterMoves(result, current, enc, pk);

        var evos = history.Gen1;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i);

        if (GetPrevious(result, pk, history, enc) is null)
            FlagEvolutionSlots(result, current, history);

        return MoveResult.AllParsed(result);
    }

    private static void FlagEvolutionSlots(Span<MoveResult> result, ReadOnlySpan<int> current, EvolutionHistory history)
    {
        for (int i = 0; i < result.Length; i++)
        {
            if (current[i] == 0)
                continue;

            var detail = result[i];
            if (!detail.Valid || detail.Generation is not (1 or 2))
                continue;

            var info = detail.Info;
            if (info.Method is LearnMethod.LevelUp)
            {
                var level = info.Argument;
                var stage = detail.EvoStage;
                var species = history[detail.Generation][stage].Species;
                if (IsAnyOtherResultALowerEvolutionStageAndHigherLevel(result, i, history, level, species))
                    result[i] = MoveResult.Unobtainable();
            }
        }
    }

    private static bool IsAnyOtherResultALowerEvolutionStageAndHigherLevel(Span<MoveResult> result, int index, EvolutionHistory history, byte level, ushort species)
    {
        // Check if any other result is a lower evolution stage and higher level.
        for (int i = 0; i < result.Length; i++)
        {
            if (i == index)
                continue;
            var detail = result[i];
            if (!detail.Valid || detail.Generation is not (1 or 2))
                continue;

            (var method, _, byte level2) = detail.Info;
            if (method is not LearnMethod.LevelUp)
                continue;

            var stage = detail.EvoStage;
            var species2 = history[detail.Generation][stage].Species;
            if (level2 > level && species2 <= species)
                return true;
        }

        return false;
    }

    private static void FlagFishyMoveSlots(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc, PK1 pk)
    {
        var present = current.Length - current.Count(0);
        if (present == 4)
            return;

        Span<int> moves = stackalloc int[4];
        GetEncounterMoves(enc, moves);

        // Count the amount of initial moves not present in the current list.
        int count = CountPresent(current, moves);
        if (count == 0)
            return;

        // There are ways to skip level up moves by leveling up more than once.
        // https://bulbapedia.bulbagarden.net/wiki/List_of_glitches_(Generation_I)#Level-up_learnset_skipping
        // Evolution canceling also leads to incorrect assumptions in the above used method, so just indicate them as fishy in that case.
        // Not leveled up? Not possible to be missing the move slot.
        var level = pk.CurrentLevel;
        var invalid = enc.LevelMin == level;
        var msg = invalid ? MoveResult.EmptyInvalid : MoveResult.EmptyFishy;
        for (int i = present; i < present + count; i++)
            result[i] = msg;
    }

    private static int CountPresent(ReadOnlySpan<int> current, ReadOnlySpan<int> moves)
    {
        int count = 0;
        foreach (int expect in moves)
        {
            if (expect == 0)
                break;
            if (!current.Contains(expect))
                count++;
        }
        return count;
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc, PKM pk)
    {
        Span<int> moves = stackalloc int[4];
        if (enc is IMoveset {Moves: int[] {Length: not 0} x})
            x.CopyTo(moves);
        else
            GetEncounterMoves(enc, moves);
        LearnVerifierHistory.MarkInitialMoves(result, current, moves);

        // Flag empty slots if never visited Gen2 move deleter.
        if (pk is not PK1 pk1)
            return;
        if (HasDefinitelyVisitedGen2(pk1))
            return;
        FlagFishyMoveSlots(result, current, enc, pk1);
    }

    private static bool HasDefinitelyVisitedGen2(PK1 pk1)
    {
        if (!ParseSettings.AllowGen1Tradeback)
            return false;
        var rate = pk1.Catch_Rate;
        return rate is 0 || GBRestrictions.IsTradebackCatchRate(rate);
    }

    private static void GetEncounterMoves(IEncounterTemplate enc, Span<int> moves)
    {
        if (enc.Version is GameVersion.YW or GameVersion.RBY)
            LearnSource1YW.Instance.GetEncounterMoves(enc, moves);
        else
            LearnSource1RB.Instance.GetEncounterMoves(enc, moves);
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage)
    {
        var rb = LearnSource1RB.Instance;
        if (!rb.TryGetPersonal(evo.Species, evo.Form, out var rp))
            return; // should never happen.

        var yw = LearnSource1YW.Instance;
        if (!yw.TryGetPersonal(evo.Species, evo.Form, out var yp))
            return; // should never happen.

        for (int i = result.Length - 1; i >= 0; i--)
        {
            ref var entry = ref result[i];
            if (entry.Valid && entry.Generation > 2)
                continue;

            var move = current[i];
            var chk = yw.GetCanLearn(pk, yp, evo, move);
            if (chk != default && GetIsPreferable(entry, chk, stage))
            {
                entry = new(chk, (byte)stage, Generation);
                continue;
            }
            chk = rb.GetCanLearn(pk, rp, evo, move);
            if (chk != default && GetIsPreferable(entry, chk, stage))
                entry = new(chk, (byte)stage, Generation);
        }
    }

    private static bool GetIsPreferable(in MoveResult entry, in MoveLearnInfo chk, int stage)
    {
        if (entry.Info.Method is LearnMethod.LevelUp)
        {
            if (chk.Method is not LearnMethod.LevelUp)
                return true;
            if (entry.EvoStage == stage)
                return entry.Info.Argument < chk.Argument;
        }
        else if (entry.Info.Method.IsEggSource())
        {
            return true;
        }
        else if (chk.Method is LearnMethod.LevelUp)
        {
            return false;
        }
        return entry.EvoStage < stage;
    }
}
