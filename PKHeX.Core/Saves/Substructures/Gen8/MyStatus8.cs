﻿using System;

namespace PKHeX.Core
{
    public sealed class MyStatus8 : SaveBlock
    {
        public MyStatus8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

        public ulong Hair
        {
            get => BitConverter.ToUInt64(Data, 0x10);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x10);
        }

        public ulong Brow
        {
            get => BitConverter.ToUInt64(Data, 0x18);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x18);
        }

        public ulong Lashes
        {
            get => BitConverter.ToUInt64(Data, 0x20);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x20);
        }

        public ulong Contacts
        {
            get => BitConverter.ToUInt64(Data, 0x28);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x28);
        }

        public ulong Lips
        {
            get => BitConverter.ToUInt64(Data, 0x30);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x30);
        }

        public ulong Glasses
        {
            get => BitConverter.ToUInt64(Data, 0x38);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x38);
        }

        public ulong Hat
        {
            get => BitConverter.ToUInt64(Data, 0x40);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x40);
        }

        public ulong Jacket
        {
            get => BitConverter.ToUInt64(Data, 0x48);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x48);
        }

        public ulong Top
        {
            get => BitConverter.ToUInt64(Data, 0x50);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x50);
        }

        public ulong Bag
        {
            get => BitConverter.ToUInt64(Data, 0x58);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x58);
        }

        public ulong Gloves
        {
            get => BitConverter.ToUInt64(Data, 0x60);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x60);
        }

        public ulong BottonOrDress
        {
            get => BitConverter.ToUInt64(Data, 0x68);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x68);
        }

        public ulong Sock
        {
            get => BitConverter.ToUInt64(Data, 0x70);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x70);
        }

        public ulong Shoe
        {
            get => BitConverter.ToUInt64(Data, 0x78);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x78);
        }

        // 80 - A0

        public int TID
        {
            get => BitConverter.ToUInt16(Data, 0xA0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA0);
        }

        public int SID
        {
            get => BitConverter.ToUInt16(Data, 0xA2);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA2);
        }

        public int Game
        {
            get => Data[0xA4];
            set => Data[0xA4] = (byte)value;
        }

        public int Gender
        {
            get => Data[0xA5];
            set => Data[0xA5] = (byte)value;
        }

        // A6
        public int Language
        {
            get => Data[Offset + 0xA7];
            set => Data[Offset + 0xA7] = (byte)value;
        }

        public string OT
        {
            get => SAV.GetString(Data, 0xB0, 0x1A);
            set => SAV.SetData(Data, SAV.SetString(value, SAV.OTLength), 0xB0);
        }
    }
}