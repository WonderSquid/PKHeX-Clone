﻿using System;
using System.Collections.Generic;
using static PKHeX.Core.EncounterServerDateCheck;

namespace PKHeX.Core;

public interface IEncounterServerDate
{
    bool IsDateRestricted { get; }
}

public enum EncounterServerDateCheck
{
    None,
    Valid,
    Invalid,
}

public static class EncounterServerDate
{
    private static bool IsValidDate(DateTime obtained, DateTime start) => obtained >= start && obtained <= DateTime.UtcNow;
    private static bool IsValidDate(DateTime obtained, DateTime start, DateTime end) => obtained >= start && obtained <= end;

  //private static bool IsValidDate(DateTime obtained, (DateTime Start, DateTime? End) value)
  //{
  //    var (start, end) = value;
  //    if (end is not { } x)
  //        return IsValidDate(obtained, start);
  //    return IsValidDate(obtained, start, x);
  //}

    private static bool IsValidDate(DateTime obtained, (DateTime Start, DateTime End) value)
    {
        var (start, end) = value;
        return IsValidDate(obtained, start, end);
    }

    private static EncounterServerDateCheck Result(bool result) => result ? Valid : Invalid;

    public static EncounterServerDateCheck IsValidDate(this IEncounterServerDate enc, DateTime obtained) => enc switch
    {
        WC8 wc8 => Result(IsValidDateWC8(wc8.CardID, obtained)),
        WA8 wa8 => Result(IsValidDateWA8(wa8.CardID, obtained)),
        _ => throw new ArgumentOutOfRangeException(nameof(enc)),
    };

    public static bool IsValidDateWC8(int cardID, DateTime obtained) => WC8Gifts.TryGetValue(cardID, out var time) && IsValidDate(obtained, time);
    public static bool IsValidDateWA8(int cardID, DateTime obtained) => WA8Gifts.TryGetValue(cardID, out var time) && IsValidDate(obtained, time);

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, DateTime> WC8Gifts = new()
    {
        {9000, new DateTime(2020, 02, 12)}, // Bulbasaur
        {9001, new DateTime(2020, 02, 12)}, // Charmander
        {9002, new DateTime(2020, 02, 12)}, // Squirtle
        {9003, new DateTime(2020, 02, 12)}, // Pikachu
        {9004, new DateTime(2020, 02, 15)}, // Original Color Magearna
        {9005, new DateTime(2020, 02, 12)}, // Eevee
        {9006, new DateTime(2020, 02, 12)}, // Rotom
        {9007, new DateTime(2020, 02, 12)}, // Pichu
        {9008, new DateTime(2020, 06, 02)}, // Hidden Ability Grookey
        {9009, new DateTime(2020, 06, 02)}, // Hidden Ability Scorbunny
        {9010, new DateTime(2020, 06, 02)}, // Hidden Ability Sobble
        {9011, new DateTime(2020, 06, 30)}, // Shiny Zeraora
        {9012, new DateTime(2020, 11, 10)}, // Gigantamax Melmetal
        {9013, new DateTime(2021, 06, 17)}, // Gigantamax Bulbasaur
        {9014, new DateTime(2021, 06, 17)}, // Gigantamax Squirtle
    };

    /// <summary>
    /// Minimum date the gift can be received.
    /// </summary>
    public static readonly Dictionary<int, (DateTime Start, DateTime End)> WA8Gifts = new()
    {
        {0138, (new(2022, 01, 27), new(2022, 11, 01))}, // Poké Center Happiny
        {0301, (new(2022, 02, 05), new(2022, 02, 24))}, // プロポチャ Piplup
        {0801, (new(2022, 02, 25), new(2022, 06, 01))}, // Teresa Roca Hisuian Growlithe
    };
}
