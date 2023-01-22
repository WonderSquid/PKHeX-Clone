using System;

namespace PKHeX.Core;

internal static class Locations8a
{
    internal static ReadOnlySpan<byte> Met0 => new byte[]
    {
        000,      002,      004,      006, 007, 008, 009,
        010, 011, 012, 013, 014, 015, 016, 017, 018, 019,
        020, 021, 022, 023, 024, 025, 026, 027, 028, 029,
        030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
        040, 041, 042, 043,      045, 046, 047, 048, 049,
        050, 051, 052, 053, 054, 055, 056, 057, 058, 059,
        060, 061,      063, 064, 065, 066, 067, 068, 069,
        070, 071, 072, 073, 074, 075, 076, 077,      079,
        080, 081, 082, 083, 084, 085, 086, 087, 088, 089,
        090,      092, 093, 094, 095, 096, 097, 098, 099,
        100, 101, 102, 103, 104, 105, 106, 107, 108, 109,
        110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
        120, 121, 122, 123, 124, 125, 126, 127, 128, 129,
        130, 131, 132, 133, 134, 135, 136, 137, 138, 139,
        140, 141, 142, 143, 144, 145, 146, 147, 148, 149,
        150, 151, 152, 153, 154, 155,
    };

    internal static readonly ushort[] Met3 =
    {
               30001, 30002, 30003, 30004, 30005, 30006, 30007, 30008, 30009,
        30010, 30011, 30012, 30013, 30014, 30015, 30016, 30017, 30018, 30019,
        30020, 30021, 30022,
    };

    internal static readonly ushort[] Met4 =
    {
               40001, 40002, 40003,        40005, 40006, 40007, 40008, 40009,
        40010, 40011, 40012, 40013, 40014,        40016, 40017, 40018, 40019,
        40020, 40021, 40022,        40024, 40025, 40026, 40027, 40028, 40029,
        40030,        40032, 40033, 40034, 40035, 40036, 40037, 40038, 40039,
        40040, 40041, 40042, 40043, 40044, 40045,        40047, 40048, 40049,
        40050, 40051, 40052, 40053,        40055, 40056, 40057, 40058, 40059,
        40060, 40061,        40063, 40064, 40065, 40066, 40067, 40068, 40069,
        40070, 40071, 40072,        40074, 40075, 40076, 40077, 40078, 40079,
        40080, 40081, 40082, 40083, 40084, 40085, 40086,
    };

    internal static readonly ushort[] Met6 = {/* XY */ 60001, 60003, /* ORAS */ 60004 };
}
