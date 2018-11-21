﻿using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EvolutionChain
    {
        private static readonly List<EvoCriteria> NONE = new List<EvoCriteria>(0);

        internal static IReadOnlyList<EvoCriteria>[] GetEvolutionChainsAllGens(PKM pkm, IEncounterable Encounter)
        {
            var CompleteEvoChain = GetEvolutionChain(pkm, Encounter, pkm.Species, pkm.CurrentLevel);
            if (Encounter is EncounterInvalid || pkm.IsEgg || CompleteEvoChain.Count == 0)
                return GetChainSingle(pkm, CompleteEvoChain);

            return GetChainAll(pkm, Encounter, CompleteEvoChain);
        }

        private static List<EvoCriteria>[] GetChainBase(int maxgen)
        {
            var GensEvoChains = new List<EvoCriteria>[maxgen + 1];
            for (int i = 0; i <= maxgen; i++)
                GensEvoChains[i] = NONE; // default no-evolutions
            return GensEvoChains;
        }

        private static List<EvoCriteria>[] GetChainSingle(PKM pkm, List<EvoCriteria> CompleteEvoChain)
        {
            var chain = GetChainBase(Math.Max(2, pkm.Format));
            chain[pkm.Format] = CompleteEvoChain;
            return chain;
        }

        private static List<EvoCriteria>[] GetChainAll(PKM pkm, IEncounterable Encounter, IEnumerable<EvoCriteria> CompleteEvoChain)
        {
            int maxgen = pkm is PK1 && !pkm.Gen1_NotTradeback ? 2 : pkm.Format;
            var GensEvoChains = GetChainBase(maxgen);

            var queue = new Queue<EvoCriteria>(CompleteEvoChain);
            var mostEvolved = queue.Dequeue();

            int lvl = pkm.CurrentLevel;
            int maxLevel = lvl;
            int pkGen = pkm.GenNumber;

            // Iterate generations backwards
            // Maximum level of an earlier generation (GenX) will never be greater than a later generation (GenX+Y).
            int mingen = (pkm is PK2 || pkm.VC2) && !pkm.Gen2_NotTradeback ? 1 : pkGen;
            bool noxfrDecremented = true;
            for (int g = GensEvoChains.Length - 1; g >= mingen; g--)
            {
                if (pkGen == 1 && pkm.Gen1_NotTradeback && g == 2)
                    continue;
                if (pkGen <= 2 && 3 <= g && g <= 6)
                    continue;

                if (g <= 4 && 2 < pkm.Format && g < pkm.Format && !pkm.HasOriginalMetLocation && lvl > pkm.Met_Level)
                {
                    // Met location was lost at this point but it also means the pokemon existed in generations 1 to 4 with maximum level equals to met level
                    lvl = pkm.Met_Level;
                }

                int maxspeciesgen = g == 2 && pkm.VC1 ? MaxSpeciesID_1 : GetMaxSpeciesOrigin(g);

                // Remove future gen evolutions after a few special considerations:
                // If the pokemon origin is illegal (e.g. Gen3 Infernape) the list will be emptied -- species lineage did not exist at any evolution stage.
                while (mostEvolved.Species > maxspeciesgen)
                {
                    if (queue.Count == 0)
                    {
                        if (g <= 2 && pkm.VC1)
                            GensEvoChains[pkm.Format] = NONE; // invalidate here since we haven't reached the regular invalidation
                        return GensEvoChains;
                    }
                    if (mostEvolved.RequiresLvlUp)
                    {
                        // This is a gen 3 pokemon in a gen 4 phase evolution that requires level up and then transfered to gen 5+
                        // We can deduce that it existed in gen 4 until met level,
                        // but if current level is met level we can also deduce it existed in gen 3 until maximum met level -1
                        if (g == 3 && pkm.Format > 4 && lvl == maxLevel)
                            lvl--;

                        // The same condition for gen2 evolution of gen 1 pokemon, level of the pokemon in gen 1 games would be CurrentLevel -1 one level below gen 2 level
                        else if (g == 1 && pkm.Format == 2 && lvl == maxLevel)
                            lvl--;
                    }
                    mostEvolved = queue.Dequeue();
                }

                // Alolan form evolutions, remove from gens 1-6 chains
                if (EvolveToAlolanForms.Contains(mostEvolved.Species))
                {
                    if (g < 7 && pkm.Format >= 7 && mostEvolved.Form > 0)
                    {
                        if (queue.Count == 0)
                            break;
                        mostEvolved = queue.Dequeue();
                    }
                }

                GensEvoChains[g] = GetEvolutionChain(pkm, Encounter, mostEvolved.Species, lvl);
                if (GensEvoChains[g].Count == 0)
                    continue;

                if (g > 2 && !pkm.HasOriginalMetLocation && g >= pkGen && noxfrDecremented)
                {
                    bool isTransferred = HasMetLocationUpdatedTransfer(pkGen, g);
                    if (!isTransferred)
                        continue;

                    noxfrDecremented = g > (pkGen != 3 ? 4 : 5);

                    // Remove previous evolutions below transfer level
                    // For example a gen3 Charizard in format 7 with current level 36 and met level 36, thus could never be Charmander / Charmeleon in Gen5+.
                    // chain level for charmander is 35, is below met level.
                    int minlvl = GetMinLevelGeneration(pkm, g);
                    GensEvoChains[g].RemoveAll(e => e.Level < minlvl);
                }
                else if (g == 2 && pkm.TradebackStatus == TradebackType.Gen1_NotTradeback)
                {
                    GensEvoChains[2] = NONE;
                }
                else if (g == 1)
                {
                    // Remove Gen2 post-evolutions (Scizor, Blissey...)
                    if (GensEvoChains[1][0].Species > MaxSpeciesID_1)
                        GensEvoChains[1].RemoveAt(0);

                    // Remove Gen2 pre-evolutions (Pichu, Cleffa...)
                    int lastIndex = GensEvoChains[1].Count - 1;
                    if (lastIndex >= 0 && GensEvoChains[1][lastIndex].Species > MaxSpeciesID_1)
                        GensEvoChains[1].RemoveAt(lastIndex);

                    // Remove Gen7 pre-evolutions and chain break scenarios
                    if (pkm.VC1)
                        TrimVC1Transfer(pkm, GensEvoChains);
                }
            }
            return GensEvoChains;
        }

        private static void TrimVC1Transfer(PKM pkm, IList<List<EvoCriteria>> GensEvoChains)
        {
            if (GensEvoChains[7].All(z => z.Species > MaxSpeciesID_1))
                GensEvoChains[pkm.Format] = NONE; // need a Gen1 present; invalidate the chain.
        }

        internal static int GetEvoChainSpeciesIndex(IReadOnlyList<EvoCriteria> chain, int species)
        {
            for (int i = 0; i < chain.Count; i++)
            {
                if (chain[i].Species == species)
                    return i;
            }
            return -1;
        }

        private static List<EvoCriteria> GetEvolutionChain(PKM pkm, IEncounterable Encounter, int maxspec, int maxlevel)
        {
            var vs = GetValidPreEvolutions(pkm);
            if (Encounter.Species == maxspec)
            {
                if (vs.Count != 1)
                {
                    vs.RemoveAll(z => z.Species != Encounter.Species);
                    vs[0].MinLevel = Encounter.LevelMin;
                }
                return vs;
            }

            // Evolution chain is in reverse order (devolution)
            // Find the index of the minimum species to determine the end of the chain
            int minindex = GetEvoChainSpeciesIndex(vs, Encounter.Species);
            bool last = minindex < 0 || minindex == vs.Count - 1;

            // If we remove a pre-evolution, update the chain if appropriate.
            if (!last)
            {
                // Remove chain species after the encounter
                int count = vs.Count;
                for (int i = minindex + 1; i < count; i++)
                    vs.RemoveAt(i);

                if (vs.Count == 0)
                    return vs; // no species left in chain
                CheckLastEncounterRemoval(Encounter, vs);
            }

            // maxspec is used to remove future gen evolutions, to gather evolution chain of a pokemon in previous generations
            int skip = Math.Max(0, GetEvoChainSpeciesIndex(vs, maxspec));
            for (int i = 0; i < skip; i++)
                vs.RemoveAt(0);

            // Maxlevel is also used for previous generations, it removes evolutions imposible before the transfer level
            // For example a fire red charizard whose current level in XY is 50 but met level is 20, it couldnt be a Charizard in gen 3 and 4 games
            vs.RemoveAll(e => e.MinLevel > maxlevel);

            // Reduce the evolution chain levels to max level to limit any later analysis/results.
            foreach (var d in vs)
                d.Level = Math.Min(d.Level, maxlevel);

            return vs;
        }

        private static void CheckLastEncounterRemoval(IEncounterable Encounter, IReadOnlyList<EvoCriteria> vs)
        {
            // Last entry from vs is removed, turn next entry into the encountered Pokémon
            var last = vs[vs.Count - 1];
            last.MinLevel = Encounter.LevelMin;
            last.RequiresLvlUp = false;

            var first = vs[0];
            if (first.RequiresLvlUp)
                return;

            if (first.MinLevel == 2)
            {
                // Example: Raichu in Gen 2 or later
                // Because Pichu requires a level up, the minimum level of the resulting Raichu must be be >2
                // But after removing Pichu (because the origin species is Pikachu), the Raichu minimum level should be 1.
                first.MinLevel = 1;
                first.RequiresLvlUp = false;
            }
            else // ingame trade / stone can evolve immediately
            {
                first.MinLevel = last.MinLevel;
            }
        }

        internal static List<EvoCriteria> GetValidPreEvolutions(PKM pkm, int maxspeciesorigin = -1, int lvl = -1, bool skipChecks = false)
        {
            if (lvl < 0)
                lvl = pkm.CurrentLevel;
            if (pkm.IsEgg && !skipChecks)
            {
                return new List<EvoCriteria>
                {
                    new EvoCriteria { Species = pkm.Species, Level = lvl, MinLevel = lvl },
                };
            }

            if (pkm.Species == 292 && lvl >= 20 && (!pkm.HasOriginalMetLocation || pkm.Met_Level + 1 <= lvl))
            {
                return new List<EvoCriteria>
                {
                    new EvoCriteria { Species = 292, Level = lvl, MinLevel = 20 },
                    new EvoCriteria { Species = 290, Level = lvl, MinLevel = 1 }
                };
            }

            if (maxspeciesorigin == -1 && pkm.InhabitedGeneration(2) && pkm.Format <= 2 && pkm.GenNumber == 1)
                maxspeciesorigin = MaxSpeciesID_2;

            int tree = maxspeciesorigin == MaxSpeciesID_2 ? 2 : pkm.Format;
            var et = EvolutionTree.GetEvolutionTree(pkm, tree);
            return et.GetValidPreEvolutions(pkm, maxLevel: lvl, maxSpeciesOrigin: maxspeciesorigin, skipChecks: skipChecks);
        }

        private static int GetMinLevelGeneration(PKM pkm, int generation)
        {
            if (!pkm.InhabitedGeneration(generation))
                return 0;

            if (pkm.Format <= 2)
                return 2;

            if (!pkm.HasOriginalMetLocation && generation != pkm.GenNumber)
                return pkm.Met_Level;

            if (pkm.GenNumber <= 3)
                return 2;

            return 1;
        }
    }
}
