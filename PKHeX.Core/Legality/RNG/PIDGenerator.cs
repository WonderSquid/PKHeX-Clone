﻿using System;

namespace PKHeX.Core
{
    public static class PIDGenerator
    {
        private static void SetValuesFromSeedLCRNG(PKM pk, PIDType type, uint seed)
        {
            var rng = RNG.LCRNG;
            var A = rng.Next(seed);
            var B = rng.Next(A);
            pk.PID = B & 0xFFFF0000 | A >> 16;

            var skipIV1Frame = type == PIDType.Method_2 || type == PIDType.Method_2_Unown;
            if (skipIV1Frame)
                B = rng.Next(B);
            var C = rng.Next(B);
            var D = rng.Next(C);

            var skipIV2Frame = type == PIDType.Method_4 || type == PIDType.Method_4_Unown;
            if (skipIV2Frame)
                D = rng.Next(D);

            var IVs = MethodFinder.GetIVsInt32(C >> 16, D >> 16);
            if (type == PIDType.Method_1_Roamer)
            {
                IVs[1] &= 7;
                for (int i = 2; i < 6; i++)
                    IVs[i] = 0;
            }
            pk.IVs = IVs;
        }
        private static void SetValuesFromSeedBACD(PKM pk, PIDType type, uint seed)
        {
            var rng = RNG.LCRNG;
            bool shiny = type == PIDType.BACD_R_S || type == PIDType.BACD_U_S;
            uint X = shiny ? rng.Next(seed) : seed;
            var A = rng.Next(X);
            var B = rng.Next(A);
            var C = rng.Next(B);
            var D = rng.Next(C);

            if (shiny)
            {
                uint PID;
                PID = X & 0xFFFF0000 | (uint)pk.SID ^ (uint)pk.TID ^ X >> 16;
                PID &= 0xFFFFFFF8;
                PID |= B >> 16 & 0x7; // lowest 3 bits

                pk.PID = PID;
            }
            else if (type == PIDType.BACD_R_AX || type == PIDType.BACD_U_AX)
            {
                uint low = B >> 16;
                pk.PID = A & 0xFFFF0000 ^ (((uint)pk.TID ^ (uint)pk.SID ^ low) << 16) | low;
            }
            else
                pk.PID = A & 0xFFFF0000 | B >> 16;

            pk.IVs = MethodFinder.GetIVsInt32(C >> 16, D >> 16);

            bool antishiny = type == PIDType.BACD_R_A || type == PIDType.BACD_U_A;
            while (antishiny && pk.IsShiny)
                pk.PID = unchecked(pk.PID + 1);
        }
        private static void SetValuesFromSeedXDRNG(PKM pk, uint seed)
        {
            var rng = RNG.XDRNG;
            var A = rng.Next(seed); // IV1
            var B = rng.Next(A); // IV2
            var C = rng.Next(B); // Ability?
            var D = rng.Next(C); // PID
            var E = rng.Next(D); // PID

            pk.PID = D & 0xFFFF0000 | E >> 16;
            pk.IVs = MethodFinder.GetIVsInt32(A >> 16, B >> 16);
        }
        private static void SetValuesFromSeedChannel(PKM pk, uint seed)
        {
            var rng = RNG.XDRNG;
            var O = rng.Next(seed); // SID
            var A = rng.Next(O); // PID
            var B = rng.Next(A); // PID
            var C = rng.Next(B); // Held Item
            var D = rng.Next(C); // Version
            var E = rng.Next(D); // OT Gender

            var TID = 40122;
            var SID = (int)(O >> 16);
            var pid1 = A >> 16;
            var pid2 = B >> 16;
            pk.TID = TID;
            pk.SID = SID;
            var pid = pid1 << 16 | pid2;
            if ((pid2 > 7 ? 0 : 1) != (pid1 ^ SID ^ TID))
                pid ^= 0x80000000;
            pk.PID = pid;
            pk.HeldItem = (int)(C >> 31) + 169; // 0-Ganlon, 1-Salac
            pk.Version = (int)(D >> 31) + 1; // 0-Sapphire, 1-Ruby
            pk.OT_Gender = (int)(E >> 31);
            pk.IVs = rng.GetSequentialIVsInt32(E);
        }

