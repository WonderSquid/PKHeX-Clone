using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen6"/>.
/// </summary>
public sealed class LearnGroup6 : ILearnGroup
{
    public static readonly LearnGroup6 Instance = new();
    private const int Generation = 6;

    public ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc) => enc.Generation is Generation ? null : LearnGroup5.Instance;
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen6.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc)
    {
        var evos = history.Gen6;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, enc);

        if (enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        return MoveResult.AllParsed(result);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, EncounterEgg egg)
    {
        ReadOnlySpan<int> eggMoves, levelMoves;
        if (egg.Version > GameVersion.Y) // OR/AS
        {
            var inst = LearnSource6AO.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).GetAllMoves()
                : ReadOnlySpan<int>.Empty;
        }
        else
        {
            var inst = LearnSource6XY.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).GetAllMoves()
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

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, IEncounterTemplate enc)
    {
        // We can check if it has visited specific sources. We won't check the games it hasn't visited.
        if (enc.Generation != 6 || !pk.IsUntraded)
            CheckBoth(result, current, pk, evo, stage);
        else if (pk.AO)
            CheckSingle(result, current, pk, evo, stage, LearnSource6AO.Instance);
        else
            CheckSingle(result, current, pk, evo, stage, LearnSource6XY.Instance);
    }

    private static void CheckBoth(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage)
    {
        var ao = LearnSource6AO.Instance;
        var species = evo.Species;
        if (!ao.TryGetPersonal(species, evo.Form, out var ao_pi))
            return; // should never happen.

        var xy = LearnSource6XY.Instance;
        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            // Level Up moves are different for each game, but others (TM/Tutor) are same.
            var move = current[i];
            var chk = ao.GetCanLearn(pk, ao_pi, evo, move);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }
            xy.GetCanLearn(pk, ao_pi, evo, move, MoveSourceType.LevelUp);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }

    private static void CheckSingle<T>(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, T game) where T : ILearnSource
    {
        var species = evo.Species;
        if (!game.TryGetPersonal(species, evo.Form, out var pi))
            return; // should never happen.

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = game.GetCanLearn(pk, pi, evo, move);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }
}
