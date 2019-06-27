﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Bundles raw string inputs into lists that can be used in data binding.
    /// </summary>
    public class GameDataSource
    {
        public static readonly IReadOnlyList<ComboItem> Regions = Util.GetCSVUnsortedCBList("regions3ds");
        public static readonly IReadOnlyList<ComboItem> LanguageList = Util.GetCSVUnsortedCBList("languages");
        public static readonly string[] Languages = LanguageList.GetArray();

        // ignores Poke/Great/Ultra
        private static readonly int[] ball_nums = { 007, 576, 013, 492, 497, 014, 495, 493, 496, 494, 011, 498, 008, 006, 012, 015, 009, 005, 499, 010, 001, 016, 851 };
        private static readonly int[] ball_vals = { 007, 025, 013, 017, 022, 014, 020, 018, 021, 019, 011, 023, 008, 006, 012, 015, 009, 005, 024, 010, 001, 016, 026 };

        public GameDataSource(GameStrings s)
        {
            Source = s;
            BallDataSource = Util.GetVariedCBListBall(s.itemlist, ball_nums, ball_vals);
            SpeciesDataSource = Util.GetCBList(s.specieslist);
            NatureDataSource = Util.GetCBList(s.natures);
            AbilityDataSource = Util.GetCBList(s.abilitylist);
            EncounterTypeDataSource = Util.GetCBList(s.encountertypelist, new[] { 0 }, Legal.Gen4EncounterTypes);

            HaXMoveDataSource = Util.GetCBList(s.movelist);
            LegalMoveDataSource = HaXMoveDataSource.Where(m => !Legal.Z_Moves.Contains(m.Value)).ToList();

            VersionDataSource = GetVersionList(s);
            InitializeMetSources();
            Memories = new MemoryStrings(s);
        }

        public readonly GameStrings Source;
        public readonly MemoryStrings Memories;

        public readonly IReadOnlyList<ComboItem> SpeciesDataSource;
        public readonly IReadOnlyList<ComboItem> BallDataSource;
        public readonly IReadOnlyList<ComboItem> NatureDataSource;
        public readonly IReadOnlyList<ComboItem> AbilityDataSource;
        public readonly IReadOnlyList<ComboItem> VersionDataSource;
        public readonly IReadOnlyList<ComboItem> LegalMoveDataSource;
        public readonly IReadOnlyList<ComboItem> HaXMoveDataSource;
        public readonly IReadOnlyList<ComboItem> EncounterTypeDataSource;

        private IReadOnlyList<ComboItem> MetGen2 { get; set; }
        private IReadOnlyList<ComboItem> MetGen3 { get; set; }
        private IReadOnlyList<ComboItem> MetGen3CXD { get; set; }
        private IReadOnlyList<ComboItem> MetGen4 { get; set; }
        private IReadOnlyList<ComboItem> MetGen5 { get; set; }
        private IReadOnlyList<ComboItem> MetGen6 { get; set; }
        private IReadOnlyList<ComboItem> MetGen7 { get; set; }
        private IReadOnlyList<ComboItem> MetGen7GG { get; set; }

        private IReadOnlyList<ComboItem> GetVersionList(GameStrings s)
        {
            var list = s.gamelist;
            var ver = Util.GetCBList(list,
                Legal.Games_7gg,
                Legal.Games_7usum, Legal.Games_7sm,
                Legal.Games_6oras, Legal.Games_6xy,
                Legal.Games_5, Legal.Games_4, Legal.Games_4e, Legal.Games_4r,
                Legal.Games_3, Legal.Games_3e, Legal.Games_3r, Legal.Games_3s);
            ver.AddRange(Util.GetCBList(list, Legal.Games_7vc1).OrderBy(g => g.Value)); // stuff to end unsorted
            ver.AddRange(Util.GetCBList(list, Legal.Games_7vc2).OrderBy(g => g.Value)); // stuff to end unsorted
            ver.AddRange(Util.GetCBList(list, Legal.Games_7go).OrderBy(g => g.Value)); // stuff to end unsorted
            return ver;
        }

        private void InitializeMetSources()
        {
            var s = Source;
            // Gen 2
            {
                var met_list = Util.GetCBList(s.metGSC_00000, Enumerable.Range(0, 0x5F).ToArray());
                Util.AddCBWithOffset(met_list, s.metGSC_00000, 00000, 0x7E, 0x7F);
                MetGen2 = met_list;
            }
            // Gen 3
            {
                var met_list = Util.GetCBList(s.metRSEFRLG_00000, Enumerable.Range(0, 213).ToArray());
                Util.AddCBWithOffset(met_list, s.metRSEFRLG_00000, 00000, 253, 254, 255);
                MetGen3 = met_list;

                MetGen3CXD = Util.GetCBList(s.metCXD_00000, Enumerable.Range(0, s.metCXD_00000.Length).ToArray()).Where(c => c.Text.Length > 0).ToList();
            }
            // Gen 4
            {
                var met_list = Util.GetCBList(s.metHGSS_00000, 0);
                Util.AddCBWithOffset(met_list, s.metHGSS_02000, 2000, Locations.Daycare4);
                Util.AddCBWithOffset(met_list, s.metHGSS_02000, 2000, Locations.LinkTrade4);
                Util.AddCBWithOffset(met_list, s.metHGSS_03000, 3000, Locations.Ranger4);
                Util.AddCBWithOffset(met_list, s.metHGSS_00000, 0000, Legal.Met_HGSS_0);
                Util.AddCBWithOffset(met_list, s.metHGSS_02000, 2000, Legal.Met_HGSS_2);
                Util.AddCBWithOffset(met_list, s.metHGSS_03000, 3000, Legal.Met_HGSS_3);
                MetGen4 = met_list;
            }
            // Gen 5
            {
                var met_list = Util.GetCBList(s.metBW2_00000, 0);
                Util.AddCBWithOffset(met_list, s.metBW2_60000, 60001, Locations.Daycare5);
                Util.AddCBWithOffset(met_list, s.metBW2_30000, 30001, Locations.LinkTrade5);
                Util.AddCBWithOffset(met_list, s.metBW2_00000, 00000, Legal.Met_BW2_0);
                Util.AddCBWithOffset(met_list, s.metBW2_30000, 30001, Legal.Met_BW2_3);
                Util.AddCBWithOffset(met_list, s.metBW2_40000, 40001, Legal.Met_BW2_4);
                Util.AddCBWithOffset(met_list, s.metBW2_60000, 60001, Legal.Met_BW2_6);
                MetGen5 = met_list;
            }
            // Gen 6
            {
                var met_list = Util.GetCBList(s.metXY_00000, 0);
                Util.AddCBWithOffset(met_list, s.metXY_60000, 60001, Locations.Daycare5);
                Util.AddCBWithOffset(met_list, s.metXY_30000, 30001, Locations.LinkTrade6);
                Util.AddCBWithOffset(met_list, s.metXY_00000, 00000, Legal.Met_XY_0);
                Util.AddCBWithOffset(met_list, s.metXY_30000, 30001, Legal.Met_XY_3);
                Util.AddCBWithOffset(met_list, s.metXY_40000, 40001, Legal.Met_XY_4);
                Util.AddCBWithOffset(met_list, s.metXY_60000, 60001, Legal.Met_XY_6);
                MetGen6 = met_list;
            }
            // Gen 7
            {
                var met_list = Util.GetCBList(s.metSM_00000, 0);
                Util.AddCBWithOffset(met_list, s.metSM_60000, 60001, Locations.Daycare5);
                Util.AddCBWithOffset(met_list, s.metSM_30000, 30001, Locations.LinkTrade6);
                Util.AddCBWithOffset(met_list, s.metSM_00000, 00000, Legal.Met_SM_0);
                Util.AddCBWithOffset(met_list, s.metSM_30000, 30001, Legal.Met_SM_3);
                Util.AddCBWithOffset(met_list, s.metSM_40000, 40001, Legal.Met_SM_4);
                Util.AddCBWithOffset(met_list, s.metSM_60000, 60001, Legal.Met_SM_6);
                MetGen7 = met_list;
            }
            // Gen 7 GG
            {
                var met_list = Util.GetCBList(s.metGG_00000, 0);
                Util.AddCBWithOffset(met_list, s.metGG_60000, 60001, 60002);
                Util.AddCBWithOffset(met_list, s.metGG_30000, 30001, Locations.LinkTrade6);
                Util.AddCBWithOffset(met_list, s.metGG_00000, 00000, Legal.Met_GG_0);
                Util.AddCBWithOffset(met_list, s.metGG_30000, 30001, Legal.Met_GG_3);
                Util.AddCBWithOffset(met_list, s.metGG_40000, 40001, Legal.Met_GG_4);
                Util.AddCBWithOffset(met_list, s.metGG_60000, 60001, Legal.Met_GG_6);
                MetGen7GG = met_list;
            }
        }

        public IReadOnlyList<ComboItem> GetItemDataSource(GameVersion game, int generation, int MaxItemID, IEnumerable<ushort> allowed = null, bool HaX = false)
        {
            var items = Source.GetItemStrings(generation, game);
            return Util.GetCBList(items, (allowed == null || HaX ? Enumerable.Range(0, MaxItemID) : allowed.Select(i => (int)i)).ToArray());
        }

        /// <summary>
        /// Fetches a Met Location list for a <see cref="version"/> that has been transferred away from and overwritten.
        /// </summary>
        /// <param name="version">Origin version</param>
        /// <param name="currentGen">Current savefile generation</param>
        /// <param name="egg">True if an egg location list, false if a regular met location list</param>
        /// <returns>Met location list</returns>
        public IReadOnlyList<ComboItem> GetLocationList(GameVersion version, int currentGen, bool egg = false)
        {
            if (currentGen == 2)
                return MetGen2;

            if (egg && version < GameVersion.W && currentGen >= 5)
                return MetGen4;

            switch (version)
            {
                case GameVersion.CXD:
                    if (currentGen == 3)
                        return MetGen3CXD;
                    break;

                case GameVersion.R:
                case GameVersion.S:
                    if (currentGen == 3)
                        return MetGen3.OrderByDescending(loc => loc.Value <= 87).ToList(); // Ferry
                    break;
                case GameVersion.E:
                    if (currentGen == 3)
                        return MetGen3.OrderByDescending(loc => loc.Value <= 87 || (loc.Value >= 196 && loc.Value <= 212)).ToList(); // Trainer Hill
                    break;
                case GameVersion.FR:
                case GameVersion.LG:
                    if (currentGen == 3)
                        return MetGen3.OrderByDescending(loc => loc.Value > 87 && loc.Value < 197).ToList(); // Celadon Dept.
                    break;

                case GameVersion.D:
                case GameVersion.P:
                    if (currentGen == 4 || (currentGen >= 5 && egg))
                        return MetGen4.Take(4).Concat(MetGen4.Skip(4).OrderByDescending(loc => loc.Value <= 111)).ToList(); // Battle Park
                    break;

                case GameVersion.Pt:
                    if (currentGen == 4 || (currentGen >= 5 && egg))
                        return MetGen4.Take(4).Concat(MetGen4.Skip(4).OrderByDescending(loc => loc.Value <= 125)).ToList(); // Rock Peak Ruins
                    break;

                case GameVersion.HG:
                case GameVersion.SS:
                    if (currentGen == 4 || (currentGen >= 5 && egg))
                        return MetGen4.Take(4).Concat(MetGen4.Skip(4).OrderByDescending(loc => loc.Value > 125 && loc.Value < 234)).ToList(); // Celadon Dept.
                    break;

                case GameVersion.B:
                case GameVersion.W:
                    return MetGen5;

                case GameVersion.B2:
                case GameVersion.W2:
                    return MetGen5.Take(3).Concat(MetGen5.Skip(3).OrderByDescending(loc => loc.Value <= 116)).ToList(); // Abyssal Ruins

                case GameVersion.X:
                case GameVersion.Y:
                    return MetGen6.Take(3).Concat(MetGen6.Skip(3).OrderByDescending(loc => loc.Value <= 168)).ToList(); // Unknown Dungeon

                case GameVersion.OR:
                case GameVersion.AS:
                    return MetGen6.Take(3).Concat(MetGen6.Skip(3).OrderByDescending(loc => loc.Value > 168 && loc.Value <= 354)).ToList(); // Secret Base

                case GameVersion.SN:
                case GameVersion.MN:
                    return MetGen7.Take(3).Concat(MetGen7.Skip(3).OrderByDescending(loc => loc.Value < 200)).ToList(); // Outer Cape

                case GameVersion.US:
                case GameVersion.UM:

                case GameVersion.RD:
                case GameVersion.BU:
                case GameVersion.GN:
                case GameVersion.YW:

                case GameVersion.GD:
                case GameVersion.SV:
                case GameVersion.C:
                    return MetGen7.Take(3).Concat(MetGen7.Skip(3).OrderByDescending(loc => loc.Value < 234)).ToList(); // Dividing Peak Tunnel

                case GameVersion.GP:
                case GameVersion.GE:
                case GameVersion.GO:
                    return MetGen7GG.Take(3).Concat(MetGen7GG.Skip(3).OrderByDescending(loc => loc.Value <= 54)).ToList(); // Pokémon League
            }

            return GetLocationListModified(version, currentGen);
        }

        /// <summary>
        /// Fetches a Met Location list for a <see cref="version"/> that has been transferred away from and overwritten.
        /// </summary>
        /// <param name="version">Origin version</param>
        /// <param name="currentGen">Current savefile generation</param>
        /// <returns>Met location list</returns>
        private IReadOnlyList<ComboItem> GetLocationListModified(GameVersion version, int currentGen)
        {
            if (version <= GameVersion.CXD && currentGen == 4)
            {
                return MetGen4.Where(loc => loc.Value == Locations.Transfer3) // Pal Park to front
                    .Concat(MetGen4.Take(4))
                    .Concat(MetGen4.Skip(4).Where(loc => loc.Value != Locations.Transfer3)).ToList();
            }

            if (version < GameVersion.X && currentGen >= 5) // PokéTransfer to front
            {
                return MetGen5.Where(loc => loc.Value == Locations.Transfer4)
                    .Concat(MetGen5.Take(3))
                    .Concat(MetGen5.Skip(3).Where(loc => loc.Value != Locations.Transfer4)).ToList();
            }

            return Array.Empty<ComboItem>();
        }

        public static IReadOnlyList<ComboItem> LanguageDataSource(int gen)
        {
            var languages = LanguageList.ToList();
            if (gen == 3)
                languages.RemoveAll(l => l.Value >= (int)LanguageID.Korean);
            else if (gen < 7)
                languages.RemoveAll(l => l.Value > (int)LanguageID.Korean);
            return languages;
        }
    }
}