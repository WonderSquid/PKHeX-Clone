﻿// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class CheckStrings
    {
        public const string splitter = " = ";
        public static void RefreshStrings(IEnumerable<string> lines)
        {
            var t = typeof (CheckStrings);
            foreach (var line in lines.Where(l => l != null))
            {
                var index = line.IndexOf(splitter, StringComparison.Ordinal);
                var prop = line.Substring(0, index);
                var value = line.Substring(index + splitter.Length);

                try
                {
                    ReflectUtil.SetValue(t, prop, value);
                }
                catch
                {
                    Console.WriteLine($"Property not present: {prop} || Value written: {value}");
                }
            }
        }
        public static IEnumerable<string> DumpStrings()
        {
            var t = typeof (CheckStrings);
            var props = ReflectUtil.getPropertiesStartWithPrefix(t, "V");
            return props.Select(p => $"{p}{splitter}{ReflectUtil.GetValue(t, p).ToString()}");
        }
        public static string[] UpdateLocalization(string[] lines)
        {
            List<string> list = new List<string>();
            var current = DumpStrings();
            foreach (var line in current)
            {
                int index = line.IndexOf(splitter, StringComparison.Ordinal);
                string prop = line.Substring(0, index);
                string match = lines.FirstOrDefault(l => l.StartsWith(prop));
                list.Add(match ?? line);
            }
            return list.ToArray();
        }

        #region General Strings

        /// <summary>Default text for indicating validity.</summary>
        public static string V {get; set;} = "Valid.";
        /// <summary>Default text for indicating legality.</summary>
        public static string V193 {get; set;} = "Legal!";
        /// <summary>Default text for indicating an error has occurred.</summary>
        public static string V190 {get; set;} = "Internal error.";
        /// <summary>Analysis not available for the <see cref="PKM"/></summary>
        public static string V189 {get; set;} = "Analysis not available for this Pokémon.";
        /// <summary>Format text for exporting a legality check result.</summary>
        public static string V196 {get; set;} = "{0}: {1}";
        /// <summary>Format text for exporting a legality check result for an invalid Move.</summary>
        public static string V191 {get; set;} = "{0} Move {1}: {2}";
        /// <summary>Format text for exporting a legality check result for an invalid Relearn Move.</summary>
        public static string V192 {get; set;} = "{0} Relearn Move {1}: {2}";
        /// <summary>Format text for exporting the type of Encounter that was matched for the the <see cref="PKM"/></summary>
        public static string V195 {get; set;} = "Encounter Type: {0}";

        /// <summary>Original Trainer string used in various string formats such as Memories.</summary>
        public static string V205 { get; set; } = "OT";
        /// <summary>Handling Trainer string used in various string formats such as Memories.</summary>
        public static string V206 { get; set; } = "HT";

        public static string V167 { get; set; } = "Empty Move.";
        public static string V171 { get; set; } = "Egg Move.";
        public static string V172 { get; set; } = "Relearnable Move.";
        public static string V173 { get; set; } = "Learned by TM/HM.";
        public static string V174 { get; set; } = "Learned by Move Tutor.";
        public static string V175 { get; set; } = "Special Non-Relearn Move.";
        public static string V177 { get; set; } = "Learned by Level-up.";

        #endregion

        #region Legality Check Result Strings

        public static string V203 {get; set;} = "Genderless Pokémon should not have a gender.";
        public static string V201 {get; set;} = "Encryption Constant is not set.";
        public static string V204 {get; set;} = "Held item is unreleased.";

        public static string V187 {get; set;} = "Species does not exist in origin game.";
        public static string V188 {get; set;} = "Fateful Encounter with no matching Encounter. Has the Mystery Gift data been contributed?";
        public static string V194 { get; set;} = "Ingame Trade for Sun/Moon not implemented."; // Valid

        public static string V207 {get; set;} = "PID is not set.";
        public static string V208 {get; set;} = "Encryption Constant matches PID.";
        public static string V209 {get; set;} = "Static Encounter shiny mismatch.";
        public static string V210 {get; set;} = "Wurmple evolution Encryption Constant mismatch.";
        public static string V211 {get; set;} = "Encryption Constant matches shinyxored PID.";
        public static string V212 {get; set;} = "Wurmple Evolution: {0}";
        public static string V213 {get; set;} = "Silcoon";
        public static string V214 {get; set;} = "Cascoon";
        public static string V215 {get; set;} = "PID should be equal to EC [with top bit flipped]!";
        public static string V216 {get; set;} = "PID should be equal to EC!";

        public static string V14 {get; set;} = "Egg matches language Egg name."; // Valid
        public static string V17 {get; set;} = "Nickname does not match another species name."; // Valid
        public static string V18 {get; set;} = "Nickname matches species name."; // Valid
        public static string V19 {get; set;} = "Nickname matches demo language name."; // Valid
        public static string V11 {get; set;} = "Ingame Trade OT and Nickname have not been altered."; // Valid
        public static string V1 {get; set;} = "Nickname too long."; // Invalid
        public static string V2 {get; set;} = "Nickname is empty."; // Invalid
        public static string V4 {get; set;} = "Language ID > 8."; // Invalid
        public static string V5 {get; set;} = "Language ID > 10."; // Invalid
        public static string V3 {get; set;} = "Species index invalid for Nickname comparison."; // Invalid
        public static string V20 {get; set;} = "Nickname does not match species name."; // Invalid
        public static string V13 {get; set;} = "Egg name does not match language Egg name."; // Invalid
        public static string V12 {get; set;} = "Eggs must be nicknamed."; // Invalid
        public static string V7 {get; set;} = "Ingame Trade invalid version?"; // Invalid
        public static string V8 {get; set;} = "Ingame Trade invalid index?"; // Invalid
        public static string V10 {get; set;} = "Ingame Trade OT has been altered."; // Invalid
        public static string V9 {get; set;} = "Ingame Trade Nickname has been altered."; // Fishy
        public static string V15 {get; set;} = "Nickname matches another species name (+language)."; // Fishy
        public static string V16 {get; set;} = "Nickname flagged, matches species name."; // Fishy

        public static string V21 {get; set;} = "Matches: {0} {1}"; // Valid

        public static string V25 {get; set;} = "EV total cannot be above 510."; // Invalid
        public static string V22 {get; set;} = "Eggs cannot receive EVs."; // Invalid
        public static string V23 {get; set;} = "All EVs are zero, but leveled above Met Level."; // Fishy
        public static string V24 {get; set;} = "2 EVs remaining."; // Fishy
        public static string V26 {get; set;} = "EVs cannot go above 252."; // Invalid
        public static string V27 {get; set;} = "EVs are all equal."; // Fishy
        public static string V31 {get; set;} = "All IVs are 0."; // Fishy
        public static string V32 {get; set;} = "All IVs are equal."; // Fishy

        public static string V28 {get; set;} = "Should have at least {0} IVs {get; set;} = 31."; // Invalid
        public static string V29 {get; set;} = "Friend Safari captures should have at least 2 IVs {get; set;} = 31."; // Invalid
        public static string V30 {get; set;} = "IVs do not match Mystery Gift Data."; // Invalid

        public static string V38 {get; set;} = "OT Name too long."; // Invalid
        public static string V39 {get; set;} = "Incorrect RBY event OT Name."; // Invalid
        public static string V34 {get; set;} = "SID should be 0."; // Invalid
        public static string V33 {get; set;} = "TID and SID are 0."; // Fishy
        public static string V35 {get; set;} = "TID and SID are equal."; // Fishy
        public static string V36 {get; set;} = "TID is zero."; // Fishy
        public static string V37 {get; set;} = "SID is zero."; // Fishy

        public static string V40 {get; set;} = "Can't Hyper Train a pokemon that isn't level 100."; // Invalid
        public static string V41 {get; set;} = "Can't Hyper Train a pokemon with perfect IVs."; // Invalid
        public static string V42 {get; set;} = "Can't Hyper Train a perfect IV."; // Invalid

        public static string V49 {get; set;} = "Valid Pokémon Link gift."; // Valid
        public static string V47 {get; set;} = "Pokémon Link gift Shiny mismatch."; // Invalid
        public static string V48 {get; set;} = "Pokémon Link gift should not be Fateful Encounter."; // Invalid
        public static string V43 {get; set;} = "Can't find matching Pokémon Link gift."; // Invalid
        public static string V44 {get; set;} = "Can't obtain this Pokémon Link gift in XY."; // Invalid
        public static string V45 {get; set;} = "Can't obtain this Pokémon Link gift in ORAS."; // Invalid
        public static string V46 {get; set;} = "Can't obtain this Pokémon Link gift in SM."; // Invalid

        public static string V63 {get; set;} = "Valid un-hatched egg."; // Valid
        public static string V53 {get; set;} = "Able to hatch an egg at Met Location."; // Valid
        public static string V56 {get; set;} = "Able to hatch a traded egg at Met Location.";
        public static string V54 {get; set;} = "Can't hatch an egg at Met Location."; // Invalid
        public static string V55 {get; set;} = "Can't obtain egg from Egg Location."; // Invalid
        public static string V57 {get; set;} = "Can't transfer eggs between generations."; // Invalid
        public static string V50 {get; set;} = "Can't obtain egg for this species."; // Invalid
        public static string V51 {get; set;} = "Invalid Met Location for hatched egg."; // Invalid
        public static string V52 {get; set;} = "Invalid Met Level, expected {0}."; // Invalid
        public static string V58 {get; set;} = "Invalid Met Level for transfer."; // Invalid
        public static string V59 {get; set;} = "Invalid Egg Location, expected none."; // Invalid
        public static string V60 {get; set;} = "Invalid Met Location, expected Pal Park."; // Invalid
        public static string V61 {get; set;} = "Invalid Met Location, expected Transporter."; // Invalid
        public static string V62 {get; set;} = "Invalid Egg Location, shouldn't be 'traded' while an egg."; // Invalid

        public static string V66 {get; set;} = "Valid Friend Safari encounter."; // Valid
        public static string V64 {get; set;} = "Friend Safari: Not valid color."; // Florges
        public static string V6 {get; set;} = "Friend Safari: Not average sized."; // Pumpkaboo
        public static string V65 {get; set;} = "Friend Safari: Not Spring form."; // Sawsbuck

        public static string V67 {get; set;} = "Valid Wild Encounter at location (Pressure/Hustle/Vital Spirit).";
        public static string V68 {get; set;} = "Valid Wild Encounter at location.";
        public static string V69 {get; set;} = "Valid Wild Encounter at location (White Flute & Pressure/Hustle/Vital Spirit).";
        public static string V70 {get; set;} = "Valid Wild Encounter at location (White Flute).";
        public static string V71 {get; set;} = "Valid Wild Encounter at location (Black Flute & Pressure/Hustle/Vital Spirit).";
        public static string V72 {get; set;} = "Valid Wild Encounter at location (Black Flute).";
        public static string V73 {get; set;} = "Valid Wild Encounter at location (DexNav).";

        public static string V76 {get; set;} = "Valid ingame trade.";
        public static string V75 {get; set;} = "Valid gift/static encounter."; // Valid
        public static string V74 {get; set;} = "Static encounter relearn move mismatch.";

        public static string V77 {get; set;} = "Can't obtain Species from Virtual Console games."; // Invalid
        public static string V79 {get; set;} = "Can't obtain Special encounter in Virtual Console games."; // Invalid
        public static string V78 {get; set;} = "Unable to match to a Mystery Gift in the database."; // Invalid
        public static string V80 {get; set;} = "Unable to match an encounter from origin game."; // Invalid
        public static string V81 {get; set;} = "Invalid Transfer Met Location."; // Invalid
        public static string V82 {get; set;} = "Mewtwo cannot be transferred while knowing Pay Day."; // Invalid

        public static string V88 {get; set;} = "Current level is not below met level.";
        public static string V83 {get; set;} = "Met Level does not match Mystery Gift level.";
        public static string V84 {get; set;} = "Current Level below Mystery Gift level.";
        public static string V85 {get; set;} = "Current level is below met level.";
        public static string V86 {get; set;} = "Evolution not valid (or level/trade evolution unsatisfied).";
        public static string V87 {get; set;} = "Current experience matches level threshold."; // Fishy

        public static string V89 {get; set;} = "Can't Super Train an egg."; // Invalid
        public static string V90 {get; set;} = "Super Training missions are not available in games visited.";
        public static string V91 {get; set;} = "Can't have active Super Training unlocked flag for origins.";
        public static string V92 {get; set;} = "Can't have active Super Training complete flag for origins.";
        public static string V93 {get; set;} = "Super Training complete flag mismatch.";
        public static string V94 {get; set;} = "Distribution Super Training missions are not released."; // Fishy

        public static string V95 {get; set;} = "Can't receive Ribbon(s) as an egg.";
        public static string V96 {get; set;} = "GBA Champion Ribbon";
        public static string V97 {get; set;} = "Artist Ribbon";
        public static string V98 {get; set;} = "National Ribbon (Purified)";
        public static string V99 {get; set;} = "Sinnoh Champion Ribbon";
        public static string V100 {get; set;} = "Legend Ribbon";
        public static string V104 {get; set;} = "Record Ribbon";
        public static string V101 {get; set;} = "Missing Ribbons: {0}";
        public static string V102 {get; set;} = "Invalid Ribbons: {0}";
        public static string V103 {get; set;} = "All ribbons accounted for.";
        public static string V105 {get; set;} = "Battle Memory Ribbon";
        public static string V106 {get; set;} = "Contest Memory Ribbon";

        public static string V107 {get; set;} = "Ability is not valid for species/form.";
        public static string V108 {get; set;} = "Hidden Ability mismatch for encounter type.";
        public static string V109 {get; set;} = "Ability modified with Ability Capsule.";
        public static string V110 {get; set;} = "Ability does not match Mystery Gift.";
        public static string V111 {get; set;} = "Hidden Ability on non-SOS wild encounter.";
        public static string V112 {get; set;} = "Hidden Ability not available.";

        public static string V115 {get; set;} = "Ability matches ability number."; // Valid
        public static string V113 {get; set;} = "Ability does not match PID.";
        public static string V114 {get; set;} = "Ability does not match ability number.";

        public static string V119 {get; set;} = "Correct ball for encounter type.";
        public static string V118 {get; set;} = "Can't have ball for encounter type.";
        public static string V116 {get; set;} = "Can't have Heavy Ball for light, low-catch rate species (Gen VII).";
        public static string V117 {get; set;} = "Can't have Master Ball for regular egg.";
        public static string V120 {get; set;} = "Can't have Cherish Ball for regular egg.";
        public static string V121 {get; set;} = "Can't obtain species in Ball.";
        public static string V122 {get; set;} = "Can't obtain Hidden Ability with Ball.";
        public static string V123 {get; set;} = "Ball possible for species.";
        public static string V125 {get; set;} = "No check satisfied, assuming illegal.";
        public static string V126 {get; set;} = "Ball unobtainable in origin generation.";

        public static string V145 {get; set;} = "History block is valid.";
        public static string V155 {get; set;} = "{0} Memory is valid.";

        public static string V127 {get; set;} = "Skipped History check due to other check being invalid.";
        public static string V128 {get; set;} = "No History Block to check.";
        public static string V129 {get; set;} = "OT Affection should be 0.";
        public static string V130 {get; set;} = "Can't have any OT Memory.";
        public static string V124 {get; set;} = "Current handler cannot be past gen OT for transferred specimen.";
        public static string V131 {get; set;} = "HT Gender invalid: {0}";
        public static string V132 {get; set;} = "Event OT Friendship does not match base friendship.";
        public static string V133 {get; set;} = "Event OT Affection should be zero.";
        public static string V134 {get; set;} = "Current handler should not be Event OT.";
        public static string V138 {get; set;} = "Contest Stats should be 0.";
        public static string V137 {get; set;} = "GeoLocation Memory: Memories should be present.";
        public static string V135 {get; set;} = "GeoLocation Memory: Gap/Blank present.";
        public static string V136 {get; set;} = "GeoLocation Memory: Region without Country.";
        public static string V146 {get; set;} = "GeoLocation Memory: HT Name present but has no previous Country.";
        public static string V147 {get; set;} = "GeoLocation Memory: Previous country of residence present with no Handling Trainer.";
        public static string V139 {get; set;} = "Untraded: Current handler should not be the Handling Trainer.";
        public static string V140 {get; set;} = "Untraded: Handling Trainer Friendship should be 0.";
        public static string V141 {get; set;} = "Untraded: Handling Trainer Affection should be 0.";
        public static string V142 {get; set;} = "Untraded: Requires a trade evolution.";
        public static string V143 {get; set;} = "Untraded: Beauty is not high enough for Level-up Evolution.";
        public static string V144 {get; set;} = "Untraded: Beauty is high enough but still Level 1.";
        public static string V148 {get; set;} = "Memory: Handling Trainer Memory present with no Handling Trainer name.";
        public static string V150 {get; set;} = "Memory: Handling Trainer Memory missing.";
        public static string V152 {get; set;} = "Memory: Original Trainer Memory missing.";
        public static string V149 {get; set;} = "Memory: Can't have Handling Trainer Memory as egg.";
        public static string V151 {get; set;} = "Memory: Can't have Original Trainer Memory as egg.";
        public static string V164 {get; set;} = "{0} Memory: Species can be captured in game.";
        public static string V153 {get; set;} = "{0} Memory: Species can't learn this move.";
        public static string V154 {get; set;} = "{0} Memory: Location doesn't have a Pokemon Center.";
        public static string V160 {get; set;} = "{0} Memory: {0} did not hatch this.";
        public static string V202 {get; set;} = "{0} Memory: {0} did not catch this.";
        public static string V161 {get; set;} = "{0} Memory: Link Trade is not a valid first memory.";
        public static string V162 {get; set;} = "{0} Memory: Can't obtain Location on {0} Version.";
        public static string V163 {get; set;} = "{0} Memory: Can't obtain Memory on {0} Version.";
        public static string V165 {get; set;} = "{0} Memory: Can't capture species in game.";
        public static string V197 {get; set;} = "{0} Memory: Should be index {1}.";
        public static string V198 {get; set;} = "{0} Memory: Intensity should be index {1}.";
        public static string V199 {get; set;} = "{0} Memory: TextVar should be index {1}.";
        public static string V200 {get; set;} = "{0} Memory: Feeling should be index {1}.";

        public static string V168 {get; set;} = "Duplicate Move.";
        public static string V176 {get; set;} = "Invalid Move.";
        public static string V166 {get; set;} = "Invalid Move (Sketch).";
        public static string V169 {get; set;} = "Keldeo Move/Form mismatch.";
        public static string V181 {get; set;} = "Expected the following Relearn Moves: {0}";
        public static string V170 {get; set;} = "Relearn Moves missing: {0}";
        public static string V178 {get; set;} = "Expected: {0}.";

        public static string V179 {get; set;} = "Base egg move.";
        public static string V180 {get; set;} = "Base egg move missing.";
        public static string V182 {get; set;} = "Not an expected Relearnable move.";
        public static string V183 {get; set;} = "Not an expected DexNav move.";
        public static string V184 {get; set;} = "Expected no Relearn Move in slot.";
        public static string V185 {get; set;} = "Egg Moves Source: {0}.";
        public static string V186 {get; set;} = "Egg Move set check unimplemented.";
        public static string V156 {get; set;} = "Should have a Link Trade HT Memory.";
        public static string V157 {get; set;} = "Should have a HT Memory TextVar value (somewhere).";
        public static string V158 {get; set;} = "Should have a HT Memory Intensity value (1st).";
        public static string V159 {get; set;} = "Should have a HT Memory Feeling value 0-9.";

        #endregion

    }
}
