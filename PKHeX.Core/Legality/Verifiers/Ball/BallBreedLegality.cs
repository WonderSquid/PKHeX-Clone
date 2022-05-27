﻿using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Tables used for <see cref="BallVerifier"/>
    /// </summary>
    internal static class BallBreedLegality
    {
        /// <summary>
        /// Species that are male only in Generation 6; for ball inheritance, these behave the same as Genderless species (no ball inherited).
        /// </summary>
        internal static readonly HashSet<int> BreedMaleOnly6 = new()
        {
            (int)Tauros,
            (int)Rufflet,
            (int)Braviary,
            (int)Tyrogue,
            (int)Sawk,
            (int)Throh,
        };

        internal static readonly HashSet<int> PastGenAlolanNativesUncapturable = new()
        {
            // Porygon++
            (int)Porygon,
            // Gen1 Fossils
            (int)Aerodactyl, (int)Omanyte, (int)Kabuto,
            // Gen3 Fossils
            (int)Lileep, (int)Anorith,
            // Gen4 Fossils
            (int)Cranidos, (int)Shieldon,
            // Gen5 Fossils
            (int)Tirtouga, (int)Archen,
            // Gen6 Fossils
            (int)Tyrunt, (int)Amaura,
        };

        internal static readonly HashSet<int> PastGenAlolanScans = new()
        {
            (int)Bellsprout,
            (int)Rhyhorn,
            (int)Horsea,
            (int)Chikorita,
            (int)Cyndaquil,
            (int)Totodile,
            (int)Swinub,
            (int)Spheal,
            (int)Venipede,
            (int)Gothita,
            (int)Klink,
            (int)Litwick,
            (int)Axew,
            (int)Deino,
            (int)Honedge,
            (int)Marill, (int)Azurill,
            (int)Roselia, (int)Budew,
            (int)Togepi,
            (int)Slakoth,
            (int)Starly,
            (int)Shinx,
            (int)Snivy,
            (int)Solosis,
            (int)Tepig,
            (int)Oshawott,
            (int)Timburr,
            (int)Sewaddle,
            (int)Tynamo,
            (int)Charmander,
            (int)Squirtle,
            (int)Onix,
            (int)Talonflame,
            (int)Scatterbug,
            (int)Bulbasaur,
            (int)Ralts,
            (int)Weedle,
            (int)Torchic,
            (int)Treecko,
            (int)Mudkip,
            (int)Piplup,
            (int)Turtwig,
            (int)Pidgey,
            (int)Chimchar,
            (int)Aron,
            (int)Rotom,
            (int)Chespin,
            (int)Fennekin,
            (int)Froakie,
        };

        internal static readonly HashSet<int> Inherit_Sport = new()
        {
            (int)Caterpie,
            (int)Weedle,
            (int)Paras,
            (int)Venonat,
            (int)Scyther,
            (int)Pinsir,
            (int)Wurmple,
            (int)Nincada,
            (int)Illumise,
            (int)Kricketot,
            (int)Combee,
            (int)Volbeat,
        };

        internal static readonly HashSet<int> Inherit_Safari = new()
        {
            (int)Pidgey,
            (int)Rattata,
            (int)Spearow,
            (int)Ekans,
            (int)Sandshrew,
            (int)NidoranF,
            (int)Zubat,
            (int)Oddish,
            (int)Paras,
            (int)Venonat,
            (int)Diglett,
            (int)Psyduck,
            (int)Poliwag,
            (int)Abra,
            (int)Machop,
            (int)Bellsprout,
            (int)Geodude,
            (int)Ponyta,
            (int)Slowpoke,
            (int)Farfetchd,
            (int)Doduo,
            (int)Grimer,
            (int)Gastly,
            (int)Onix,
            (int)Drowzee,
            (int)Krabby,
            (int)Exeggcute,
            (int)Cubone,
            (int)Lickitung,
            (int)Koffing,
            (int)Rhyhorn,
            (int)Chansey,
            (int)Tangela,
            (int)Kangaskhan,
            (int)Goldeen,
            (int)MrMime,
            (int)Scyther,
            (int)Pinsir,
            (int)Magikarp,
            (int)Lapras,
            (int)Dratini,
            (int)Sentret,
            (int)Hoothoot,
            (int)Ledyba,
            (int)Spinarak,
            (int)Natu,
            (int)Mareep,
            (int)Marill,
            (int)Hoppip,
            (int)Jumpluff,
            (int)Aipom,
            (int)Sunkern,
            (int)Yanma,
            (int)Wooper,
            (int)Murkrow,
            (int)Misdreavus,
            (int)Wobbuffet,
            (int)Girafarig,
            (int)Pineco,
            (int)Gligar,
            (int)Snubbull,
            (int)Shuckle,
            (int)Heracross,
            (int)Teddiursa,
            (int)Remoraid,
            (int)Houndour,
            (int)Phanpy,
            (int)Stantler,
            (int)Smeargle,
            (int)Miltank,
            (int)Larvitar,
            (int)Zigzagoon,
            (int)Linoone,
            (int)Lotad,
            (int)Seedot,
            (int)Surskit,
            (int)Shroomish,
            (int)Slakoth,
            (int)Nosepass,
            (int)Aron,
            (int)Lairon,
            (int)Meditite,
            (int)Electrike,
            (int)Illumise,
            (int)Roselia,
            (int)Gulpin,
            (int)Carvanha,
            (int)Torkoal,
            (int)Spinda,
            (int)Trapinch,
            (int)Cacnea,
            (int)Zangoose,
            (int)Seviper,
            (int)Barboach,
            (int)Corphish,
            (int)Kecleon,
            (int)Shuppet,
            (int)Duskull,
            (int)Tropius,
            (int)Chimecho,
            (int)Spheal,
            (int)Bagon,
            (int)Starly,
            (int)Bidoof,
            (int)Shinx,
            (int)Budew,
            (int)Pachirisu,
            (int)Buizel,
            (int)Chingling,
            (int)Gible,
            (int)Riolu,
            (int)Hippopotas,
            (int)Skorupi,
            (int)Croagunk,
            (int)Carnivine,

            // Splitbreed/Baby
            (int)NidoranM, // Via Nidoran-F
            (int)Volbeat, // Via Illumise
            (int)Pichu,
            (int)Cleffa,
            (int)Igglybuff,
            (int)Elekid,
            (int)Magby,
            (int)Azurill,
            (int)Wynaut,
            (int)Budew,
            (int)Chingling,
            (int)MimeJr,
            (int)Happiny,
        };

        internal static readonly HashSet<int> Inherit_Dream = new()
        {
            (int)Caterpie,
            (int)Weedle,
            (int)Pidgey,
            (int)Rattata,
            (int)Spearow,
            (int)Ekans,
            (int)Sandshrew,
            (int)NidoranF,
            (int)Vulpix,
            (int)Zubat,
            (int)Oddish,
            (int)Paras,
            (int)Venonat,
            (int)Diglett,
            (int)Meowth,
            (int)Psyduck,
            (int)Mankey,
            (int)Growlithe,
            (int)Poliwag,
            (int)Abra,
            (int)Machop,
            (int)Bellsprout,
            (int)Tentacool,
            (int)Geodude,
            (int)Ponyta,
            (int)Slowpoke,
            (int)Farfetchd,
            (int)Doduo,
            (int)Seel,
            (int)Grimer,
            (int)Shellder,
            (int)Gastly,
            (int)Onix,
            (int)Drowzee,
            (int)Krabby,
            (int)Exeggcute,
            (int)Cubone,
            (int)Lickitung,
            (int)Koffing,
            (int)Rhyhorn,
            (int)Chansey,
            (int)Tangela,
            (int)Kangaskhan,
            (int)Horsea,
            (int)Goldeen,
            (int)MrMime,
            (int)Scyther,
            (int)Pinsir,
            (int)Magikarp,
            (int)Lapras,
            (int)Eevee,
            (int)Omanyte,
            (int)Kabuto,
            (int)Aerodactyl,
            (int)Snorlax,
            (int)Dratini,
            (int)Sentret,
            (int)Hoothoot,
            (int)Ledyba,
            (int)Spinarak,
            (int)Chinchou,
            (int)Cleffa,
            (int)Igglybuff,
            (int)Togepi,
            (int)Natu,
            (int)Mareep,
            (int)Marill,
            (int)Sudowoodo,
            (int)Hoppip,
            (int)Aipom,
            (int)Sunkern,
            (int)Yanma,
            (int)Wooper,
            (int)Murkrow,
            (int)Misdreavus,
            (int)Wobbuffet,
            (int)Girafarig,
            (int)Pineco,
            (int)Dunsparce,
            (int)Gligar,
            (int)Snubbull,
            (int)Qwilfish,
            (int)Shuckle,
            (int)Heracross,
            (int)Sneasel,
            (int)Teddiursa,
            (int)Slugma,
            (int)Swinub,
            (int)Corsola,
            (int)Remoraid,
            (int)Delibird,
            (int)Mantine,
            (int)Skarmory,
            (int)Houndour,
            (int)Phanpy,
            (int)Stantler,
            (int)Smeargle,
            (int)Smoochum,
            (int)Elekid,
            (int)Magby,
            (int)Miltank,
            (int)Larvitar,
            (int)Poochyena,
            (int)Zigzagoon,
            (int)Wurmple,
            (int)Lotad,
            (int)Seedot,
            (int)Taillow,
            (int)Wingull,
            (int)Ralts,
            (int)Surskit,
            (int)Shroomish,
            (int)Slakoth,
            (int)Nincada,
            (int)Whismur,
            (int)Makuhita,
            (int)Nosepass,
            (int)Skitty,
            (int)Sableye,
            (int)Mawile,
            (int)Aron,
            (int)Meditite,
            (int)Electrike,
            (int)Plusle,
            (int)Minun,
            (int)Illumise,
            (int)Roselia,
            (int)Gulpin,
            (int)Carvanha,
            (int)Wailmer,
            (int)Numel,
            (int)Torkoal,
            (int)Spoink,
            (int)Spinda,
            (int)Trapinch,
            (int)Cacnea,
            (int)Swablu,
            (int)Zangoose,
            (int)Seviper,
            (int)Barboach,
            (int)Corphish,
            (int)Lileep,
            (int)Anorith,
            (int)Feebas,
            (int)Castform,
            (int)Kecleon,
            (int)Shuppet,
            (int)Duskull,
            (int)Tropius,
            (int)Chimecho,
            (int)Absol,
            (int)Snorunt,
            (int)Spheal,
            (int)Clamperl,
            (int)Relicanth,
            (int)Luvdisc,
            (int)Bagon,
            (int)Starly,
            (int)Bidoof,
            (int)Kricketot,
            (int)Shinx,
            (int)Cranidos,
            (int)Shieldon,
            (int)Burmy,
            (int)Combee,
            (int)Pachirisu,
            (int)Buizel,
            (int)Cherubi,
            (int)Shellos,
            (int)Drifloon,
            (int)Buneary,
            (int)Glameow,
            (int)Stunky,
            (int)Chatot,
            (int)Spiritomb,
            (int)Gible,
            (int)Riolu,
            (int)Hippopotas,
            (int)Skorupi,
            (int)Croagunk,
            (int)Carnivine,
            (int)Finneon,
            (int)Snover,
            (int)Munna,
            (int)Pidove,
            (int)Boldore,
            (int)Drilbur,
            (int)Audino,
            (int)Gurdurr,
            (int)Tympole,
            (int)Scolipede,
            (int)Cottonee,
            (int)Petilil,
            (int)Basculin,
            (int)Krookodile,
            (int)Maractus,
            (int)Crustle,
            (int)Scraggy,
            (int)Sigilyph,
            (int)Tirtouga,
            (int)Duosion,
            (int)Ducklett,
            (int)Vanillish,
            (int)Emolga,
            (int)Karrablast,
            (int)Alomomola,
            (int)Galvantula,
            (int)Elgyem,
            (int)Axew,
            (int)Shelmet,
            (int)Stunfisk,
            (int)Druddigon,
            (int)Pawniard,
            (int)Heatmor,
            (int)Durant,
            (int)NidoranM, // Via Nidoran-F
            (int)Volbeat, // Via Illumise

            // Via Evolution
            (int)Roggenrola,
            (int)Timburr,
            (int)Venipede,
            (int)Sandile,
            (int)Dwebble,
            (int)Solosis,
            (int)Vanillite,
            (int)Joltik,

            // Via Incense Breeding
            (int)Azurill,
            (int)Wynaut,
            (int)Budew,
            (int)Chingling,
            (int)Bonsly,
            (int)MimeJr,
            (int)Happiny,
            (int)Munchlax,
            (int)Mantyke,
        };

        internal static readonly HashSet<int> Ban_DreamHidden = new()
        {
            (int)Plusle,
            (int)Minun,
            (int)Kecleon,
            (int)Duskull,
        };

        internal static readonly HashSet<int> Ban_Gen3Ball = new()
        {
            (int)Treecko, (int)Torchic, (int)Mudkip,
            (int)Turtwig, (int)Chimchar, (int)Piplup,
            (int)Snivy, (int)Tepig, (int)Oshawott,

            // Fossil Only obtain
            (int)Archen, (int)Tyrunt, (int)Amaura,
        };

        internal static readonly HashSet<int> Ban_Gen3BallHidden = new()
        {
            // can have HA and can be in gen 3 ball as eggs but can not at same time.
            (int)Chikorita, (int)Cyndaquil, (int)Totodile,
            (int)Deerling + (1 << 11), //Deerling-Summer
            (int)Deerling + (2 << 11), //Deerling-Autumn
            (int)Deerling + (3 << 11), //Deerling-Winter
            (int)Pumpkaboo + (3 << 11), //Pumpkaboo-Super
        };

        internal static readonly HashSet<int> Ban_Gen4Ball_6 = new()
        {
            (int)Chikorita, (int)Cyndaquil, (int)Totodile,
            (int)Treecko, (int)Torchic, (int)Mudkip,
            (int)Turtwig, (int)Chimchar, (int)Piplup,
            (int)Snivy, (int)Tepig, (int)Oshawott,

            // Fossil Only obtain
            (int)Archen, (int)Tyrunt, (int)Amaura,
        };

        internal static readonly HashSet<int> Inherit_Apricorn6 = new()
        {
            (int)Caterpie,
            (int)Weedle,
            (int)Pidgey,
            (int)Rattata,
            (int)Spearow,
            (int)Ekans,
            (int)Sandshrew,
            (int)NidoranF,
            (int)Vulpix,
            (int)Zubat,
            (int)Oddish,
            (int)Paras,
            (int)Venonat,
            (int)Diglett,
            (int)Meowth,
            (int)Psyduck,
            (int)Mankey,
            (int)Growlithe,
            (int)Poliwag,
            (int)Abra,
            (int)Machop,
            (int)Bellsprout,
            (int)Tentacool,
            (int)Geodude,
            (int)Ponyta,
            (int)Slowpoke,
            (int)Farfetchd,
            (int)Doduo,
            (int)Seel,
            (int)Grimer,
            (int)Shellder,
            (int)Gastly,
            (int)Onix,
            (int)Drowzee,
            (int)Krabby,
            (int)Exeggcute,
            (int)Cubone,
            (int)Lickitung,
            (int)Koffing,
            (int)Rhyhorn,
            (int)Chansey,
            (int)Tangela,
            (int)Kangaskhan,
            (int)Horsea,
            (int)Goldeen,
            (int)MrMime,
            (int)Magikarp,
            (int)Lapras,
            (int)Snorlax,
            (int)Dratini,
            (int)Sentret,
            (int)Hoothoot,
            (int)Ledyba,
            (int)Spinarak,
            (int)Chinchou,
            (int)Natu,
            (int)Mareep,
            (int)Marill,
            (int)Sudowoodo,
            (int)Hoppip,
            (int)Aipom,
            (int)Sunkern,
            (int)Yanma,
            (int)Wooper,
            (int)Murkrow,
            (int)Misdreavus,
            (int)Wobbuffet,
            (int)Girafarig,
            (int)Pineco,
            (int)Dunsparce,
            (int)Gligar,
            (int)Snubbull,
            (int)Qwilfish,
            (int)Shuckle,
            (int)Heracross,
            (int)Sneasel,
            (int)Teddiursa,
            (int)Slugma,
            (int)Swinub,
            (int)Corsola,
            (int)Remoraid,
            (int)Delibird,
            (int)Mantine,
            (int)Skarmory,
            (int)Houndour,
            (int)Phanpy,
            (int)Stantler,
            (int)Smeargle,
            (int)Miltank,
            (int)Larvitar,
            (int)Poochyena,
            (int)Zigzagoon,
            (int)Wurmple,
            (int)Seedot,
            (int)Taillow,
            (int)Wingull,
            (int)Ralts,
            (int)Shroomish,
            (int)Slakoth,
            (int)Whismur,
            (int)Makuhita,
            (int)Sableye,
            (int)Mawile,
            (int)Meditite,
            (int)Plusle,
            (int)Minun,
            (int)Gulpin,
            (int)Numel,
            (int)Spoink,
            (int)Spinda,
            (int)Swablu,
            (int)Barboach,
            (int)Absol,
            (int)Clamperl,
            (int)Relicanth,
            (int)Luvdisc,
            (int)Starly,
            (int)Bidoof,
            (int)Kricketot,
            (int)Shinx,
            (int)Budew,
            (int)Burmy,
            (int)Combee,
            (int)Buizel,
            (int)Cherubi,
            (int)Buneary,
            (int)Chingling,
            (int)Chatot,
            (int)Carnivine,
            (int)NidoranM, // Via Nidoran-F
            (int)Happiny, // Via Chansey
            (int)Smoochum, // Via Jynx
            (int)Elekid, // Via Electabuzz
            (int)Magby, // Via Magmar
            (int)Azurill, // Via Marill
            (int)Wynaut, // Via Wobbuffet
            (int)Bonsly, // Via Sudowoodo
            (int)MimeJr, // Via Mr. Mime
            (int)Munchlax, // Via Snorlax
            (int)Mantyke, // Via Mantine
            (int)Chimecho, // Via Chingling
            (int)Pichu, // Via Pikachu
            (int)Cleffa, // Via Clefairy
            (int)Igglybuff, // Via Jigglypuff
        };

        internal static readonly HashSet<int> AlolanCaptureOffspring = new()
        {
            (int)Caterpie,
            (int)Rattata,
            (int)Spearow,
            (int)Sandshrew,
            (int)Vulpix,
            (int)Jigglypuff,
            (int)Zubat,
            (int)Paras,
            (int)Diglett,
            (int)Meowth,
            (int)Psyduck,
            (int)Mankey,
            (int)Growlithe,
            (int)Poliwag,
            (int)Abra,
            (int)Machop,
            (int)Tentacool,
            (int)Geodude,
            (int)Slowpoke,
            (int)Magnemite,
            (int)Grimer,
            (int)Shellder,
            (int)Gastly,
            (int)Drowzee,
            (int)Exeggcute,
            (int)Cubone,
            (int)Chansey,
            (int)Kangaskhan,
            (int)Goldeen,
            (int)Staryu,
            (int)Scyther,
            (int)Pinsir,
            (int)Tauros,
            (int)Magikarp,
            (int)Lapras,
            (int)Ditto,
            (int)Eevee,
            (int)Snorlax,
            (int)Dratini,
            (int)Ledyba,
            (int)Spinarak,
            (int)Chinchou,
            (int)Pichu,
            (int)Cleffa,
            (int)Igglybuff,
            (int)Sudowoodo,
            (int)Murkrow,
            (int)Misdreavus,
            (int)Snubbull,
            (int)Scizor,
            (int)Sneasel,
            (int)Corsola,
            (int)Delibird,
            (int)Skarmory,
            (int)Smeargle,
            (int)Elekid,
            (int)Magby,
            (int)Miltank,
            (int)Wingull,
            (int)Surskit,
            (int)Makuhita,
            (int)Nosepass,
            (int)Sableye,
            (int)Carvanha,
            (int)Wailmer,
            (int)Torkoal,
            (int)Spinda,
            (int)Trapinch,
            (int)Barboach,
            (int)Feebas,
            (int)Castform,
            (int)Absol,
            (int)Snorunt,
            (int)Relicanth,
            (int)Luvdisc,
            (int)Bagon,
            (int)Beldum,
            (int)Shellos,
            (int)Drifloon,
            (int)Bonsly,
            (int)Happiny,
            (int)Gible,
            (int)Munchlax,
            (int)Riolu,
            (int)Finneon,
            (int)Lillipup,
            (int)Roggenrola,
            (int)Cottonee,
            (int)Petilil,
            (int)Sandile,
            (int)Trubbish,
            (int)Vanillite,
            (int)Emolga,
            (int)Alomomola,
            (int)Rufflet,
            (int)Vullaby,
            (int)Fletchling,
            (int)Pancham,
            (int)Carbink,
            (int)Goomy,
            (int)Klefki,
            (int)Phantump,
            (int)Pikipek,
            (int)Yungoos,
            (int)Grubbin,
            (int)Crabrawler,
            (int)Oricorio,
            (int)Cutiefly,
            (int)Rockruff,
            (int)Wishiwashi,
            (int)Mareanie,
            (int)Mudbray,
            (int)Dewpider,
            (int)Fomantis,
            (int)Morelull,
            (int)Salandit,
            (int)Stufful,
            (int)Bounsweet,
            (int)Comfey,
            (int)Oranguru,
            (int)Passimian,
            (int)Wimpod,
            (int)Sandygast,
            (int)Pyukumuku,
            (int)Minior,
            (int)Komala,
            (int)Turtonator,
            (int)Togedemaru,
            (int)Mimikyu,
            (int)Bruxish,
            (int)Drampa,
            (int)Dhelmise,
            (int)Jangmoo,

            // USUM Additions
            (int)Ekans,
            (int)Seel,
            (int)Lickitung,
            (int)MrMime,
            (int)Hoothoot,
            (int)Natu,
            (int)Mareep,
            (int)Aipom,
            (int)Pineco,
            (int)Dunsparce,
            (int)Heracross,
            (int)Remoraid,
            (int)Mantine,
            (int)Houndour,
            (int)Smoochum,
            (int)Larvitar,
            (int)Mawile,
            (int)Electrike,
            (int)Corphish,
            (int)Baltoy,
            (int)Kecleon,
            (int)Shuppet,
            (int)Tropius,
            (int)Clamperl,
            (int)Buneary,
            (int)MimeJr,
            (int)Mantyke,
            (int)Basculin,
            (int)Scraggy,
            (int)Zorua,
            (int)Minccino,
            (int)Frillish,
            (int)Elgyem,
            (int)Mienfoo,
            (int)Druddigon,
            (int)Golett,
            (int)Pawniard,
            (int)Larvesta,
            (int)Litleo,
            (int)Flabébé,
            (int)Furfrou,
            (int)Inkay,
            (int)Skrelp,
            (int)Clauncher,
            (int)Hawlucha,
            (int)Dedenne,
            (int)Noibat,

            // Wormhole
            (int)Swablu,
            (int)Yanma,
            (int)Sigilyph,
            (int)Ducklett,
            (int)Taillow,
            (int)Skorupi,
            (int)Audino,
            (int)Helioptile,
            (int)Seedot,
            (int)Spoink,
            (int)Snover,
            (int)Meditite,
            (int)Hippopotas,
            (int)Dwebble,
            (int)Slugma,
            (int)Binacle,
            (int)Lotad,
            (int)Stunfisk,
            (int)Buizel,
            (int)Wooper,

            // Static Encounters
            (int)Voltorb,
        };

        internal static readonly HashSet<int> Ban_NoHidden7Apricorn = new()
        {
            (int)NidoranF,
            (int)NidoranM,
            (int)Voltorb,
            (int)Bronzor,
            (int)Flabébé + (3 << 11), // Flabébé-Blue
        };

        internal static readonly HashSet<int> AlolanCaptureNoHeavyBall = new() { (int)Beldum, (int)TapuKoko, (int)TapuLele, (int)TapuBulu, (int)TapuFini };

        private static readonly HashSet<int> Inherit_ApricornMale7 = new()
        {
            (int)Voltorb,
            (int)Baltoy,
            (int)Bronzor,

            // Others are capturable in the Alola region
            // Magnemite, Staryu, Tauros
        };

        internal static readonly HashSet<int> Inherit_Apricorn7 = new(Inherit_Apricorn6.Concat(Inherit_ApricornMale7).Concat(PastGenAlolanScans).Concat(AlolanCaptureOffspring).Distinct());

        internal static readonly HashSet<int> Inherit_SafariMale = new()
        {
            (int)Tauros,

            (int)Magnemite,
            (int)Voltorb,
            (int)Lunatone,
            (int)Solrock,
            (int)Beldum,
            (int)Bronzor,
        };

        internal static readonly HashSet<int> Inherit_DreamMale = new()
        {
            // Starting with Gen7, Males pass Ball via breeding with Ditto.
            (int)Bulbasaur, (int)Charmander, (int)Squirtle,
            (int)Tauros,
            (int)Pichu,
            (int)Tyrogue,
            (int)Treecko,
            (int)Torchic, (int)Mudkip, (int)Turtwig,
            (int)Chimchar, (int)Piplup, (int)Pansage,
            (int)Pansear, (int)Panpour,
            (int)Throh, (int)Sawk,
            (int)Gothita,

            (int)Magnemite,
            (int)Voltorb,
            (int)Staryu,
            (int)Porygon,
            (int)Lunatone,
            (int)Solrock,
            (int)Baltoy,
            (int)Beldum,
            (int)Bronzor,
            (int)Rotom,
            (int)Klink,
            (int)Golett,
        };

        internal static readonly HashSet<int> Ban_Gen3Ball_7 = new()
        {
            (int)Phione,
            (int)Archen,
            (int)Tyrunt,
            (int)Amaura,
        };

        // Same as Gen3 Balls
        internal static readonly HashSet<int> Ban_Gen4Ball_7 = Ban_Gen3Ball_7;

        internal static readonly HashSet<int> Ban_SafariBallHidden_7 = new()
        {
            (int)NidoranF,
            (int)NidoranM,
            (int)Volbeat, (int)Illumise,
            (int)Magnemite,
            (int)Voltorb,
            (int)Kangaskhan,
            (int)Tauros,
            (int)Ditto,
            (int)Miltank,
            (int)Beldum,
            (int)Bronzor,
            (int)Happiny,
            (int)Tyrogue,
            (int)Staryu,
            (int)Lunatone,
            (int)Solrock,
            (int)Rotom,
            (int)Klink,
            (int)Golett,
        };

        internal static readonly HashSet<int> Ban_NoHidden8Apricorn = new()
        {
            // Nidoran, Bronzor -- Used to not be encounterable in Gen7 with HA; Gen8 now can via Raids
            (int)Voltorb, // Voltorb
            (int)Flabébé + (3 << 11), // Flabébé-Blue
        };

        /// <summary>
        /// Gets a legal <see cref="Ball"/> value for a bred egg encounter.
        /// </summary>
        /// <param name="version">Version the egg was created on.</param>
        /// <param name="species">Species the egg contained.</param>
        /// <returns>Valid ball to hatch with.</returns>
        /// <remarks>Not all things can hatch with a Poké Ball!</remarks>
#pragma warning disable RCS1163, IDE0060 // Unused parameter.
        public static Ball GetDefaultBall(GameVersion version, int species)
        {
            return Ball.None;
        }
    }
}
