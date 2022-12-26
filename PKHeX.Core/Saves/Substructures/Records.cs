using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// <see cref="SaveFile"/> lifetime stat tracking
/// </summary>
public static class Records
{
    private const byte LargeRecordCount = 100; // int32
    private const byte SmallRecordCount = 100; // int16
    private const byte Count = LargeRecordCount + SmallRecordCount;

    /// <summary>
    /// Gets the maximum value for the specified record using the provided maximum list.
    /// </summary>
    /// <param name="recordID">Record ID to retrieve the maximum for</param>
    /// <param name="maxes">Maximum enum values for each record</param>
    /// <returns>Maximum the record can be</returns>
    public static int GetMax(int recordID, IReadOnlyList<byte> maxes)
    {
        if ((byte)recordID >= Count)
            return 0;
        return MaxByType[maxes[recordID]];
    }

    public static int GetOffset(int baseOfs, int recordID) => recordID switch
    {
        < LargeRecordCount => baseOfs + (recordID * sizeof(int)),
        < Count => baseOfs + (LargeRecordCount * sizeof(int)) + ((recordID - LargeRecordCount) * sizeof(ushort)),
        _ => -1,
    };

    private static readonly int[] MaxByType = {999_999_999, 9_999_999, 999_999, 99_999, 65535, 9_999, 999, 7};

    public static ReadOnlySpan<byte> DailyPairs_6 => new byte[] {29, 30, 110, 111, 112, 113, 114, 115, 116, 117};
    public static ReadOnlySpan<byte> DailyPairs_7 => new byte[] {22, 23, 110, 111, 112, 113, 114, 115, 116, 117};

    /// <summary>
    /// Festa pairs; if updating the lower index record, update the Festa Mission record if currently active?
    /// </summary>
    public static ReadOnlySpan<byte> FestaPairs_7 => new byte[]
    {
        175, 6,
        176, 33,
        177, 8,
        179, 38,
        181, 74,
        182, 73,
        183, 7,
        184, 159,
        185, 9,
    };

    public static readonly IReadOnlyList<byte> MaxType_XY = new byte[]
    {
        0, 0, 0, 0, 0, 0, 0, 2, 2, 2,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 2, 2, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 3,
        3, 0, 0, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 7, 5, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 6, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
    };

    public static readonly IReadOnlyList<byte> MaxType_AO = new byte[]
    {
        0, 0, 0, 0, 0, 0, 0, 2, 2, 2,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 2, 2, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 3,
        3, 0, 0, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 2, 2, 2, 2, 2, 2, 2, 2,

        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 7, 5, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 6, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        7, 7, 7, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
    };

    public static readonly IReadOnlyList<byte> MaxType_SM = new byte[]
    {
        0, 0, 0, 0, 0, 0, 2, 2, 2, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 2, 2, 2, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 2, 2, 0, 0, 0, 2, 2, 0,
        0, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 1, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 6, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
    };

    public static readonly IReadOnlyList<byte> MaxType_USUM = new byte[]
    {
        0, 0, 0, 0, 0, 0, 2, 2, 2, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 2, 2, 2, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 2, 2, 0, 0, 0, 2, 2, 0,
        0, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 1, 2, 2, 2,
        0, 0, 0, 0, 0, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 6, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 4, 4, 4, 5, 5, 4, 5, 5,
    };

