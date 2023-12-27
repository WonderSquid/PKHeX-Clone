using System;

namespace PKHeX.Core;

internal static class Locations6
{
    public static ReadOnlySpan<ushort> Met0 =>
    [
        /* X/Y */
             002,      006, 008,
        010, 012, 014, 016, 018,
        020, 022, 024, 026, 028,
        030, 032, 034, 036, 038,
        040, 042, 044, 046, 048,
        050, 052, 054, 056, 058,
        060, 062, 064, 066, 068,
        070, 072, 074, 076, 078,
             082, 084, 086, 088,
        090, 092, 094, 096, 098,
        100, 102, 104, 106, 108,
        110, 112, 114, 116, 118,
        120, 122, 124, 126, 128,
        130, 132, 134, 136, 138,
        140, 142, 144, 146, 148,
        150, 152, 154, 156, 158,
        160, 162, 164, 166, 168,

        /* OR/AS */
        170, 172, 174, 176, 178,
        180, 182, 184, 186, 188,
        190, 192, 194, 196, 198,
        200, 202, 204, 206, 208,
        210, 212, 214, 216, 218,
        220, 222, 224, 226, 228,
        230, 232, 234, 236, 238,
        240, 242, 244, 246, 248,
        250, 252, 254, 256, 258,
        260, 262, 264, 266, 268,
        270, 272, 274, 276, 278,
        280, 282, 284, 286, 288,
        290, 292, 294, 296, 298,
        300, 302, 304, 306, 308,
        310, 312, 314, 316, 318,
        320, 322, 324, 326, 328,
        330, 332, 334, 336, 338,
        340, 342, 344, 346, 348,
        350, 352, 354,
    ];

    public static ReadOnlySpan<ushort> Met3 =>
    [
               30001, 30003, 30004, 30005, 30006, 30007, 30008, 30009,
        30010, 30011,
    ];

    public static ReadOnlySpan<ushort> Met4 =>
    [
               40001, 40002, 40003, 40004, 40005, 40006, 40007, 40008, 40009,
        40010, 40011, 40012, 40013, 40014, 40015, 40016, 40017, 40018, 40019,
        40020, 40021, 40022, 40023, 40024, 40025, 40026, 40027, 40028, 40029,
        40030, 40031, 40032, 40033, 40034, 40035, 40036, 40037, 40038, 40039,
        40040, 40041, 40042, 40043, 40044, 40045, 40046, 40047, 40048, 40049,
        40050, 40051, 40052, 40053, 40054, 40055, 40056, 40057, 40058, 40059,
        40060, 40061, 40062, 40063, 40064, 40065, 40066, 40067, 40068, 40069,
        40070, 40071, 40072, 40073, 40074, 40075, 40076, 40077, 40078, 40079,
    ];

    public static ReadOnlySpan<ushort> Met6 => [/* X/Y */ 60001, 60003, /* OR/AS */ 60004];
}
