using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen7"/>.
/// </summary>
public sealed class LearnGroup7 : ILearnGroup
{
    public static readonly LearnGroup7 Instance = new();
    private const int Generation = 7;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => enc.Generation switch
    {
        <= 0 when history.Gen2.Length != 0 => LearnGroup2.Instance,
        1 when history.Gen1.Length != 0 => LearnGroup1.Instance,
        2 when history.Gen2.Length != 0 => LearnGroup2.Instance,
        _ => null,
    };

    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen7.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        var mode = GetCheckMode(pk);
        var evos = history.Gen7;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, types, option, mode);

        if (option is not LearnOption.Current && types.HasFlagFast(MoveSourceType.Encounter) && pk.IsOriginalMovesetDeleted() && enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        return MoveResult.AllParsed(result);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, EncounterEgg egg)
    {
        ReadOnlySpan<int> eggMoves, levelMoves;
        if (egg.Version > GameVersion.Y) // OR/AS
        {
            var inst = LearnSource7USUM.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).Moves
                : ReadOnlySpan<int>.Empty;
        }
        else
        {
            var inst = LearnSource7SM.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).Moves
                : ReadOnlySpan<int>.Empty;
        }

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (eggMoves.Contains(move))
                result[i] = new(LearnMethod.EggMove);
            else if (levelMoves.Contains(move))
                result[i] = new(LearnMethod.InheritLevelUp);
            else if (move is (int)Move.VoltTackle && egg.CanHaveVoltTackle)
                result[i] = new(LearnMethod.SpecialEgg);
        }
    }

    private static CheckMode GetCheckMode(PKM pk)
    {
        // We can check if it has visited specific sources. We won't check the games it hasn't visited.
        if (!pk.IsUntraded)
            return CheckMode.Both;
        if (pk.USUM)
            return CheckMode.USUM;
        return CheckMode.SM;
    }

    private enum CheckMode
    {
        Both,
        SM,
        USUM,
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType types, LearnOption option, CheckMode mode)
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, Generation, option))
        {
            CheckInternal(result, current, pk, evo, stage, types, option, mode);
            return;
        }

        // Check all forms
        var inst = LearnSource7USUM.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            CheckInternal(result, current, pk, evo with { Form = (byte)i }, stage, types, option, mode);
    }

    private static void CheckInternal(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType types, LearnOption option, CheckMode mode)
    {
        // We can check if it has visited specific sources. We won't check the games it hasn't visited.
        if (mode == CheckMode.Both)
            CheckBoth(result, current, pk, evo, stage, types, option);
        else if (mode == CheckMode.USUM)
            CheckSingle(result, current, pk, evo, stage, LearnSource7USUM.Instance, types, option);
        else
            CheckSingle(result, current, pk, evo, stage, LearnSource7SM.Instance, types, option);
    }

    private static void CheckBoth(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType types, LearnOption option)
    {
        var uu = LearnSource7USUM.Instance;
        var species = evo.Species;
        if (!uu.TryGetPersonal(species, evo.Form, out var uu_pi))
            return; // should never happen.

        var sm = LearnSource7SM.Instance;
        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            // Level Up moves are different for each game, but others (TM/Tutor) are same.
            var move = current[i];
            var chk = uu.GetCanLearn(pk, uu_pi, evo, move, types, option);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }

            if (evo.Species > Legal.MaxSpeciesID_7)
                continue;
            sm.GetCanLearn(pk, uu_pi, evo, move, types & MoveSourceType.LevelUp, option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }

    private static void CheckSingle<T>(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, T game, MoveSourceType types, LearnOption option) where T : ILearnSource
    {
        var species = evo.Species;
        if (!game.TryGetPersonal(species, evo.Form, out var pi))
            return; // should never happen.

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = game.GetCanLearn(pk, pi, evo, move, types, option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.Encounter) && enc.Generation == Generation)
            FlagEncounterMoves(enc, result);

        var mode = GetCheckMode(pk);
        foreach (var evo in history.Gen7)
            GetAllMoves(result, pk, evo, types, option, mode);
    }

    private static void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types, LearnOption option, CheckMode mode)
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, Generation, option))
        {
            GetAllMovesInternal(result, pk, evo, types, mode);
            return;
        }

        // Check all forms
        var inst = LearnSource6AO.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            GetAllMovesInternal(result, pk, evo with { Form = (byte)i }, types, mode);
    }

    private static void GetAllMovesInternal(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types, CheckMode mode)
    {
        if (mode is CheckMode.Both)
            GetAllMovesBoth(result, pk, evo, types);
        else if (mode is CheckMode.USUM)
            GetAllMovesSingle(result, pk, evo, LearnSource7USUM.Instance, types);
        else
            GetAllMovesSingle(result, pk, evo, LearnSource7SM.Instance, types);
    }

    private static void GetAllMovesSingle<T>(Span<bool> result, PKM pk, EvoCriteria evo, T instance, MoveSourceType types) where T : ILearnSource
    {
        instance.GetAllMoves(result, pk, evo, types);
    }

    private static void GetAllMovesBoth(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types)
    {
        LearnSource7USUM.Instance.GetAllMoves(result, pk, evo, types);
        LearnSource7SM.Instance.GetAllMoves(result, pk, evo, types & MoveSourceType.LevelUp);
    }

    private static void FlagEncounterMoves(IEncounterTemplate enc, Span<bool> result)
    {
        if (enc is IMoveset { Moves: int[] { Length: not 0 } x })
        {
            foreach (var move in x)
                result[move] = true;
        }
        if (enc is IRelearn { Relearn: int[] { Length: not 0 } r })
        {
            foreach (var move in r)
                result[move] = true;
        }
    }
}
