﻿using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class Misc7 : SaveBlock
    {
        public Misc7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public Misc7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        public uint Money
        {
            get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x4));
            set
            {
                if (value > 9_999_999)
                    value = 9_999_999;
                SAV.SetData(BitConverter.GetBytes(value), Offset + 0x4);
            }
        }

        public uint Stamps
        {
            get => (ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x08)) << 13) >> 17;  // 15 stamps; discard top13, lowest4
            set
            {
                uint flags = ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x08)) & 0xFFF8000F;
                flags |= (value & 0x7FFF) << 4;
                SAV.SetData(BitConverter.GetBytes(flags), Offset + 0x08);
            }
        }

        public uint BP
        {
            get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x11C));
            set
            {
                if (value > 9999)
                    value = 9999;
                SAV.SetData(BitConverter.GetBytes(value), Offset + 0x11C);
            }
        }

        public int Vivillon
        {
            get => Data[Offset + 0x130] & 0x1F;
            set => Data[Offset + 0x130] = (byte)((Data[Offset + 0x130] & ~0x1F) | (value & 0x1F));
        }

        public uint StarterEncryptionConstant
        {
            get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x148));
            set => SAV.SetData(BitConverter.GetBytes(value), Offset + 0x148);
        }

        public int DaysFromRefreshed
        {
            get => Data[Offset + 0x123];
            set => Data[Offset + 0x123] = (byte)value;
        }

        public int GetSurfScore(int recordID)
        {
            if ((uint)recordID >= 4)
                recordID = 0;
            return ReadInt32LittleEndian(Data.AsSpan(Offset + 0x138 + (4 * recordID)));
        }

        public void SetSurfScore(int recordID, int score)
        {
            if ((uint)recordID >= 4)
                recordID = 0;
            SAV.SetData(BitConverter.GetBytes(score), Offset + 0x138 + (4 * recordID));
        }
    }
}