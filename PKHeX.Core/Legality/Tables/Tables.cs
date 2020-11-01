﻿using System.Collections.Generic;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        public static readonly int[] Items_Ball =
        {
            000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012,
            013, 014, 015, 016, 492, 493, 494, 495, 496, 497, 498, 499, 576,
            851
        };

        public static readonly int[] Gen4EncounterTypes = { 1, 2, 4, 5, 7, 9, 10, 12, 23, 24 };
        public static readonly HashSet<int> LightBall = new HashSet<int> { 25, 26, 172 };
        public static readonly int[] RotomMoves = { 315, 056, 059, 403, 437 };

        /// <summary>
        /// Species that can be captured normally in the wild and can change between their forms.
        /// </summary>
        public static readonly HashSet<int> WildChangeFormAfter = new HashSet<int>
        {
            412, // Burmy
            479, // Rotom
            676, // Furfrou
            741, // Oricorio
        };

        /// <summary>
        /// Species that can yield a different baby species when bred.
        /// </summary>
        public static readonly HashSet<int> SplitBreed = new HashSet<int>
        {
            // Incense
            113, 242, // Chansey
            122, 866, // Mr. Mime
            143, // Snorlax
            183, 184, // Marill
            185, // Sudowoodo
            202, // Wobbuffet
            226, // Mantine
            315, 407, // Roselia
            358, // Chimecho
        };

        /// <summary>
        /// Species that can change between their forms, regardless of origin.
        /// </summary>
        /// <remarks>Excludes Zygarde as it has special conditions. Check separately.</remarks>
        public static readonly HashSet<int> FormChange = new HashSet<int>(WildChangeFormAfter)
        {
            386, // Deoxys
            487, // Giratina
            492, // Shaymin
            493, // Arceus
            641, // Tornadus
            642, // Thundurus
            645, // Landorus
            646, // Kyurem
            647, // Keldeo
            649, // Genesect
            720, // Hoopa
            773, // Silvally
            800, // Necrozma
            898, // Calyrex
        };

        /// <summary>
        /// Species that can change between their forms and get access to form-specific moves.
        /// </summary>
        public static readonly HashSet<int> FormChangeMoves = new HashSet<int>
        {
            386, // Deoxys
            487, // Giratina
            492, // Shaymin
            720, // Hoopa
        };

        /// <summary>
        /// Species that are male only in Generation 6; for ball inheritance, these behave the same as Genderless species.
        /// </summary>
        public static readonly HashSet<int> BreedMaleOnly = new HashSet<int>
        {
            128, // Tauros
            627, 628, // Rufflet
            236, 106, 107, 237, // Tyrogue
            538, 539, // Sawk & Throh
        };

        /// <summary>
        /// Species that cannot hatch from an egg.
        /// </summary>
        public static readonly HashSet<int> NoHatchFromEgg = new HashSet<int>
        {
            132, // Ditto
            144, // Articuno
            145, // Zapdos
            146, // Moltres
            150, // Mewtwo
            151, // Mew

            201, // Unown
            243, // Raikou
            244, // Entei
            245, // Suicune
            249, // Lugia
            250, // Ho-Oh
            251, // Celebi

            377, // Regirock
            378, // Regice
            379, // Registeel
            380, // Latias
            381, // Latios
            382, // Kyogre
            383, // Groudon
            384, // Rayquaza
            385, // Jirachi
            386, // Deoxys

            480, // Uxie
            481, // Mesprit
            482, // Azelf
            483, // Dialga
            484, // Palkia
            485, // Heatran
            486, // Regigigas
            487, // Giratina
            488, // Cresselia
            490, // Manaphy
            491, // Darkrai
            492, // Shaymin
            493, // Arceus

            494, // Victini
            638, // Cobalion
            639, // Terrakion
            640, // Virizion
            641, // Tornadus
            642, // Thundurus
            643, // Reshiram
            644, // Zekrom
            645, // Landorus
            646, // Kyurem
            647, // Keldeo
            648, // Meloetta
            649, // Genesect

            716, // Xerneas
            717, // Yveltal
            718, // Zygarde
            719, // Diancie
            720, // Hoopa
            721, // Volcanion

            772, // Type: Null
            773, // Silvally
            785, // Tapu Koko
            786, // Tapu Lele
            787, // Tapu Bulu
            788, // Tapu Fini
            789, // Cosmog
            790, // Cosmoem
            791, // Solgaleo
            792, // Lunala
            793, // Nihilego
            794, // Buzzwole
            795, // Pheromosa
            796, // Xurkitree
            797, // Celesteela
            798, // Kartana
            799, // Guzzlord
            800, // Necrozma
            801, // Magearna
            802, // Marshadow

            803, // Poipole
            804, // Naganadel
            805, // Stakataka
            806, // Blacephalon
            807, // Zeraora

            808, // Meltan
            809, // Melmetal

            (int)Species.Dracozolt,
            (int)Species.Arctozolt,
            (int)Species.Dracovish,
            (int)Species.Arctovish,

            (int)Species.Zacian,
            (int)Species.Zamazenta,
            (int)Species.Eternatus,

            (int)Species.Kubfu,
            (int)Species.Urshifu,
            (int)Species.Zarude,

            (int)Species.Regieleki,
            (int)Species.Regidrago,
            (int)Species.Glastrier,
            (int)Species.Spectrier,
            (int)Species.Calyrex,
        };

        /// <summary>
        /// Generation 3 &amp; 4 Battle Frontier Species banlist. When referencing this in context to generation 4, be sure to disallow <see cref="Species.Pichu"/> with Form 1 (Spiky).
        /// </summary>
        public static readonly HashSet<int> BattleFrontierBanlist = new HashSet<int>
        {
            150, // Mewtwo
            151, // Mew

            249, // Lugia
            250, // Ho-Oh
            251, // Celebi

            382, // Kyogre
            383, // Groudon
            384, // Rayquaza
            385, // Jirachi
            386, // Deoxys

            483, // Dialga
            484, // Palkia
            487, // Giratina
            489, // Phione
            490, // Manaphy
            491, // Darkrai
            492, // Shaymin
            493, // Arceus

            494, // Victini
            643, // Reshiram
            644, // Zekrom
            646, // Kyurem
            647, // Keldeo
            648, // Meloetta
            649, // Genesect

            716, // Xerneas
            717, // Yveltal
            718, // Zygarde
            719, // Diancie
            720, // Hoopa
            721, // Volcanion

            789, // Cosmog
            790, // Cosmoem
            791, // Solgaleo
            792, // Lunala
            800, // Necrozma
            801, // Magearna
            802, // Marshadow
            807, // Zeraora
            808, // Meltan
            809, // Melmetal
        };

        public static readonly HashSet<int> Z_Moves = new HashSet<int>
        {
            622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658,
            695, 696, 697, 698, 699, 700, 701, 702, 703,
            719,
            723, 724, 725, 726, 727, 728
        };

        /// <summary>
        /// Moves that can not be obtained by using Sketch with Smeargle.
        /// </summary>
        internal static readonly HashSet<int> InvalidSketch = new HashSet<int>(Z_Moves)
        {
            // Can't Sketch
            165, // Struggle
            448, // Chatter

            // Unreleased
            617, // Light of Ruin
        };

        /// <summary>
        /// Species classified as "Legend" by the game code.
        /// </summary>
        public static readonly HashSet<int> Legends = new HashSet<int>
        {
            (int)Species.Mewtwo, (int)Species.Mew,
            (int)Species.Lugia, (int)Species.HoOh, (int)Species.Celebi,
            (int)Species.Kyogre, (int)Species.Groudon, (int)Species.Rayquaza, (int)Species.Jirachi, (int)Species.Deoxys,
            (int)Species.Dialga, (int)Species.Palkia, (int)Species.Giratina, (int)Species.Phione, (int)Species.Manaphy, (int)Species.Darkrai, (int)Species.Shaymin, (int)Species.Arceus,
            (int)Species.Victini, (int)Species.Reshiram, (int)Species.Zekrom, (int)Species.Kyurem, (int)Species.Keldeo, (int)Species.Meloetta, (int)Species.Genesect,
            (int)Species.Xerneas, (int)Species.Yveltal, (int)Species.Zygarde, (int)Species.Diancie, (int)Species.Hoopa, (int)Species.Volcanion,
            (int)Species.Cosmog, (int)Species.Cosmoem, (int)Species.Solgaleo, (int)Species.Lunala, (int)Species.Necrozma, (int)Species.Magearna, (int)Species.Marshadow, (int)Species.Zeraora,
            (int)Species.Meltan, (int)Species.Melmetal,
            (int)Species.Zacian, (int)Species.Zamazenta, (int)Species.Eternatus, (int)Species.Zarude, (int)Species.Calyrex,
        };

        /// <summary>
        /// Species classified as "SubLegend" by the game code.
        /// </summary>
        public static readonly HashSet<int> SubLegends = new HashSet<int>
        {
            (int)Species.Articuno, (int)Species.Zapdos, (int)Species.Moltres,
            (int)Species.Raikou, (int)Species.Entei, (int)Species.Suicune,
            (int)Species.Regirock, (int)Species.Regice, (int)Species.Registeel, (int)Species.Latias, (int)Species.Latios,
            (int)Species.Uxie, (int)Species.Mesprit, (int)Species.Azelf, (int)Species.Heatran, (int)Species.Regigigas, (int)Species.Cresselia,
            (int)Species.Cobalion, (int)Species.Terrakion, (int)Species.Virizion, (int)Species.Tornadus, (int)Species.Thundurus, (int)Species.Landorus,
            (int)Species.TypeNull, (int)Species.Silvally, (int)Species.TapuKoko, (int)Species.TapuLele, (int)Species.TapuBulu, (int)Species.TapuFini,
            (int)Species.Nihilego, (int)Species.Buzzwole, (int)Species.Pheromosa, (int)Species.Xurkitree, (int)Species.Celesteela, (int)Species.Kartana, (int)Species.Guzzlord,
            (int)Species.Poipole, (int)Species.Naganadel, (int)Species.Stakataka, (int)Species.Blacephalon,
            (int)Species.Kubfu, (int)Species.Urshifu, (int)Species.Regieleki, (int)Species.Regidrago, (int)Species.Glastrier, (int)Species.Spectrier,
        };

        public static readonly ushort[] Arceus_Plate = {303, 306, 304, 305, 309, 308, 310, 313, 298, 299, 301, 300, 307, 302, 311, 312, 644};
        public static readonly ushort[] Arceus_ZCrystal = {782, 785, 783, 784, 788, 787, 789, 792, 777, 778, 780, 779, 786, 781, 790, 791, 793};

        internal static readonly int[] BabyEvolutionWithMove =
        {
            122, // Mr. Mime (Mime Jr with Mimic)
            185, // Sudowoodo (Bonsly with Mimic)
        };

        // List of species that evolve from a previous species having a move while leveling up
        internal static readonly int[] SpeciesEvolutionWithMove =
        {
            122, // Mr. Mime (Mime Jr with Mimic)
            185, // Sudowoodo (Bonsly with Mimic)
            424, // Ambipom (Aipom with Double Hit)
            463, // Lickilicky (Lickitung with Rollout)
            465, // Tangrowth (Tangela with Ancient Power)
            469, // Yanmega (Yamma with Ancient Power)
            473, // Mamoswine (Piloswine with Ancient Power)
            700, // Sylveon (Eevee with Fairy Move)
            763, // Tsareena (Steenee with Stomp)
            (int)Species.Grapploct // (Clobbopus with Taunt)
        };

        internal static readonly int[] FairyMoves =
        {
            186, // Sweet Kiss
            204, // Charm
            236, // Moonlight
            574, // Disarming Voice
            577, // Draining Kiss
            578, // Crafty Shield
            579, // Flower Shield
            581, // Misty Terrain
            583, // Play Rough
            584, // Fairy Wind
            585, // Moonblast
            587, // Fairy Lock
            597, // Aromatic Mist
            601, // Geomancy
            605, // Dazzling Gleam
            608, // Baby-Doll Eyes
            617, // Light of Ruin
            656, // Twinkle Tackle
            657, // Twinkle Tackle
            666, // Floral Healing
            698, // Guardian of Alola
            705, // Fleur Cannon
            717, // Nature's Madness
            726, // Let's Snuggle Forever
            740, // Sparkly Swirl
            767, // Max Starfall
            777, // Decorate
            789, // Spirit Break
            790, // Strange Steam
            802, // Misty Explosion
        };

        // Moves that trigger the evolution by move
        internal static readonly int[][] MoveEvolutionWithMove =
        {
            new [] { 102 }, // Mr. Mime (Mime Jr with Mimic)
            new [] { 102 }, // Sudowoodo (Bonsly with Mimic)
            new [] { 458 }, // Ambipom (Aipom with Double Hit)
            new [] { 205 }, // Lickilicky (Lickitung with Rollout)
            new [] { 246 }, // Tangrowth (Tangela with Ancient Power)
            new [] { 246 }, // Yanmega (Yamma with Ancient Power)
            new [] { 246 }, // Mamoswine (Piloswine with Ancient Power)
            FairyMoves, // Sylveon (Eevee with Fairy Move)
            new [] { 023 }, // Tsareena (Steenee with Stomp)
            new [] { 269 }, // Grapploct (Clobbopus with Taunt)
        };

        // Min level for any species for every generation to learn the move for evolution by move
        // 0 means it cant be learned in that generation
        internal static readonly int[][] MinLevelEvolutionWithMove =
        {
            // Mr. Mime (Mime Jr with Mimic)
            new [] { 0, 0, 0, 0, 18, 15, 15, 2, 2 },
            // Sudowoodo (Bonsly with Mimic)
            new [] { 0, 0, 0, 0, 17, 17, 15, 2, 2 },
            // Ambipom (Aipom with Double Hit)
            new [] { 0, 0, 0, 0, 32, 32, 32, 2, 2 },
            // Lickilicky (Lickitung with Rollout)
            new [] { 0, 0, 2, 0, 2, 33, 33, 2, 2 },
            // Tangrowth (Tangela with Ancient Power)
            new [] { 0, 0, 0, 0, 2, 36, 38, 2, 2 },
            // Yanmega (Yanma with Ancient Power)
            new [] { 0, 0, 0, 0, 2, 33, 33, 2, 2 },
            // Mamoswine (Piloswine with Ancient Power)
            new [] { 0, 0, 0, 0, 2, 2, 2, 2, 2 },
            // Sylveon (Eevee with Fairy Move)
            new [] { 0, 0, 0, 0, 0, 29, 9, 2, 2 },
            // Tsareena (Steenee with Stomp)
            new [] { 0, 0, 0, 0, 0, 0, 0, 2, 28 },
            // Grapploct (Clobbopus with Taunt)
            new [] { 0, 0, 0, 0, 0, 0, 0, 0, 35 },
        };

        // True -> the pokemon could hatch from an egg with the move for evolution as an egg move
        internal static readonly bool[][] EggMoveEvolutionWithMove =
        {
            // Mr. Mime (Mime Jr with Mimic)
            new [] { false, false, false, false, true, true, true, true, true },
            // Sudowoodo (Bonsly with Mimic)
            new [] { false, false, false, false, true, true, true, true, true },
            // Ambipom (Aipom with Double Hit)
            new [] { false, false, false, false, true, true, true, true, true },
            // Lickilicky (Lickitung with Rollout)
            new [] { false, false, true, false, true, true, true, true, true },
            // Tangrowth (Tangela with Ancient Power)
            new [] { false, false, false, false, true, true, true, true, true },
            // Yanmega (Yanma with Ancient Power)
            new [] { false, false, false, false, true, true, true, true, true },
            // Mamoswine (Piloswine with Ancient Power)
            new [] { false, false, true, true, true, true, true, true, true },
            // Sylveon (Eevee with Fairy Move)
            new [] { false, false, true, true, true, true, true, true, true },
            // Tsareena (Steenee with Stomp)
            new [] { false, false, false, false, false, false, false, false, false },
            // Grapploct (Clobbopus with Taunt)
            new [] { false, false, false, false, false, false, false, false, true },
        };

        internal static readonly HashSet<int> MixedGenderBreeding = new HashSet<int>
        {
            29, // Nidoran♀
            32, // Nidoran♂
            313, // Volbeat
            314, // Illumise
            (int)Species.Indeedee, // male/female
        };

        public static readonly HashSet<int> FixedGenderFromBiGender = new HashSet<int>
        {
            290, // Nincada
            292, // Shedinja (G)

            412, // Burmy
            413, // Wormadam (F)
            414, // Mothim (M)

            280, // Ralts
            475, // Gallade (M)

            361, // Snorunt
            478, // Froslass (F)

            677, // Espurr
            678, // Meowstic (M/F) forme specific
        };
    }
}
