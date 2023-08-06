using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterTypeGroup;

namespace PKHeX.Core;

public sealed class EncounterGenerator8b : IEncounterGenerator
{
    public static readonly EncounterGenerator8b Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G8B;
            foreach (var enc in GetPossibleAll(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            if (TryGetEgg(chain, game, out var egg))
            {
                yield return egg;
                if (TryGetSplit(egg, chain, out var split))
                    yield return split;
            }
        }
        if (groups.HasFlag(Static))
        {
            foreach (var enc in GetPossibleAll(chain, Encounters8b.Encounter_BDSP))
                yield return enc;
            var table = game == GameVersion.BD ? Encounters8b.StaticBD : Encounters8b.StaticSP;
            foreach (var enc in GetPossibleAll(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = game == GameVersion.BD ? Encounters8b.SlotsBD : Encounters8b.SlotsSP;
            foreach (var enc in GetPossibleSlots<EncounterArea8b, EncounterSlot8b>(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            foreach (var enc in GetPossibleAll(chain, Encounters8b.TradeGift_BDSP))
                yield return enc;
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var game = (GameVersion)pk.Version;
        var iterator = new EncounterEnumerator8b(pk, chain, game);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    public IEnumerable<IEncounterable> GetEncountersSWSH(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        var iterator = new EncounterEnumerator8bSWSH(pk, chain, game);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    private const int Generation = 8;
    private const EntityContext Context = EntityContext.Gen8b;
    private const byte EggLevel = 1;

    public static bool TryGetEgg(ReadOnlySpan<EvoCriteria> chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            return false;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            return false; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            return false;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            return false;
        // Sanity Check 3
        if (!PersonalTable.BDSP.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    public static bool TryGetSplit(EncounterEgg other, ReadOnlySpan<EvoCriteria> chain, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        // Check for split-breed
        var devolved = chain[^1];
        if (other.Species == devolved.Species)
        {
            if (chain.Length < 2)
                return false; // no split-breed
            devolved = chain[^2];
        }
        if (!Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            return false;

        result = other with { Species = devolved.Species, Form = devolved.Form };
        return true;
    }

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves8b.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }
}
