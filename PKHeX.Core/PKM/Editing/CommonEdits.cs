﻿using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Contains extension logic for modifying <see cref="PKM"/> data.
    /// </summary>
    public static class CommonEdits
    {
        public static bool ShowdownSetIVMarkings { get; set; } = true;

        /// <summary>
        /// Sets the <see cref="PKM.Nickname"/> to the provided value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="nick"><see cref="PKM.Nickname"/> to set. If no nickname is provided, the <see cref="PKM.Nickname"/> is set to the default value for its current language and format.</param>
        public static void SetNickname(this PKM pk, string nick = null)
        {
            if (nick != null)
            {
                pk.IsNicknamed = true;
                pk.Nickname = nick;
            }
            else
            {
                pk.IsNicknamed = false;
                pk.Nickname = PKX.GetSpeciesNameGeneration(pk.Species, pk.Language, pk.Format);
                if (pk is PK1 pk1) pk1.SetNotNicknamed();
                if (pk is PK2 pk2) pk2.SetNotNicknamed();
            }
        }

        /// <summary>
        /// Sets the <see cref="PKM.AltForm"/> value, with special consideration for <see cref="PKM.Format"/> values which derive the <see cref="PKM.AltForm"/> value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="form">Desired <see cref="PKM.AltForm"/> value to set.</param>
        public static void SetAltForm(this PKM pk, int form)
        {
            switch (pk.Format)
            {
                case 2:
                    while (pk.AltForm != form)
                        pk.SetRandomIVs();
                    break;
                case 3:
                    pk.SetPIDUnown3(form);
                    break;
                default:
                    pk.AltForm = form;
                    break;
            }
        }

        /// <summary>
        /// Sets the <see cref="PKM.Ability"/> value by sanity checking the provided <see cref="PKM.Ability"/> against the possible pool of abilities.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="abil">Desired <see cref="PKM.Ability"/> to set.</param>
        public static void SetAbility(this PKM pk, int abil)
        {
            var abilities = pk.PersonalInfo.Abilities;
            int abilIndex = Array.IndexOf(abilities, abil);
            abilIndex = Math.Max(0, abilIndex);

            if (pk is PK5 pk5 && abilIndex == 2)
                pk5.HiddenAbility = true;
            else if (pk.Format <= 5)
                pk.PID = PKX.GetRandomPID(pk.Species, pk.Gender, pk.Version, pk.Nature, pk.Format, (uint)(abilIndex * 0x10001));
            pk.RefreshAbility(abilIndex);
        }

        /// <summary>
        /// Sets a Random <see cref="PKM.EncryptionConstant"/> value. The <see cref="PKM.EncryptionConstant"/> is not updated if the value should match the <see cref="PKM.PID"/> instead.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetRandomEC(this PKM pk)
        {
            int gen = pk.GenNumber;
            if (gen < 6 && gen > 2)
                pk.EncryptionConstant = pk.PID;
            else
                pk.EncryptionConstant = Util.Rand32();
        }

        /// <summary>
        /// Sets the <see cref="PKM.IsShiny"/> derived value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="shiny">Desired <see cref="PKM.IsShiny"/> state to set.</param>
        /// <returns></returns>
        public static bool SetIsShiny(this PKM pk, bool shiny) => shiny ? pk.SetShiny() : pk.SetUnshiny();

        /// <summary>
        /// Makes a <see cref="PKM"/> shiny.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <returns>Returns <see cref="bool.True"/> if the <see cref="PKM"/> data was modified.</returns>
        public static bool SetShiny(this PKM pk)
        {
            if (pk.IsShiny)
                return false;

            if (pk.Format > 2)
                pk.SetShinyPID();
            else
                pk.SetShinyIVs();
            return true;
        }

        /// <summary>
        /// Makes a <see cref="PKM"/> not-shiny.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <returns>Returns <see cref="bool.True"/> if the <see cref="PKM"/> data was modified.</returns>
        public static bool SetUnshiny(this PKM pk)
        {
            if (!pk.IsShiny)
                return false;

            pk.SetPIDGender(pk.Gender);
            return true;
        }

        /// <summary>
        /// Sets the <see cref="PKM.Nature"/> value, with special consideration for the <see cref="PKM.Format"/> values which derive the <see cref="PKM.Nature"/> value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="nature">Desired <see cref="PKM.Nature"/> value to set.</param>
        public static void SetNature(this PKM pk, int nature)
        {
            if (pk.Format <= 4)
                pk.SetPIDNature(Math.Max(0, nature));
            else
                pk.Nature = Math.Max(0, nature);
        }

        /// <summary>
        /// Sets the <see cref="PKM.Nature"/> value, with special consideration for the <see cref="PKM.Format"/> values which derive the <see cref="PKM.Nature"/> value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="nature">Desired <see cref="PKM.Nature"/> value to set.</param>
        public static void SetNature(this PKM pk, Nature nature) => pk.SetNature((int) nature);

        /// <summary>
        /// Sets the individual PP Up count values depending if a Move is present in the moveslot or not.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="Moves"><see cref="PKM.Moves"/> to use (if already known). Will fetch the current <see cref="PKM.Moves"/> if not provided.</param>
        public static void SetMaximumPPUps(this PKM pk, int[] Moves = null)
        {
            if (Moves == null)
                Moves = pk.Moves;

            pk.Move1_PPUps = GetPPUpCount(Moves[0]);
            pk.Move2_PPUps = GetPPUpCount(Moves[1]);
            pk.Move3_PPUps = GetPPUpCount(Moves[2]);
            pk.Move4_PPUps = GetPPUpCount(Moves[3]);

            pk.SetMaximumPPCurrent(Moves);
            int GetPPUpCount(int moveID) => moveID > 0 ? 3 : 0;
        }

        /// <summary>
        /// Updates the individual PP count values for each moveslot based on the maximum possible value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="Moves"><see cref="PKM.Moves"/> to use (if already known). Will fetch the current <see cref="PKM.Moves"/> if not provided.</param>
        public static void SetMaximumPPCurrent(this PKM pk, int[] Moves = null)
        {
            if (Moves == null)
                Moves = pk.Moves;

            pk.Move1_PP = pk.GetMovePP(Moves[0], pk.Move1_PPUps);
            pk.Move2_PP = pk.GetMovePP(Moves[1], pk.Move2_PPUps);
            pk.Move3_PP = pk.GetMovePP(Moves[2], pk.Move3_PPUps);
            pk.Move4_PP = pk.GetMovePP(Moves[3], pk.Move4_PPUps);
        }

        /// <summary>
        /// Sets the <see cref="PKM.Gender"/> value, with special consideration for the <see cref="PKM.Format"/> values which derive the <see cref="PKM.Gender"/> value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="gender">Desired <see cref="PKM.Gender"/> value to set.</param>
        public static void SetGender(this PKM pk, string gender)
        {
            if (gender == null)
            {
                int cg = pk.Gender;
                int sane = pk.GetSaneGender(cg);
                if (cg != sane)
                    pk.Gender = sane;
                return;
            }

            int Gender = PKX.GetGenderFromString(gender);
            pk.SetGender(Gender);
        }

        /// <summary>
        /// Sets the <see cref="PKM.Gender"/> value, with special consideration for the <see cref="PKM.Format"/> values which derive the <see cref="PKM.Gender"/> value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="gender">Desired <see cref="PKM.Gender"/> value to set.</param>
        public static void SetGender(this PKM pk, int gender)
        {
            gender = Math.Min(2, Math.Max(0, gender));
            if (pk.Format <= 2)
                pk.SetATKIVGender(gender);
            else if (pk.Format <= 4)
                pk.SetPIDGender(gender);
            else
                pk.Gender = gender;
        }

        /// <summary>
        /// Sets <see cref="PKM.HyperTrainFlags"/> to valid values which may best enhance the <see cref="PKM"/> stats.
        /// </summary>
        /// <param name="pkm"></param>
        /// <param name="IVs"><see cref="PKM.IVs"/> to use (if already known). Will fetch the current <see cref="PKM.IVs"/> if not provided.</param>
        public static void SetSuggestedHyperTrainingData(this PKM pkm, int[] IVs = null)
        {
            if (pkm.Format < 7)
                return;
            if (pkm.CurrentLevel < 100)
            {
                pkm.HyperTrainFlags = 0;
                return;
            }
            if (IVs == null)
                IVs = pkm.IVs;

            pkm.HT_HP = IVs[0] != 31;
            pkm.HT_ATK = IVs[1] != 31 && IVs[1] > 2;
            pkm.HT_DEF = IVs[2] != 31;
            pkm.HT_SPE = IVs[3] != 31 && IVs[3] > 2;
            pkm.HT_SPA = IVs[4] != 31;
            pkm.HT_SPD = IVs[5] != 31;
        }

        /// <summary>
        /// Fetches <see cref="PKM.RelearnMoves"/> based on the provided <see cref="LegalityAnalysis"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="legal"><see cref="LegalityAnalysis"/> which contains parsed information pertaining to legality.</param>
        /// <returns><see cref="PKM.RelearnMoves"/> best suited for the current <see cref="PKM"/> data.</returns>
        public static int[] GetSuggestedRelearnMoves(this PKM pk, LegalityAnalysis legal)
        {
            int[] m = legal.GetSuggestedRelearn();
            if (!m.All(z => z == 0))
                return m;

            if (pk.WasEgg || pk.WasEvent || pk.WasEventEgg || pk.WasLink)
                return m;

            var encounter = legal.GetSuggestedMetInfo();
            if (encounter != null)
                m = encounter.Relearn;

            return m;
        }

        /// <summary>
        /// Sanity checks the provided <see cref="PKM.Gender"/> value, and returns a sane value.
        /// </summary>
        /// <param name="pkm"></param>
        /// <param name="cg">Current <see cref="PKM.Gender"/> preference</param>
        /// <returns>Most-legal <see cref="PKM.Gender"/> value</returns>
        public static int GetSaneGender(this PKM pkm, int cg)
        {
            int gt = pkm.PersonalInfo.Gender;
            switch (gt)
            {
                case 255: return 2; // Genderless
                case 254: return 1; // Female-Only
                case 0: return 0; // Male-Only
            }
            if (cg == 2 || pkm.GenNumber < 6)
                return (byte)pkm.PID <= gt ? 1 : 0;
            return cg;
        }

        /// <summary>
        /// Copies <see cref="ShowdownSet"/> details to the <see cref="PKM"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="Set"><see cref="ShowdownSet"/> details to copy from.</param>
        public static void ApplySetDetails(this PKM pk, ShowdownSet Set)
        {
            pk.Species = Set.Species;
            pk.Moves = Set.Moves;
            pk.HeldItem = Set.HeldItem < 0 ? 0 : Set.HeldItem;
            pk.CurrentLevel = Set.Level;
            pk.CurrentFriendship = Set.Friendship;
            pk.IVs = Set.IVs;
            pk.EVs = Set.EVs;

            pk.SetSuggestedHyperTrainingData(Set.IVs);
            if (ShowdownSetIVMarkings)
                pk.SetMarkings();

            pk.SetNickname(Set.Nickname);
            pk.SetGender(Set.Gender);
            pk.SetAltForm(Set.FormIndex);
            pk.SetMaximumPPUps(Set.Moves);
            pk.SetAbility(Set.Ability);
            pk.SetNature(Set.Nature);
            pk.SetIsShiny(Set.Shiny);
            pk.SetRandomEC();

            var legal = new LegalityAnalysis(pk);
            if (legal.Info.Relearn.Any(z => !z.Valid))
                pk.RelearnMoves = pk.GetSuggestedRelearnMoves(legal);
        }

        /// <summary>
        /// Sets all Memory related data to the default value (zero).
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void ClearMemories(this PKM pk)
        {
            pk.OT_Memory = pk.OT_Affection = pk.OT_Feeling = pk.OT_Intensity = pk.OT_TextVar =
            pk.HT_Memory = pk.HT_Affection = pk.HT_Feeling = pk.HT_Intensity = pk.HT_TextVar = 0;
        }

        /// <summary>
        /// Sets the <see cref="PKM.Markings"/> to indicate flawless (or near-flawless) <see cref="PKM.IVs"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="IVs"><see cref="PKM.IVs"/> to use (if already known). Will fetch the current <see cref="PKM.IVs"/> if not provided.</param>
        public static void SetMarkings(this PKM pk, int[] IVs = null)
        {
            if (pk.Format <= 3)
                return; // no markings (gen3 only has 4; can't mark stats intelligently

            if (IVs == null)
                IVs = pk.IVs;
            pk.Markings = IVs.Select(MarkingMethod(pk)).ToArray();
        }

        public static Func<PKM, Func<int, int, int>> MarkingMethod { get; set; } = FlagHighLow;
        private static Func<int, int, int> FlagHighLow(PKM pk)
        {
            if (pk.Format < 7)
                return GetSimpleMarking;
            return GetComplexMarking;

            int GetSimpleMarking(int val, int index) => val == 31 ? 1 : 0;
            int GetComplexMarking(int val, int index)
            {
                if (val == 31 || val == 1)
                    return 1;
                if (val == 30 || val == 0)
                    return 2;
                return 0;
            }
        }

        /// <summary>
        /// Sets one of the <see cref="PKM.EVs"/> based on its index within the array.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Index to set to</param>
        /// <param name="value">Value to set</param>
        public static void SetEV(this PKM pk, int index, int value)
        {
            switch (index)
            {
                case 0: pk.EV_HP = value; break;
                case 1: pk.EV_ATK = value; break;
                case 2: pk.EV_DEF = value; break;
                case 3: pk.EV_SPE = value; break;
                case 4: pk.EV_SPA = value; break;
                case 5: pk.EV_SPD = value; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
        /// <summary>
        /// Sets one of the <see cref="PKM.IVs"/> based on its index within the array.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Index to set to</param>
        /// <param name="value">Value to set</param>
        public static void SetIV(this PKM pk, int index, int value)
        {
            switch (index)
            {
                case 0: pk.IV_HP = value; break;
                case 1: pk.IV_ATK = value; break;
                case 2: pk.IV_DEF = value; break;
                case 3: pk.IV_SPE = value; break;
                case 4: pk.IV_SPA = value; break;
                case 5: pk.IV_SPD = value; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// Updates the <see cref="PKM.IV_ATK"/> for a Generation 1/2 format <see cref="PKM"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="gender">Desired <see cref="PKM.Gender"/>.</param>
        public static void SetATKIVGender(this PKM pk, int gender)
        {
            do { pk.IV_ATK = (int)(Util.Rand32() & pk.MaxIV); }
            while (pk.Gender != gender);
        }

        /// <summary>
        /// Fetches the highest value the provided <see cref="PKM.EVs"/> index can be while considering others.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Index to fetch for</param>
        /// <returns>Highest value the value can be.</returns>
        public static int GetMaximumEV(this PKM pk, int index)
        {
            if (pk.Format < 3)
                return ushort.MaxValue;

            var EVs = pk.EVs;
            EVs[index] = 0;
            var sum = EVs.Sum();
            int remaining = 510 - sum;
            var newEV = Math.Min(Math.Max(remaining, 0), 252);
            return newEV;
        }

        /// <summary>
        /// Fetches the highest value the provided <see cref="PKM.IVs"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Index to fetch for</param>
        /// <param name="Allow30">Causes the returned value to be dropped down -1 if the value is already at a maxmimum.</param>
        /// <returns>Highest value the value can be.</returns>
        public static int GetMaximumIV(this PKM pk, int index, bool Allow30 = false)
        {
            if (pk.IVs[index] == pk.MaxIV && Allow30)
                return pk.MaxIV - 1;
            return pk.MaxIV;
        }

        /// <summary>
        /// Sets the <see cref="PKM.IVs"/> to match a provided <see cref="hptype"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="hptype">Desired Hidden Power typing.</param>
        public static void SetHiddenPower(this PKM pk, int hptype)
        {
            var IVs = pk.IVs;
            HiddenPower.SetIVsForType(hptype, pk.IVs);
            pk.IVs = IVs;
        }

        /// <summary>
        /// Sets the <see cref="PKM.IVs"/> to match a provided <see cref="hptype"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="hptype">Desired Hidden Power typing.</param>
        public static void SetHiddenPower(this PKM pk, MoveType hptype) => pk.SetHiddenPower((int) hptype);
    }
}
