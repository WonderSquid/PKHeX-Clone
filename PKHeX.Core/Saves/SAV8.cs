﻿using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 <see cref="SaveFile"/> object.
    /// </summary>
    public abstract class SAV8 : SAV_BEEF, ITrainerStatRecord
    {
        // Save Data Attributes
        protected override string BAKText => $"{OT} ({Version}) - {Played.LastSavedTime}";
        public override string Filter => "savedata|*.bin";
        public override string Extension => ".bin";

        public override string[] PKMExtensions => PKM.Extensions.Where(f =>
        {
            int gen = f.Last() - 0x30;
            return gen == 8; // future: change to <= when HOME released
        }).ToArray();

        protected SAV8(byte[] data, BlockInfo[] blocks, int biOffset) : base(data, blocks, biOffset)
        {
            Initialize();
        }

        protected SAV8(int size, BlockInfo[] blocks, int biOffset) : base(size, blocks, biOffset)
        {
            Initialize();
            ClearBoxes();
        }

        private void Initialize()
        {
            BoxLayout = new BoxLayout8(this, GetBlockOffset(SAV8BlockIndex.BOX));

            Box = GetBlockOffset(SAV8BlockIndex.BoxPokemon);
            Party = GetBlockOffset(SAV8BlockIndex.PokePartySave);
            EventFlag = GetBlockOffset(SAV8BlockIndex.EventWork);
            PokeDex = GetBlockOffset(SAV8BlockIndex.ZukanData);

            const int langFlagStart = 0x550; // todo
            Zukan = new Zukan8(this, PokeDex, langFlagStart);
            Items = new MyItem8(this);
            MyStatus = new MyStatus8(this, GetBlockOffset(SAV8BlockIndex.MyStatus));
            Played = new PlayTime8(this, GetBlockOffset(SAV8BlockIndex.PlayTime));
            MiscBlock = new Misc8(this, GetBlockOffset(SAV8BlockIndex.Misc));
            GameTime = new GameTime8(this, GetBlockOffset(SAV8BlockIndex.GameTime));
            OverworldBlock = new FieldMoveModelSave8(this, GetBlockOffset(SAV8BlockIndex.FieldMoveModelSave));
            Records = new Record8(this, GetBlockOffset(SAV8BlockIndex.Records), Core.Records.MaxType_SWSH);
            Situation = new Situation8(this, GetBlockOffset(SAV8BlockIndex.Situation));
            EventWork = new EventWork8(this);
        }

        // Configuration
        public override int SIZE_STORED => PKX.SIZE_8STORED;
        protected override int SIZE_PARTY => PKX.SIZE_8PARTY;
        public override PKM BlankPKM => new PK8();
        public override Type PKMType => typeof(PK8);

        public override int BoxCount => BoxLayout8.BoxCount;
        public override int MaxEV => 252;
        public override int Generation => 8;
        // protected override int GiftCountMax => 48;
        // protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventConstMax => 1000;
        public override int OTLength => 12;
        public override int NickLength => 12;
        protected override PKM GetPKM(byte[] data) => new PK8(data);
        protected override byte[] DecryptPKM(byte[] data) => PKX.DecryptArray8(data);

        // Feature Overrides
        protected override void SetChecksums()
        {
            Blocks.SetChecksums(Data);
        }

        protected override byte[] GetFinalData()
        {
            SetChecksums();
            return Data;
        }

        protected MyItem Items { private get; set; }
        protected Record8 Records { get; private set; }
        public PlayTime8 Played { get; private set; }
        public MyStatus8 MyStatus { get; protected set; }
        public ConfigSave8 Config { get; protected set; }
        public GameTime8 GameTime { get; private set; }
        public Misc8 MiscBlock { get; private set; }
        public Zukan8 Zukan { get; protected set; }
        public EventWork8 EventWork { get; protected set; }
        private BoxLayout8 BoxLayout { get; set; }
        public Situation8 Situation { get; private set; }
        public FieldMoveModelSave8 OverworldBlock { get; private set; }

        public BlockInfo GetBlock(SAV8BlockIndex index) => Blocks[(int)index];
        public int GetBlockOffset(SAV8BlockIndex index) => GetBlock(index).Offset;

        public override GameVersion Version
        {
            get
            {
                var game = (GameVersion)Game;
                if (game == GameVersion.SW || game == GameVersion.SH)
                    return game;
                return GameVersion.Invalid;
            }
        }

        public override string MiscSaveInfo() => string.Join(Environment.NewLine, Blocks.Select(b => b.Summary));
        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString7(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString7(value, maxLength, Language, PadToSize, PadWith);
        }

        // Player Information
        public override int TID { get => MyStatus.TID; set => MyStatus.TID = value; }
        public override int SID { get => MyStatus.SID; set => MyStatus.SID = value; }
        public override int Game { get => MyStatus.Game; set => MyStatus.Game = value; }
        public override int Gender { get => MyStatus.Gender; set => MyStatus.Gender = value; }
        public override int SubRegion { get => MyStatus.SubRegion; set => MyStatus.SubRegion = value; }
        public override int Country { get => MyStatus.Country; set => MyStatus.Country = value; }
        public override int ConsoleRegion { get => MyStatus.ConsoleRegion; set => MyStatus.ConsoleRegion = value; }
        public override int Language { get => MyStatus.Language; set => MyStatus.Language = value; }
        public override string OT { get => MyStatus.OT; set => MyStatus.OT = value; }
        public override uint Money { get => MiscBlock.Money; set => MiscBlock.Money = value; }

        public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
        public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
        public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }
        public override uint SecondsToStart { get => GameTime.SecondsToStart; set => GameTime.SecondsToStart = value; }
        public override uint SecondsToFame { get => GameTime.SecondsToFame; set => GameTime.SecondsToFame = value; }

        // Stat Records
        public int RecordCount => 200;
        public int GetRecord(int recordID) => Records.GetRecord(recordID);
        public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
        public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
        public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);

        // Inventory
        public override InventoryPouch[] Inventory { get => Items.Inventory; set => Items.Inventory = value; }

        // Storage
        public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
        public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30);
        protected override int GetBoxWallpaperOffset(int box) => BoxLayout.GetBoxWallpaperOffset(box);
        public override int GetBoxWallpaper(int box) => BoxLayout.GetBoxWallpaper(box);
        public override void SetBoxWallpaper(int box, int value) => BoxLayout.SetBoxWallpaper(box, value);
        public override string GetBoxName(int box) => BoxLayout[box];
        public override void SetBoxName(int box, string value) => BoxLayout[box] = value;
        public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }
        public override int BoxesUnlocked { get => BoxLayout.BoxesUnlocked; set => BoxLayout.BoxesUnlocked = value; }
        public override byte[] BoxFlags { get => BoxLayout.BoxFlags; set => BoxLayout.BoxFlags = value; }

        protected override void SetPKM(PKM pkm)
        {
            PK8 pk = (PK8)pkm;
            // Apply to this Save File
            int CT = pk.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk.Trade(this, Date.Day, Date.Month, Date.Year);
            if (CT != pk.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk.Moves.Contains(216)) // Return
                    pk.CurrentFriendship = pk.OppositeFriendship;
                else if (pk.Moves.Contains(218)) // Frustration
                    pkm.CurrentFriendship = pk.OppositeFriendship;
            }
            pkm.RefreshChecksum();
            AddCountAcquired(pkm);
        }

        private void AddCountAcquired(PKM pkm)
        {
            Records.AddRecord(pkm.WasEgg ? 008 : 006); // egg, capture
            if (pkm.CurrentHandler == 1)
                Records.AddRecord(011); // trade
            if (!pkm.WasEgg)
                Records.AddRecord(004); // wild encounters
        }

        protected override void SetPartyValues(PKM pkm, bool isParty)
        {
            base.SetPartyValues(pkm, isParty);
            ((PK8)pkm).FormDuration = GetFormDuration(pkm, isParty);
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

        protected override void SetDex(PKM pkm) => Zukan.SetDex(pkm);
        public override bool GetCaught(int species) => Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Zukan.GetSeen(species);

        public override int PartyCount
        {
            get => Data[Party + (6 * SIZE_PARTY)];
            protected set => Data[Party + (6 * SIZE_PARTY)] = (byte)value;
        }
    }
}