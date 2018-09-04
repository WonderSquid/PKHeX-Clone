using System;

namespace PKHeX.Core
{
    public sealed class InventoryPouch4 : InventoryPouch
    {
        public InventoryPouch4(InventoryType type, ushort[] legal, int maxcount, int offset)
            : base(type, legal, maxcount, offset)
        {

        }

        public override void GetPouch(byte[] Data)
        {
            InventoryItem[] items = new InventoryItem[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new InventoryItem
                {
                    Index = BitConverter.ToUInt16(Data, Offset + (i * 4)),
                    Count = BitConverter.ToUInt16(Data, Offset + (i * 4) + 2)
                };
            }
            Items = items;
        }

        public override void SetPouch(byte[] Data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            for (int i = 0; i < Items.Length; i++)
            {
                BitConverter.GetBytes((ushort)Items[i].Index).CopyTo(Data, Offset + (i * 4));
                BitConverter.GetBytes((ushort)Items[i].Count).CopyTo(Data, Offset + (i * 4) + 2);
            }
        }
    }
}