        public static void SetValuesFromSeed(PKM pk, PIDType type, uint seed)
        {
            var method = GetGeneratorMethod(type);
            method(pk, seed);
        }
        private static Action<PKM, uint> GetGeneratorMethod(PIDType t)
        {
            switch (t)
            {
                case PIDType.Channel:
                    return SetValuesFromSeedChannel;
                case PIDType.CXD:
                    return SetValuesFromSeedXDRNG;

                case PIDType.Method_1:
                case PIDType.Method_2:
                case PIDType.Method_4:
                case PIDType.Method_1_Unown:
                case PIDType.Method_2_Unown:
                case PIDType.Method_4_Unown:
                case PIDType.Method_1_Roamer:
                    return (pk, seed) => SetValuesFromSeedLCRNG(pk, t, seed);

                case PIDType.BACD_R:
                case PIDType.BACD_R_A:
                case PIDType.BACD_R_S:
                    return (pk, seed) => SetValuesFromSeedBACD(pk, t, seed & 0xFFFF);
                case PIDType.BACD_U:
                case PIDType.BACD_U_A:
                case PIDType.BACD_U_S:
                    return (pk, seed) => SetValuesFromSeedBACD(pk, t, seed);

                case PIDType.PokeSpot:
                    return SetRandomPIDIV;

                case PIDType.G5MGShiny:
                    return SetValuesFromSeedMG5Shiny;

                case PIDType.Pokewalker:
                    return (pk, seed) => pk.PID = GetPokeWalkerPID(pk.TID, pk.SID, seed%24, pk.Gender, pk.PersonalInfo.Gender);

                // others: unimplemented
                case PIDType.CuteCharm:
                    break;
                case PIDType.ChainShiny:
                    break;
                case PIDType.G4MGAntiShiny:
                    break;
            }
            return (pk, seed) => { };
        }

        public static void SetRandomPokeSpotPID(PKM pk, int nature, int gender, int ability, int slot)
        {
            while (true)
            {
                var seed = Util.Rand32();
                if (!MethodFinder.IsPokeSpotActivation(slot, seed, out var _))
                    continue;

                var rng = RNG.XDRNG;
                var D = rng.Next(seed); // PID
                var E = rng.Next(D); // PID

                pk.PID = D & 0xFFFF0000 | E >> 16;
                if (!IsValidCriteria4(pk, nature, ability, gender))
                    continue;

                pk.SetRandomIVs();
                return;
            }
        }

        public static uint GetMG5ShinyPID(uint gval, uint av, int TID, int SID)
        {
            uint PID = (uint)((TID ^ SID ^ gval) << 16 | gval);
            if ((PID & 0x10000) != av << 16)
                PID ^= 0x10000;
            return PID;
        }

        public static uint GetPokeWalkerPID(int TID, int SID, uint nature, int gender, int gr)
        {
            if (nature >= 24)
                nature = 0;
            uint pid = (uint)((TID ^ SID) >> 8 ^ 0xFF) << 24; // the most significant byte of the PID is chosen so the Pokémon can never be shiny.
            // Ensure nature is set to required nature without affecting shininess
            pid += nature - pid % 25;

            // Ensure Gender is set to required gender without affecting other properties
            // If Gender is modified, modify the ability if appropriate
            int currentGender = gender;
            if (currentGender == 2)
                return pid;

            // either m/f
            var pidGender = (pid & 0xFF) < gr ? 1 : 0;
            if (currentGender == pidGender)
                return pid;

            if (currentGender == 0) // Male
            {
                pid += (uint)(((gr - (pid & 0xFF)) / 25 + 1) * 25);
                if ((nature & 1) != (pid & 1))
                    pid += 25;
            }
            else
            {
                pid -= (uint)((((pid & 0xFF) - gr) / 25 + 1) * 25);
                if ((nature & 1) != (pid & 1))
                    pid -= 25;
            }
            return pid;
        }