    public static readonly IReadOnlyList<byte> MaxType_SWSH = new byte[]
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    };

    public static readonly Dictionary<int, string> RecordList_6 = new()
    {
        {000, "Steps Taken"},
        {001, "Times Saved"},
        {002, "Storyline Completed Time"},
        {003, "Times Bicycled"},
        {004, "Total Battles"},
        {005, "Wild Pokémon Battles"},
        {006, "Trainer Battles"},
        {007, "Pokemon Caught"},
        {008, "Pokemon Caught Fishing"},
        {009, "Eggs Hatched"},
        {010, "Pokémon Evolved"},
        {011, "Pokémon Healed at Pokémon Centers"},
        {012, "Link Trades"},
        {013, "Link Battles"},
        {014, "Link Battle Wins"},
        {015, "Link Battle Losses"},
        {016, "WiFi Trades"},
        {017, "WiFi Battles"},
        {018, "WiFi Battle Wins"},
        {019, "WiFi Battle Losses"},
        {020, "IR Trades"},
        {021, "IR Battles"},
        {022, "IR Battle Wins"},
        {023, "IR Battle Losses"},
        {024, "Mart Stack Purchases"},
        {025, "Money Spent"},
        {026, "Times watched TV"},
        {027, "Pokémon deposited at Nursery"},
        {028, "Pokémon Defeated"},
        {029, "Exp. Points Collected (Highest)"},
        {030, "Exp. Points Collected (Today)"},
        {031, "Deposited in the GTS"},
        {032, "Nicknames Given"},
        {033, "Bonus Premier Balls Received"},
        {034, "Battle Points Earned"},
        {035, "Battle Points Spent"},

        {037, "Tips at Restaurant: ★☆☆"},
        {038, "Tips at Restaurant: ★★☆"},
        {039, "Tips at Restaurant: ★★★"},
        {040, "Tips at Restaurant: Sushi High Roller"},
        {041, "Tips at Café 1"},
        {042, "Tips at Café 2"},
        {043, "Tips at Café 3"},
        {044, "Tips at Cameraman"},
        {045, "Tips at Drink Vendors"},
        {046, "Tips at Poet"},
        {047, "Tips at Furfrou Trimmer"},
        {048, "Tips at Battle Maison 1"},
        {049, "Tips at Battle Maison 2"},
        {050, "Tips at Battle Maison 3"},
        {051, "Tips at Battle Maison 4"},
        {052, "Tips at Maid"},
        {053, "Tips at Butler"},
        {054, "Tips at Scary House"},
        {055, "Tips at Traveling Minstrel"},
        {056, "Tips at Special BGM 1"},
        {057, "Tips at Special BGM 2"},
        {058, "Tips at Frieser Furfrou"},
        {059, "Nice! Received"},
        {060, "Birthday Wishes"},
        {061, "Total People Met Online"},
        {062, "Total People Passed By"},
        {063, "Current Pokemiles"},
        {064, "Total Pokemiles Received"},
        {065, "Total Pokemiles sent to PGL"},
        {066, "Total Super Training Attempts"},
        {067, "Total Super Training Cleared"},
        {068, "IV Judge Evaluations"},
        {069, "Trash Cans inspected"},

        {070, "Inverse Battles"},
        {071, "Maison Battles"},
        {072, "Times changed character clothing"},
        {073, "Times changed character hairstyle"},
        {074, "Berries harvested"},
        {075, "Berry Field mutations"},
        {076, "PR Videos"},
        {077, "Friend Safari Encounters"},
        {078, "O-Powers Used"},

        {079, "Secret Base Updates"},
        {080, "Secret Base Flags Captured"},
        {081, "Contests Participated Count"},
        {082, "GTS Trades"},
        {083, "Wonder Trades"},
        {084, "Steps Sneaked"},
        {085, "Multiplayer Contests"},
        {086, "Pokeblocks used"},
        {087, "Times AreaNav Used"},
        {088, "Times DexNav Used"},
        {089, "Times BuzzNav Used"},
        {090, "Times PlayNav Used"},

        {100, "Champion Title Defense"},
        {101, "Times rested at home"},
        {102, "Times Splash used"},
        {103, "Times Struggle used"},
        {104, "Moves used with No Effect"},
        {105, "Own Fainted Pokémon"},
        {106, "Times attacked ally in battle"},
        {107, "Failed Run Attempts"},
        {108, "Wild encounters that fled"},
        {109, "Failed Fishing Attempts"},
        {110, "Pokemon Defeated (Highest)"},
        {111, "Pokemon Defeated (Today)"},
        {112, "Pokemon Caught (Highest)"},
        {113, "Pokemon Caught (Today)"},
        {114, "Trainers Battled (Highest)"},
        {115, "Trainers Battled (Today)"},
        {116, "Pokemon Evolved (Highest)"},
        {117, "Pokemon Evolved (Today)"},
        {118, "Fossils Restored"},
        {119, "Sweet Scent Encounters"},
        {120, "Battle Institute Tests"},
        {121, "Battle Institute Rank"},
        {122, "Battle Institute Score"},

        {123, "Last Tip at Restaurant: ★☆☆"},
        {124, "Last Tip at Restaurant: ★★☆"},
        {125, "Last Tip at Restaurant: ★★★"},
        {126, "Last Tip at Restaurant: Sushi High Roller"},
        {127, "Last Tip at Café 1"},
        {128, "Last Tip at Café 2"},
        {129, "Last Tip at Café 3"},
        {130, "Last Tip at Cameraman"},
        {131, "Last Tip at Drink Vendors"},
        {132, "Last Tip at Poet"},
        {133, "Last Tip at Furfrou Trimmer"},
        {134, "Last Tip at Battle Maison 1"},
        {135, "Last Tip at Battle Maison 2"},
        {136, "Last Tip at Battle Maison 3"},
        {137, "Last Tip at Battle Maison 4"},
        {138, "Last Tip at Maid"},
        {139, "Last Tip at Butler"},
        {140, "Last Tip at Scary House"},
        {141, "Last Tip at Traveling Minstrel"},
        {142, "Last Tip at Special BGM 1"},
        {143, "Last Tip at Special BGM 2"},
        {144, "Last Tip at Frieser Furfrou"},

        {145, "Photos Taken"},
        {146, "Sky Wild Battles (?)"},
        {147, "Battle Maison Streak: Singles"},
        {148, "Battle Maison Streak: Doubles"},
        {149, "Battle Maison Streak: Triples"},
        {150, "Battle Maison Streak: Rotation"},
        {151, "Battle Maison Streak: Multi"},
        {152, "Loto-ID Wins"},
        {153, "PP Ups used"},
        {154, "PSS Passerby Count (Today)"},
        {155, "Amie Used"},

        {156, "Roller Skate Count: Spin Left"},
        {157, "Roller Skate Count: Spin Right"},
        {158, "Roller Skate Count: Running Start"},
        {159, "Roller Skate Count: Parallel Swizzle"},
        {160, "Roller Skate Count: Drift-and-dash"},
        {161, "Roller Skate Count: 360 right"},
        {162, "Roller Skate Count: 360 left"},
        {163, "Roller Skate Count: Flips"},
        {164, "Roller Skate Count: Grind"},
        {165, "Roller Skate Count: Combos"},

        {166, "Fishing Chains"},
        {167, "Secret Base Battles in your base"},
        {168, "Secret Base Battles in another base"},
        {169, "Contest Spectacular Photos taken"},
        {170, "Times used Fly"},
        {171, "Times used Soaring in the Sky"},
        {172, "Times used Dive"},
        {173, "Times used Sky Holes"},
        {174, "Times healed by Mom"},
        {175, "Times used Escape Rope"},
        {176, "Times used Dowsing Machine"},
        {177, "Trainer's Eye Rematches"},
        {178, "FUREAI Interest ???"}, // similar to USUM idb

        {179, "Shiny Pokemon Encountered"},
        {180, "Trick House Clears"},
        {181, "Eon Ticket 1 (SpotPass)"},
        {182, "Eon Ticket 2 (Mystery Gift)"},
    };

    public static readonly Dictionary<int, string> RecordList_7 = new()
    {
        {000, "Steps Taken"},
        {001, "Times Saved"},
        {002, "Storyline Completed Time"},
        {003, "Total Battles"},
        {004, "Wild Pokémon Battles"},
        {005, "Trainer Battles"},
        {006, "Pokemon Caught"},
        {007, "Pokemon Caught Fishing"},
        {008, "Eggs Hatched"},
        {009, "Pokémon Evolved"},
        {010, "Pokémon Healed at Pokémon Centers"},
        {011, "Link Trades"},
        {012, "Link Battles"},
        {013, "Link Battle Wins"},
        {014, "Link Battle Losses"},
        {015, "Battle Spot Battles"},
        {016, "Battle Spot Wins"},
        {017, "Battle Spot Losses"},
        {018, "Mart Stack Purchases"},
        {019, "Money Spent"},
        {020, "Pokémon deposited at Nursery"},
        {021, "Pokémon Defeated"},
        {022, "Exp. Points Collected (Highest)"},
        {023, "Exp. Points Collected (Today)"},
        {024, "Deposited in the GTS"},
        {025, "Nicknames Given"},
        {026, "Bonus Premier Balls Received"},
        {027, "Battle Points Earned"},
        {028, "Battle Points Spent"},
        {029, "Super Effective Moves Used"},
        {030, "Clothing Count"},
        {031, "Salon Uses"},
        {032, "Berry Harvests"},
        {033, "Trades at the GTS"},
        {034, "Wonder Trades"},
        {035, "Quick Links"},
        {036, "Pokemon Rides"},
        {037, "Beans Given"},
        {038, "Festival Coins Spent"},
        {039, "Poke Beans Collected"},
        {040, "Battle Tree Challenges"},
        {041, "Z-Moves Used"},
        {042, "Balls Used"},
        {043, "Items Thieved"},
        {044, "Moves Used"},
        {045, "Levels Raised"},
        {046, "Ran From Battles"},
        {047, "Rock Smash Items"},
        {048, "Medicine Used"},
        {049, "Pay Day Money Received"},
        {050, "Total Thumbs-Ups"},
        {051, "Times Twirled (Pirouette)"},
        {052, "Record Thumbs-ups"},
        {053, "Pokemon Petted"},
        {054, "Poké Pelago Visits"},
        {055, "Poké Pelago Bean Trades"},
        {056, "Poké Pelago Tapped Pokémon"},
        {057, "Poké Pelago Bean Stacks put in Crate"},
        {058, "Poké Pelago Levels Gained"},
        {059, "Poké Pelago Friendship Increased"},
        {060, "Poké Pelago Eggs Hatched"},
        {061, "Poké Pelago ???"},
        {062, "Battle Video QR Teams Scanned"},
        {063, "Battle Videos Watched"},
        {064, "Battle Videos Rebattled"},
        {065, "RotomDex Interactions"},
        {066, "Guests Interacted With"},
        {067, "Berry Piles (not full) Collected"},
        {068, "Berry Piles (full) Collected"},
        {069, "Items Reeled In"},
        // USUM
        {070, "Roto Lotos"},

        {072, "Stickers Collected"},
        {073, "Mantine Surf BP Earned"},
        {074, "Battle Agency Wins"},

        {100, "Champion Title Defense"},
        {101, "Times rested at home"},
        {102, "Times Splash used"},
        {103, "Times Struggle used"},
        {104, "Moves used with No Effect"},
        {105, "Own Fainted Pokémon"},
        {106, "Times attacked ally in battle"},
        {107, "Failed Run Attempts"},
        {108, "Wild encounters that fled"},
        {109, "Failed Fishing Attempts"},
        {110, "Pokemon Defeated (Highest)"},
        {111, "Pokemon Defeated (Today)"},
        {112, "Pokemon Caught (Highest)"},
        {113, "Pokemon Caught (Today)"},
        {114, "Trainers Battled (Highest)"},
        {115, "Trainers Battled (Today)"},
        {116, "Pokemon Evolved (Highest)"},
        {117, "Pokemon Evolved (Today)"},
        {118, "Fossils Restored"},
        {119, "Photos Rated"},
        {120, "Best (Super) Singles Streak"},
        {121, "Best (Super) Doubles Streak"},
        {122, "Best (Super) Multi Streak"},
        {123, "Loto-ID Wins"},
        {124, "PP Raised"},
        {125, "Amie Used"},
        {126, "Fishing Chains"},
        {127, "Shiny Pokemon Encountered"},
        {128, "Missions Participated In"},
        {129, "Facilities Hosted"},
        {130, "QR Code Scans"},
        {131, "Moves learned with TMs"},
        {132, "Café Drinks Bought"},
        {133, "Trainer Card Photos Taken"},
        {134, "Evolutions Cancelled"},
        {135, "SOS Battle Allies Called"},
        {136, "Friendship Raised"},
        {137, "Battle Royal Dome Battles"},
        {138, "Items Picked Up after Battle"},
        {139, "Ate in Malasadas Shop"},
        {140, "Hyper Trainings Received"},
        {141, "Dishes eaten in Battle Buffet"},
        {142, "Pokémon Refresh Accessed"},
        {143, "Pokémon Storage System Log-outs"},
        {144, "Lomi Lomi Massages"},
        {145, "Times laid down in Ilima's Bed"},
        {146, "Times laid down in Guzma's Bed"},
        {147, "Times laid down in Kiawe's Bed"},
        {148, "Times laid down in Lana's Bed"},
        {149, "Times laid down in Mallow's Bed"},
        {150, "Times laid down in Olivia's Bed"},
        {151, "Times laid down in Hapu's Bed"},
        {152, "Times laid down in Lusamine's Bed"},
        {153, "Ambush/Smash post-battle items received"},
        {154, "Rustling Tree Encounters"},
        {155, "Ledges Jumped Down"},
        {156, "Water Splash Encounters"},
        {157, "Sand Cloud Encounters"},
        {158, "Outfit Changes"},
        {159, "Battle Royal Dome Wins"},
        {160, "Pelago Treasure Hunts"},
        {161, "Pelago Training Sessions"},
        {162, "Pelago Hot Spring Sessions"},
        {163, "Special QR 1"},
        {164, "Special QR 2"},
        {165, "Special QR Code Scans"},
        {166, "Island Scans"},
        {167, "Rustling Bush Encounters"},
        {168, "Fly Shadow Encounters"},
        {169, "Rustling Grass Encounters"},
        {170, "Dirt Cloud Encounters"},
        {171, "Wimpod Chases"},
        {172, "Berry Tree Battles won"},
        {173, "Bubbling Spot Encounters/Items"},
        {174, "Times laid down in Own Bed"},
        // global missions
        {175, "Catch a lot of Pokémon!"},
        {176, "Trade Pokémon at the GTS!"},
        {177, "Hatch a lot of Eggs!"},
        {178, "Harvest Poké Beans!"},
        {179, "Get high scores with your Poké Finder!"},
        {180, "Find Pokémon using Island Scan!"},
        {181, "Catch Crabrawler!"},
        {182, "Defend your Champion title!"},
        {183, "Fish Pokémon at rare spots!"},
        {184, "Battle Royal!"},
        {185, "Try your luck!"},
        {186, "Get BP at the Battle Tree!"},
        {187, "Catch a lot of Pokémon!"},

        // USUM
        {188, "Ultra Wormhole Travels"},
        {189, "Mantine Surf Plays"},
        {190, "Photo Club Photos saved"},
        {191, "Battle Agency Battles"},
        // 192-194 unknown
        {195, "Photo Club Sticker usage"},
        {196, "Photo Club Photo Shoots"},
        {197, "Highest Wormhole Travel Distance"},
        {198, "Highest Mantine Surf BP Earned"},
    };

    public static readonly Dictionary<int, string> RecordList_8 = new()
    {
        {00, "egg_hatching"},
        {01, "capture_wild"},
        {02, "capture_symbol"},
        {03, "capture_raid"},
        {04, "capture_camp"},
        {05, "capture_fishing"},
        {06, "total_capture"},
        {07, "dress_up"},
        {08, "training"},
        {09, "personal_change"},
        {10, "rotomu_circuit"},
        {11, "npc_trade"},
        {12, "pretty"},
        {13, "chain_encount"},
        {14, "hall_of_fame"},
        {15, "fossil_restore"},
        {16, "wild_pokemon_encount"},
        {17, "trade"},
        {18, "magical_trade"},
        {19, "one_day_captured"},
        {20, "one_day_evolution"},
        {21, "total_walk"},
        {22, "total_watt"},
        {23, "total_all_battle"},
        {24, "campin"},
        {25, "battle_point"},
        {26, "win_battle_point"},
        {27, "license_trade"},
        {28, "use_skill_record"},
        {29, "use_exp_ball"},
        {30, "use_personal_change_item"},
        {31, "clothes"},
        {32, "evolution"},
        {33, "net_battle"},
        {34, "cooking"},
        {35, "poke_job_return"},
        {36, "get_rare_item"},
        {37, "whistle"},
        {38, "bike_dash"},
        {39, "tree_shake"},
        {40, "tree_nut"},
        {41, "battle_lose"},
        {42, "recipe"},
        {43, "raid_battle"},
        {44, "total_money"},
        {45, "create_license_card"},
        {46, "change_hair"},
        /* 47 */ {G8BattleTowerSingleWin, "battle_tower_single_win"},
        /* 48 */ {G8BattleTowerDoubleWin, "battle_tower_double_win"},
        {49, "now_money"},

        // The Records Block only stores 50 entries.
        // Record IDs for future expansion content is instead handled separately.

        // DLC
        {50, "cormorant_robo"}, // saved in B9C0ECFC 
        {51, "battle_rom_mark"}, // saved in BB1DE8EF
    };

    public const int G8BattleTowerSingleWin = 47;
    public const int G8BattleTowerDoubleWin = 48;

    public static readonly IReadOnlyList<int> MaxValue_BDSP = new[]
    {
        int.MaxValue, // CLEAR_TIME
        9_999, // DENDOU_CNT
        999_999, // CAPTURE_POKE
        999_999, // FISHING_SUCCESS
        999_999, // TAMAGO_HATCHING
        999_999, // BEAT_DOWN_POKE
        9_999, // RENSHOU_SINGLE
        9_999, // RENSHOU_SINGLE_NOW
        9_999, // RENSHOU_DOUBLE
        9_999, // RENSHOU_DOUBLE_NOW
        9_999, // RENSHOU_MASTER_SINGLE
        9_999, // RENSHOU_MASTER_SINGLE_NOW
        9_999, // RENSHOU_MASTER_DOUBLE
        9_999, // RENSHOU_MASTER_DOUBLE_NOW
        7, // BTL_TOWER_AVERAGE
        5, // CONTEST_STYLE_RANK
        5, // CONTEST_BEATIFUL_RANK
        5, // CONTEST_CUTE_RANK
        5, // CONTEST_CLEVER_RANK
        5, // CONTEST_STRONG_RANK
        9_999, // CONTEST_PLAY_SINGLE
        9_999, // CONTEST_PLAY_LOCAL
        9_999, // CONTEST_PLAY_NETWORK
        9_999, // CONTEST_WIN_SINGLE
        9_999, // CONTEST_WIN_LOCAL
        9_999, // CONTEST_WIN_NETWORK
        100,  // CONTEST_RATE_SINGLE
        100,  // CONTEST_RATE_LOCAL
        100,  // CONTEST_RATE_NETWORK
        65_536,// CONTEST_GET_RIBBON
    };

    public static readonly Dictionary<int, string> RecordList_8b = new()
    {
        { 00, "CLEAR_TIME" },
        { 01, "DENDOU_CNT" },
        { 02, "CAPTURE_POKE" },
        { 03, "FISHING_SUCCESS" },
        { 04, "TAMAGO_HATCHING" },
        { 05, "BEAT_DOWN_POKE" },
        { 06, "RENSHOU_SINGLE" },
        { 07, "RENSHOU_SINGLE_NOW" },
        { 08, "RENSHOU_DOUBLE" },
        { 09, "RENSHOU_DOUBLE_NOW" },
        { 10, "RENSHOU_MASTER_SINGLE" },
        { 11, "RENSHOU_MASTER_SINGLE_NOW" },
        { 12, "RENSHOU_MASTER_DOUBLE" },
        { 13, "RENSHOU_MASTER_DOUBLE_NOW" },
        { 14, "BTL_TOWER_AVERAGE" },
        { 15, "CONTEST_STYLE_RANK" },
        { 16, "CONTEST_BEATIFUL_RANK" },
        { 17, "CONTEST_CUTE_RANK" },
        { 18, "CONTEST_CLEVER_RANK" },
        { 19, "CONTEST_STRONG_RANK" },
        { 20, "CONTEST_PLAY_SINGLE" },
        { 21, "CONTEST_PLAY_LOCAL" },
        { 22, "CONTEST_PLAY_NETWORK" },
        { 23, "CONTEST_WIN_SINGLE" },
        { 24, "CONTEST_WIN_LOCAL" },
        { 25, "CONTEST_WIN_NETWORK" },
        { 26, "CONTEST_RATE_SINGLE" },
        { 27, "CONTEST_RATE_LOCAL" },
        { 28, "CONTEST_RATE_NETWORK" },
        { 29, "CONTEST_GET_RIBBON" },
    };
}
