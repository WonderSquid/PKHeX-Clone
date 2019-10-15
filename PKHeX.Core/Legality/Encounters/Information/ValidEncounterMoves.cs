﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Object which stores information useful for analyzing a moveset relative to the encounter data.
    /// </summary>
    public sealed class ValidEncounterMoves
    {
        public IReadOnlyList<int>[] LevelUpMoves { get; } = Empty;
        public IReadOnlyList<int>[] TMHMMoves { get; } = Empty;
        public IReadOnlyList<int>[] TutorMoves { get; } = Empty;
        public int[] Relearn = Array.Empty<int>();

        private const int EmptyCount = PKX.Generation + 1; // one for each generation index (and 0th)
        private static readonly List<int>[] Empty = new int[EmptyCount].Select(_ => new List<int>()).ToArray();

        public ValidEncounterMoves(PKM pkm, LevelUpRestriction restrict)
        {
            LevelUpMoves = Legal.GetValidMovesAllGens(pkm, restrict.EvolutionChains, minLvLG1: restrict.MinimumLevelGen1, minLvLG2: restrict.MinimumLevelGen2, Tutor: false, Machine: false, RemoveTransferHM: false);
            TMHMMoves = Legal.GetValidMovesAllGens(pkm, restrict.EvolutionChains, LVL: false, Tutor: false, MoveReminder: false, RemoveTransferHM: false);
            TutorMoves = Legal.GetValidMovesAllGens(pkm, restrict.EvolutionChains, LVL: false, Machine: false, MoveReminder: false, RemoveTransferHM: false);
        }

        public ValidEncounterMoves(List<int>[] levelup)
        {
            LevelUpMoves = levelup;
        }
        public ValidEncounterMoves()
        {
            LevelUpMoves = Array.Empty<int[]>();
        }
    }

    public sealed class LevelUpRestriction
    {
        public readonly IReadOnlyList<EvoCriteria>[] EvolutionChains;
        public readonly int MinimumLevelGen1;
        public readonly int MinimumLevelGen2;

        public LevelUpRestriction(PKM pkm, LegalInfo info)
        {
            MinimumLevelGen1 = info.Generation <= 2 ? info.EncounterMatch.LevelMin + 1 : 0;
            MinimumLevelGen2 = ParseSettings.AllowGen2MoveReminder(pkm) ? 1 : info.EncounterMatch.LevelMin + 1;
            EvolutionChains = info.EvoChainsAllGens;
        }
    }
}