        public static void SetValuesFromSeedMG5Shiny(PKM pk, uint seed)
        {
            var gv = seed >> 24;
            var av = seed & 1;
            pk.PID = GetMG5ShinyPID(gv, av, pk.TID, pk.SID);
            SetRandomIVs(pk);
        }

        public static void SetRandomWildPID(PKM pk, int gen, int nature, int ability, int gender, PIDType specific = PIDType.None)
        {
            if (specific == PIDType.Pokewalker)
            {
                pk.Gender = gender;
                do
                {
                    pk.PID = GetPokeWalkerPID(pk.TID, pk.SID, (uint) nature, gender, pk.PersonalInfo.Gender);
                } while (!pk.IsGenderValid());
                pk.RefreshAbility((int)(pk.PID & 1));
                SetRandomIVs(pk);
                return;
            }
            switch (gen)
            {
                case 3:
                case 4:
                    SetRandomWildPID4(pk, nature, ability, gender, specific);
                    break;
                case 5:
                    SetRandomWildPID5(pk, nature, ability, gender);
                    break;
                default:
                    SetRandomWildPID(pk, nature, ability, gender);
                    break;
            }
        }

        /// <summary>
        /// Generates a <see cref="PKM.PID"/> and <see cref="PKM.IVs"/> that are unrelated.
        /// </summary>
        /// <param name="pkm">Pokémon to modify.</param>
        /// <param name="seed">Seed which is used for the <see cref="PKM.PID"/>.</param>
        private static void SetRandomPIDIV(PKM pkm, uint seed)
        {
            pkm.PID = seed;
            SetRandomIVs(pkm);
        }

        private static void SetRandomWildPID4(PKM pk, int nature, int ability, int gender, PIDType specific = PIDType.None)
        {
            pk.RefreshAbility(ability);
            pk.Gender = gender;
            var type = GetPIDType(pk, specific);
            var method = GetGeneratorMethod(type);

            while (true)
            {
                method(pk, Util.Rand32());
                if (!IsValidCriteria4(pk, nature, ability, gender))
                    continue;
                return;
            }
        }

        private static bool IsValidCriteria4(PKM pk, int nature, int ability, int gender)
        {
            if (pk.GetSaneGender(gender) != gender)
                return false;

            if (pk.Nature != nature)
                return false;

            if ((pk.PID & 1) != ability)
                return false;

            return true;
        }

        private static PIDType GetPIDType(PKM pk, PIDType specific)
        {
            if (specific != PIDType.None)
                return specific;
            if (pk.Version == 15)
                return PIDType.CXD;
            if (pk.GenNumber == 3 && pk.Species == 201)
                return PIDType.Method_1_Unown + Util.Rand.Next(0, 3);

            return PIDType.Method_1;
        }
        private static void SetRandomWildPID5(PKM pk, int nature, int ability, int gender)
        {
            var tidbit = (pk.TID ^ pk.SID) & 1;
            pk.RefreshAbility(ability);
            pk.Gender = gender;
            pk.Nature = nature;

            if (ability == 2)
                ability = 0;

            while (true)
            {
                uint seed = Util.Rand32();
                var bitxor = (seed >> 31) ^ (seed & 1);
                if (bitxor != tidbit)
                    seed ^= 1;

                if (seed % 25 != nature)
                    continue;
                if (((seed >> 16) & 1) != ability)
                    continue;

                pk.PID = seed;
                if (pk.GetSaneGender(gender) != gender)
                    continue;

                SetRandomIVs(pk);
                return;
            }
        }

        private static void SetRandomWildPID(PKM pk, int nature, int ability, int gender)
        {
            uint seed = Util.Rand32();
            pk.PID = seed;
            pk.Nature = nature;
            pk.Gender = gender;
            pk.RefreshAbility(ability);
            SetRandomIVs(pk);
        }

        private static void SetRandomIVs(PKM pk)
        {
            pk.IVs = new[]
            {
                Util.Rand.Next(32),
                Util.Rand.Next(32),
                Util.Rand.Next(32),
                Util.Rand.Next(32),
                Util.Rand.Next(32),
                Util.Rand.Next(32),
            };
        }
    }
}
