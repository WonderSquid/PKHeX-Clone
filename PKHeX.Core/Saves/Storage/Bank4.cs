namespace PKHeX.Core
{
    /// <summary>
    /// PokeStock .stk
    /// </summary>
    public sealed class Bank4 : BulkStorage
    {
        public Bank4(byte[] data) : base(data, typeof(PK4), 0)
        {
            Personal = PersonalTable.HGSS;
            Version = GameVersion.HGSS;
            HeldItems = Legal.HeldItems_HGSS;
        }

        public override string PlayTimeString => SaveUtil.CRC16(Data, 0, Data.Length).ToString("X4");
        protected override string BAKText => PlayTimeString;
        public override string Extension => ".stk";
        public override string Filter { get; } = "PokeStock G4 Storage|*.stk*";

        public override int BoxCount => 64;
        private const int BoxNameSize = 0x18;

        private int BoxDataSize => SlotsPerBox * SIZE_STORED;
        public override int GetBoxOffset(int box) => Box + (BoxDataSize * box);
        public override string GetBoxName(int box) => GetString(GetBoxNameOffset(box), BoxNameSize / 2);
        private int GetBoxNameOffset(int box) => 0x3FC00 + (0x19 * box);

        public static Bank4 GetBank4(byte[] data) => new Bank4(data);
    }
}