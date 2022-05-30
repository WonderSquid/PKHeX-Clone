﻿using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class GameDataPK8 : IGameDataSide, IGigantamax, IDynamaxLevel, ISociability
{
    // Internal Attributes set on creation
    public readonly byte[] Data; // Raw Storage
    public readonly int Offset;

    private const int SIZE = HomeCrypto.SIZE_1GAME_PK8;
    private const HomeGameDataFormat Format = HomeGameDataFormat.PK8;

    public GameDataPK8 Clone() => new(Data.AsSpan(Offset, SIZE).ToArray());

    public GameDataPK8()
    {
        Data = new byte[SIZE];
        Data[0] = (byte)Format;
        WriteUInt16LittleEndian(Data.AsSpan(1, 2), SIZE);
    }

    public GameDataPK8(byte[] data, int offset = 0)
    {
        if ((HomeGameDataFormat)data[offset] != Format)
            throw new ArgumentException($"Invalid GameDataFormat for {Format}");

        if (ReadUInt16LittleEndian(data.AsSpan(offset + 1)) != SIZE)
            throw new ArgumentException($"Invalid GameDataSize for {Format}");

        Data = data;
        Offset = offset + 3;
    }

    public bool CanGigantamax { get => Data[Offset + 0x00] != 0; set => Data[Offset + 0x00] = (byte)(value ? 1 : 0); }
    public uint Sociability { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x01)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x01), value); }

    public int Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x05)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x05), (ushort)value); }
    public int Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x07)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x07), (ushort)value); }
    public int Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x09)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x09), (ushort)value); }
    public int Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0B)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0B), (ushort)value); }

    public int Move1_PP { get => Data[Offset + 0x0D]; set => Data[Offset + 0x0D] = (byte)value; }
    public int Move2_PP { get => Data[Offset + 0x0E]; set => Data[Offset + 0x0E] = (byte)value; }
    public int Move3_PP { get => Data[Offset + 0x0F]; set => Data[Offset + 0x0F] = (byte)value; }
    public int Move4_PP { get => Data[Offset + 0x10]; set => Data[Offset + 0x10] = (byte)value; }
    public int Move1_PPUps { get => Data[Offset + 0x11]; set => Data[Offset + 0x11] = (byte)value; }
    public int Move2_PPUps { get => Data[Offset + 0x12]; set => Data[Offset + 0x12] = (byte)value; }
    public int Move3_PPUps { get => Data[Offset + 0x13]; set => Data[Offset + 0x13] = (byte)value; }
    public int Move4_PPUps { get => Data[Offset + 0x14]; set => Data[Offset + 0x14] = (byte)value; }

    public int RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x15)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x15), (ushort)value); }
    public int RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x17)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x17), (ushort)value); }
    public int RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x19)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x19), (ushort)value); }
    public int RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x1B)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x1B), (ushort)value); }
    public byte DynamaxLevel { get => Data[Offset + 0x1D]; set => Data[Offset + 0x1D] = value; }

    public bool GetPokeJobFlag(int index)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, Offset + 0x1E + ofs, index & 7);
    }

    public void SetPokeJobFlag(int index, bool value)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, Offset + 0x1E + ofs, index & 7, value);
    }

    public bool GetPokeJobFlagAny() => Array.FindIndex(Data, Offset + 0x1E, 14, z => z != 0) >= 0;
    public byte Fullness { get => Data[Offset + 0x2C]; set => Data[Offset + 0x2C] = value; }

    public bool GetMoveRecordFlag(int index)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, Offset + 0x2D + ofs, index & 7);
    }

    public void SetMoveRecordFlag(int index, bool value)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, Offset + 0x2D + ofs, index & 7, value);
    }

    public bool GetMoveRecordFlagAny() => Array.FindIndex(Data, Offset + 0x2D, 14, z => z != 0) >= 0;

    public int Palma { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x3B)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x3B), value); }
    public int Ball { get => Data[Offset + 0x3F]; set => Data[Offset + 0x3F] = (byte)value; }
    public int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x40)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x40), (ushort)value); }
    public int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x42)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x42), (ushort)value); }

    // Not stored.
    public PersonalInfo GetPersonalInfo(int species, int form) => PersonalTable.SWSH.GetFormEntry(species, form);

    public int CopyTo(Span<byte> result)
    {
        result[0] = (byte)Format;
        WriteUInt16LittleEndian(result[1..], SIZE);
        Data.AsSpan(Offset, SIZE).CopyTo(result[3..]);
        return 3 + SIZE;
    }
}