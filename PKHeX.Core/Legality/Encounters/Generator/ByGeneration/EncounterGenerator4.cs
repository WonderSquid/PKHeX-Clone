using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator4 : IEncounterGenerator
{
    public static readonly EncounterGenerator4 Instance = new();

    // Utility
    private static readonly PGT RangerManaphy = new() { Data = { [0] = 7, [8] = 1 } };

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 4);
        if (chain.Length == 0)
            return Array.Empty<IEncounterable>();
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            if (chain[^1].Species == (int)Species.Manaphy)
                yield return RangerManaphy;

            var table = EncounterEvent.MGDB_G4;
            foreach (var enc in GetPossibleGifts(chain, table, game))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            var eggs = GetEggs(chain, game);
            foreach (var enc in eggs)
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            if (game is GameVersion.HG or GameVersion.SS)
            {
                foreach (var enc in GetPossible(chain, Encounters4HGSS.Encounter_HGSS))
                    yield return enc;
                var specific = game == GameVersion.HG ? Encounters4HGSS.StaticHG : Encounters4HGSS.StaticSS;
                foreach (var enc in GetPossible(chain, specific))
                    yield return enc;
                foreach (var enc in GetPossible(chain, Encounters4HGSS.Encounter_PokeWalker))
                    yield return enc;
            }
            else
            {
                foreach (var enc in GetPossible(chain, Encounters4DPPt.StaticDPPt))
                    yield return enc;
                if (game is GameVersion.Pt)
                {
                    foreach (var enc in GetPossible(chain, Encounters4DPPt.StaticPt))
                        yield return enc;
                }
                else
                {
                    foreach (var enc in GetPossible(chain, Encounters4DPPt.StaticDP))
                        yield return enc;
                    var specific = game == GameVersion.D ? Encounters4DPPt.StaticD : Encounters4DPPt.StaticP;
                    foreach (var enc in GetPossible(chain, specific))
                        yield return enc;
                }
            }
        }
        if (groups.HasFlag(Slot))
        {
            var areas = GetAreas(game);
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            if (game is GameVersion.HG or GameVersion.SS)
            {
                foreach (var enc in GetPossible(chain, Encounters4HGSS.TradeGift_HGSS))
                    yield return enc;
            }
            else
            {
                foreach (var enc in GetPossible(chain, Encounters4DPPt.TradeGift_DPPtIngame))
                    yield return enc;
                if (game is GameVersion.D)
                {
                    foreach (var enc in GetPossible(chain, Encounters4DPPt.RanchGifts))
                        yield return enc;
                }
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, IReadOnlyList<PCD> table, GameVersion game)
    {
        foreach (var enc in table)
        {
            if (!enc.CanBeReceivedByVersion((int)game))
                continue;
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<T> GetPossible<T>(EvoCriteria[] chain, T[] table) where T : IEncounterable
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea4[] areas)
    {
        foreach (var area in areas)
        {
            foreach (var slot in area.Slots)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != slot.Species)
                        continue;
                    yield return slot;
                    break;
                }
            }
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        info.PIDIV = MethodFinder.Analyze(pk);
        var deferredPIDIV = new List<IEncounterable>();
        var deferredEType = new List<IEncounterable>();

        foreach (var z in GetEncountersInner(pk, chain, info))
        {
            if (!info.PIDIV.Type.IsCompatible4(z, pk))
                deferredPIDIV.Add(z);
            else if (pk is IGroundTile e && !(z is IGroundTypeTile t ? t.GroundTile.Contains(e.GroundTile) : e.GroundTile == 0))
                deferredEType.Add(z);
            else
                yield return z;
        }

        foreach (var z in deferredEType)
            yield return z;

        if (deferredPIDIV.Count == 0)
            yield break;

        info.PIDIVMatches = false;
        foreach (var z in deferredPIDIV)
            yield return z;
    }

    private static IEnumerable<IEncounterable> GetEncountersInner(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var game = (GameVersion)pk.Version;
        if (pk.FatefulEncounter)
        {
            if (PGT.IsRangerManaphy(pk))
            {
                yield return RangerManaphy;
                yield break;
            }

            bool yielded = false;
            foreach (var mg in EncounterEvent.MGDB_G4)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != mg.Species)
                        continue;
                    if (mg.IsMatchExact(pk, evo))
                    {
                        yield return mg;
                        yielded = true;
                    }
                    break;
                }
            }
            if (yielded)
                yield break;
        }
        if (Locations.IsEggLocationBred4(pk.Egg_Location, game))
        {
            var eggs = GetEggs(chain, game);
            foreach (var egg in eggs)
                yield return egg;
        }

        if (game is GameVersion.HG or GameVersion.SS)
        {
            foreach (var enc in Encounters4HGSS.TradeGift_HGSS)
            {
                if (!enc.Version.Contains(game))
                    continue;
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (enc.IsMatchExact(pk, evo))
                        yield return enc;
                    break;
                }
            }
        }
        else
        {
            foreach (var enc in Encounters4DPPt.TradeGift_DPPtIngame)
            {
                if (!enc.Version.Contains(game))
                    continue;
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (enc.IsMatchExact(pk, evo))
                        yield return enc;
                    break;
                }
            }
            if (game is GameVersion.D)
            {
                foreach (var enc in Encounters4DPPt.RanchGifts)
                {
                    foreach (var evo in chain)
                    {
                        if (evo.Species != enc.Species)
                            continue;
                        if (enc.IsMatchExact(pk, evo))
                            yield return enc;
                        break;
                    }
                }
            }
        }

        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        bool safariSport = pk.Ball is (int)Ball.Sport or (int)Ball.Safari; // never static encounters
        if (!safariSport)
        {
            if (game is GameVersion.HG or GameVersion.SS)
            {
                foreach (var enc in Encounters4HGSS.Encounter_HGSS)
                {
                    foreach (var evo in chain)
                    {
                        if (evo.Species != enc.Species)
                            continue;
                        if (!enc.IsMatchExact(pk, evo))
                            break;

                        var match = enc.GetMatchRating(pk);
                        if (match == PartialMatch)
                            partial ??= enc;
                        else
                            yield return enc;
                        break;
                    }
                }
                var specific = game == GameVersion.HG ? Encounters4HGSS.StaticHG : Encounters4HGSS.StaticSS;
                foreach (var enc in specific)
                {
                    foreach (var evo in chain)
                    {
                        if (evo.Species != enc.Species)
                            continue;
                        if (!enc.IsMatchExact(pk, evo))
                            break;

                        var match = enc.GetMatchRating(pk);
                        if (match == PartialMatch)
                            partial ??= enc;
                        else
                            yield return enc;
                        break;
                    }
                }
                foreach (var enc in Encounters4HGSS.Encounter_PokeWalker)
                {
                    foreach (var evo in chain)
                    {
                        if (evo.Species != enc.Species)
                            continue;
                        if (!enc.IsMatchExact(pk, evo))
                            break;

                        var match = enc.GetMatchRating(pk);
                        if (match == PartialMatch)
                            partial ??= enc;
                        else
                            yield return enc;
                        break;
                    }
                }
            }
            else
            {
                foreach (var enc in Encounters4DPPt.StaticDPPt)
                {
                    foreach (var evo in chain)
                    {
                        if (evo.Species != enc.Species)
                            continue;
                        if (!enc.IsMatchExact(pk, evo))
                            break;

                        var match = enc.GetMatchRating(pk);
                        if (match == PartialMatch)
                            partial ??= enc;
                        else
                            yield return enc;
                        break;
                    }
                }
                if (game is GameVersion.Pt)
                {
                    foreach (var enc in Encounters4DPPt.StaticPt)
                    {
                        foreach (var evo in chain)
                        {
                            if (evo.Species != enc.Species)
                                continue;
                            if (!enc.IsMatchExact(pk, evo))
                                break;

                            var match = enc.GetMatchRating(pk);
                            if (match == PartialMatch)
                                partial ??= enc;
                            else
                                yield return enc;
                            break;
                        }
                    }
                }
                else
                {
                    var specific = game == GameVersion.D ? Encounters4DPPt.StaticD : Encounters4DPPt.StaticP;
                    foreach (var enc in specific)
                    {
                        foreach (var evo in chain)
                        {
                            if (evo.Species != enc.Species)
                                continue;
                            if (!enc.IsMatchExact(pk, evo))
                                break;

                            var match = enc.GetMatchRating(pk);
                            if (match == PartialMatch)
                                partial ??= enc;
                            else
                                yield return enc;
                            break;
                        }
                    }
                    foreach (var enc in Encounters4DPPt.StaticDP)
                    {
                        foreach (var evo in chain)
                        {
                            if (evo.Species != enc.Species)
                                continue;
                            if (!enc.IsMatchExact(pk, evo))
                                break;

                            var match = enc.GetMatchRating(pk);
                            if (match == PartialMatch)
                                partial ??= enc;
                            else
                                yield return enc;
                            break;
                        }
                    }
                }
            }
        }

        if (CanBeWildEncounter(pk))
        {
            var wildFrames = AnalyzeFrames(pk, info);
            var areas = GetAreas(game);
            foreach (var area in areas)
            {
                var slots = area.GetMatchingSlots(pk, chain);
                foreach (var slot in slots)
                {
                    var match = slot.GetMatchRating(pk);
                    if (match == PartialMatch)
                    {
                        partial ??= slot;
                        continue;
                    }

                    // Can use Radar to force the encounter slot to stay consistent across encounters.
                    if (slot.CanUseRadar)
                    {
                        yield return slot;
                        continue;
                    }

                    var frame = wildFrames.Find(s => s.IsSlotCompatibile(slot, pk));
                    if (frame == null)
                    {
                        deferred ??= slot;
                        continue;
                    }
                    yield return slot;
                }
            }

            info.FrameMatches = false;
            if (deferred is EncounterSlot4 x)
                yield return x;

            if (partial is EncounterSlot4 y)
            {
                var frame = wildFrames.Find(s => s.IsSlotCompatibile(y, pk));
                info.FrameMatches = frame != null;
                yield return y;
            }
        }

        // do static encounters if they were deferred to end, spit out any possible encounters for invalid pk
        if (!safariSport)
            yield break;
        {
            if (game is GameVersion.HG or GameVersion.SS)
            {
                foreach (var enc in Encounters4HGSS.Encounter_HGSS)
                {
                    foreach (var evo in chain)
                    {
                        if (evo.Species != enc.Species)
                            continue;
                        if (!enc.IsMatchExact(pk, evo))
                            break;

                        var match = enc.GetMatchRating(pk);
                        if (match == PartialMatch)
                            partial ??= enc;
                        else
                            yield return enc;
                        break;
                    }
                }
                var specific = game == GameVersion.HG ? Encounters4HGSS.StaticHG : Encounters4HGSS.StaticSS;
                foreach (var enc in specific)
                {
                    foreach (var evo in chain)
                    {
                        if (evo.Species != enc.Species)
                            continue;
                        if (!enc.IsMatchExact(pk, evo))
                            break;

                        var match = enc.GetMatchRating(pk);
                        if (match == PartialMatch)
                            partial ??= enc;
                        else
                            yield return enc;
                        break;
                    }
                }
                foreach (var enc in Encounters4HGSS.Encounter_PokeWalker)
                {
                    foreach (var evo in chain)
                    {
                        if (evo.Species != enc.Species)
                            continue;
                        if (!enc.IsMatchExact(pk, evo))
                            break;

                        var match = enc.GetMatchRating(pk);
                        if (match == PartialMatch)
                            partial ??= enc;
                        else
                            yield return enc;
                        break;
                    }
                }
            }
            else
            {
                foreach (var enc in Encounters4DPPt.StaticDPPt)
                {
                    foreach (var evo in chain)
                    {
                        if (evo.Species != enc.Species)
                            continue;
                        if (!enc.IsMatchExact(pk, evo))
                            break;

                        var match = enc.GetMatchRating(pk);
                        if (match == PartialMatch)
                            partial ??= enc;
                        else
                            yield return enc;
                        break;
                    }
                }
                if (game is GameVersion.Pt)
                {
                    foreach (var enc in Encounters4DPPt.StaticPt)
                    {
                        foreach (var evo in chain)
                        {
                            if (evo.Species != enc.Species)
                                continue;
                            if (!enc.IsMatchExact(pk, evo))
                                break;

                            var match = enc.GetMatchRating(pk);
                            if (match == PartialMatch)
                                partial ??= enc;
                            else
                                yield return enc;
                            break;
                        }
                    }
                }
                else
                {
                    var specific = game == GameVersion.D ? Encounters4DPPt.StaticD : Encounters4DPPt.StaticP;
                    foreach (var enc in specific)
                    {
                        foreach (var evo in chain)
                        {
                            if (evo.Species != enc.Species)
                                continue;
                            if (!enc.IsMatchExact(pk, evo))
                                break;

                            var match = enc.GetMatchRating(pk);
                            if (match == PartialMatch)
                                partial ??= enc;
                            else
                                yield return enc;
                            break;
                        }
                    }
                    foreach (var enc in Encounters4DPPt.StaticDP)
                    {
                        foreach (var evo in chain)
                        {
                            if (evo.Species != enc.Species)
                                continue;
                            if (!enc.IsMatchExact(pk, evo))
                                break;

                            var match = enc.GetMatchRating(pk);
                            if (match == PartialMatch)
                                partial ??= enc;
                            else
                                yield return enc;
                            break;
                        }
                    }
                }
            }
        }

        if (partial is not null)
            yield return partial;
    }

    private static List<Frame> AnalyzeFrames(PKM pk, LegalInfo info)
    {
        return FrameFinder.GetFrames(info.PIDIV, pk).ToList();
    }

    private static EncounterArea4[] GetAreas(GameVersion gameSource) => gameSource switch
    {
        GameVersion.D => Encounters4DPPt.SlotsD,
        GameVersion.P => Encounters4DPPt.SlotsP,
        GameVersion.Pt => Encounters4DPPt.SlotsPt,
        GameVersion.HG => Encounters4HGSS.SlotsHG,
        GameVersion.SS => Encounters4HGSS.SlotsSS,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private const int Generation = 4;
    private const EntityContext Context = EntityContext.Gen4;
    private const byte EggLevel = 1;

    private static IEnumerable<EncounterEgg> GetEggs(EvoCriteria[] chain, GameVersion version)
    {
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            yield break;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            yield break; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            yield break;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            yield break;
        // Sanity Check 3
        if (!PersonalTable.HGSS.IsPresentInGame(species, form))
            yield break;

        yield return CreateEggEncounter(species, form, version);
        // Version is not updated when hatching an Egg in Gen4. Version is a clear indicator of the game it originated on.

        // Check for split-breed
        if (species == devolved.Species)
        {
            if (chain.Length < 2)
                yield break; // no split-breed
            devolved = chain[^2];
        }
        if (!Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            yield break;

        species = devolved.Species;
        form = devolved.Form;
        yield return CreateEggEncounter(species, form, version);
    }

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves4.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }
}
