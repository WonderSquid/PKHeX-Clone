using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public sealed class EvolutionGroup8 : IEvolutionGroup
{
    public static readonly EvolutionGroup8 Instance = new();
    private static readonly EvolutionTree Tree8  = EvolutionTree.Evolves8;
    private static readonly EvolutionTree Tree8a = EvolutionTree.Evolves8a;
    private static readonly EvolutionTree Tree8b = EvolutionTree.Evolves8b;
    private const int MaxSpecies = Legal.MaxSpeciesID_8a;
    private const int Generation = 8;

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => null;
    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc)
    {
        if (enc.Generation >= Generation)
            return null;
        if (enc.Version is GP or GE or GG or GO)
            return EvolutionGroup7b.Instance;
        return EvolutionGroup7.Instance;
    }

    public bool Append(PKM pk, EvolutionHistory history, ref ReadOnlySpan<EvoCriteria> chain, EvolutionOrigin enc)
    {
        if (chain.Length == 0)
            return false;

        var swsh = Append(pk, chain, enc, PersonalTable.SWSH, Tree8 , ref history.Gen8 );
        var pla  = Append(pk, chain, enc, PersonalTable.LA  , Tree8a, ref history.Gen8a);
        var bdsp = Append(pk, chain, enc, PersonalTable.BDSP, Tree8b, ref history.Gen8b);

        if (!(swsh || pla || bdsp))
            return false;

        if (!pk.IsUntraded)
            CrossPropagate(history);

        chain = GetMaxChain(history);

        return true;
    }

    private static ReadOnlySpan<EvoCriteria> GetMaxChain(EvolutionHistory history)
    {
        var arr0 = history.Gen8;
        var arr1 = history.Gen8a;
        var arr2 = history.Gen8b;
        if (arr0.Length >= arr1.Length && arr0.Length >= arr2.Length)
            return arr0;
        if (arr1.Length >= arr2.Length)
            return arr1;
        return arr2;
    }

    private static void CrossPropagate(EvolutionHistory history)
    {
        var arr0 = history.Gen8;
        var arr1 = history.Gen8a;
        var arr2 = history.Gen8b;

        ReplaceIfBetter(arr0, arr1, arr2);
        ReplaceIfBetter(arr1, arr0, arr2);
        ReplaceIfBetter(arr2, arr0, arr1);
    }

    private static void ReplaceIfBetter(Span<EvoCriteria> local, ReadOnlySpan<EvoCriteria> other1, ReadOnlySpan<EvoCriteria> other2)
    {
        for (int i = 0; i < local.Length; i++)
        {
            ReplaceIfBetter(local, other1, i);
            ReplaceIfBetter(local, other2, i);
        }
    }

    private static void ReplaceIfBetter(Span<EvoCriteria> local, ReadOnlySpan<EvoCriteria> other, int parentIndex)
    {
        // Replace the evolution entry if another connected game has a better evolution method (different min/max).
        var native = local[parentIndex];

        // Check if the evo is in the other game; if not, we're done here.
        var index = IndexOfSpecies(other, native.Species);
        if (index == -1)
            return;

        var alt = other[index];
        if (alt.LevelMin < native.LevelMin || alt.LevelMax > native.LevelMax)
            local[parentIndex] = alt;
    }

    private static int IndexOfSpecies(ReadOnlySpan<EvoCriteria> evos, ushort species)
    {
        // Returns the index of the first evo that matches the species
        for (int i = 0; i < evos.Length; i++)
        {
            if (evos[i].Species == species)
                return i;
        }
        return -1;
    }

    private static bool Append(PKM pk, ReadOnlySpan<EvoCriteria> chain, EvolutionOrigin enc, PersonalTable pt, EvolutionTree tree, ref EvoCriteria[] dest)
    {
        // Get the first evolution in the chain that can be present in this group
        var any = GetFirstEvolution(pt, chain, out var evo);
        if (!any)
            return false;

        // Get the evolution tree from this group and get the new chain from it.
        var criteria = enc with { LevelMax = evo.LevelMax, LevelMin = (byte)pk.Met_Level };
        var local = GetInitialChain(pk, criteria, evo.Species, evo.Form, tree);

        // Revise the tree
        var revised = Prune(local);

        // Set the tree to the history field
        dest = revised;

        return revised.Length != 0;
    }

    public EvoCriteria[] GetInitialChain(PKM pk, EvolutionOrigin enc, ushort species, byte form)
    {
        if (!GetPreferredGroup(species, form, out var group))
            return Array.Empty<EvoCriteria>();
        var tree = GetTree(group);
        return GetInitialChain(pk, enc, species, form, tree);
    }

    private static EvoCriteria[] GetInitialChain(PKM pk, EvolutionOrigin enc, ushort species, byte form, EvolutionTree tree)
    {
        return tree.GetExplicitLineage(species, form, pk, enc.LevelMin, enc.LevelMax, MaxSpecies, enc.SkipChecks);
    }

    private static EvolutionTree GetTree(PreferredGroup group) => group switch
    {
        PreferredGroup.BDSP => Tree8b,
        PreferredGroup.LA => Tree8a,
        _ => Tree8,
    };

    private static bool GetPreferredGroup(ushort species, byte form, out PreferredGroup result)
    {
        if (PersonalTable.LA.IsPresentInGame(species, form))
            result = PreferredGroup.LA;
        else if (PersonalTable.SWSH.IsPresentInGame(species, form))
            result = PreferredGroup.SWSH;
        else if (PersonalTable.BDSP.IsPresentInGame(species, form))
            result = PreferredGroup.BDSP;
        else
            result = PreferredGroup.None;
        return result != 0;
    }

    private static EvoCriteria[] Prune(EvoCriteria[] chain) => chain;

    private enum PreferredGroup
    {
        None,
        LA,
        SWSH,
        BDSP,
    }

    private static bool GetFirstEvolution(PersonalTable pt, ReadOnlySpan<EvoCriteria> chain, out EvoCriteria result)
    {
        foreach (var evo in chain)
        {
            // If the evo can't exist in the game, it must be a future evolution.
            if (!pt.IsPresentInGame(evo.Species, evo.Form))
                continue;

            result = evo;
            return true;
        }

        result = default;
        return false;
    }
}
