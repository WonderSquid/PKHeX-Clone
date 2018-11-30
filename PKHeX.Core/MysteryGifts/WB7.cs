﻿using System;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Mystery Gift Template File
    /// </summary>
    public sealed class WB7 : MysteryGift, IRibbonSetEvent3, IRibbonSetEvent4, ILangNick, IAwakened
    {
        public const int Size = 0x108;
        public const int SizeFull = 0x310;
        private const int CardStart = SizeFull - Size;

        public override int Format => 7;

        public WB7() => Data = new byte[SizeFull];
        public WB7(byte[] data) => Data = data;

        public byte RestrictVersion { get => Data[0]; set => Data[0] = value; }

        public bool CanBeReceivedByVersion(int v)
        {
            if (v < (int)GameVersion.GP || v > (int)GameVersion.GE)
                return false;
            if (RestrictVersion == 0)
                return true; // no data
            var bitIndex = v - (int)GameVersion.GP;
            var bit = 1 << bitIndex;
            return (RestrictVersion & bit) != 0;
        }

        // General Card Properties
        public override int CardID
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0);
        }

        public override string CardTitle
        {
            // Max len 36 char, followed by null terminator
            get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, CardStart + 2, 72));
            set => Encoding.Unicode.GetBytes(value.PadRight(36, '\0')).CopyTo(Data, CardStart + 2);
        }

        private uint RawDate
        {
            get => BitConverter.ToUInt32(Data, CardStart + 0x4C);
            set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0x4C);
        }

        private uint Year
        {
            get => (RawDate / 10000) + 2000;
            set => RawDate = ((value - 2000) * 10000) + (RawDate % 10000);
        }

        private uint Month
        {
            get => RawDate % 10000 / 100;
            set => RawDate = ((Year - 2000) * 10000) + (value * 100) + (RawDate % 100);
        }

        private uint Day
        {
            get => RawDate % 100;
            set => RawDate = ((Year - 2000) * 10000) + (Month * 100) + value;
        }

        /// <summary>
        /// Gets or sets the date of the card.
        /// </summary>
        public DateTime? Date
        {
            get
            {
                // Check to see if date is valid
                if (!Util.IsDateValid(Year, Month, Day))
                    return null;

                return new DateTime((int)Year, (int)Month, (int)Day);
            }
            set
            {
                if (value.HasValue)
                {
                    // Only update the properties if a value is provided.
                    Year = (ushort)value.Value.Year;
                    Month = (byte)value.Value.Month;
                    Day = (byte)value.Value.Day;
                }
                else
                {
                    // Clear the Met Date.
                    // If code tries to access MetDate again, null will be returned.
                    Year = 0;
                    Month = 0;
                    Day = 0;
                }
            }
        }

        public int CardLocation { get => Data[CardStart + 0x50]; set => Data[CardStart + 0x50] = (byte)value; }

        public int CardType { get => Data[CardStart + 0x51]; set => Data[CardStart + 0x51] = (byte)value; }
        public byte CardFlags { get => Data[CardStart + 0x52]; set => Data[CardStart + 0x52] = value; }

        public bool GiftRepeatable { get => (CardFlags & 1) == 0; set => CardFlags = (byte)((CardFlags & ~1) | (value ? 0 : 1)); }
        public override bool GiftUsed { get => (CardFlags & 2) == 2; set => CardFlags = (byte)((CardFlags & ~2) | (value ? 2 : 0)); }
        public bool GiftOncePerDay { get => (CardFlags & 4) == 4; set => CardFlags = (byte)((CardFlags & ~4) | (value ? 4 : 0)); }

        public bool MultiObtain { get => Data[CardStart + 0x53] == 1; set => Data[CardStart + 0x53] = (byte)(value ? 1 : 0); }

        // BP Properties
        public override bool IsBP { get => CardType == 3; set { if (value) CardType = 3; } }
        public override int BP { get => ItemID; set => ItemID = value; }

        // Bean (Mame) Properties
        public override bool IsBean { get => CardType == 2; set { if (value) CardType = 2; } }
        public override int Bean { get => ItemID; set => ItemID = value; }

        // Item Properties
        public override bool IsItem { get => CardType == 1; set { if (value) CardType = 1; } }
        public override int ItemID { get => BitConverter.ToUInt16(Data, CardStart + 0x68); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x68); }
        public int GetItem(int index) => BitConverter.ToUInt16(Data, CardStart + 0x68 + (0x4 * index));
        public void SetItem(int index, ushort item) => BitConverter.GetBytes(item).CopyTo(Data, CardStart + 0x68 + (4 * index));
        public int GetQuantity(int index) => BitConverter.ToUInt16(Data, CardStart + 0x6A + (0x4 * index));
        public void SetQuantity(int index, ushort quantity) => BitConverter.GetBytes(quantity).CopyTo(Data, CardStart + 0x6A + (4 * index));

        public override int Quantity
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0x6A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x6A);
        }

        // Pokémon Properties
        public override bool IsPokémon { get => CardType == 0; set { if (value) CardType = 0; } }
        public override bool IsShiny => PIDType == Shiny.Always;

        public override int TID
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0x68);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x68);
        }

        public override int SID {
            get => BitConverter.ToUInt16(Data, CardStart + 0x6A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x6A);
        }

        public int OriginGame
        {
            get => BitConverter.ToInt32(Data, CardStart + 0x6C);
            set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0x6C);
        }

        public uint EncryptionConstant {
            get => BitConverter.ToUInt32(Data, CardStart + 0x70);
            set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0x70);
        }

        public override int Ball
        {
            get => Data[CardStart + 0x76];
            set => Data[CardStart + 0x76] = (byte)value; }

        public override int HeldItem // no references
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0x78);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x78);
        }

        public int Move1 { get => BitConverter.ToUInt16(Data, CardStart + 0x7A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x7A); }
        public int Move2 { get => BitConverter.ToUInt16(Data, CardStart + 0x7C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x7C); }
        public int Move3 { get => BitConverter.ToUInt16(Data, CardStart + 0x7E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x7E); }
        public int Move4 { get => BitConverter.ToUInt16(Data, CardStart + 0x80); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x80); }
        public override int Species { get => BitConverter.ToUInt16(Data, CardStart + 0x82); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x82); }
        public override int Form { get => Data[CardStart + 0x84]; set => Data[CardStart + 0x84] = (byte)value; }

        // public int Language { get => Data[CardStart + 0x85]; set => Data[CardStart + 0x85] = (byte)value; }

        // public string Nickname
        // {
        //     get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, CardStart + 0x86, 0x1A));
        //     set => Encoding.Unicode.GetBytes(value.PadRight(12 + 1, '\0')).CopyTo(Data, CardStart + 0x86);
        // }

        public int Nature { get => (sbyte)Data[CardStart + 0xA0]; set => Data[CardStart + 0xA0] = (byte)value; }
        public override int Gender { get => Data[CardStart + 0xA1]; set => Data[CardStart + 0xA1] = (byte)value; }
        public override int AbilityType { get => Data[CardStart + 0xA2]; set => Data[CardStart + 0xA2] = (byte)value; } // no references
        public Shiny PIDType { get => (Shiny)Data[CardStart + 0xA3]; set => Data[CardStart + 0xA3] = (byte)value; }
        public override int EggLocation { get => BitConverter.ToUInt16(Data, CardStart + 0xA4); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xA4); }
        public int MetLocation  { get => BitConverter.ToUInt16(Data, CardStart + 0xA6); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xA6); }
        public int MetLevel { get => Data[CardStart + 0xA8]; set => Data[CardStart + 0xA8] = (byte)value; }

        public int IV_HP { get => Data[CardStart + 0xAF]; set => Data[CardStart + 0xAF] = (byte)value; }
        public int IV_ATK { get => Data[CardStart + 0xB0]; set => Data[CardStart + 0xB0] = (byte)value; }
        public int IV_DEF { get => Data[CardStart + 0xB1]; set => Data[CardStart + 0xB1] = (byte)value; }
        public int IV_SPE { get => Data[CardStart + 0xB2]; set => Data[CardStart + 0xB2] = (byte)value; }
        public int IV_SPA { get => Data[CardStart + 0xB3]; set => Data[CardStart + 0xB3] = (byte)value; }
        public int IV_SPD { get => Data[CardStart + 0xB4]; set => Data[CardStart + 0xB4] = (byte)value; }

        public int OTGender { get => Data[CardStart + 0xB5]; set => Data[CardStart + 0xB5] = (byte)value; }

        // public override string OT_Name
        // {
        //     get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, CardStart + 0xB6, 0x1A));
        //     set => Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, CardStart + 0xB6);
        // }

        public override int Level { get => Data[CardStart + 0xD0]; set => Data[CardStart + 0xD0] = (byte)value; }
        public override bool IsEgg { get => Data[CardStart + 0xD1] == 1; set => Data[CardStart + 0xD1] = (byte)(value ? 1 : 0); }
        public ushort AdditionalItem { get => BitConverter.ToUInt16(Data, CardStart + 0xD2); set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0xD2); }

        public uint PID { get => BitConverter.ToUInt32(Data, 0xD4); set => BitConverter.GetBytes(value).CopyTo(Data, 0xD4); }
        public int RelearnMove1 { get => BitConverter.ToUInt16(Data, CardStart + 0xD8); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xD8); }
        public int RelearnMove2 { get => BitConverter.ToUInt16(Data, CardStart + 0xDA); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xDA); }
        public int RelearnMove3 { get => BitConverter.ToUInt16(Data, CardStart + 0xDC); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xDC); }
        public int RelearnMove4 { get => BitConverter.ToUInt16(Data, CardStart + 0xDE); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0xDE); }

        public int AV_HP {  get => Data[CardStart + 0xE6]; set => Data[CardStart + 0xE6] = (byte)value; }
        public int AV_ATK { get => Data[CardStart + 0xE7]; set => Data[CardStart + 0xE7] = (byte)value; }
        public int AV_DEF { get => Data[CardStart + 0xE8]; set => Data[CardStart + 0xE8] = (byte)value; }
        public int AV_SPE { get => Data[CardStart + 0xE9]; set => Data[CardStart + 0xE9] = (byte)value; }
        public int AV_SPA { get => Data[CardStart + 0xEA]; set => Data[CardStart + 0xEA] = (byte)value; }
        public int AV_SPD { get => Data[CardStart + 0xEB]; set => Data[CardStart + 0xEB] = (byte)value; }

        private byte RIB0 { get => Data[CardStart + 0x74]; set => Data[CardStart + 0x74] = value; }
        private byte RIB1 { get => Data[CardStart + 0x75]; set => Data[CardStart + 0x75] = value; }
        public bool RibbonChampionBattle   { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonChampionRegional { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonChampionNational { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonCountry          { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonNational         { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonEarth            { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonWorld            { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RibbonEvent            { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
        public bool RibbonChampionWorld    { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
        public bool RibbonBirthday         { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
        public bool RibbonSpecial          { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
        public bool RibbonSouvenir         { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
        public bool RibbonWishing          { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
        public bool RibbonClassic          { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
        public bool RibbonPremier          { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
        public bool RIB1_7                 { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

        // Meta Accessible Properties
        public override int[] IVs
        {
            get => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
            set
            {
                if (value?.Length != 6) return;
                IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
                IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
            }
        }

        public bool GetIsNicknamed(int language) => Data[GetNicknameOffset(language)] != 0;

        private int GetLanguageIndex(int language)
        {
            var lang = (LanguageID) language;
            if (lang < LanguageID.Japanese || lang == LanguageID.UNUSED_6)
                return (int) LanguageID.English; // fallback
            return lang < LanguageID.UNUSED_6 ? language - 1 : language - 2;
        }

        public override int Location { get => MetLocation; set => MetLocation = (ushort)value; }

        public override int[] Moves
        {
            get => new[] { Move1, Move2, Move3, Move4 };
            set
            {
                if (value.Length > 0) Move1 = value[0];
                if (value.Length > 1) Move2 = value[1];
                if (value.Length > 2) Move3 = value[2];
                if (value.Length > 3) Move4 = value[3];
            }
        }

        public override int[] RelearnMoves
        {
            get => new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 };
            set
            {
                if (value.Length > 0) RelearnMove1 = value[0];
                if (value.Length > 1) RelearnMove2 = value[1];
                if (value.Length > 2) RelearnMove3 = value[2];
                if (value.Length > 3) RelearnMove4 = value[3];
            }
        }

        public override string OT_Name { get; set; } = string.Empty;
        public string Nickname => string.Empty;
        public bool IsNicknamed => false;
        public int Language => 2;

        public string GetNickname(int language) => Util.TrimFromZero(Encoding.Unicode.GetString(Data, GetNicknameOffset(language), 0x1A));
        public void SetNickname(int language, string value) => Encoding.Unicode.GetBytes(value.PadRight(0x1A / 2, '\0')).CopyTo(Data, GetNicknameOffset(language));

        public string GetOT(int language) => Util.TrimFromZero(Encoding.Unicode.GetString(Data, GetOTOffset(language), 0x1A));
        public void SetOT(int language, string value) => Encoding.Unicode.GetBytes(value.PadRight(0x1A / 2, '\0')).CopyTo(Data, GetOTOffset(language));

        private int GetNicknameOffset(int language)
        {
            int index = GetLanguageIndex(language);
            return 0x04 + (index * 0x1A);
        }

        private int GetOTOffset(int language)
        {
            int index = GetLanguageIndex(language);
            return 0xEE + (index * 0x1A);
        }

        public override PKM ConvertToPKM(ITrainerInfo SAV)
        {
            if (!IsPokémon)
                return null;

            int currentLevel = Level > 0 ? Level : Util.Rand.Next(100) + 1;
            int metLevel = MetLevel > 0 ? MetLevel : currentLevel;
            var pi = PersonalTable.USUM.GetFormeEntry(Species, Form);
            var OT = GetOT(SAV.Language);
            var pk = new PB7
            {
                Species = Species,
                HeldItem = HeldItem,
                TID = TID,
                SID = SID,
                Met_Level = metLevel,
                Nature = Nature != -1 ? Nature : Util.Rand.Next(25),
                Gender = Gender != 3 ? Gender : pi.RandomGender,
                AltForm = Form,
                EncryptionConstant = EncryptionConstant != 0 ? EncryptionConstant : Util.Rand32(),
                Version = OriginGame != 0 ? OriginGame : SAV.Game,
                Language = SAV.Language,
                Ball = Ball,
                Country = SAV.Country,
                Region = SAV.SubRegion,
                ConsoleRegion = SAV.ConsoleRegion,
                Move1 = Move1, Move2 = Move2, Move3 = Move3, Move4 = Move4,
                RelearnMove1 = RelearnMove1, RelearnMove2 = RelearnMove2,
                RelearnMove3 = RelearnMove3, RelearnMove4 = RelearnMove4,
                Met_Location = MetLocation,
                Egg_Location = EggLocation,
                AV_HP = AV_HP,
                AV_ATK = AV_ATK,
                AV_DEF = AV_DEF,
                AV_SPE = AV_SPE,
                AV_SPA = AV_SPA,
                AV_SPD = AV_SPD,

                OT_Name = OT.Length > 0 ? OT : SAV.OT,
                OT_Gender = OTGender != 3 ? OTGender % 2 : SAV.Gender,
                HT_Name = OT_Name.Length > 0 ? SAV.OT : string.Empty,
                HT_Gender = OT_Name.Length > 0 ? SAV.Gender : 0,
                CurrentHandler = OT_Name.Length > 0 ? 1 : 0,

                EXP = Experience.GetEXP(currentLevel, Species, 0),

                // Ribbons
                RibbonCountry = RibbonCountry,
                RibbonNational = RibbonNational,

                RibbonEarth = RibbonEarth,
                RibbonWorld = RibbonWorld,
                RibbonClassic = RibbonClassic,
                RibbonPremier = RibbonPremier,
                RibbonEvent = RibbonEvent,
                RibbonBirthday = RibbonBirthday,
                RibbonSpecial = RibbonSpecial,
                RibbonSouvenir = RibbonSouvenir,

                RibbonWishing = RibbonWishing,
                RibbonChampionBattle = RibbonChampionBattle,
                RibbonChampionRegional = RibbonChampionRegional,
                RibbonChampionNational = RibbonChampionNational,
                RibbonChampionWorld = RibbonChampionWorld,

                OT_Friendship = pi.BaseFriendship,
                FatefulEncounter = true,
            };

            if ((SAV.Generation > Format && OriginGame == 0) || !CanBeReceivedByVersion(pk.Version))
            {
                // give random valid game
                do { pk.Version = (int)GameVersion.GP + Util.Rand.Next(2); }
                while (!CanBeReceivedByVersion(pk.Version));
            }

            pk.SetMaximumPPCurrent();

            if (OTGender == 3)
            {
                pk.TID = SAV.TID;
                pk.SID = SAV.SID;
            }

            pk.MetDate = Date ?? DateTime.Now;

            pk.IsNicknamed = GetIsNicknamed(pk.Language);
            pk.Nickname = pk.IsNicknamed ? GetNickname(pk.Language) : PKX.GetSpeciesNameGeneration(Species, pk.Language, Format);

            int[] finalIVs = new int[6];
            var ivflag = Array.Find(IVs, iv => (byte)(iv - 0xFC) < 3);
            if (ivflag == 0) // Random IVs
            {
                for (int i = 0; i < 6; i++)
                    finalIVs[i] = IVs[i] > 31 ? Util.Rand.Next(pk.MaxIV + 1) : IVs[i];
            }
            else // 1/2/3 perfect IVs
            {
                int IVCount = ivflag - 0xFB;
                do { finalIVs[Util.Rand.Next(6)] = 31; }
                while (finalIVs.Count(r => r == 31) < IVCount);
                for (int i = 0; i < 6; i++)
                    finalIVs[i] = finalIVs[i] == 31 ? pk.MaxIV : Util.Rand.Next(pk.MaxIV + 1);
            }
            pk.IVs = finalIVs;

            int av = 0;
            switch (AbilityType)
            {
                case 00: // 0 - 0
                case 01: // 1 - 1
                case 02: // 2 - H
                    av = AbilityType;
                    break;
                case 03: // 0/1
                case 04: // 0/1/H
                    av = Util.Rand.Next(AbilityType - 1);
                    break;
            }
            pk.Ability = pi.Abilities[av];
            pk.AbilityNumber = 1 << av;

            switch (PIDType)
            {
                case Shiny.FixedValue: // Specified
                    pk.PID = PID;
                    break;
                case Shiny.Random: // Random
                    pk.PID = Util.Rand32();
                    break;
                case Shiny.Always: // Random Shiny
                    pk.PID = Util.Rand32();
                    pk.PID = (uint)(((pk.TID ^ pk.SID ^ (pk.PID & 0xFFFF)) << 16) | (pk.PID & 0xFFFF));
                    break;
                case Shiny.Never: // Random Nonshiny
                    pk.PID = Util.Rand32();
                    if (pk.IsShiny) pk.PID ^= 0x10000000;
                    break;
            }

            if (IsEgg)
            {
                pk.IsEgg = true;
                pk.EggMetDate = Date;
                pk.Nickname = PKX.GetSpeciesNameGeneration(0, pk.Language, Format);
                pk.IsNicknamed = true;
            }
            pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

            pk.HeightScalar = Util.Rand.Next(0x100);
            pk.WeightScalar = Util.Rand.Next(0x100);
            pk.ResetCalculatedValues(); // cp & dimensions

            pk.RefreshChecksum();
            return pk;
        }
    }
}
