using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public class LearnSource2C : ILearnSource, IEggSource
{
    public static readonly LearnSource2C Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.C;
    private static readonly EggMoves2[] EggMoves = Legal.EggMovesC;
    private static readonly Learnset[] LevelUp = Legal.LevelUpC;
    private const int MaxSpecies = Legal.MaxSpeciesID_2;

    public Learnset GetLearnset(int species, int form) => LevelUp[species];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if (species > Legal.MaxSpeciesID_2)
            return false;
        pi = Personal[species];
        return true;
    }

    public bool GetIsEggMove(int species, int form, int move)
    {
        if ((uint)species > MaxSpecies)
            return false;
        var moves = EggMoves[species];
        return moves.GetHasEggMove(move);
    }

    public ReadOnlySpan<int> GetEggMoves(int species, int form)
    {
        if ((uint)species > MaxSpecies)
            return ReadOnlySpan<int>.Empty;
        return EggMoves[species].Moves;
    }

    public LearnMethod GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return LearnMethod.LevelUp;
        }

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return LearnMethod.TMHM;

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pk, evo.Species, move))
            return LearnMethod.Tutor;

        return LearnMethod.None;
    }

    private static bool GetIsSpecialTutor(PKM pk, int species, int move)
    {
        if (!ParseSettings.AllowGen2Crystal(pk))
            return false;
        var tutor = Array.IndexOf(Legal.Tutors_GSC, move);
        if (tutor == -1)
            return false;
        var info = PersonalTable.C[species];
        return info.TMHM[57 + tutor];
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_GSC, move);
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
            bool removeVC = pk.Format == 1 || pk.VC1;
            var learn = GetLearnset(evo.Species, evo.Form);
            foreach (var move in learn.GetMoves(evo.LevelMin, evo.LevelMax))
            {
                if (removeVC && move >= Legal.MaxMoveID_1)
                    continue;
                yield return move;
            }
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var permit = pi.TMHM;
            var moveIDs = Legal.TMHM_GSC;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            var tutorSpan = pi.TMHM.AsSpan(57, 3);
            for (int i = 0; i < tutorSpan.Length; i++)
            {
                if (tutorSpan[i])
                    yield return Legal.Tutors_GSC[i];
            }
        }
    }
}
