using System;
using System.Buffers;

namespace PKHeX.Core;

public class LearnGroupHOME : ILearnGroup
{
    public static readonly LearnGroupHOME Instance = new();
    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => null;
    public bool HasVisited(PKM pk, EvolutionHistory history) => pk is IHomeTrack { HasTracker: true } || !ParseSettings.IgnoreTransferIfNoTracker;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history,
        IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        // For now until we discover edge cases, just handle the top-species for a given game.
        var evos = history.Get(pk.Context);
        if (history.HasVisitedGen9 && pk is not PK9)
        {
            var instance = LearnSource9SV.Instance;
            for (var i = 0; i < evos.Length; i++)
                Check<LearnSource9SV, PersonalInfo9SV>(result, current, pk, evos[i], 9, i, types, instance, LearnOption.AtAnyTime);
        }
        if (history.HasVisitedSWSH && pk is not PK8)
        {
            var instance = LearnSource8SWSH.Instance;
            for (var i = 0; i < evos.Length; i++)
                Check<LearnSource8SWSH, PersonalInfo8SWSH>(result, current, pk, evos[i], 8, i, types, instance, LearnOption.AtAnyTime);
        }
        if (history.HasVisitedPLA && pk is not PA8)
        {
            var instance = LearnSource8LA.Instance;
            for (var i = 0; i < evos.Length; i++)
                Check<LearnSource8LA, PersonalInfo8LA>(result, current, pk, evos[i], 8, i, types, instance, LearnOption.AtAnyTime);
        }
        if (history.HasVisitedBDSP && pk is not PB8)
        {
            var instance = LearnSource8BDSP.Instance;
            for (var i = 0; i < evos.Length; i++)
                Check<LearnSource8BDSP, PersonalInfo8BDSP>(result, current, pk, evos[i], 8, i, types, instance, LearnOption.AtAnyTime);
        }

        IHomeSource local = GetCurrent(pk.Context);
        for (int i = 0; i < result.Length; i++)
        {
            var r = result[i];
            if (!r.Valid)
                continue;

            if (r.Info.Environment == local.Environment)
                continue;

            var move = current[i];
            bool valid = true;
            foreach (var evo in evos)
            {
                var chk = local.GetCanLearnHOME(pk, evo, move, types);
                if (chk.Method != LearnMethod.None)
                    valid = true;
            }
            if (!valid)
                result[i] = default;
        }

        return MoveResult.AllParsed(result);
    }

    private static IHomeSource GetCurrent(EntityContext context) => context switch
    {
        EntityContext.Gen8 => LearnSource8SWSH.Instance,
        EntityContext.Gen8a => LearnSource8LA.Instance,
        EntityContext.Gen8b => LearnSource8BDSP.Instance,
        EntityContext.Gen9 => LearnSource9SV.Instance,
        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };

    private static void Check<TSource, TPersonal>(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, byte generation, int stage, MoveSourceType type,
        TSource inst, LearnOption option) where TSource : ILearnSource<TPersonal> where TPersonal : PersonalInfo
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, generation, option))
        {
            CheckInternal<TSource, TPersonal>(result, current, pk, evo, generation, stage, type, inst, option);
            return;
        }

        // Check all forms
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            CheckInternal<TSource, TPersonal>(result, current, pk, evo with { Form = (byte)i }, generation, stage, type, inst, option);
    }

    private static void CheckInternal<TSource, TPersonal>(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo,
        byte generation, int stage, MoveSourceType type, TSource inst, LearnOption option) where TSource : ILearnSource<TPersonal> where TPersonal : PersonalInfo
    {
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = inst.GetCanLearn(pk, pi, evo, move, type, option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, generation);
        }
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        option = LearnOption.AtAnyTime;
        var evos = history.Get(pk.Context);
        IHomeSource local = GetCurrent(pk.Context);
        if (history.HasVisitedGen9 && pk is not PK9)
        {
            var instance = LearnSource9SV.Instance;
            foreach (var evo in evos)
                GetAllMoves<LearnSource9SV, PersonalInfo9SV>(result, pk, evo, types, instance, 9, local, option);
        }
        if (history.HasVisitedSWSH && pk is not PK8)
        {
            var instance = LearnSource8SWSH.Instance;
            foreach (var evo in evos)
                GetAllMoves<LearnSource8SWSH, PersonalInfo8SWSH>(result, pk, evo, types, instance, 8, local, option);
        }
        if (history.HasVisitedPLA && pk is not PA8)
        {
            var instance = LearnSource8LA.Instance;
            foreach (var evo in evos)
                GetAllMoves<LearnSource8LA, PersonalInfo8LA>(result, pk, evo, types, instance, 8, local, option);
        }
        if (history.HasVisitedBDSP && pk is not PB8)
        {
            var instance = LearnSource8BDSP.Instance;
            foreach (var evo in evos)
                GetAllMoves<LearnSource8BDSP, PersonalInfo8BDSP>(result, pk, evo, types, instance, 8, local, option);
        }
    }

    private static void GetAllMoves<TSource, TPersonal>(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types,
        TSource inst, byte generation, IHomeSource dest, LearnOption option = LearnOption.AtAnyTime) where TSource : ILearnSource<TPersonal> where TPersonal : PersonalInfo
    {
        // Get all the moves that could have been learned in another context and kept in that context's coat.
        // For each of these possible moves, we will check if the current context can learn it.
        var rent = ArrayPool<bool>.Shared.Rent(result.Length);
        var temp = rent.AsSpan(0, result.Length);
        GetAllMovesInternal<TSource, TPersonal>(temp, pk, evo, types, inst, generation, option);

        for (int i = 0; i < temp.Length; i++)
        {
            if (!temp[i])
                continue;
            if (result[i])
                continue;

            var chk = dest.GetCanLearnHOME(pk, evo, (ushort)i, types);
            if (chk.Method != LearnMethod.None)
                result[i] = true;
        }

        ArrayPool<bool>.Shared.Return(rent);
    }

    private static void GetAllMovesInternal<TSource, TPersonal>(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types,
        TSource inst, byte generation, LearnOption option) where TSource : ILearnSource<TPersonal>
        where TPersonal : PersonalInfo
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, generation, option))
        {
            inst.GetAllMoves(result, pk, evo, types);
            return;
        }

        // Check all forms
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            inst.GetAllMoves(result, pk, evo with { Form = (byte)i }, types);
    }
}