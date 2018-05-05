﻿using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class GameData
    {
        public static Learnset[] GetLearnsets(GameVersion game) => Learnsets[game];
        public static PersonalTable GetPersonal(GameVersion game) => Personal[game];

        private static readonly Dictionary<GameVersion, Learnset[]> Learnsets = new Dictionary<GameVersion, Learnset[]>
        {
            { GameVersion.RD, Legal.LevelUpRB },
            { GameVersion.BU, Legal.LevelUpRB },
            { GameVersion.GN, Legal.LevelUpRB },
            { GameVersion.YW, Legal.LevelUpY },
            { GameVersion.GD, Legal.LevelUpGS },
            { GameVersion.SV, Legal.LevelUpGS },
            { GameVersion.C, Legal.LevelUpC },

            { GameVersion.R, Legal.LevelUpRS },
            { GameVersion.S, Legal.LevelUpRS },
            { GameVersion.E, Legal.LevelUpE },
            { GameVersion.FR, Legal.LevelUpFR },
            { GameVersion.LG, Legal.LevelUpLG },
            { GameVersion.COLO, Legal.LevelUpE },
            { GameVersion.XD, Legal.LevelUpE },

            { GameVersion.D, Legal.LevelUpDP },
            { GameVersion.P, Legal.LevelUpDP },
            { GameVersion.Pt, Legal.LevelUpPt },
            { GameVersion.HG, Legal.LevelUpHGSS },
            { GameVersion.SS, Legal.LevelUpHGSS },

            { GameVersion.B, Legal.LevelUpBW },
            { GameVersion.W, Legal.LevelUpBW },
            { GameVersion.B2, Legal.LevelUpB2W2 },
            { GameVersion.W2, Legal.LevelUpB2W2 },

            { GameVersion.X, Legal.LevelUpXY },
            { GameVersion.Y, Legal.LevelUpXY },
            { GameVersion.AS, Legal.LevelUpAO },
            { GameVersion.OR, Legal.LevelUpAO },

            { GameVersion.SN, Legal.LevelUpSM },
            { GameVersion.MN, Legal.LevelUpSM },
            { GameVersion.US, Legal.LevelUpUSUM },
            { GameVersion.UM, Legal.LevelUpUSUM },

            { GameVersion.RB, Legal.LevelUpRB },
            { GameVersion.RBY, Legal.LevelUpY },
            { GameVersion.GSC, Legal.LevelUpC },
            { GameVersion.RS, Legal.LevelUpRS },
            { GameVersion.FRLG, Legal.LevelUpE },
            { GameVersion.CXD, Legal.LevelUpE },
            { GameVersion.DP, Legal.LevelUpDP },
            { GameVersion.HGSS, Legal.LevelUpHGSS },
            { GameVersion.BW, Legal.LevelUpBW },
            { GameVersion.B2W2, Legal.LevelUpB2W2 },
            { GameVersion.XY, Legal.LevelUpXY },
            { GameVersion.ORAS, Legal.LevelUpAO },
            { GameVersion.SM, Legal.LevelUpSM },
            { GameVersion.USUM, Legal.LevelUpUSUM },

            { GameVersion.Gen1, Legal.LevelUpY },
            { GameVersion.Gen2, Legal.LevelUpC },
            { GameVersion.Gen3, Legal.LevelUpE },
            { GameVersion.Gen4, Legal.LevelUpHGSS },
            { GameVersion.Gen5, Legal.LevelUpB2W2 },
            { GameVersion.Gen6, Legal.LevelUpAO },
            { GameVersion.Gen7, Legal.LevelUpSM },
            { GameVersion.VCEvents, Legal.LevelUpY },
        };
        private static readonly Dictionary<GameVersion, PersonalTable> Personal = new Dictionary<GameVersion, PersonalTable>
        {
            { GameVersion.RD, PersonalTable.RB },
            { GameVersion.BU, PersonalTable.RB },
            { GameVersion.GN, PersonalTable.RB },
            { GameVersion.YW, PersonalTable.Y },
            { GameVersion.GD, PersonalTable.GS },
            { GameVersion.SV, PersonalTable.GS },
            { GameVersion.C, PersonalTable.C },

            { GameVersion.R, PersonalTable.RS },
            { GameVersion.S, PersonalTable.RS },
            { GameVersion.E, PersonalTable.E },
            { GameVersion.FR, PersonalTable.FR },
            { GameVersion.LG, PersonalTable.LG },
            { GameVersion.COLO, PersonalTable.E },
            { GameVersion.XD, PersonalTable.E },

            { GameVersion.D, PersonalTable.DP },
            { GameVersion.P, PersonalTable.DP },
            { GameVersion.Pt, PersonalTable.Pt },
            { GameVersion.HG, PersonalTable.HGSS },
            { GameVersion.SS, PersonalTable.HGSS },

            { GameVersion.B, PersonalTable.BW },
            { GameVersion.W, PersonalTable.BW },
            { GameVersion.B2, PersonalTable.B2W2 },
            { GameVersion.W2, PersonalTable.B2W2 },

            { GameVersion.X, PersonalTable.XY },
            { GameVersion.Y, PersonalTable.XY },
            { GameVersion.AS, PersonalTable.AO },
            { GameVersion.OR, PersonalTable.AO },

            { GameVersion.SN, PersonalTable.SM },
            { GameVersion.MN, PersonalTable.SM },
            { GameVersion.US, PersonalTable.USUM },
            { GameVersion.UM, PersonalTable.USUM },

            { GameVersion.RB, PersonalTable.RB },
            { GameVersion.RBY, PersonalTable.Y },
            { GameVersion.GSC, PersonalTable.C },
            { GameVersion.RS, PersonalTable.RS },
            { GameVersion.FRLG, PersonalTable.E },
            { GameVersion.CXD, PersonalTable.E },
            { GameVersion.DP, PersonalTable.DP },
            { GameVersion.HGSS, PersonalTable.HGSS },
            { GameVersion.BW, PersonalTable.BW },
            { GameVersion.B2W2, PersonalTable.B2W2 },
            { GameVersion.XY, PersonalTable.XY },
            { GameVersion.ORAS, PersonalTable.AO },
            { GameVersion.SM, PersonalTable.SM },
            { GameVersion.USUM, PersonalTable.USUM },

            { GameVersion.Gen1, PersonalTable.Y },
            { GameVersion.Gen2, PersonalTable.C },
            { GameVersion.Gen3, PersonalTable.E },
            { GameVersion.Gen4, PersonalTable.HGSS },
            { GameVersion.Gen5, PersonalTable.B2W2 },
            { GameVersion.Gen6, PersonalTable.AO },
            { GameVersion.Gen7, PersonalTable.SM },
            { GameVersion.VCEvents, PersonalTable.Y },
        };
    }
}
