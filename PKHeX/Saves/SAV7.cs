﻿using System;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public sealed class SAV7 : SaveFile
    {
        // Save Data Attributes
        public override string BAKName => $"{FileName} [{OT} ({Version}) - {LastSavedTime}].bak";
        public override string Filter => "Main SAV|*.*";
        public override string Extension => "";
        public SAV7(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G7SMDEMO] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            // Load Info
            getBlockInfo();
            getSAVOffsets();

            HeldItems = Legal.HeldItems_SM;
            Personal = PersonalTable.SM;
            if (!Exportable)
                resetBoxes();
        }

        // Configuration
        public override SaveFile Clone() { return new SAV6(Data); }
        
        public override int SIZE_STORED => PKX.SIZE_6STORED;
        public override int SIZE_PARTY => PKX.SIZE_6PARTY;
        public override PKM BlankPKM => new PK7();
        public override Type PKMType => typeof(PK7);

        public override int BoxCount => 32;
        public override int MaxEV => 252;
        public override int Generation => 7;
        protected override int GiftCountMax => -1;
        protected override int GiftFlagMax => -1;
        protected override int EventFlagMax => -1;
        protected override int EventConstMax => (EventFlag - EventConst) / 2;
        public override int OTLength => 12;
        public override int NickLength => 12;

        public override int MaxMoveID => 720;
        public override int MaxSpeciesID => 802;
        public override int MaxItemID => 920;
        public override int MaxAbilityID => 232;
        public override int MaxBallID => 0x1A; // 26
        public override int MaxGameID => 31; // MN

        // Feature Overrides
        public override bool HasGeolocation => true;

        // Blocks & Offsets
        private int BlockInfoOffset;
        private BlockInfo[] Blocks;
        private void getBlockInfo()
        {
            BlockInfoOffset = Data.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(Data, BlockInfoOffset) != SaveUtil.BEEF)
                BlockInfoOffset -= 0x200; // No savegames have more than 0x3D blocks, maybe in the future?
            int count = (Data.Length - BlockInfoOffset - 0x8) / 8;
            BlockInfoOffset += 4;

            Blocks = new BlockInfo[count];
            int CurrentPosition = 0;
            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = new BlockInfo
                {
                    Offset = CurrentPosition,
                    Length = BitConverter.ToInt32(Data, BlockInfoOffset + 0 + 8 * i),
                    ID = BitConverter.ToUInt16(Data, BlockInfoOffset + 4 + 8 * i),
                    Checksum = BitConverter.ToUInt16(Data, BlockInfoOffset + 6 + 8 * i)
                };

                // Expand out to nearest 0x200
                CurrentPosition += Blocks[i].Length % 0x200 == 0 ? Blocks[i].Length : 0x200 - Blocks[i].Length % 0x200 + Blocks[i].Length;

                if ((Blocks[i].ID != 0) || i == 0) continue;
                count = i;
                break;
            }
            // Fix Final Array Lengths
            Array.Resize(ref Blocks, count);
        }
        protected override void setChecksums()
        {
            // Check for invalid block lengths
            if (Blocks.Length < 3) // arbitrary...
            {
                Console.WriteLine("Not enough blocks ({0}), aborting setChecksums", Blocks.Length);
                return;
            }
            // Apply checksums
            for (int i = 0; i < Blocks.Length; i++)
            {
                byte[] array = new byte[Blocks[i].Length];
                Array.Copy(Data, Blocks[i].Offset, array, 0, array.Length);
                BitConverter.GetBytes(SaveUtil.check16(array, Blocks[i].ID)).CopyTo(Data, BlockInfoOffset + 6 + i * 8);
            }

            // MemeCrypto -- provided dll is present.
            try
            {
                byte[] mcSAV = SaveUtil.Resign7(Data);
                if (mcSAV == new byte[0])
                    throw new Exception("MemeCrypto is not present. Dll may not be public at this time.");
                if (mcSAV == null)
                    throw new Exception("MemeCrypto received an invalid input.");
                Data = mcSAV;
            }
            catch (Exception e)
            {
                Util.Alert(e.Message, "Checksums have been applied but MemeCrypto has not.");
            }

        }
        public override bool ChecksumsValid
        {
            get
            {
                for (int i = 0; i < Blocks.Length; i++)
                {
                    byte[] array = new byte[Blocks[i].Length];
                    Array.Copy(Data, Blocks[i].Offset, array, 0, array.Length);
                    if (SaveUtil.check16(array, Blocks[i].ID) != BitConverter.ToUInt16(Data, BlockInfoOffset + 6 + i * 8))
                        return false;
                }
                return true;
            }
        }
        public override string ChecksumInfo
        {
            get
            {
                int invalid = 0;
                string rv = "";
                for (int i = 0; i < Blocks.Length; i++)
                {
                    byte[] array = new byte[Blocks[i].Length];
                    Array.Copy(Data, Blocks[i].Offset, array, 0, array.Length);
                    if (SaveUtil.check16(array, Blocks[i].ID) == BitConverter.ToUInt16(Data, BlockInfoOffset + 6 + i * 8))
                        continue;

                    invalid++;
                    rv += $"Invalid: {i.ToString("X2")} @ Region {Blocks[i].Offset.ToString("X5") + Environment.NewLine}";
                }
                // Return Outputs
                rv += $"SAV: {Blocks.Length - invalid}/{Blocks.Length + Environment.NewLine}";
                return rv;
            }   
        }
        
        private void getSAVOffsets()
        {
            if (SMDEMO)
            {
                /* 00 */ Item = 0x00000; // [DE0]
                /* 01 */ // = 0x00E00; // [07C]
                /* 02 */ // = 0x01000; // [014]
                /* 03 */ TrainerCard = 0x01200; // [0C0]
                /* 04 */ Party = 0x01400; // [61C]
                /* 05 */ // = 0x01C00; // [E00]
                /* 06 */ // = 0x02A00; // [F78]
                /* 07 */ // = 0x03A00; // [228]
                /* 08 */ // = 0x03E00; // [104]
                /* 09 */ // = 0x04000; // [200]
                /* 10 */ Trainer2 = 0x04200; // [020]
                /* 11 */ // = 0x04400; // [004]
                /* 12 */ // = 0x04600; // [058]
                /* 13 */ PCLayout = 0x04800; // [5E6]
                PCBackgrounds = PCLayout + 0x5C0;
                LastViewedBox = PCLayout + 0x5E5; // guess!?
                /* 14 */ Box = 0x04E00; // [36600]
                /* 15 */ // RentalPKM = 0x3B400; // [572C];
                /* 16 */ PlayTime = 0x40C00; // [008];
                /* 17 */ // = 0x40E00; // [1080];
                /* 18 */ // = 0x42000; // [1A08];
                /* 19 */ // = 0x43C00; // [6408];
                /* 20 */ // = 0x4A200; // [6408];
                /* 21 */ // = 0x50800; // [3998];
                /* 22 */ // = 0x54200; // [100];
                /* 23 */ // = 0x54400; // [100];
                /* 24 */ JPEG = 0x54600; // [10528];
                /* 25 */ // = 0x64C00; // [204];
                /* 26 */ // = 0x65000; // [B60];
                /* 27 */ // = 0x65C00; // [3F50];
                /* 28 */ // = 0x69C00; // [358];
                /* 29 */ // = 0x6A000; // [728]; // Data Block
                /* 30 */ // = 0x6A800; // [200];
                /* 31 */ // = 0x6AA00; // [718];
                /* 32 */ // = 0x6B200; // [1FC];
                /* 33 */ // = 0x6B400; // [200];
                /* 34 */ // = 0x6B600; // [120];
                /* 35 */ // = 0x6B800; // [1C8];
                /* 36 */ // = 0x6BA00; // [200];

                OFS_PouchHeldItem =     Item + 0; // 430
                OFS_PouchKeyItem =      OFS_PouchHeldItem + 430*2; // 184
                OFS_PouchTMHM =         OFS_PouchKeyItem + 184*2; // 108
                OFS_PouchMedicine =     OFS_PouchTMHM + 108*2; // 64
                OFS_PouchBerry =        OFS_PouchMedicine + 64*2; // 72
                OFS_PouchZCrystals =    OFS_PouchBerry + 72*2; // 30
            }
            else // Empty input
            {
                Party = 0x0;
                Box = Party + SIZE_PARTY * 6 + 0x1000;
            }
        }

        // Private Only
        private int Item { get; set; } = int.MinValue;
        private int AdventureInfo { get; set; } = int.MinValue;
        private int Trainer2 { get; set; } = int.MinValue;
        private int LastViewedBox { get; set; } = int.MinValue;
        private int WondercardFlags { get; set; } = int.MinValue;
        private int PlayTime { get; set; } = int.MinValue;
        private int JPEG { get; set; } = int.MinValue;
        private int ItemInfo { get; set; } = int.MinValue;
        private int Daycare2 { get; set; } = int.MinValue;
        private int LinkInfo { get; set; } = int.MinValue;

        // Accessible as SAV6
        public int TrainerCard { get; private set; } = 0x14000;
        public int PCFlags { get; private set; } = int.MinValue;
        public int PSSStats { get; private set; } = int.MinValue;
        public int MaisonStats { get; private set; } = int.MinValue;
        public int EonTicket { get; private set; } = int.MinValue;
        public int PCBackgrounds { get; private set; } = int.MinValue;
        public int Contest { get; private set; } = int.MinValue;
        public int Accessories { get; private set; } = int.MinValue;
        public int PokeDexLanguageFlags { get; private set; } = int.MinValue;
        public int Spinda { get; private set; } = int.MinValue;
        public int EncounterCount { get; private set; } = int.MinValue;

        public override GameVersion Version
        {
            get
            {
                switch (Game)
                {
                    case 30: return GameVersion.SN;
                    // case 31: return GameVersion.MN;
                }
                return GameVersion.Unknown;
            }
        }
        
        // Player Information
        public override ushort TID
        {
            get { return BitConverter.ToUInt16(Data, TrainerCard + 0); }
            set { BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 0); }
        }
        public override ushort SID
        {
            get { return BitConverter.ToUInt16(Data, TrainerCard + 2); }
            set { BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 2); }
        }
        public override int Game
        {
            get { return Data[TrainerCard + 4]; }
            set { Data[TrainerCard + 4] = (byte)value; }
        }
        public override int Gender
        {
            get { return Data[TrainerCard + 5]; }
            set { Data[TrainerCard + 5] = (byte)value; }
        }
        public ulong GameSyncID
        {
            get { return BitConverter.ToUInt64(Data, TrainerCard + 0x18); }
            set { BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 0x18); }
        }
        public override int SubRegion
        {
            get { return Data[TrainerCard + 0x26]; }
            set { Data[TrainerCard + 0x26] = (byte)value; }
        }
        public override int Country
        {
            get { return Data[TrainerCard + 0x27]; }
            set { Data[TrainerCard + 0x27] = (byte)value; }
        }
        public override int ConsoleRegion
        {
            get { return Data[TrainerCard + 0x2C]; }
            set { Data[TrainerCard + 0x2C] = (byte)value; }
        }
        public override int Language
        {
            get { return Data[TrainerCard + 0x2D]; }
            set { Data[TrainerCard + 0x2D] = (byte)value; }
        }
        public override string OT
        {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, TrainerCard + 0x38, 0x1A)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(13, '\0')).CopyTo(Data, TrainerCard + 0x38); }
        }
        
        public override uint Money
        {
            get { return BitConverter.ToUInt32(Data, Trainer2 + 0x4); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer2 + 0x4); }
        }
        public override int PlayedHours
        { 
            get { return BitConverter.ToUInt16(Data, PlayTime); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, PlayTime); } 
        }
        public override int PlayedMinutes
        {
            get { return Data[PlayTime + 2]; }
            set { Data[PlayTime + 2] = (byte)value; } 
        }
        public override int PlayedSeconds
        {
            get { return Data[PlayTime + 3]; }
            set { Data[PlayTime + 3] = (byte)value; }
        }
        public uint LastSaved { get { return BitConverter.ToUInt32(Data, PlayTime + 0x4); } set { BitConverter.GetBytes(value).CopyTo(Data, PlayTime + 0x4); } }
        public int LastSavedYear { get { return (int)(LastSaved & 0xFFF); } set { LastSaved = LastSaved & 0xFFFFF000 | (uint)value; } }
        public int LastSavedMonth { get { return (int)(LastSaved >> 12 & 0xF); } set { LastSaved = LastSaved & 0xFFFF0FFF | ((uint)value & 0xF) << 12; } }
        public int LastSavedDay { get { return (int)(LastSaved >> 16 & 0x1F); } set { LastSaved = LastSaved & 0xFFE0FFFF | ((uint)value & 0x1F) << 16; } }
        public int LastSavedHour { get { return (int)(LastSaved >> 21 & 0x1F); } set { LastSaved = LastSaved & 0xFC1FFFFF | ((uint)value & 0x1F) << 21; } }
        public int LastSavedMinute { get { return (int)(LastSaved >> 26 & 0x3F); } set { LastSaved = LastSaved & 0x03FFFFFF | ((uint)value & 0x3F) << 26; } }
        public string LastSavedTime => $"{LastSavedYear.ToString("0000")}{LastSavedMonth.ToString("00")}{LastSavedDay.ToString("00")}{LastSavedHour.ToString("00")}{LastSavedMinute.ToString("00")}";

        public int ResumeYear { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x4); } set { BitConverter.GetBytes(value).CopyTo(Data,AdventureInfo + 0x4); } }
        public int ResumeMonth { get { return Data[AdventureInfo + 0x8]; } set { Data[AdventureInfo + 0x8] = (byte)value; } }
        public int ResumeDay { get { return Data[AdventureInfo + 0x9]; } set { Data[AdventureInfo + 0x9] = (byte)value; } }
        public int ResumeHour { get { return Data[AdventureInfo + 0xB]; } set { Data[AdventureInfo + 0xB] = (byte)value; } }
        public int ResumeMinute { get { return Data[AdventureInfo + 0xC]; } set { Data[AdventureInfo + 0xC] = (byte)value; } }
        public int ResumeSeconds { get { return Data[AdventureInfo + 0xD]; } set { Data[AdventureInfo + 0xD] = (byte)value; } }
        public override int SecondsToStart { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x18); } set { BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x18); } }
        public override int SecondsToFame { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x20); } set { BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x20); } }

        public uint getPSSStat(int index) { return BitConverter.ToUInt32(Data, PSSStats + 4*index); }
        public void setPSSStat(int index, uint value) { BitConverter.GetBytes(value).CopyTo(Data, PSSStats + 4*index); }
        public ushort getMaisonStat(int index) { return BitConverter.ToUInt16(Data, MaisonStats + 2 * index); }
        public void setMaisonStat(int index, ushort value) { BitConverter.GetBytes(value).CopyTo(Data, MaisonStats + 2*index); }

        public int[] SelectItems
        {
            // UP,RIGHT,DOWN,LEFT
            get
            {
                int[] list = new int[4];
                for (int i = 0; i < list.Length; i++)
                    list[i] = BitConverter.ToUInt16(Data, ItemInfo + 10 + 2 * i);
                return list;
            }
            set
            {
                if (value == null || value.Length > 4)
                    return;
                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes((ushort)value[i]).CopyTo(Data, ItemInfo + 10 + 2 * i);
            }
        }
        public int[] RecentItems
        {
            // Items recently interacted with (Give, Use)
            get
            {
                int[] list = new int[12];
                for (int i = 0; i < list.Length; i++)
                    list[i] = BitConverter.ToUInt16(Data, ItemInfo + 20 + 2 * i);
                return list;
            }
            set
            {
                if (value == null || value.Length > 12)
                    return;
                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes((ushort)value[i]).CopyTo(Data, ItemInfo + 20 + 2 * i);
            }
        }

        public override string JPEGTitle => JPEG < 0 ? null : Util.TrimFromZero(Encoding.Unicode.GetString(Data, JPEG, 0x1A));
        public override byte[] JPEGData => JPEG < 0 || Data[JPEG + 0x54] != 0xFF ? null : Data.Skip(JPEG + 0x54).Take(0xE004).ToArray();

        // Inventory
        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Items, Legal.Pouch_Items_SM, 995, OFS_PouchHeldItem),
                    new InventoryPouch(InventoryType.KeyItems, Legal.Pouch_Key_SM, 1, OFS_PouchKeyItem),
                    new InventoryPouch(InventoryType.TMHMs, Legal.Pouch_TMHM_SM, 1, OFS_PouchTMHM),
                    new InventoryPouch(InventoryType.Medicine, Legal.Pouch_Medicine_SM, 995, OFS_PouchMedicine),
                    new InventoryPouch(InventoryType.Berries, Legal.Pouch_Berries_SM, 995, OFS_PouchBerry),
                    new InventoryPouch(InventoryType.ZCrystals, Legal.Pouch_ZCrystal_SM, 995, OFS_PouchZCrystals),
                };
                foreach (var p in pouch)
                    p.getPouch(ref Data);
                return pouch;
            }
            set
            {
                foreach (var p in value)
                    p.setPouch(ref Data);
            }
        }

        // Storage
        public override int CurrentBox { get { return Data[LastViewedBox]; } set { Data[LastViewedBox] = (byte)value; } }
        public override int getPartyOffset(int slot)
        {
            return Party + SIZE_PARTY * slot;
        }
        public override int getBoxOffset(int box)
        {
            return Box + SIZE_STORED*box*30;
        }
        public override int getBoxWallpaper(int box)
        {
            int ofs = PCBackgrounds > 0 && PCBackgrounds < Data.Length ? PCBackgrounds : 0;
            return Data[ofs + box];
        }
        public override string getBoxName(int box)
        {
            if (PCLayout < 0)
                return "B" + (box + 1);
            return Util.TrimFromZero(Encoding.Unicode.GetString(Data, PCLayout + 0x22*box, 0x22));
        }
        public override void setBoxName(int box, string val)
        {
            Encoding.Unicode.GetBytes(val.PadRight(0x11, '\0')).CopyTo(Data, PCLayout + 0x22*box);
            Edited = true;
        }
        public override PKM getPKM(byte[] data)
        {
            return new PK7(data);
        }
        protected override void setPKM(PKM pkm)
        {
            PK7 pk7 = pkm as PK7;
            // Apply to this Save File
            int CT = pk7.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk7.Trade(OT, TID, SID, Country, SubRegion, Gender, false, Date.Day, Date.Month, Date.Year);
            if (CT != pk7.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk7.Moves.Contains(216)) // Return
                    pk7.CurrentFriendship = pk7.OppositeFriendship;
                else if (pk7.Moves.Contains(218)) // Frustration
                    pkm.CurrentFriendship = pk7.OppositeFriendship;
                else if (pk7.CurrentHandler == 1) // OT->HT, needs new Friendship/Affection
                    pk7.TradeFriendshipAffection(OT);
            }
            pkm.RefreshChecksum();
        }
        protected override void setDex(PKM pkm)
        {
            return;
            if (PokeDex < 0)
                return;
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;
            if (Version == GameVersion.Unknown)
                return;

            const int brSize = 0x60;
            int bit = pkm.Species - 1;
            int lang = pkm.Language - 1; if (lang > 5) lang--; // 0-6 language vals
            int origin = pkm.Version;
            int gender = pkm.Gender % 2; // genderless -> male
            int shiny = pkm.IsShiny ? 1 : 0;
            int shiftoff = shiny * brSize * 2 + gender * brSize + brSize;

            // Set the [Species/Gender/Shiny] Owned Flag
            Data[PokeDex + shiftoff + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Owned quality flag
            if (origin < 0x18 && bit < 649 && !ORAS) // Species: 1-649 for X/Y, and not for ORAS; Set the Foreign Owned Flag
                Data[PokeDex + 0x64C + bit / 8] |= (byte)(1 << (bit % 8));
            else if (origin >= 0x18 || ORAS) // Set Native Owned Flag (should always happen)
                Data[PokeDex + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Set the Display flag if none are set
            bool Displayed = false;
            Displayed |= (Data[PokeDex + brSize * 5 + bit / 8 + 0x8] & (byte)(1 << (bit % 8))) != 0;
            Displayed |= (Data[PokeDex + brSize * 6 + bit / 8 + 0x8] & (byte)(1 << (bit % 8))) != 0;
            Displayed |= (Data[PokeDex + brSize * 7 + bit / 8 + 0x8] & (byte)(1 << (bit % 8))) != 0;
            Displayed |= (Data[PokeDex + brSize * 8 + bit / 8 + 0x8] & (byte)(1 << (bit % 8))) != 0;
            if (!Displayed) // offset is already biased by brSize, reuse shiftoff but for the display flags.
                Data[PokeDex + shiftoff + brSize * 4 + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Set the Language
            if (lang < 0) lang = 1;
            Data[PokeDexLanguageFlags + (bit * 7 + lang) / 8] |= (byte)(1 << ((bit * 7 + lang) % 8));
            
            // Set Form flags
            int fc = Personal[pkm.Species].FormeCount;
            int f = ORAS ? SaveUtil.getDexFormIndexORAS(pkm.Species, fc) : SaveUtil.getDexFormIndexXY(pkm.Species, fc);
            if (f < 0) return;

            int FormLen = ORAS ? 0x26 : 0x18;
            int FormDex = PokeDex + 0x8 + brSize*9;
            bit = f + pkm.AltForm;

            // Set Form Seen Flag
            Data[FormDex + FormLen*shiny + bit/8] |= (byte)(1 << (bit%8));

            // Set Displayed Flag if necessary, check all flags
            for (int i = 0; i < fc; i++)
            {
                bit = f + i;
                if ((Data[FormDex + FormLen*2 + bit/8] & (byte) (1 << (bit%8))) != 0) // Nonshiny
                    return; // already set
                if ((Data[FormDex + FormLen*3 + bit/8] & (byte) (1 << (bit%8))) != 0) // Shiny
                    return; // already set
            }
            bit = f + pkm.AltForm;
            Data[FormDex + FormLen * (2 + shiny) + bit / 8] |= (byte)(1 << (bit % 8));
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return PKX.decryptArray(data);
        }
        public override int PartyCount
        {
            get { return Data[Party + 6 * SIZE_PARTY]; }
            protected set { Data[Party + 6 * SIZE_PARTY] = (byte)value; }
        }

        // Mystery Gift
        protected override bool[] MysteryGiftReceivedFlags
        {
            get
            {
                if (WondercardData < 0 || WondercardFlags < 0)
                    return null;

                bool[] r = new bool[(WondercardData-WondercardFlags)*8];
                for (int i = 0; i < r.Length; i++)
                    r[i] = (Data[WondercardFlags + (i>>3)] >> (i&7) & 0x1) == 1;
                return r;
            }
            set
            {
                if (WondercardData < 0 || WondercardFlags < 0)
                    return;
                if ((WondercardData - WondercardFlags)*8 != value?.Length)
                    return;

                byte[] data = new byte[value.Length/8];
                for (int i = 0; i < value.Length; i++)
                    if (value[i])
                        data[i>>3] |= (byte)(1 << (i&7));

                data.CopyTo(Data, WondercardFlags);
                Edited = true;
            }
        }
        protected override MysteryGift[] MysteryGiftCards
        {
            get
            {
                if (WondercardData < 0)
                    return null;
                MysteryGift[] cards = new MysteryGift[GiftCountMax];
                for (int i = 0; i < cards.Length; i++)
                    cards[i] = getWC6(i);

                return cards;
            }
            set
            {
                if (value == null)
                    return;
                if (value.Length > 24)
                    Array.Resize(ref value, 24);
                
                for (int i = 0; i < value.Length; i++)
                    setWC6(value[i], i);
                for (int i = value.Length; i < GiftCountMax; i++)
                    setWC6(new WC6(), i);
            }
        }

        public byte[] LinkBlock
        {
            get
            {
                if (LinkInfo < 0)
                    return null;
                return Data.Skip(LinkInfo).Take(0xC48).ToArray();
            }
            set
            {
                if (LinkInfo < 0)
                    return;
                if (value.Length != 0xC48)
                    return;
                value.CopyTo(Data, LinkInfo);
            }
        }

        private WC6 getWC6(int index)
        {
            if (WondercardData < 0)
                return null;
            if (index < 0 || index > GiftCountMax)
                return null;

            return new WC6(Data.Skip(WondercardData + index * WC6.Size).Take(WC6.Size).ToArray());
        }
        private void setWC6(MysteryGift wc6, int index)
        {
            if (WondercardData < 0)
                return;
            if (index < 0 || index > GiftCountMax)
                return;

            wc6.Data.CopyTo(Data, WondercardData + index * WC6.Size);

            for (int i = 0; i < GiftCountMax; i++)
                if (BitConverter.ToUInt16(Data, WondercardData + i * WC6.Size) == 0)
                    for (int j = i + 1; j < GiftCountMax - i; j++) // Shift everything down
                        Array.Copy(Data, WondercardData + j * WC6.Size, Data, WondercardData + (j - 1) * WC6.Size, WC6.Size);

            Edited = true;
        }

        // Writeback Validity
        public override string MiscSaveChecks()
        {
            string r = "";
            byte[] FFFF = Enumerable.Repeat((byte)0xFF, 0x200).ToArray();
            for (int i = 0; i < Data.Length / 0x200; i++)
            {
                if (!FFFF.SequenceEqual(Data.Skip(i * 0x200).Take(0x200))) continue;
                r = $"0x200 chunk @ 0x{(i*0x200).ToString("X5")} is FF'd."
                    + Environment.NewLine + "Cyber will screw up (as of August 31st 2014)." + Environment.NewLine + Environment.NewLine;

                // Check to see if it is in the Pokedex
                if (i * 0x200 > PokeDex && i * 0x200 < PokeDex + 0x900)
                {
                    r += "Problem lies in the Pokedex. ";
                    if (i * 0x200 == PokeDex + 0x400)
                        r += "Remove a language flag for a species < 585, ie Petilil";
                }
                break;
            }
            return r;
        }
        public override string MiscSaveInfo()
        {
            return Blocks.Aggregate("", (current, b) => current +
                $"{b.ID.ToString("00")}: {b.Offset.ToString("X5")}-{(b.Offset + b.Length).ToString("X5")}, {b.Length.ToString("X5")}{Environment.NewLine}");
        }
    }
}
