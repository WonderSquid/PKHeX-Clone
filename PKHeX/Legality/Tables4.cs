﻿using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesIndex_4_DP = 500;
        internal const int MaxSpeciesIndex_4_HGSSPt = 507;
        internal const int MaxSpeciesID_4 = 493;
        internal const int MaxMoveID_4 = 467;
        internal const int MaxItemID_4_DP = 464;
        internal const int MaxItemID_4_Pt = 467;
        internal const int MaxItemID_4_HGSS = 536;
        internal const int MaxAbilityID_4 = 123;
        internal const int MaxBallID_4 = 0x18;

        internal static readonly int[] Met_HGSS_0 =
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
            19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45,
            46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72,
            73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99,
            100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121,
            122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143,
            144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165,
            166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187,
            188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
            210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231,
            232, 233, 234,
        };

        internal static readonly int[] Met_HGSS_2 =
        {
            2000, 2001, 2002, 2003, 2004, 2005, 2006, 2008, 2009, 2010, 2011,
            2012, 2013, 2014,
        };

        internal static readonly int[] Met_HGSS_3 =
        {
            3000, 3001, 3002, 3003, 3004, 3005, 3006, 3007, 3008, 3009, 3010,
            3011, 3012, 3013, 3014, 3015, 3016, 3017, 3018, 3019, 3020, 3021, 3022, 3023, 3024, 3025, 3026, 3027, 3028,
            3029, 3030, 3031, 3032, 3033, 3034, 3035, 3036, 3037, 3038, 3039, 3040, 3041, 3042, 3043, 3044, 3045, 3046,
            3047, 3048, 3049, 3050, 3051, 3052, 3053, 3054, 3055, 3056, 3057, 3058, 3059, 3060, 3061, 3062, 3063, 3064,
            3065, 3066, 3067, 3068, 3069, 3070, 3071, 3072, 3073, 3074, 3075, 3076
        };

        #region DP
        internal static readonly ushort[] Pouch_Items_DP = {
            68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 135, 136, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327
        };
        internal static readonly ushort[] Pouch_Key_DP = {
            428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464
        };
        internal static readonly ushort[] Pouch_TMHM_DP = {
            328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427
        };
        internal static readonly ushort[] Pouch_Mail_DP = {
            137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148
        };
        internal static readonly ushort[] Pouch_Medicine_DP = {
            17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54
        };
        internal static readonly ushort[] Pouch_Berries_DP = {
            149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212
        };
        internal static readonly ushort[] Pouch_Ball_DP = {
            1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
        };
        internal static readonly ushort[] Pouch_Battle_DP = {
            55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67
        };
        internal static readonly ushort[] HeldItems_DP = new ushort[1].Concat(Pouch_Items_DP).Concat(Pouch_Mail_DP).Concat(Pouch_Medicine_DP).Concat(Pouch_Berries_DP).Concat(Pouch_Ball_DP).Concat(Pouch_TMHM_DP.Take(Pouch_TMHM_DP.Length - 8)).ToArray();
        #endregion

        #region Pt
        internal static readonly ushort[] Pouch_Items_Pt = {
            68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 135, 136, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327
        };
        internal static readonly ushort[] Pouch_Key_Pt = {
            428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467
        };
        internal static readonly ushort[] Pouch_TMHM_Pt = Pouch_TMHM_DP;
        internal static readonly ushort[] Pouch_Mail_Pt = Pouch_Mail_DP;
        internal static readonly ushort[] Pouch_Medicine_Pt = Pouch_Medicine_DP;
        internal static readonly ushort[] Pouch_Berries_Pt = Pouch_Berries_DP;
        internal static readonly ushort[] Pouch_Ball_Pt = Pouch_Ball_DP;
        internal static readonly ushort[] Pouch_Battle_Pt = Pouch_Battle_DP;

        internal static readonly ushort[] HeldItems_Pt = new ushort[1].Concat(Pouch_Items_Pt).Concat(Pouch_Mail_Pt).Concat(Pouch_Medicine_Pt).Concat(Pouch_Berries_Pt).Concat(Pouch_Ball_Pt).Concat(Pouch_TMHM_Pt.Take(Pouch_TMHM_Pt.Length - 8)).ToArray();
        #endregion

        #region HGSS
        internal static readonly ushort[] Pouch_Items_HGSS = Pouch_Items_Pt;
        internal static readonly ushort[] Pouch_Key_HGSS = {
            434, 435, 437, 444, 445, 446, 447, 450, 456, 464, 465, 466, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 501, 502, 503, 504, 532, 533, 534, 535, 536
        };
        internal static readonly ushort[] Pouch_TMHM_HGSS = Pouch_TMHM_DP;
        internal static readonly ushort[] Pouch_Mail_HGSS = Pouch_Mail_DP;
        internal static readonly ushort[] Pouch_Medicine_HGSS = Pouch_Medicine_DP;
        internal static readonly ushort[] Pouch_Berries_HGSS = Pouch_Berries_DP;
        internal static readonly ushort[] Pouch_Ball_HGSS = {
            1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 492, 493, 494, 495, 496, 497, 498, 499, 500
        };
        internal static readonly ushort[] Pouch_Battle_HGSS = Pouch_Battle_DP;

        internal static readonly ushort[] HeldItems_HGSS = new ushort[1].Concat(Pouch_Items_HGSS).Concat(Pouch_Mail_HGSS).Concat(Pouch_Medicine_HGSS).Concat(Pouch_Berries_HGSS).Concat(Pouch_Ball_Pt).Concat(Pouch_TMHM_HGSS.Take(Pouch_TMHM_HGSS.Length - 8)).ToArray();
        #endregion

        internal static readonly int[] TM_4 =
        {
            264, 337, 352, 347, 046, 092, 258, 339, 331, 237,
            241, 269, 058, 059, 063, 113, 182, 240, 202, 219,
            218, 076, 231, 085, 087, 089, 216, 091, 094, 247,
            280, 104, 115, 351, 053, 188, 201, 126, 317, 332,
            259, 263, 290, 156, 213, 168, 211, 285, 289, 315,
            355, 411, 412, 206, 362, 374, 451, 203, 406, 409,
            261, 318, 373, 153, 421, 371, 278, 416, 397, 148,
            444, 419, 086, 360, 014, 446, 244, 445, 399, 157,
            404, 214, 363, 398, 138, 447, 207, 365, 369, 164,
            430, 433,
        };

        internal static readonly int[] HM_HGSS =
        {
            015, 019, 057, 070, 250, 249, 127, 431 // Defog(DPPt) & Whirlpool(HGSS)
        };

        internal static readonly int[] HM_DPPt =
        {
            015, 019, 057, 070, 432, 249, 127, 431 // Defog(DPPt) & Whirlpool(HGSS)
        };


        internal static readonly int[] HM_4_RemovePokeTransfer =
        {
            015, 019, 057, 070, 249, 127, 431 // Defog(DPPt) & Whirlpool(HGSS) excluded
        };

        internal static readonly int[] MovePP_DP =
        {
            00,
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 30, 30, 35, 35, 20, 15, 20, 20, 15, 20, 30, 05, 25, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 20, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20,
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 25, 15, 10, 40, 25, 10, 35, 30, 15, 20, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20,
            15, 10, 40, 15, 10, 30, 20, 20, 10, 40, 40, 30, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 10, 20, 15, 15, 35, 20, 15, 10, 20, 30, 15, 40, 20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40,
            40, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 01, 01, 10, 10, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 15,
            10, 10, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 05, 15, 20, 10, 05, 05, 15, 15, 15, 15,
            10, 10, 10, 20, 10, 10, 10, 10, 15, 15, 15, 10, 20, 20, 10, 20, 20, 20, 20, 20, 10, 10, 10, 20, 20, 05, 15, 10, 10, 15, 10, 20, 05, 05, 10, 10, 20, 05, 10, 20, 10, 20, 20, 20, 05, 05, 15, 20, 10, 15,
            20, 15, 10, 10, 15, 10, 05, 05, 10, 15, 10, 05, 20, 25, 05, 40, 10, 05, 40, 15, 20, 20, 05, 15, 20, 30, 15, 15, 05, 10, 30, 20, 30, 15, 05, 40, 15, 05, 20, 05, 15, 25, 40, 15, 20, 15, 20, 15, 20, 10,
            20, 20, 05, 05, 10, 05, 40, 10, 10, 05, 10, 10, 15, 10, 20, 30, 30, 10, 20, 05, 10, 10, 15, 10, 10, 05, 15, 05, 10, 10, 30, 20, 20, 10, 10, 05, 05, 10, 05, 20, 10, 20, 10, 15, 10, 20, 20, 20, 15, 15,
            10, 15, 20, 15, 10, 10, 10, 20, 05, 30, 05, 10, 15, 10, 10, 05, 20, 30, 10, 30, 15, 15, 15, 15, 30, 10, 20, 15, 10, 10, 20, 15, 05, 05, 15, 15, 05, 10, 05, 20, 05, 15, 20, 05, 20, 20, 20, 20, 10, 20,
            10, 15, 20, 15, 10, 10, 05, 10, 05, 05, 10, 05, 05, 10, 05, 05, 05,
        };
        internal static readonly int[] WildPokeBalls4_DPPt =
        {
            1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            // Cherish ball not usable
        };
        internal static readonly int[] WildPokeBalls4_HGSS =
        {
            1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            // Cherish ball not usable
            17, 18, 19, 20, 21, 22,
            // Comp Ball not usable in wild
        };

        internal static readonly int[] FutureEvolutionsGen4 =
        {
            700
        };
        internal static readonly int[] UnreleasedItems_4 =
        {
            // todo
        };
        internal static readonly bool[] ReleasedHeldItems_4 = Enumerable.Range(0, MaxItemID_4_HGSS+1).Select(i => HeldItems_HGSS.Contains((ushort)i) && !UnreleasedItems_4.Contains(i)).ToArray();
        internal static readonly int[] CrownBeasts = {251, 243, 244, 245};

        internal static readonly int[] Tutors_4 =
        {
            291, 189, 210, 196, 205, 009, 007, 276,
            008, 442, 401, 466, 380, 173, 180, 314,
            270, 283, 200, 246, 235, 324, 428, 410,
            414, 441, 239, 402, 334, 393, 387, 340,
            271, 257, 282, 389, 129, 253, 162, 220,
            081, 366, 356, 388, 277, 272, 215, 067,
            143, 335, 450,
        };

        internal static readonly int[] SpecialTutors_4 =
        {
            307, 308, 338, 434
        };

        internal static readonly int[][] SpecialTutors_Compatibility_4 =
        {
            new int[] { 006, 157, 257, 392 },
            new int[] { 009, 160, 260, 395 },
            new int[] { 003, 154, 254, 389 },
            new int[] { 147, 148, 149, 230, 329, 330, 334, 371, 372, 373, 380, 381, 384, 443, 444, 445, 483, 484, 487 }
        };

        internal static readonly EncounterStatic[] Encounter_HGSS_Swarm =
        {
            //Swarm 
            //reference http://bulbapedia.bulbagarden.net/wiki/Pokémon_outbreak
            new EncounterStatic { Species = 113, Level = 23, Location = 161, }, //Chansey @ Route 13
            new EncounterStatic { Species = 132, Level = 34, Location = 195, }, //Ditto @ Route 47
            new EncounterStatic { Species = 183, Level = 15, Location = 216, }, //Marill @ Mt. Mortar
            new EncounterStatic { Species = 193, Level = 12, Location = 183, }, //Yanma @ Route 35
            new EncounterStatic { Species = 206, Level = 2, Location = 220, }, //Dunsparce @ Dark Cave
            new EncounterStatic { Species = 206, Level = 3, Location = 220, }, //Dunsparce @ Dark Cave
            new EncounterStatic { Species = 209, Level = 16, Location = 186, }, //Snubbull @ Route 38
            new EncounterStatic { Species = 211, Level = 40, Location = 180, }, //Qwilfish @ Route 32
            new EncounterStatic { Species = 223, Level = 20, Location = 192, }, //Remoraid @ Route 44
            new EncounterStatic { Species = 261, Level = 2, Location = 149, }, //Poochyena @ Route 1
            new EncounterStatic { Species = 278, Level = 35, Location = 143, }, //Wingull @ Vermillion City
            new EncounterStatic { Species = 280, Level = 10, Location = 182, }, //Ralts @ Route 34
            new EncounterStatic { Species = 280, Level = 11, Location = 182, }, //Ralts @ Route 34
            new EncounterStatic { Species = 302, Level = 13, Location = 157, Version = GameVersion.HG,}, //Sableye @ Route 9
            new EncounterStatic { Species = 302, Level = 14, Location = 157, Version = GameVersion.HG,}, //Sableye @ Route 9
            new EncounterStatic { Species = 302, Level = 15, Location = 157, Version = GameVersion.HG,}, //Sableye @ Route 9
            new EncounterStatic { Species = 303, Level = 13, Location = 157, Version = GameVersion.SS,}, //Mawile @ Route 9
            new EncounterStatic { Species = 303, Level = 14, Location = 157, Version = GameVersion.SS,}, //Mawile @ Route 9
            new EncounterStatic { Species = 303, Level = 15, Location = 157, Version = GameVersion.SS,}, //Mawile @ Route 9
            new EncounterStatic { Species = 316, Level = 5, Location = 151, Version = GameVersion.SS,}, //Gulpin @ Route 3
            new EncounterStatic { Species = 333, Level = 23, Location = 193, }, //Swablu @ Route 45
            new EncounterStatic { Species = 340, Level = 10, Location = 128, }, //Whiscash @ Violet City Old Rod
            new EncounterStatic { Species = 340, Level = 20, Location = 128, }, //Whiscash @ Violet City Good Rod
            new EncounterStatic { Species = 340, Level = 40, Location = 128, }, //Whiscash @ Violet City Super Rod
            new EncounterStatic { Species = 343, Level = 5, Location = 151, Version = GameVersion.HG,}, //Baltoy @ Route 3
            new EncounterStatic { Species = 366, Level = 35, Location = 167, }, //Clamperl @ Route 19
            new EncounterStatic { Species = 369, Level = 40, Location = 160, }, //Relicanth @ Route 12
            new EncounterStatic { Species = 370, Level = 20, Location = 175, }, //Luvdisc @ Route 27
            new EncounterStatic { Species = 401, Level = 3, Location = 224, }, //Kricketot @ Viridian Forest
            new EncounterStatic { Species = 427, Level = 8, Location = 173, }, //Buneary @ Route 25
            new EncounterStatic { Species = 427, Level = 9, Location = 173, }, //Buneary @ Route 25
            new EncounterStatic { Species = 427, Level = 10, Location = 173, }, //Buneary @ Route 25
        };

        internal static readonly EncounterStatic[] Encounter_DPPt =
        {
            //Starters
            new EncounterStatic { Gift = true, Species = 387, Level = 5, Location = 076, Version = GameVersion.DP,}, // Turtwig @ Lake Verity
            new EncounterStatic { Gift = true, Species = 390, Level = 5, Location = 076, Version = GameVersion.DP,}, // Chimchar
            new EncounterStatic { Gift = true, Species = 393, Level = 5, Location = 076, Version = GameVersion.DP,}, // Piplup
            new EncounterStatic { Gift = true, Species = 387, Level = 5, Location = 016, Version = GameVersion.Pt,}, // Turtwig @ Route 201
            new EncounterStatic { Gift = true, Species = 390, Level = 5, Location = 016, Version = GameVersion.Pt,}, // Chimchar
            new EncounterStatic { Gift = true, Species = 393, Level = 5, Location = 016, Version = GameVersion.Pt,}, // Piplup

            //Fossil @ Mining Museum
            new EncounterStatic { Gift = true, Species = 138, Level = 20, Location = 094, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 20, Location = 094, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 20, Location = 094, }, // Aerodactyl
            new EncounterStatic { Gift = true, Species = 345, Level = 20, Location = 094, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 20, Location = 094, }, // Anorith
            new EncounterStatic { Gift = true, Species = 408, Level = 20, Location = 094, }, // Cranidos
            new EncounterStatic { Gift = true, Species = 410, Level = 20, Location = 094, }, // Shieldon

            //Gift
            new EncounterStatic { Gift = true, Species = 133, Level = 5, Location = 010, Version = GameVersion.DP,}, //Eevee @ Hearthome City 
            new EncounterStatic { Gift = true, Species = 133, Level = 20, Location = 010, Version = GameVersion.Pt,}, //Eevee @ Hearthome City 
            new EncounterStatic { Gift = true, Species = 137, Level = 25, Location = 012, Version = GameVersion.Pt,}, //Porygon @ Veilstone City
            new EncounterStatic { Gift = true, Species = 175, Level = 1, EggLocation = 2011, Version = GameVersion.Pt,}, //Togepi Egg from Cynthia
            new EncounterStatic { Gift = true, Species = 440, Level = 1, EggLocation = 2009, Version = GameVersion.DP,}, //Happiny Egg from Traveling Man
            new EncounterStatic { Gift = true, Species = 447, Level = 1, EggLocation = 2010,}, //Riolu Egg from Riley

            //Stationary Lengerdary
            new EncounterStatic { Species = 377, Level = 30, Location = 125, Version = GameVersion.Pt,}, //Regirock @ Rock Peak Ruins
            new EncounterStatic { Species = 378, Level = 30, Location = 124, Version = GameVersion.Pt,}, //Regice @ Iceberg Ruins
            new EncounterStatic { Species = 379, Level = 30, Location = 123, Version = GameVersion.Pt,}, //Registeel @ Iron Ruins
            new EncounterStatic { Species = 480, Level = 50, Location = 078,}, //Uxie @ Lake Acuity
            new EncounterStatic { Species = 482, Level = 50, Location = 077,}, //Azelf @ Lake Valor
            new EncounterStatic { Species = 483, Level = 47, Location = 051, Version = GameVersion.D,}, //Dialga @ Spear Pillar
            new EncounterStatic { Species = 483, Level = 70, Location = 051, Version = GameVersion.Pt,}, //Dialga @ Spear Pillar
            new EncounterStatic { Species = 484, Level = 47, Location = 051, Version = GameVersion.P,}, //Palkia @ Spear Pillar
            new EncounterStatic { Species = 484, Level = 70, Location = 051, Version = GameVersion.Pt,}, //Palkia @ Spear Pillar
            new EncounterStatic { Species = 485, Level = 70, Location = 084, Version = GameVersion.DP,}, //Heatran @ Stark Mountain
            new EncounterStatic { Species = 485, Level = 50, Location = 084, Version = GameVersion.Pt,}, //Heatran @ Stark Mountain
            new EncounterStatic { Species = 486, Level = 70, Location = 064, Version = GameVersion.DP,}, //Regigigas @ Snowpoint Temple
            new EncounterStatic { Species = 486, Level = 1, Location = 064, Version = GameVersion.Pt,}, //Regigigas @ Snowpoint Temple
            new EncounterStatic { Species = 487, Form = 0, Level = 70, Location = 062, Version = GameVersion.DP,}, //Giratina @ Turnback Cave
            new EncounterStatic { Species = 487, Form = 1, Level = 47, Location = 117, Version = GameVersion.Pt,}, //Giratina @ Distortion World
            new EncounterStatic { Species = 487, Form = 0, Level = 47, Location = 062, Version = GameVersion.Pt,}, //Giratina @ Turnback Cave

            //Event
            new EncounterStatic { Species = 491, Level = 40, Location = 079, Version = GameVersion.DP,}, //Darkrai @ Newmoon Island
            new EncounterStatic { Species = 491, Level = 50, Location = 079, Version = GameVersion.Pt,}, //Darkrai @ Newmoon Island
            new EncounterStatic { Species = 492, Form = 0, Level = 30, Location = 063,}, //Shaymin @ Flower Paradise
            //new EncounterStatic { Species = 493, Level = 80, Location = 086,}, //Arceus @ Hall of Origin

            //Roaming
            new EncounterStatic { Species = 481, Level = 50, }, //Mesprit
            new EncounterStatic { Species = 488, Level = 50, }, //Cresselia
            new EncounterStatic { Species = 144, Level = 60, Version = GameVersion.Pt, }, //Articuno
            new EncounterStatic { Species = 145, Level = 60, Version = GameVersion.Pt, }, //Zapdos
            new EncounterStatic { Species = 146, Level = 60, Version = GameVersion.Pt, }, //Moltres
        };
        internal static readonly EncounterStatic[] Encounter_HGSS =
        {
            //Starters
            new EncounterStatic { Gift = true, Species = 1, Level = 5,  Location = 138, }, // Bulbasaur @ Pallet Town
            new EncounterStatic { Gift = true, Species = 4, Level = 5,  Location = 138, }, // Charmander
            new EncounterStatic { Gift = true, Species = 9, Level = 5,  Location = 138, }, // Squirtle
            new EncounterStatic { Gift = true, Species = 152, Level = 5,  Location = 126, }, // Chikorita @ New Bark Town
            new EncounterStatic { Gift = true, Species = 155, Level = 5,  Location = 126, }, // Cyndaquil
            new EncounterStatic { Gift = true, Species = 158, Level = 5,  Location = 126, }, // Totodile
            new EncounterStatic { Gift = true, Species = 252, Level = 5,  Location = 148, }, // Treecko @ Saffron City
            new EncounterStatic { Gift = true, Species = 255, Level = 5,  Location = 148, }, // Torchic
            new EncounterStatic { Gift = true, Species = 258, Level = 5,  Location = 148, }, // Mudkip

            //Fossil @ Pewter City
            new EncounterStatic { Gift = true, Species = 138, Level = 20,  Location = 140, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 20,  Location = 140, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 20,  Location = 140, }, // Aerodactyl
            new EncounterStatic { Gift = true, Species = 345, Level = 20,  Location = 140, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 20,  Location = 140, }, // Anorith
            new EncounterStatic { Gift = true, Species = 408, Level = 20,  Location = 140, }, // Cranidos
            new EncounterStatic { Gift = true, Species = 410, Level = 20,  Location = 140, }, // Shieldon

            //Gift
            new EncounterStatic { Gift = true, Species = 133, Level = 5,  Location = 131, }, // Eevee @ Goldenrod City
            new EncounterStatic { Gift = true, Species = 147, Level = 15,  Location = 222, Moves = new[] {245, 086, 239, 082}, }, // Dratini @ Dragon's Den (ExtremeSpeed)
            new EncounterStatic { Gift = true, Species = 147, Level = 15,  Location = 222, Moves = new[] {043, 086, 239, 082}, }, // Dratini @ Dragon's Den (Non-ExtremeSpeed)
            new EncounterStatic { Gift = true, Species = 236, Level = 10,  Location = 216, }, // Tyrogue @ Mt. Mortar
            new EncounterStatic { Gift = true, Species = 175, Level = 1, EggLocation = 2013, Moves = new[] {045, 204, 326, -1},}, // Togepi Egg from Mr. Pokemon (Extrasensory as Egg move)
            new EncounterStatic { Gift = true, Species = 179, Level = 1, EggLocation = 2014,}, // Mareep Egg from Primo
            new EncounterStatic { Gift = true, Species = 194, Level = 1, EggLocation = 2014,}, // Wooper Egg from Primo
            new EncounterStatic { Gift = true, Species = 218, Level = 1, EggLocation = 2014,}, // Slugma Egg from Primo

            //Stationary
            new EncounterStatic { Species = 130, Level = 30, Shiny = true ,Location = 135, }, //Gyarados @ Lake of Rage
            new EncounterStatic { Species = 131, Level = 20, Location = 210, }, //Lapras @ Union Cave Friday Only
            new EncounterStatic { Species = 143, Level = 50, Location = 159, }, //Snorlax @ Route 11
            new EncounterStatic { Species = 143, Level = 50, Location = 160, }, //Snorlax @ Route 12
            new EncounterStatic { Species = 185, Level = 20, Location = 184, }, //Sudowoodo @ Route 36

            //Stationary Lengerdary
            new EncounterStatic { Species = 144, Level = 50, Location = 203, }, //Articuno @ Seafoam Islands
            new EncounterStatic { Species = 145, Level = 50, Location = 158, }, //Zapdos @ Route 10
            new EncounterStatic { Species = 146, Level = 50, Location = 137, }, //Moltres @ Mt. Silver
            new EncounterStatic { Species = 150, Level = 70, Location = 199, }, //Mewtwo @ Cerulean Cave
            new EncounterStatic { Species = 245, Level = 40, Location = 173, }, //Suicune @ Route 25
            new EncounterStatic { Species = 245, Level = 40, Location = 206, }, //Suicune @ Burned Tower
            new EncounterStatic { Species = 249, Level = 45, Location = 218, Version = GameVersion.SS, }, //Lugia @ Whirl Islands
            new EncounterStatic { Species = 249, Level = 70, Location = 218, Version = GameVersion.HG, }, //Lugia @ Whirl Islands
            new EncounterStatic { Species = 250, Level = 45, Location = 205, Version = GameVersion.HG, }, //Ho-Oh @ Bell Tower
            new EncounterStatic { Species = 250, Level = 70, Location = 205, Version = GameVersion.SS, }, //Ho-Oh @ Bell Tower
            new EncounterStatic { Species = 380, Level = 40, Location = 140, Version = GameVersion.SS, }, //Latias @ Pewter City
            new EncounterStatic { Species = 381, Level = 40, Location = 140, Version = GameVersion.HG, }, //Latios @ Pewter City
            new EncounterStatic { Species = 382, Level = 50, Location = 232, Version = GameVersion.HG, }, //Kyogre @ Embedded Tower
            new EncounterStatic { Species = 383, Level = 50, Location = 232, Version = GameVersion.SS, }, //Groudon @ Embedded Tower
            new EncounterStatic { Species = 384, Level = 50, Location = 232, }, //Rayquaza @ Embedded Tower
            new EncounterStatic { Species = 483, Level = 1, Location = 231, }, //Dialga @ Sinjoh Ruins
            new EncounterStatic { Species = 484, Level = 1, Location = 231, }, //Palkia @ Sinjoh Ruins
            new EncounterStatic { Species = 487, Level = 1, Location = 231, Form = 1}, //Giratina @ Sinjoh Ruins

            //Roaming
            new EncounterStatic { Species = 243, Level = 40, }, //Raikou
            new EncounterStatic { Species = 244, Level = 40, }, //Entei
            new EncounterStatic { Species = 380, Level = 35, Version = GameVersion.HG, }, //Latias
            new EncounterStatic { Species = 381, Level = 35, Version = GameVersion.SS, }, //Latios
        };
        internal static readonly EncounterTrade[] TradeGift_DPPt =
        {
            new EncounterTrade { Species = 063, Ability = 1, TID = 25643, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {15,15,15,25,25,20}, Nature = Nature.Quiet,}, // Abra
            new EncounterTrade { Species = 441, Ability = 2, TID = 44142, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,20,15,25,15,25}, Nature = Nature.Lonely, Contest = new[] {20,20,20,20,20,0} }, // Chatot
            new EncounterTrade { Species = 093, Ability = 1, TID = 19248, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {20,25,15,15,15,25}, Nature = Nature.Hasty,}, // Haunter
            new EncounterTrade { Species = 129, Ability = 1, TID = 53277, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,25,15,25,15,20}, Nature = Nature.Mild}, // Magikarp
        };
        internal static readonly EncounterTrade[] TradeGift_HGSS =
        {
            new EncounterTrade { Species = 095, Ability = 2, TID = 48926, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {25,20,25,15,15,15}, Nature = Nature.Hasty,}, // Onix
            new EncounterTrade { Species = 066, Ability = 1, TID = 37460, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,25,20,15,15,20}, Nature = Nature.Lonely,}, // Machop
            new EncounterTrade { Species = 100, Ability = 2, TID = 29189, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {15,20,15,25,15,25}, Nature = Nature.Hardy,}, // Voltorb
            new EncounterTrade { Species = 085, Ability = 1, TID = 00283, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,15,15,15}, Nature = Nature.Impish,}, // Dodrio
            new EncounterTrade { Species = 082, Ability = 1, TID = 50082, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {15,20,15,20,20,20}, Nature = Nature.Impish,}, // Magneton
            new EncounterTrade { Species = 178, Ability = 1, TID = 15616, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {15,20,15,20,20,20}, Nature = Nature.Modest,}, // Xatu
            new EncounterTrade { Species = 025, Ability = 1, TID = 33038, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {20,25,18,25,13,31}, Nature = Nature.Jolly,}, // Pikachu
            new EncounterTrade { Species = 374, Ability = 1, TID = 23478, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {28,29,24,24,25,23}, Nature = Nature.Brave,}, // Beldum
            new EncounterTrade { Species = 111, Ability = 1, TID = 06845, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {22,31,13,22,9,0}, Nature = Nature.Relaxed, Moves= new[]{422,-1,-1,-1} }, // Rhyhorn
            new EncounterTrade { Species = 208, Ability = 1, TID = 26491, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {8,30,28,18,20,6}, Nature = Nature.Brave,}, // Steelix

            //Gift
            new EncounterTrade { Species = 021, Ability = 1, TID = 01001, SID = 00000, OTGender = 0, Gender = 1, Nature = Nature.Hasty,   Level = 20, Location = 183, Moves= new[]{043,031,228,332}},//Webster's Spearow
            new EncounterTrade { Species = 213, Ability = 2, TID = 04336, SID = 00000, OTGender = 0, Gender = 1, Nature = Nature.Relaxed, Level = 20, Location = 130, Moves= new[]{132,117,227,219}},//Kirk's Shuckle
        };

        // Encounter Slots that are replaced
        internal static readonly int[] Slot4_Time = {2, 3};
        internal static readonly int[] Slot4_Sound = {4, 5};
        internal static readonly int[] Slot4_Radar = {6, 7, 10, 11};
        internal static readonly int[] Slot4_Dual = {8, 9};
        #region Alt Slots
        private static readonly EncounterArea[] SlotsDPPPtAlt =
        {
            new EncounterArea {
                Location = 50, // Mount Coronet
                Slots = new[]
                {
                     new EncounterSlot { Species = 349, LevelMin = 10, LevelMax = 20, Type = SlotType.Old_Rod }, // Feebas
                     new EncounterSlot { Species = 349, LevelMin = 10, LevelMax = 20, Type = SlotType.Good_Rod }, // Feebas
                     new EncounterSlot { Species = 349, LevelMin = 10, LevelMax = 20, Type = SlotType.Super_Rod }, // Feebas
                },},
            new EncounterArea {
                Location = 53, //Solaceon Ruins
                Slots = new[]
                {
                    //new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 0 }, // Unown A
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 1 }, // Unown B
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 2 }, // Unown C
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 3 }, // Unown D
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 4 }, // Unown E
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 5 }, // Unown F
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 6 }, // Unown G
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 7 }, // Unown H
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 8 }, // Unown I
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 9 }, // Unown J
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 10 }, // Unown K
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 11 }, // Unown L
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 12 }, // Unown M
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 13 }, // Unown N
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 14 }, // Unown O
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 15 }, // Unown P
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 16 }, // Unown Q
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 17 }, // Unown R
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 18 }, // Unown S
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 19 }, // Unown T
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 20 }, // Unown U
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 21 }, // Unown V
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 22 }, // Unown W
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 23 }, // Unown X
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 24 }, // Unown Y
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 25 }, // Unown !
                    new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = 26 }, // Unown ?
                },},
        };

        private static readonly EncounterArea[] SlotsHGSSAlt =
        {
            new EncounterArea {
                Location = 209, // Ruins of Alph
                Slots = new[]
                {
                    //new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 0 }, // Unown A
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 1 }, // Unown B
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 2 }, // Unown C
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 3 }, // Unown D
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 4 }, // Unown E
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 5 }, // Unown F
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 6 }, // Unown G
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 7 }, // Unown H
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 8 }, // Unown I
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 9 }, // Unown J
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 10 }, // Unown K
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 11 }, // Unown L
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 12 }, // Unown M
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 13 }, // Unown N
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 14 }, // Unown O
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 15 }, // Unown P
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 16 }, // Unown Q
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 17 }, // Unown R
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 18 }, // Unown S
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 19 }, // Unown T
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 20 }, // Unown U
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 21 }, // Unown V
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 22 }, // Unown W
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 23 }, // Unown X
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 24 }, // Unown Y
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 25 }, // Unown !
                    new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = 26 }, // Unown ?
                },},
        };

        private static readonly EncounterArea SlotsPt_HoneyTree =
            new EncounterArea
            {
                Slots = new[]
                {
                    new EncounterSlot { Species = 190, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree }, // Aipom 
                    new EncounterSlot { Species = 214, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree }, // Heracross
                    new EncounterSlot { Species = 265, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree }, // Wurmple
                    new EncounterSlot { Species = 412, LevelMin = 5, LevelMax = 15, Form = 0, Type = SlotType.HoneyTree }, // Burmy Plant Cloak
                    new EncounterSlot { Species = 415, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree }, // Combee 
                    new EncounterSlot { Species = 420, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree }, // Cheruby
                    new EncounterSlot { Species = 446, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree }, // Munchlax 
                },
            };

        private static readonly EncounterArea SlotsD_HoneyTree =
            new EncounterArea {
                Slots = SlotsPt_HoneyTree.Slots.Concat( new[]
                {
                    new EncounterSlot { Species = 266, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree }, // Silcoon
                }).ToArray()
        };

        private static readonly EncounterArea SlotsP_HoneyTree =
            new EncounterArea
            {
                Slots = SlotsPt_HoneyTree.Slots.Concat(new[]
                {
                    new EncounterSlot { Species = 268, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree }, // Cascoon
                }).ToArray()
            };

        private static readonly int[] HoneyTreesLocation = new[]
        {
            20, // Route 205
            21, // Route 206
            22, // Route 207
            23, // Route 208
            24, // Route 209
            25, // Route 210
            26, // Route 211 
            27, // Route 212 
            28, // Route 213
            29, // Route 214
            30, // Route 215
            33, // Route 218
            36, // Route 221
            37, // Route 222
            47, // Valley Windworks 
            49, // Fuego Ironworks
            58, //Floaroma Meadow
        };

        private static readonly EncounterArea[] SlotsDPPT_Swarm =
        {
            //Swarm 
            //reference http://bulbapedia.bulbagarden.net/wiki/Pokémon_outbreak
            new EncounterArea
            {
                Location = 16, // Route 201
                Slots = new[]
                {
                     new EncounterSlot { Species = 084, Type = SlotType.Grass }, // Doduo
                },
            },
            new EncounterArea
            {
                Location = 17, // Route 202
                Slots = new[]
                {
                     new EncounterSlot { Species = 263, Type = SlotType.Grass }, // Zigzagoon
                },
            },
            new EncounterArea
            {
                Location = 18, // Route 203
                Slots = new[]
                {
                     new EncounterSlot { Species = 104, Type = SlotType.Grass }, // Cubone
                },
            },
            new EncounterArea
            {
                Location = 22, // Route 207
                Slots = new[]
                {
                     new EncounterSlot { Species = 231, Type = SlotType.Grass }, // Phanpy
                },
            },
            new EncounterArea
            {
                Location = 23, // Route 208
                Slots = new[]
                {
                     new EncounterSlot { Species = 206, Type = SlotType.Grass }, // Dunsparce
                },
            },
            new EncounterArea
            {
                Location = 24, // Route 209
                Slots = new[]
                {
                     new EncounterSlot { Species = 209, Type = SlotType.Grass }, // Snubbull
                },
            },
            new EncounterArea
            {
                Location = 30, // Route 215
                Slots = new[]
                {
                     new EncounterSlot { Species = 096, Type = SlotType.Grass }, // Drowzee
                },
            },
            new EncounterArea
            {
                Location = 31, // Route 216
                Slots = new[]
                {
                     new EncounterSlot { Species = 225, Type = SlotType.Grass }, // Delibird
                },
            },
            new EncounterArea
            {
                Location = 33, // Route 218
                Slots = new[]
                {
                     new EncounterSlot { Species = 100, Type = SlotType.Grass }, // Voltorb
                },
            },
            new EncounterArea
            {
                Location = 36, // Route 221
                Slots = new[]
                {
                     new EncounterSlot { Species = 081, Type = SlotType.Grass }, // Farfetch'd
                },
            },
            new EncounterArea
            {
                Location = 37, // Route 222
                Slots = new[]
                {
                     new EncounterSlot { Species = 300, Type = SlotType.Grass }, // Skitty
                },
            },
            new EncounterArea
            {
                Location = 39, // Route 224
                Slots = new[]
                {
                     new EncounterSlot { Species = 177, Type = SlotType.Grass }, // Natu
                     new EncounterSlot { Species = 325, Type = SlotType.Grass }, // Spoink
                },
            },
            new EncounterArea
            {
                Location = 40, // Route 225
                Slots = new[]
                {
                     new EncounterSlot { Species = 296, Type = SlotType.Grass }, // Makuhita
                },
            },
            new EncounterArea
            {
                Location = 41, // Route 226
                Slots = new[]
                {
                     new EncounterSlot { Species = 098, Type = SlotType.Grass }, // Krabby
                },
            },
            new EncounterArea
            {
                Location = 42, // Route 227
                Slots = new[]
                {
                     new EncounterSlot { Species = 327, Type = SlotType.Grass }, // Spinda
                },
            },
            new EncounterArea
            {
                Location = 43, // Route 228
                Slots = new[]
                {
                     new EncounterSlot { Species = 374, Type = SlotType.Grass }, // Beldum
                },
            },
            new EncounterArea
            {
                Location = 45, // Route 230
                Slots = new[]
                {
                     new EncounterSlot { Species = 222, Type = SlotType.Grass }, // Corsola
                },
            },
            new EncounterArea
            {
                Location = 47, // Valley Windworks
                Slots = new[]
                {
                     new EncounterSlot { Species = 309, Type = SlotType.Grass }, // Electrike
                },
            },
            new EncounterArea
            {
                Location = 48, // Eterna Forest
                Slots = new[]
                {
                     new EncounterSlot { Species = 287, Type = SlotType.Grass }, // Slakoth
                },
            },
        };

        private static readonly EncounterArea[] SlotsDP_Swarm = SlotsDPPT_Swarm.Concat(
            new EncounterArea[] {
                new EncounterArea
                {
                    Location = 21, // Route 206
                    Slots = new[]
                    {
                         new EncounterSlot { Species = 299, Type = SlotType.Grass }, // Nosepass
                    },
                },
                new EncounterArea
                {
                    Location = 28, // Route 213
                    Slots = new[]
                    {
                         new EncounterSlot { Species = 359, Type = SlotType.Grass }, // Absol
                    },
                },
                new EncounterArea
                {
                    Location = 32, // Route 217
                    Slots = new[]
                    {
                         new EncounterSlot { Species = 220, Type = SlotType.Grass }, // Swinub
                         new EncounterSlot { Species = 225, Type = SlotType.Grass }, // Delibird
                    },
                },
                new EncounterArea
                {
                    Location = 44, // Route 229
                    Slots = new[]
                    {
                         new EncounterSlot { Species = 016, Type = SlotType.Grass }, // Pidgey
                    },
                },
                new EncounterArea
                {
                    Location = 49, // Fuego Ironworks
                    Slots = new[]
                    {
                         new EncounterSlot { Species = 081, Type = SlotType.Grass }, // Magnemite
                    },
                },
                new EncounterArea
                {
                    Location = 76, // Lake Verity
                    Slots = new[]
                    {
                         new EncounterSlot { Species = 283, Type = SlotType.Grass }, // Surskit
                    },
                },
                new EncounterArea
                {
                    Location = 77, // Lake Valor
                    Slots = new[]
                    {
                         new EncounterSlot { Species = 108, Type = SlotType.Grass }, // Lickitung
                    },
                },
                new EncounterArea
                {
                    Location = 78, // Lake Acuity
                    Slots = new[]
                    {
                         new EncounterSlot { Species = 238, Type = SlotType.Grass }, // Smoochum
                    },
                },
            }).ToArray();

        private static readonly EncounterArea[] SlotsPt_Swarm = SlotsDPPT_Swarm.Concat(
            new EncounterArea[] {
                new EncounterArea
                {
                    Location = 21, // Route 206
                    Slots = new[]
                    {
                            new EncounterSlot { Species = 246, Type = SlotType.Grass }, // Larvitar
                    },
                },
                new EncounterArea
                {
                    Location = 44, // Route 229
                    Slots = new[]
                    {
                            new EncounterSlot { Species = 127, Type = SlotType.Grass }, // Pinsir
                    },
                },
            }).ToArray();

        private static readonly EncounterArea[] SlotsHGSS_Swarm =
        {
            //Swarm 
            //reference http://bulbapedia.bulbagarden.net/wiki/Pokémon_outbreak
            new EncounterArea
            {
                Location = 128, // Violet City
                Slots = new[]
                {
                     new EncounterSlot { Species = 340, Type = SlotType.Old_Rod }, // Whiscash
                     new EncounterSlot { Species = 340, Type = SlotType.Good_Rod }, // Whiscash
                     new EncounterSlot { Species = 340, Type = SlotType.Super_Rod }, // Whiscash
                },
            },
            new EncounterArea
            {
                Location = 143, // Vermillion City
                Slots = new[]
                {
                     new EncounterSlot { Species = 278, Type = SlotType.Surf }, // Wingull
                },
            },
            new EncounterArea
            {
                Location = 149, // Route 1
                Slots = new[]
                {
                     new EncounterSlot { Species = 261, Type = SlotType.Grass }, // Poochyena
                },
            },
            new EncounterArea
            {
                Location = 160, // Route 12
                Slots = new[]
                {
                     new EncounterSlot { Species = 369, Type = SlotType.Old_Rod }, // Relicanth
                     new EncounterSlot { Species = 369, Type = SlotType.Good_Rod }, // Relicanth
                     new EncounterSlot { Species = 369, Type = SlotType.Super_Rod }, // Relicanth
                },
            },
            new EncounterArea
            {
                Location = 161, // Route 113
                Slots = new[]
                {
                     new EncounterSlot { Species = 113, Type = SlotType.Grass }, // Chansey
                },
            },
            new EncounterArea
            {
                Location = 167, // Route 19
                Slots = new[]
                {
                     new EncounterSlot { Species = 366, Type = SlotType.Surf }, // Clamperl
                },
            },
            new EncounterArea
            {
                Location = 173, // Route 25
                Slots = new[]
                {
                     new EncounterSlot { Species = 427, Type = SlotType.Grass }, // Buneary
                },
            },
            new EncounterArea
            {
                Location = 175, // Route 27
                Slots = new[]
                {
                     new EncounterSlot { Species = 370, Type = SlotType.Surf }, // Luvdisc
                },
            },
            new EncounterArea
            {
                Location = 180, // Route 32
                Slots = new[]
                {
                     new EncounterSlot { Species = 211, Type = SlotType.Old_Rod }, // Qwilfish
                     new EncounterSlot { Species = 211, Type = SlotType.Good_Rod }, // Qwilfish
                     new EncounterSlot { Species = 211, Type = SlotType.Super_Rod }, // Qwilfish
                },
            },
            new EncounterArea
            {
                Location = 182, // Route 34
                Slots = new[]
                {
                     new EncounterSlot { Species = 280, Type = SlotType.Grass }, // Ralts
                },
            },
            new EncounterArea
            {
                Location = 183, // Route 35
                Slots = new[]
                {
                     new EncounterSlot { Species = 193, Type = SlotType.Grass }, // Yanma
                },
            },
            new EncounterArea
            {
                Location = 186, // Route 38
                Slots = new[]
                {
                     new EncounterSlot { Species = 209, Type = SlotType.Grass }, // Snubbull
                },
            },
            new EncounterArea
            {
                Location = 192, // Route 44
                Slots = new[]
                {
                     new EncounterSlot { Species = 223, Type = SlotType.Old_Rod }, // Remoraid
                     new EncounterSlot { Species = 223, Type = SlotType.Good_Rod }, // Remoraid
                     new EncounterSlot { Species = 223, Type = SlotType.Super_Rod }, // Remoraid
                },
            },
            new EncounterArea
            {
                Location = 193, // Route 45
                Slots = new[]
                {
                     new EncounterSlot { Species = 333, Type = SlotType.Grass }, // Swablu
                },
            },
            new EncounterArea
            {
                Location = 195, // Route 43
                Slots = new[]
                {
                     new EncounterSlot { Species = 132, Type = SlotType.Grass }, // Ditto
                },
            },
            new EncounterArea
            {
                Location = 216, // Mt. Mortar
                Slots = new[]
                {
                     new EncounterSlot { Species = 189, Type = SlotType.Grass }, // Marill
                },
            },
            new EncounterArea
            {
                Location = 220, // Dark Cave
                Slots = new[]
                {
                     new EncounterSlot { Species = 206, Type = SlotType.Grass }, // Dunsparce
                },
            },
            new EncounterArea
            {
                Location = 224, // Viridian Forest
                Slots = new[]
                {
                     new EncounterSlot { Species = 401, Type = SlotType.Grass }, // Kricketot
                },
            },
        };

        private static readonly EncounterArea[] SlotsHG_Swarm = SlotsHGSS_Swarm.Concat(
            new EncounterArea[] {
                new EncounterArea
                {
                    Location = 151, // Route 3
                    Slots = new[]
                    {
                            new EncounterSlot { Species = 343, Type = SlotType.Grass }, // Baltoy
                    },
                },
                new EncounterArea
                {
                    Location = 157, // Route 9
                    Slots = new[]
                    {
                            new EncounterSlot { Species = 302, Type = SlotType.Grass }, // Sableye
                    },
                },
            }).ToArray();

        private static readonly EncounterArea[] SlotsSS_Swarm = SlotsHGSS_Swarm.Concat(
            new EncounterArea[] {
                new EncounterArea
                {
                    Location = 151, // Route 3
                    Slots = new[]
                    {
                            new EncounterSlot { Species = 316, Type = SlotType.Grass }, // Gulpin
                    },
                },
                new EncounterArea
                {
                    Location = 157, // Route 9
                    Slots = new[]
                    {
                            new EncounterSlot { Species = 303, Type = SlotType.Grass }, // Mawile
                    },
                },
            }).ToArray();

        #endregion

        internal static readonly int[] ValidMet_DP =
        {
            //todo
        };
        internal static readonly int[] ValidMet_Pt = ValidMet_DP.Concat(new int[]
        {
            //todo
        }).ToArray();
        internal static readonly int[] ValidMet_HGSS =
        {
            //todo
        };
    }
}
