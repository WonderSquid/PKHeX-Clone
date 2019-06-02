﻿using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 <see cref="SaveFile"/> object.
    /// </summary>
    public abstract class SAV6 : SAV_BEEF, ITrainerStatRecord
    {
        // Save Data Attributes
        protected override string BAKText => $"{OT} ({Version}) - {Played.LastSavedTime}";
        public override string Filter => "Main SAV|*.*";
        public override string Extension => string.Empty;

        protected SAV6(byte[] data, BlockInfo[] blocks, int biOffset) : base(data, blocks, biOffset) { }
        protected SAV6(int size, BlockInfo[] blocks, int biOffset) : base(size, blocks, biOffset) => ClearBoxes();

        // Configuration
        public override int SIZE_STORED => PKX.SIZE_6STORED;
        protected override int SIZE_PARTY => PKX.SIZE_6PARTY;
        public override PKM BlankPKM => new PK6();
        public override Type PKMType => typeof(PK6);

        public override int BoxCount => 31;
        public override int MaxEV => 252;
        public override int Generation => 6;
        protected override int GiftCountMax => 24;
        protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventFlagMax => 8 * 0x180;
        protected override int EventConstMax => (EventFlag - EventConst) / sizeof(ushort);
        public override int OTLength => 12;
        public override int NickLength => 12;

        public override int MaxSpeciesID => Legal.MaxSpeciesID_6;
        public override int MaxBallID => Legal.MaxBallID_6;
        public override int MaxGameID => Legal.MaxGameID_6; // OR

        protected override PKM GetPKM(byte[] data) => new PK6(data);
        protected override byte[] DecryptPKM(byte[] data) => PKX.DecryptArray(data);

        public MyItem Items { get; protected set; }
        public ItemInfo6 ItemInfo { get; protected set; }
        public GameTime6 GameTime { get; protected set; }
        public Situation6 Situation { get; protected set; }
        public PlayTime6 Played { get; protected set; }
        public MyStatus6 Status { get; protected set; }
        public Record6 Records { get; set; }

        // Private Only
        protected int Trainer2 { get; set; } = int.MinValue;
        protected int WondercardFlags { get; set; } = int.MinValue;
        protected int PlayTime { get; set; } = int.MinValue;
        protected int Daycare2 { get; set; } = int.MinValue;
        protected int LinkInfo { get; set; } = int.MinValue;
        protected int JPEG { get; set; } = int.MinValue;

        // Accessible as SAV6
        public int MaisonStats { get; protected set; } = int.MinValue;
        public int Accessories { get; protected set; } = int.MinValue;
        public int PokeDexLanguageFlags { get; protected set; } = int.MinValue;
        public int Spinda { get; protected set; } = int.MinValue;
        public int EncounterCount { get; protected set; } = int.MinValue;

        protected internal const int LongStringLength = 0x22; // bytes, not characters
        protected internal const int ShortStringLength = 0x1A; // bytes, not characters

        // Player Information
        public override int TID { get => Status.TID; set => Status.TID = value; }
        public override int SID { get => Status.SID; set => Status.SID = value; }
        public override int Game { get => Status.Game; set => Status.Game = value; }
        public override int Gender { get => Status.Gender; set => Status.Gender = value; }
        public override int Language { get => Status.Language; set => Status.Language = value; }
        public override string OT { get => Status.OT; set => Status.OT = value; }

        public override uint Money
        {
            get => BitConverter.ToUInt32(Data, Trainer2 + 0x8);
            set => BitConverter.GetBytes(value).CopyTo(Data, Trainer2 + 0x8);
        }

        public int Badges
        {
            get => Data[Trainer2 + 0xC];
            set => Data[Trainer2 + 0xC] = (byte)value;
        }

        public int BP
        {
            get
            {
                int offset = Trainer2 + 0x3C;
                if (ORAS) offset -= 0xC; // 0x30
                return BitConverter.ToUInt16(Data, offset);
            }
            set
            {
                int offset = Trainer2 + 0x3C;
                if (ORAS) offset -= 0xC; // 0x30
                BitConverter.GetBytes((ushort)value).CopyTo(Data, offset);
            }
        }

        public int Vivillon
        {
            get
            {
                int offset = Trainer2 + 0x50;
                if (ORAS) offset -= 0xC; // 0x44
                return Data[offset];
            }
            set
            {
                int offset = Trainer2 + 0x50;
                if (ORAS) offset -= 0xC; // 0x44
                Data[offset] = (byte)value;
            }
        }

        public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
        public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
        public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }

        public override uint SecondsToStart { get => GameTime.SecondsToStart; set => GameTime.SecondsToStart = value; }
        public override uint SecondsToFame { get => GameTime.SecondsToFame; set => GameTime.SecondsToFame = value; }
        public override InventoryPouch[] Inventory { get => Items.Inventory; set => Items.Inventory = value; }

        public ushort GetMaisonStat(int index) { return BitConverter.ToUInt16(Data, MaisonStats + (2 * index)); }
        public void SetMaisonStat(int index, ushort value) { BitConverter.GetBytes(value).CopyTo(Data, MaisonStats + (2 * index)); }

        // Daycare
        public override int DaycareSeedSize => 16;
        public override bool HasTwoDaycares => ORAS;

        // Storage

        public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);

        public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30);

        private int GetBoxNameOffset(int box) => PCLayout + (LongStringLength * box);

        public override string GetBoxName(int box)
        {
            if (PCLayout < 0)
                return $"B{box + 1}";
            return GetString(Data, GetBoxNameOffset(box), LongStringLength / 2);
        }

        public override void SetBoxName(int box, string value)
        {
            var data = SetString(value, LongStringLength / 2, LongStringLength / 2);
            SetData(data, PCLayout + (LongStringLength * box));
        }

        protected override void SetPKM(PKM pkm)
        {
            PK6 pk6 = (PK6)pkm;
            // Apply to this Save File
            int CT = pk6.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk6.Trade(this, Date.Day, Date.Month, Date.Year);
            if (CT != pk6.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk6.Moves.Contains(216)) // Return
                    pk6.CurrentFriendship = pk6.OppositeFriendship;
                else if (pk6.Moves.Contains(218)) // Frustration
                    pkm.CurrentFriendship = pk6.OppositeFriendship;
            }
            pkm.RefreshChecksum();
            AddCountAcquired(pkm);
        }

        private void AddCountAcquired(PKM pkm)
        {
            Records.AddRecord(pkm.WasEgg ? 009 : 007); // egg, capture
            if (pkm.CurrentHandler == 1)
                Records.AddRecord(012); // trade
            if (!pkm.WasEgg)
                Records.AddRecord(005); // wild encounters
        }

        protected override void SetPartyValues(PKM pkm, bool isParty)
        {
            base.SetPartyValues(pkm, isParty);
            ((PK6)pkm).FormDuration = GetFormDuration(pkm, isParty);
        }

        private static uint GetFormDuration(PKM pkm, bool isParty)
        {
            if (!isParty || pkm.AltForm == 0)
                return 0;
            switch (pkm.Species)
            {
                case 676: return 5; // Furfrou
                case 720: return 3; // Hoopa
                default: return 0;
            }
        }

        public override int PartyCount
        {
            get => Data[Party + (6 * SIZE_PARTY)];
            protected set => Data[Party + (6 * SIZE_PARTY)] = (byte)value;
        }

        private int LockedFlagOffset => BattleBox + (6 * SIZE_STORED);

        public override bool BattleBoxLocked
        {
            get => BattleBoxLockedWiFiTournament || BattleBoxLockedLiveTournament;
            set => BattleBoxLockedWiFiTournament = BattleBoxLockedLiveTournament = value;
        }

        public bool BattleBoxLockedWiFiTournament
        {
            get => (Data[LockedFlagOffset] & 1) != 0;
            set => Data[LockedFlagOffset] = (byte)((Data[LockedFlagOffset] & ~1) | (value ? 1 : 0));
        }

        public bool BattleBoxLockedLiveTournament
        {
            get => (Data[LockedFlagOffset] & 2) != 0;
            set => Data[LockedFlagOffset] = (byte)((Data[LockedFlagOffset] & ~2) | (value ? 2 : 0));
        }

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString6(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString6(value, maxLength, PadToSize, PadWith);
        }

        public int GetRecord(int recordID) => Records.GetRecord(recordID);
        public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);
        public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
        public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
        public int RecordCount => Record6.RecordCount;
    }
}