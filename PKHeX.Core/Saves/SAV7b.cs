﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class SAV7b : SaveFile
    {
        protected override string BAKText => $"{OT} ({Version}) - {Played.LastSavedTime}";
        public override string Filter => "savedata|*.bin";
        public override string Extension => string.Empty;
        public override string[] PKMExtensions => PKM.Extensions.Where(f => f[0] == 'B' && 7 == f[f.Length - 1] - 0x30).ToArray();

        public override Type PKMType => typeof(PB7);
        public override PKM BlankPKM => new PB7();
        public override int SIZE_STORED => SIZE_PARTY;
        protected override int SIZE_PARTY => 260;

        public override SaveFile Clone() => new SAV7b(Data);

        public SAV7b() : this(new byte[SaveUtil.SIZE_G7GG]) { }

        public SAV7b(byte[] data)
        {
            Data = data;
            BAK = (byte[])Data.Clone();
            Exportable = !IsRangeEmpty(0, Data.Length);

            // Load Info
            const int len = 0xB8800; // 1mb always allocated
            BlockInfoOffset = len - 0x1F0;
            Blocks = !Exportable ? BlockInfoGG : BlockInfo3DS.GetBlockInfoData(Data, ref BlockInfoOffset, SaveUtil.CRC16NoInvert, len);
            Personal = PersonalTable.GG;

            Box = GetBlockOffset(BelugaBlockIndex.PokeListPokemon);
            Party = GetBlockOffset(BelugaBlockIndex.PokeListPokemon);
            EventFlag = GetBlockOffset(BelugaBlockIndex.EventWork);
            PokeDex = GetBlockOffset(BelugaBlockIndex.Zukan);
            Zukan = new Zukan7b(this, PokeDex, 0x550);
            Config = new ConfigSave7b(this);
            Items = new MyItem7b(this);
            Storage = new PokeListHeader(this);
            Status = new MyStatus7b(this);
            Played = new PlayTime7b(this);
            Misc = new Misc7b(this);
            EventWork = new EventWork7b(this);

            HeldItems = Legal.HeldItems_GG;

            if (Exportable)
                CanReadChecksums();
            else
                ClearBoxes();
        }

        // Save Block accessors
        public readonly MyItem Items;
        public readonly Misc7b Misc;
        public readonly Zukan7b Zukan;
        public readonly MyStatus7b Status;
        public readonly PlayTime7b Played;
        public readonly ConfigSave7b Config;
        public readonly EventWork7b EventWork;
        public readonly PokeListHeader Storage;

        public override InventoryPouch[] Inventory { get => Items.Inventory; set => Items.Inventory = value; }

        // Feature Overrides
        public override int Generation => 7;
        public override int MaxMoveID => Legal.MaxMoveID_7b;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_7b;
        public override int MaxItemID => Legal.MaxItemID_7b;
        public override int MaxBallID => Legal.MaxBallID_7b;
        public override int MaxGameID => Legal.MaxGameID_7b;
        public override int MaxAbilityID => Legal.MaxAbilityID_7b;

        public override int MaxIV => 31;
        public override int MaxEV => 252;
        public override int OTLength => 12;
        public override int NickLength => 12;
        protected override int GiftCountMax => 48;
        protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventFlagMax => 4160; // 0xDC0 (true max may be up to 0x7F less. 23A8 starts u64 hashvals)
        protected override int EventConstMax => 1000;

        public override bool HasParty => false; // handled via team slots
        public override bool HasEvents => true; // advanced!

        // BoxSlotCount => 1000 -- why couldn't this be a multiple of 30...
        public override int BoxSlotCount => 25;
        public override int BoxCount => 40; // 1000/25

        // Blocks & Offsets
        private readonly int BlockInfoOffset;
        public readonly BlockInfo[] Blocks;
        public override bool ChecksumsValid => CanReadChecksums() && Blocks.GetChecksumsValid(Data);
        public override string ChecksumInfo => CanReadChecksums() ? Blocks.GetChecksumInfo(Data) : string.Empty;

        public BlockInfo GetBlock(BelugaBlockIndex index) => Blocks[(int)index >= Blocks.Length ? 0 : (int)index];
        public int GetBlockOffset(BelugaBlockIndex index) => GetBlock(index).Offset;

        private static readonly BlockInfo[] BlockInfoGG =
        {
            new BlockInfo3DS {Length = 3472, Offset = 0},
            new BlockInfo3DS {Length = 512, Offset = 3584},
            new BlockInfo3DS {Length = 360, Offset = 4096},
            new BlockInfo3DS {Length = 6144, Offset = 4608},
            new BlockInfo3DS {Length = 8424, Offset = 10752},
            new BlockInfo3DS {Length = 2352, Offset = 19456},
            new BlockInfo3DS {Length = 4, Offset = 22016},
            new BlockInfo3DS {Length = 304, Offset = 22528},
            new BlockInfo3DS {Length = 18, Offset = 23040},
            new BlockInfo3DS {Length = 260000, Offset = 23552},
            new BlockInfo3DS {Length = 8, Offset = 283648},
            new BlockInfo3DS {Length = 3728, Offset = 284160},
            new BlockInfo3DS {Length = 4260, Offset = 288256},
            new BlockInfo3DS {Length = 240, Offset = 292864},
            new BlockInfo3DS {Length = 24592, Offset = 293376},
            new BlockInfo3DS {Length = 512, Offset = 318464},
            new BlockInfo3DS {Length = 152, Offset = 318976},
            new BlockInfo3DS {Length = 104, Offset = 319488},
            new BlockInfo3DS {Length = 432000, Offset = 320000},
            new BlockInfo3DS {Length = 176, Offset = 752128},
            new BlockInfo3DS {Length = 2368, Offset = 752640},
        };

        private bool CanReadChecksums()
        {
            if (Blocks.Length <= 3)
            { Debug.WriteLine($"Not enough blocks ({Blocks.Length}), aborting {nameof(CanReadChecksums)}"); return false; }
            return true;
        }

        protected override void SetChecksums()
        {
            if (!CanReadChecksums())
                return;
            Blocks.SetChecksums(Data);
        }

        public bool FixPreWrite() => Storage.CompressStorage();

        protected override void SetPKM(PKM pkm)
        {
            var pk = (PB7)pkm;
            // Apply to this Save File
            int CT = pk.CurrentHandler;
            var Date = DateTime.Now;
            pk.Trade(OT, TID, SID, Gender, Date.Day, Date.Month, Date.Year);
            if (CT != pk.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk.Moves.Contains(216)) // Return
                    pk.CurrentFriendship = pk.OppositeFriendship;
                else if (pk.Moves.Contains(218)) // Frustration
                    pk.CurrentFriendship = pk.OppositeFriendship;
            }
            pk.RefreshChecksum();
        }

        protected override void SetDex(PKM pkm) => Zukan.SetDex(pkm);
        public override bool GetCaught(int species) => Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Zukan.GetSeen(species);

        public override PKM GetPKM(byte[] data) => new PB7(data);
        public override byte[] DecryptPKM(byte[] data) => PKX.DecryptArray(data);
        public override int GetBoxOffset(int box) => Box + (box * BoxSlotCount * SIZE_STORED);
        protected override IList<int>[] SlotPointers => new[] { Storage.PokeListInfo };

        public override int GetPartyOffset(int slot) => Storage.GetPartyOffset(slot);
        public override int PartyCount { get => Storage.PartyCount; protected set => Storage.PartyCount = value; }
        protected override void SetPartyValues(PKM pkm, bool isParty) => base.SetPartyValues(pkm, true);

        public override StorageSlotFlag GetSlotFlags(int index)
        {
            var val = StorageSlotFlag.None;
            if (Storage.PokeListInfo[6] == index)
                val |= StorageSlotFlag.Starter;
            int position = Array.IndexOf(Storage.PokeListInfo, index);
            if ((uint) position < 6)
                val |= (StorageSlotFlag)((int)StorageSlotFlag.Party1 << position);
            return val;
        }

        public override string GetBoxName(int box) => $"Box {box + 1}";
        public override void SetBoxName(int box, string value) { }

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString7(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString7b(value, maxLength, Language, PadToSize, PadWith);
        }

        public override ulong? Secure1
        {
            get => BitConverter.ToUInt64(Data, BlockInfoOffset - 0x14);
            set => BitConverter.GetBytes(value ?? 0).CopyTo(Data, BlockInfoOffset - 0x14);
        }

        public override ulong? Secure2
        {
            get => BitConverter.ToUInt64(Data, BlockInfoOffset - 0xC);
            set => BitConverter.GetBytes(value ?? 0).CopyTo(Data, BlockInfoOffset - 0xC);
        }

        public override GameVersion Version
        {
            get
            {
                switch (Game)
                {
                    case (int)GameVersion.GP: return GameVersion.GP;
                    case (int)GameVersion.GE: return GameVersion.GE;
                    default: return GameVersion.Invalid;
                }
            }
        }

        // Player Information
        public override int TID { get => Status.TID; set => Status.TID = value; }
        public override int SID { get => Status.SID; set => Status.SID = value; }
        public override int Game { get => Status.Game; set => Status.Game = value; }
        public override int Gender { get => Status.Gender; set => Status.Gender = value; }
        public override int Language { get => Status.Language; set => Status.Language = value; }
        public override string OT { get => Status.OT; set => Status.OT = value; }
        public override uint Money { get => Misc.Money; set => Misc.Money = value; }

        public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
        public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
        public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }

        /// <summary>
        /// Gets the <see cref="bool"/> status of a desired Event Flag
        /// </summary>
        /// <param name="flagNumber">Event Flag to check</param>
        /// <returns>Flag is Set (true) or not Set (false)</returns>
        public override bool GetEventFlag(int flagNumber) => EventWork.GetFlag(flagNumber);

        /// <summary>
        /// Sets the <see cref="bool"/> status of a desired Event Flag
        /// </summary>
        /// <param name="flagNumber">Event Flag to check</param>
        /// <param name="value">Event Flag status to set</param>
        /// <remarks>Flag is Set (true) or not Set (false)</remarks>
        public override void SetEventFlag(int flagNumber, bool value) => EventWork.SetFlag(flagNumber, value);
    }
}
