﻿using System;

namespace PKHeX.Core
{
    /// <summary> Generation 3 <see cref="PKM"/> format, exclusively for Pokémon XD. </summary>
    public sealed class XK3 : _K3, IShadowPKM
    {
        private static readonly byte[] Unused =
        {
            0x0A, 0x0B, 0x0C, 0x0D, 0x1E, 0x1F,
            0x2A, 0x2B,
            0x7A, 0x7B,
            0x7E, 0x7F
        };

        public override byte[] ExtraBytes => Unused;

        public override int SIZE_PARTY => PKX.SIZE_3XSTORED;
        public override int SIZE_STORED => PKX.SIZE_3XSTORED;
        public override int Format => 3;
        public override PersonalInfo PersonalInfo => PersonalTable.RS[Species];

        public XK3(byte[] decryptedData = null, string ident = null)
        {
            Data = decryptedData ?? new byte[SIZE_PARTY];
            Identifier = ident;
        }

        public XK3() => Data = new byte[SIZE_PARTY];
        public override PKM Clone() => new XK3((byte[])Data.Clone(), Identifier) {Purification = Purification};

        private string GetString(int Offset, int Count) => StringConverter.GetBEString3(Data, Offset, Count);
        private byte[] SetString(string value, int maxLength) => StringConverter.SetBEString3(value, maxLength);

        // Trash Bytes
        public override byte[] Nickname_Trash { get => GetData(0x4E, 20); set { if (value?.Length == 20) value.CopyTo(Data, 0x4E); } }
        public override byte[] OT_Trash { get => GetData(0x38, 20); set { if (value?.Length == 20) value.CopyTo(Data, 0x38); } }

        // Silly Attributes
        public override ushort Sanity { get => 0; set { } } // valid flag set in pkm structure.
        public override ushort Checksum { get => SaveUtil.CRC16_CCITT(Data); set { } } // totally false, just a way to get a 'random' ident for the pkm.
        public override bool ChecksumValid => Valid;

