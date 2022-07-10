using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="YW"/>.
/// </summary>
public sealed class LearnSource1YW : ILearnSource
{
    public static readonly LearnSource1YW Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.Y;
    private static readonly Learnset[] Learnsets = Legal.LevelUpY;
    private const GameVersion Game = YW;

    public Learnset GetLearnset(int species, int form) => Learnsets[species];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if (form is not 0 || species > Legal.MaxSpeciesID_1)
            return false;
        pi = Personal[species];
        return true;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsTutor(evo.Species, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var info = MoveLevelUp.GetIsLevelUp1(evo.Species, evo.Form, move, evo.LevelMax, evo.LevelMin, YW);
            if (info != default)
                return new(LevelUp, Game, (byte)info.Level);
        }

        return default;
    }

    private static bool GetIsTutor(int species, int move)
    {
        // No special tutors besides Stadium, which is GB-era only.
        if (!ParseSettings.AllowGBCartEra)
            return false;

        // Surf Pikachu via Stadium
        if (move != (int)Move.Surf)
            return false;
        return species is (int)Species.Pikachu or (int)Species.Raichu;
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_RBY, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    public IEnumerable<int> GetAllMoves(PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            yield break;

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax, evo.LevelMin);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                    yield return moves[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var permit = pi.TMHM;
            var moveIDs = Legal.TMHM_RBY;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            if (GetIsTutor(evo.Species, (int)Move.Surf))
                yield return (int)Move.Surf;
        }
    }

    public void GetEncounterMoves(IEncounterTemplate enc, Span<int> init)
    {
        var species = enc.Species;
        if (!TryGetPersonal(species, 0, out var personal))
            return;

        var pi = (PersonalInfoG1)personal;
        var learn = Learnsets[species];
        pi.GetMoves(init);
        learn.SetEncounterMoves(enc.LevelMin, init, 4 - init.Count(0));
    }
}