        public override int Species { get => SpeciesConverter.GetG4Species(BigEndian.ToUInt16(Data, 0x00)); set => BigEndian.GetBytes((ushort)SpeciesConverter.GetG3Species(value)).CopyTo(Data, 0x00); }
        public override int SpriteItem => ItemConverter.GetG4Item((ushort)HeldItem);
        public override int HeldItem { get => BigEndian.ToUInt16(Data, 0x02); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x02); }
        public override int Stat_HPCurrent { get => BigEndian.ToUInt16(Data, 0x04); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x04); }
        public override int OT_Friendship { get => BigEndian.ToUInt16(Data, 0x06); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x06); }
        public override int Met_Location { get => BigEndian.ToUInt16(Data, 0x08); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x08); }
        // 0x0A-0x0B Unknown
        // 0x0C-0x0D Unknown
        public override int Met_Level { get => Data[0x0E]; set => Data[0x0E] = (byte)value; }
        public override int Ball { get => Data[0x0F]; set => Data[0x0F] = (byte)value; }
        public override int OT_Gender { get => Data[0x10]; set => Data[0x10] = (byte)value; }
        public override int Stat_Level { get => Data[0x11]; set => Data[0x11] = (byte)value; }
        public override int CNT_Sheen { get => Data[0x12]; set => Data[0x12] = (byte)value; }
        public override int PKRS_Strain { get => Data[0x13] & 0xF; set => Data[0x13] = (byte)(value & 0xF); }
        public override int MarkValue { get => SwapBits(Data[0x14], 1, 2); protected set => Data[0x14] = (byte)SwapBits(value, 1, 2); }
        public override int PKRS_Days { get => Math.Max((sbyte)Data[0x15], (sbyte)0); set => Data[0x15] = (byte)(value == 0 ? 0xFF : value & 0xF); }
        // 0x16-0x1C Battle Related
        private int XDPKMFLAGS { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }
        public bool UnusedFlag0     { get => (XDPKMFLAGS & (1 << 0)) == 1 << 0; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 0)) | (value ? 1 << 0 : 0); }
        public bool UnusedFlag1     { get => (XDPKMFLAGS & (1 << 1)) == 1 << 1; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 1)) | (value ? 1 << 1 : 0); }
        public bool CapturedFlag    { get => (XDPKMFLAGS & (1 << 2)) == 1 << 2; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 2)) | (value ? 1 << 2 : 0); }
        public bool UnusedFlag3     { get => (XDPKMFLAGS & (1 << 3)) == 1 << 3; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 3)) | (value ? 1 << 3 : 0); }
        public bool BlockTrades     { get => (XDPKMFLAGS & (1 << 4)) == 1 << 4; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 4)) | (value ? 1 << 4 : 0); }
        public override bool Valid  { get => (XDPKMFLAGS & (1 << 5)) == 0; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 5)) | (value ? 0 : 1 << 5); } // invalid flag
        public override bool AbilityBit { get => 1 << ((XDPKMFLAGS >> 6) & 1) == 1; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 6)) | ((value ? 1 : 0) << 6); }
        public override bool IsEgg  { get => (XDPKMFLAGS & (1 << 7)) == 1 << 7; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 7)) | (value ? 1 << 7 : 0); }
        // 0x1E-0x1F Unknown
        public override uint EXP { get => BigEndian.ToUInt32(Data, 0x20); set => BigEndian.GetBytes(value).CopyTo(Data, 0x20); }
        public override int SID { get => BigEndian.ToUInt16(Data, 0x24); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x24); }
        public override int TID { get => BigEndian.ToUInt16(Data, 0x26); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x26); }
        public override uint PID { get => BigEndian.ToUInt32(Data, 0x28); set => BigEndian.GetBytes(value).CopyTo(Data, 0x28); }
        // 0x2A-0x2B Unknown
        // 0x2C-0x2F Battle Related
        public override bool FatefulEncounter { get => Data[0x30] == 1; set => Data[0x30] = (byte)(value ? 1 : 0); }
        // 0x31-0x32 Unknown
        public new int EncounterType { get => Data[0x33]; set => Data[0x33] = (byte)value; }
        public override int Version { get => SaveUtil.GetG3VersionID(Data[0x34]); set => Data[0x34] = (byte)SaveUtil.GetCXDVersionID(value); }
        public int CurrentRegion { get => Data[0x35]; set => Data[0x35] = (byte)value; }
        public int OriginalRegion { get => Data[0x36]; set => Data[0x36] = (byte)value; }
        public override int Language { get => PKX.GetMainLangIDfromGC(Data[0x37]); set => Data[0x37] = PKX.GetGCLangIDfromMain((byte)value); }
        public override string OT_Name { get => GetString(0x38, 20); set => SetString(value, 10).CopyTo(Data, 0x38); } // +2 terminator
        public override string Nickname { get => GetString(0x4E, 20); set { SetString(value, 10).CopyTo(Data, 0x4E); Nickname2 = value; } } // +2 terminator
        private string Nickname2 { get => GetString(0x64, 20); set => SetString(value, 10).CopyTo(Data, 0x64); } // +2 terminator
        // 0x7A-0x7B Unknown
        private ushort RIB0 { get => BigEndian.ToUInt16(Data, 0x7C); set => BigEndian.GetBytes(value).CopyTo(Data, 0x7C); }
        public override bool RibbonChampionG3Hoenn   { get => (RIB0 & (1 << 15)) == 1 << 15; set => RIB0 = (ushort)((RIB0 & ~(1 << 15)) | (ushort)(value ? 1 << 15 : 0)); }
        public override bool RibbonWinning           { get => (RIB0 & (1 << 14)) == 1 << 14; set => RIB0 = (ushort)((RIB0 & ~(1 << 14)) | (ushort)(value ? 1 << 14 : 0)); }
        public override bool RibbonVictory           { get => (RIB0 & (1 << 13)) == 1 << 13; set => RIB0 = (ushort)((RIB0 & ~(1 << 13)) | (ushort)(value ? 1 << 13 : 0)); }
        public override bool RibbonArtist            { get => (RIB0 & (1 << 12)) == 1 << 12; set => RIB0 = (ushort)((RIB0 & ~(1 << 12)) | (ushort)(value ? 1 << 12 : 0)); }
        public override bool RibbonEffort            { get => (RIB0 & (1 << 11)) == 1 << 11; set => RIB0 = (ushort)((RIB0 & ~(1 << 11)) | (ushort)(value ? 1 << 11 : 0)); }
        public override bool RibbonChampionBattle    { get => (RIB0 & (1 << 10)) == 1 << 10; set => RIB0 = (ushort)((RIB0 & ~(1 << 10)) | (ushort)(value ? 1 << 10 : 0)); }
        public override bool RibbonChampionRegional  { get => (RIB0 & (1 << 09)) == 1 << 09; set => RIB0 = (ushort)((RIB0 & ~(1 << 09)) | (ushort)(value ? 1 << 09 : 0)); }
        public override bool RibbonChampionNational  { get => (RIB0 & (1 << 08)) == 1 << 08; set => RIB0 = (ushort)((RIB0 & ~(1 << 08)) | (ushort)(value ? 1 << 08 : 0)); }
        public override bool RibbonCountry           { get => (RIB0 & (1 << 07)) == 1 << 07; set => RIB0 = (ushort)((RIB0 & ~(1 << 07)) | (ushort)(value ? 1 << 07 : 0)); }
        public override bool RibbonNational          { get => (RIB0 & (1 << 06)) == 1 << 06; set => RIB0 = (ushort)((RIB0 & ~(1 << 06)) | (ushort)(value ? 1 << 06 : 0)); }
        public override bool RibbonEarth             { get => (RIB0 & (1 << 05)) == 1 << 05; set => RIB0 = (ushort)((RIB0 & ~(1 << 05)) | (ushort)(value ? 1 << 05 : 0)); }
        public override bool RibbonWorld             { get => (RIB0 & (1 << 04)) == 1 << 04; set => RIB0 = (ushort)((RIB0 & ~(1 << 04)) | (ushort)(value ? 1 << 04 : 0)); }
        public override bool Unused1                 { get => (RIB0 & (1 << 03)) == 1 << 03; set => RIB0 = (ushort)((RIB0 & ~(1 << 03)) | (ushort)(value ? 1 << 03 : 0)); }
        public override bool Unused2                 { get => (RIB0 & (1 << 02)) == 1 << 02; set => RIB0 = (ushort)((RIB0 & ~(1 << 02)) | (ushort)(value ? 1 << 02 : 0)); }
        public override bool Unused3                 { get => (RIB0 & (1 << 01)) == 1 << 01; set => RIB0 = (ushort)((RIB0 & ~(1 << 01)) | (ushort)(value ? 1 << 01 : 0)); }
        public override bool Unused4                 { get => (RIB0 & (1 << 00)) == 1 << 00; set => RIB0 = (ushort)((RIB0 & ~(1 << 00)) | (ushort)(value ? 1 << 00 : 0)); }
        // 0x7E-0x7F Unknown

        // Moves
        public override int Move1 { get => BigEndian.ToUInt16(Data, 0x80); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x80); }
        public override int Move1_PP { get => Data[0x82]; set => Data[0x82] = (byte)value; }
        public override int Move1_PPUps { get => Data[0x83]; set => Data[0x83] = (byte)value; }
        public override int Move2 { get => BigEndian.ToUInt16(Data, 0x84); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x84); }
        public override int Move2_PP { get => Data[0x86]; set => Data[0x86] = (byte)value; }
        public override int Move2_PPUps { get => Data[0x87]; set => Data[0x87] = (byte)value; }
        public override int Move3 { get => BigEndian.ToUInt16(Data, 0x88); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x88); }
        public override int Move3_PP { get => Data[0x8A]; set => Data[0x8A] = (byte)value; }
        public override int Move3_PPUps { get => Data[0x8B]; set => Data[0x8B] = (byte)value; }
        public override int Move4 { get => BigEndian.ToUInt16(Data, 0x8C); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x8C); }
        public override int Move4_PP { get => Data[0x8E]; set => Data[0x8E] = (byte)value; }
        public override int Move4_PPUps { get => Data[0x8F]; set => Data[0x8F] = (byte)value; }

        // More party stats
        public override int Stat_HPMax { get => BigEndian.ToUInt16(Data, 0x90); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x90); }
        public override int Stat_ATK { get => BigEndian.ToUInt16(Data, 0x92); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x92); }
        public override int Stat_DEF { get => BigEndian.ToUInt16(Data, 0x94); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x94); }
        public override int Stat_SPA { get => BigEndian.ToUInt16(Data, 0x96); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x96); }
        public override int Stat_SPD { get => BigEndian.ToUInt16(Data, 0x98); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x98); }
        public override int Stat_SPE { get => BigEndian.ToUInt16(Data, 0x9A); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x9A); }

        // EVs
        public override int EV_HP
        {
            get => Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0x9C));
            set => BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0x9C);
        }

        public override int EV_ATK
        {
            get => Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0x9E));
            set => BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0x9E);
        }

        public override int EV_DEF
        {
            get => Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0xA0));
            set => BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0xA0);
        }

        public override int EV_SPA
        {
            get => Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0xA2));
            set => BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0xA2);
        }

        public override int EV_SPD
        {
            get => Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0xA4));
            set => BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0xA4);
        }

        public override int EV_SPE
        {
            get => Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0xA6));
            set => BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0xA6);
        }

        // IVs
        public override int IV_HP { get => Data[0xA8]; set => Data[0xA8] = (byte)(value & 0x1F); }
        public override int IV_ATK { get => Data[0xA9]; set => Data[0xA9] = (byte)(value & 0x1F); }
        public override int IV_DEF { get => Data[0xAA]; set => Data[0xAA] = (byte)(value & 0x1F); }
        public override int IV_SPA { get => Data[0xAB]; set => Data[0xAB] = (byte)(value & 0x1F); }
        public override int IV_SPD { get => Data[0xAC]; set => Data[0xAC] = (byte)(value & 0x1F); }
        public override int IV_SPE { get => Data[0xAD]; set => Data[0xAD] = (byte)(value & 0x1F); }

        // Contest
        public override int CNT_Cool { get => Data[0xAE]; set => Data[0xAE] = (byte)value; }
        public override int CNT_Beauty { get => Data[0xAF]; set => Data[0xAF] = (byte)value; }
        public override int CNT_Cute { get => Data[0xB0]; set => Data[0xB0] = (byte)value; }
        public override int CNT_Smart { get => Data[0xB1]; set => Data[0xB1] = (byte)value; }
        public override int CNT_Tough { get => Data[0xB2]; set => Data[0xB2] = (byte)value; }
        public override int RibbonCountG3Cool { get => Data[0xB3]; set => Data[0xB3] = (byte)value; }
        public override int RibbonCountG3Beauty { get => Data[0xB4]; set => Data[0xB4] = (byte)value; }
        public override int RibbonCountG3Cute { get => Data[0xB5]; set => Data[0xB5] = (byte)value; }
        public override int RibbonCountG3Smart { get => Data[0xB6]; set => Data[0xB6] = (byte)value; }
        public override int RibbonCountG3Tough { get => Data[0xB7]; set => Data[0xB7] = (byte)value; }

        public int ShadowID { get => BigEndian.ToUInt16(Data, 0xBA); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xBA); }

        // Purification information is stored in the save file and accessed based on the Shadow ID.
        public int Purification { get; set; }

        protected override byte[] Encrypt()
        {
            return (byte[])Data.Clone();
        }

        public PK3 ConvertToPK3()
        {
            var pk = ConvertTo<PK3>();
            if (Version == 15)
            {
                // Transferring XK3 to PK3 when it originates from XD sets the fateful encounter (obedience) flag.
                if (ShadowID != 0)
                    pk.RibbonNational = true; // must be purified before trading away; force purify
                if (IsOriginXD())
                    pk.FatefulEncounter = true;
            }
            pk.RefreshChecksum();
            return pk;
        }

        private bool IsOriginXD()
        {
            if (ShadowID != 0)
                return true;
            return IsOriginXD(Species, Met_Level);
        }

        private static bool IsOriginXD(int species, int metLevel)
        {
            switch (species)
            {
                case 296: // Makuhita    30 Colo 18 XD
                case 297:
                    return metLevel != 30;
                case 175: // Togepi      20 Colo 25 XD, also 20 as Togetic in Colo
                case 176:
                    return metLevel != 20;
                case 179: // Mareep      37 Colo 17 XD
                case 180: // Flaafy      30 Colo
                case 181:
                    return metLevel != 37 && metLevel != 30;
                case 219: // Magcargo    30 Colo 38 XD (Slugma in Colo)
                    return metLevel != 30;
                case 195: // Quagsire    30 Colo // ** Wooper XD
                    return metLevel != 30;
                case 334: // Altaria     33 Colo // 36 XD (Swablu in Colo)
                    return metLevel != 33;
                case 167: // Ledian      40 Colo // 10 Ledyba XD
                    return metLevel != 40;
                case 207: // Gligar      43 Colo // ** Gligar XD
                    return metLevel != 43;
                case 221: // Piloswine   43 Colo // 22 Swinub XD
                    return metLevel != 43;
                case 205: // Forretress  43 Colo // 20 Pineco XD
                    return metLevel != 43;
                case 168: // Ariados     43 Colo // 14 Spinarak XD
                    return metLevel != 43;
                case 229: // Houndoom    48 Colo // 17 Houndour XD
                    return metLevel != 48;
                case 217: // Ursaring    45 Colo // 11 Teddiursa XD
                    return metLevel != 45;
                case 212: // Scizor      50 Colo // 40 Scyther XD
                    return metLevel != 50;
                case 196: // Espeon
                    return metLevel != 25;
                case 197: // Umbreon
                    return metLevel != 26;

                    // Gifts
                case 213: // Shuckle
                case 239: case 240: // Elekid
                case 246: case 247: case 248: // Larvitar
                case 307: case 308: // Meditite
                    return metLevel == 20;
            }
            // all other cases handled, if not in Colo's table it's from XD.
            return !Legal.ValidSpecies_Colo.Contains(species);
        }
    }
}
