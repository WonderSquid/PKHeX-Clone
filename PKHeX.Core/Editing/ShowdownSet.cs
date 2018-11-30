﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for exporting and importing <see cref="PKM"/> data in Pokémon Showdown's text format.
    /// </summary>
    public class ShowdownSet
    {
        // String to Values
        private static readonly string[] genders = {"M", "F", ""};
        private static readonly string[] genderForms = {"", "F", ""};
        private static readonly string[] StatNames = { "HP", "Atk", "Def", "SpA", "SpD", "Spe" };
        private static readonly string[] Splitters = {"\r\n", "\n"};
        private static readonly string[] StatSplitters = { " / ", " " };
        private static readonly string[] LineSplit = {": "};
        private static readonly string[] ItemSplit = {" @ "};
        private static readonly int[] DashedSpecies = {782, 783, 784, 250, 032, 029}; // Kommo-o, Ho-Oh, Nidoran-M, Nidoran-F
        private static int MAX_SPECIES => PKX.Personal.MaxSpeciesID;
        private const string Language = "en";
        private const int DefaultLanguageID = (int)Core.LanguageID.English;
        private static readonly GameStrings DefaultStrings = GameInfo.GetStrings(Language);

        /// <summary>
        /// <see cref="PKM.Species"/> of the Set entity.
        /// </summary>
        public int Species { get; private set; } = -1;

        /// <summary>
        /// <see cref="PKM.Format"/> of the Set entity it is specific to.
        /// </summary>
        public int Format { get; } = PKMConverter.Format;

        /// <summary>
        /// <see cref="PKM.Nickname"/> of the Set entity.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// <see cref="PKM.Gender"/> name of the Set entity.
        /// </summary>
        public string Gender { get; private set; }

        /// <summary>
        /// <see cref="PKM.HeldItem"/> of the Set entity.
        /// </summary>
        public int HeldItem { get; private set; }

        /// <summary>
        /// <see cref="PKM.Ability"/> of the Set entity.
        /// </summary>
        public int Ability { get; private set; } = -1;

        /// <summary>
        /// <see cref="PKM.CurrentLevel"/> of the Set entity.
        /// </summary>
        public int Level { get; private set; } = 100;

        /// <summary>
        /// <see cref="PKM.CurrentLevel"/> of the Set entity.
        /// </summary>
        public bool Shiny { get; private set; }

        /// <summary>
        /// <see cref="PKM.CurrentFriendship"/> of the Set entity.
        /// </summary>
        public int Friendship { get; private set; } = 255;

        /// <summary>
        /// <see cref="PKM.Nature"/> of the Set entity.
        /// </summary>
        public int Nature { get; set; }

        /// <summary>
        /// <see cref="PKM.AltForm"/> name of the Set entity.
        /// </summary>
        public string Form { get; private set; }

        /// <summary>
        /// <see cref="PKM.AltForm"/> of the Set entity.
        /// </summary>
        public int FormIndex { get; private set; }

        /// <summary>
        /// <see cref="PKM.EVs"/> of the Set entity.
        /// </summary>
        public int[] EVs { get; private set; } = {00, 00, 00, 00, 00, 00};

        /// <summary>
        /// <see cref="PKM.IVs"/> of the Set entity.
        /// </summary>
        public int[] IVs { get; private set; } = {31, 31, 31, 31, 31, 31};

        /// <summary>
        /// <see cref="PKM.Moves"/> of the Set entity.
        /// </summary>
        public int[] Moves { get; } = {0, 0, 0, 0};

        /// <summary>
        /// Any lines that failed to be parsed.
        /// </summary>
        public readonly List<string> InvalidLines = new List<string>();

        private GameStrings Strings { get; set; } = DefaultStrings;
        private int LanguageID { get; set; } = DefaultLanguageID;

        private int[] IVsSpeedFirst => new[] {IVs[0], IVs[1], IVs[2], IVs[5], IVs[3], IVs[4]};
        private int[] IVsSpeedLast => new[] {IVs[0], IVs[1], IVs[2], IVs[4], IVs[5], IVs[3]};
        private int[] EVsSpeedFirst => new[] {EVs[0], EVs[1], EVs[2], EVs[5], EVs[3], EVs[4]};
        private int[] EVsSpeedLast => new[] {EVs[0], EVs[1], EVs[2], EVs[4], EVs[5], EVs[3]};

        /// <summary>
        /// Loads a new <see cref="ShowdownSet"/> from the input string. If no string is provided, a blank set is returned.
        /// </summary>
        /// <param name="input">Single-line string which will be split before loading.</param>
        public ShowdownSet(string input = null)
        {
            if (input == null)
                return;

            string[] lines = input.Split(Splitters, StringSplitOptions.None);
            LoadLines(lines);
        }

        /// <summary>
        /// Loads a new <see cref="ShowdownSet"/> from the input string. If no string is provided, a blank set is returned.
        /// </summary>
        /// <param name="lines">Enumerable list of lines.</param>
        public ShowdownSet(IEnumerable<string> lines)
        {
            if (lines == null)
                return;
            LoadLines(lines);
        }

        private void LoadLines(IEnumerable<string> lines)
        {
            lines = lines.Select(z => z.Replace("'", "’").Replace("–", "-").Trim()); // Sanitize apostrophes & dashes
            lines = lines.Where(z => z.Length > 2);

            ParseLines(lines);

            // Showdown Quirks
            Form = ConvertFormFromShowdown(Form, Species, Ability);
            // Set Form
            string[] formStrings = PKX.GetFormList(Species, Strings.Types, Strings.forms, genderForms);
            FormIndex = string.IsNullOrWhiteSpace(Form) ? 0 : Math.Max(0, Array.FindIndex(formStrings, z => z.Contains(Form)));
        }

        private void ParseLines(IEnumerable<string> lines)
        {
            int movectr = 0;
            var e = lines.GetEnumerator();
            if (!e.MoveNext())
                return;

            ParseFirstLine(e.Current);
            while (e.MoveNext())
            {
                var line = e.Current;
                if (line.StartsWith("-"))
                {
                    string moveString = ParseLineMove(line);
                    int move = Array.IndexOf(Strings.movelist, moveString);
                    if (move < 0)
                        InvalidLines.Add($"Unknown Move: {moveString}");
                    else
                        Moves[movectr++] = move;

                    if (movectr == 4)
                        return; // End of moves, end of set data
                    continue;
                }

                string[] brokenline = line.Split(LineSplit, StringSplitOptions.None);
                var piece1 = brokenline[0].Trim();
                var piece2 = brokenline.Length == 1 ? string.Empty : brokenline[1].Trim();
                if (!ParseEntry(piece1, piece2))
                    InvalidLines.Add(line);
            }
            e.Dispose();
        }

        private bool ParseEntry(string first, string second)
        {
            switch (first)
            {
                case "Trait": case "Ability": return (Ability = Array.IndexOf(Strings.abilitylist, second)) >= 0;
                case "Shiny": return Shiny = second.Trim() == "Yes";
                case "Nature": return (Nature = Array.IndexOf(Strings.natures, second)) >= 0;
                case "EV": case "EVs": ParseLineEVs(second); return true;
                case "IV": case "IVs": ParseLineIVs(second); return true;
                case "Level":
                {
                    if (!int.TryParse(second.Trim(), out int val))
                        return false;
                    Level = val;
                    return true;
                }
                case "Happiness": case "Friendship":
                {
                    if (!int.TryParse(second.Trim(), out int val))
                        return false;
                    Friendship = val;
                    return true;
                }
                default:
                {
                    if (first.EndsWith("Nature")) // XXX Nature
                    {
                        string naturestr = first.Split(' ')[0].Trim();
                        return (Nature = Array.IndexOf(Strings.natures, naturestr)) >= 0;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the standard Text representation of the set details.
        /// </summary>
        public string Text => GetText();

        /// <summary>
        /// Gets the localized Text representation of the set details.
        /// </summary>
        /// <param name="lang">2 character language code</param>
        public string LocalizedText(string lang) => LocalizedText(GameInfo.Language(lang));

        /// <summary>
        /// Gets the localized Text representation of the set details.
        /// </summary>
        /// <param name="lang">Language ID</param>
        private string LocalizedText(int lang)
        {
            var strings = GameInfo.GetStrings(lang);
            lang += lang >= 5 ? 2 : 1; // shift from array index to LanguageID
            LanguageID = lang;
            return GetText(strings);
        }

        private string GetText(GameStrings strings = null)
        {
            if (Species <= 0 || Species > MAX_SPECIES)
                return string.Empty;

            if (strings != null)
                Strings = strings;

            var result = new List<string>();

            // First Line: Name, Nickname, Gender, Item
            string form = ConvertFormToShowdown(Form, Species);
            result.Add(GetStringFirstLine(form));

            // IVs
            if (GetStringStats(out IEnumerable<string> ivstr, IVsSpeedLast, 31))
                result.Add($"IVs: {string.Join(" / ", ivstr)}");

            // EVs
            if (GetStringStats(out IEnumerable<string> evstr, EVsSpeedLast, 0))
                result.Add($"EVs: {string.Join(" / ", evstr)}");

            // Secondary Stats
            if (Ability > -1 && Ability < Strings.Ability.Count)
                result.Add($"Ability: {Strings.Ability[Ability]}");
            if (Level != 100)
                result.Add($"Level: {Level}");
            if (Shiny)
                result.Add("Shiny: Yes");

            if (Nature > -1)
                result.Add($"{Strings.Natures[Nature]} Nature");

            // Moves
            result.AddRange(GetStringMoves());

            return string.Join(Environment.NewLine, result);
        }

        private string GetStringFirstLine(string form)
        {
            string specForm = Strings.Species[Species];
            if (!string.IsNullOrWhiteSpace(form))
                specForm += $"-{form.Replace("Mega ", "Mega-")}";

            string result = GetSpeciesNickname(specForm);
            if (!string.IsNullOrEmpty(Gender))
                result += $" ({Gender})";
            if (HeldItem > 0)
            {
                var items = Strings.GetItemStrings(Format);
                if (HeldItem < items.Count)
                    result += $" @ {items[HeldItem]}";
            }
            return result;
        }

        private string GetSpeciesNickname(string specForm)
        {
            if (Nickname == null)
                return specForm;
            var name = PKX.GetSpeciesNameGeneration(Species, LanguageID, Format);
            if (name == Nickname)
                return specForm;
            return $"{Nickname} ({specForm})";
        }

        private static bool GetStringStats(out IEnumerable<string> result, int[] stats, int ignore)
        {
            var list = new List<string>();
            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i] == ignore) continue; // ignore unused EVs
                list.Add($"{stats[i]} {StatNames[i]}");
            }
            result = list;
            return list.Count > 0;
        }

        private IEnumerable<string> GetStringMoves()
        {
            foreach (int move in Moves.Where(move => move != 0 && move < Strings.Move.Count))
            {
                var str = $"- {Strings.Move[move]}";
                if (move == 237) // Hidden Power
                    str += $" [{Strings.Types[1+HiddenPower.GetType(IVs, Format)]}]";
                yield return str;
            }
        }

        /// <summary>
        /// Converts the <see cref="PKM"/> data into an importable set format for Pokémon Showdown.
        /// </summary>
        /// <param name="pkm">PKM to convert to string</param>
        /// <returns>Multi line set data</returns>
        public static string GetShowdownText(PKM pkm)
        {
            if (pkm.Species == 0)
                return string.Empty;
            return new ShowdownSet(pkm).Text;
        }

        /// <summary>
        /// Converts the <see cref="PKM"/> data into an importable set format for Pokémon Showdown.
        /// </summary>
        /// <param name="pkm">PKM to convert to string</param>
        /// <returns>New ShowdownSet object representing the input <see cref="pkm"/></returns>
        public ShowdownSet(PKM pkm)
        {
            if (pkm.Species <= 0)
                return;

            Nickname = pkm.Nickname;
            Species = pkm.Species;
            HeldItem = pkm.HeldItem;
            Ability = pkm.Ability;
            EVs = pkm.EVs;
            IVs = pkm.IVs;
            Moves = pkm.Moves;
            Nature = pkm.Nature;
            Gender = genders[pkm.Gender < 2 ? pkm.Gender : 2];
            Friendship = pkm.CurrentFriendship;
            Level = Experience.GetLevel(pkm.EXP, pkm.Species, pkm.AltForm);
            Shiny = pkm.IsShiny;

            FormIndex = pkm.AltForm;
            string[] Forms = PKX.GetFormList(Species, Strings.Types, Strings.forms, genderForms, pkm.Format);
            Form = pkm.AltForm > 0 && pkm.AltForm < Forms.Length ? Forms[pkm.AltForm] : string.Empty;
            Format = pkm.Format;
        }

        private void ParseFirstLine(string first)
        {
            if (first.Contains(" @ "))
            {
                string[] pieces = first.Split(ItemSplit, StringSplitOptions.None);
                string itemstr = pieces.Last().Trim();

                ParseItemStr(itemstr);
                ParseFirstLineNoItem(pieces[0]);
            }
            else
            {
                ParseFirstLineNoItem(first.Trim());
            }
        }

        private void ParseItemStr(string itemstr)
        {
            if (tryGetItem(Format))
                return;
            if (tryGetItem(3))
                return;
            if (tryGetItem(2))
                return;
            InvalidLines.Add($"Unknown Item: {itemstr}");

            bool tryGetItem(int format)
            {
                var items = (string[])Strings.GetItemStrings(format); // ireadonlylist->string[] must be possible for the provided strings
                int item = Array.IndexOf(items, itemstr);
                if (item < 0)
                    return false;
                HeldItem = item;
                return true;
            }
        }

        private void ParseFirstLineNoItem(string line)
        {
            // Gender Detection
            string last3 = line.Substring(line.Length - 3);
            if (last3 == "(M)" || last3 == "(F)")
            {
                Gender = last3.Substring(1, 1);
                line = line.Substring(0, line.Length - 3);
            }
            else if (line.Contains(Strings.Species[678])) // Meowstic Edge Case with no gender provided
            {
                Gender = "M";
            }

            // Nickname Detection
            if (line.Contains("(") && line.Contains(")"))
                ParseSpeciesNickname(line);
            else
                ParseSpeciesForm(line);
        }

        private bool ParseSpeciesForm(string spec)
        {
            spec = spec.Trim();
            if ((Species = Array.IndexOf(Strings.specieslist, spec)) >= 0) // success, nothing else!
                return true;

            // Forme string present.
            int end = spec.LastIndexOf('-');
            if (end < 0)
                return false;

            Species = Array.IndexOf(Strings.specieslist, spec.Substring(0, end).Trim());
            Form = spec.Substring(end + 1);

            if (Species >= 0)
                return true;

            // failure to parse, check edge cases
            foreach (var e in DashedSpecies)
            {
                if (!spec.StartsWith(Strings.Species[e].Replace("♂", "-M").Replace("♀", "-F")))
                    continue;
                Species = e;
                Form = spec.Substring(Strings.Species[e].Length);
                return true;
            }

            // Version Megas
            end = spec.LastIndexOf('-', Math.Max(0, end - 1));
            if (end < 0)
                return false;
            Species = Array.IndexOf(Strings.specieslist, spec.Substring(0, end).Trim());
            Form = spec.Substring(end + 1);

            return Species >= 0;
        }

        private void ParseSpeciesNickname(string line)
        {
            int index = line.LastIndexOf('(');
            string n1, n2;
            if (index > 1) // correct format
            {
                n1 = line.Substring(0, index).Trim();
                n2 = line.Substring(index).Trim();
                n2 = ReplaceAll(n2, string.Empty, "[", "]", "(", ")"); // Trim out excess data
            }
            else // nickname first (manually created set, incorrect)
            {
                int end = line.IndexOf(')');
                n2 = line.Substring(index + 1, end - 1);
                n1 = line.Substring(end + 2);
            }

            if (ParseSpeciesForm(n2))
            {
                // successful parse on n2=>Species/Form, n1 is nickname
                Nickname = n1;
                return;
            }
            // other case is possibly true (or both invalid).
            Nickname = n2;
            ParseSpeciesForm(n1);
        }

        private string ParseLineMove(string line)
        {
            string moveString = line.Substring(line[1] == ' ' ? 2 : 1);
            if (!moveString.Contains(Strings.Move[237])) // Hidden Power
                return moveString;

            if (moveString.Length <= 13)
                return Strings.Move[237];

            // Defined Hidden Power
            string type = moveString.Remove(0, 13);
            type = ReplaceAll(type, string.Empty, "[", "]", "(", ")"); // Trim out excess data
            int hpVal = Array.IndexOf(Strings.types, type) - 1; // Get HP Type

            if (IVs.Any(z => z != 31))
            {
                if (!HiddenPower.SetIVsForType(hpVal, IVs, Format))
                    InvalidLines.Add($"Invalid IVs for Hidden Power Type: {type}");
            }
            else if (hpVal >= 0)
            {
                IVs = PKX.SetHPIVs(hpVal, IVs, Format); // Get IVs
            }
            else
            {
                InvalidLines.Add($"Invalid Hidden Power Type: {type}");
            }
            return Strings.Move[237];
        }

        private void ParseLineEVs(string line)
        {
            var evlist = SplitLineStats(line);
            if (evlist.Length == 1)
                InvalidLines.Add("Unknown EV input.");
            for (int i = 0; i < evlist.Length / 2; i++)
            {
                int pos = i * 2;
                bool valid = ushort.TryParse(evlist[pos + 0], out ushort EV);
                int index = Array.IndexOf(StatNames, evlist[pos + 1]);
                if (valid && index > -1)
                    EVs[index] = EV;
                else
                    InvalidLines.Add($"Unknown EV Type input: {evlist[pos]}");
            }
            EVs = EVsSpeedFirst;
        }

        private void ParseLineIVs(string line)
        {
            string[] ivlist = SplitLineStats(line);
            if (ivlist.Length == 1)
                InvalidLines.Add("Unknown IV input.");
            for (int i = 0; i < ivlist.Length / 2; i++)
            {
                var pos = i * 2;
                bool valid = byte.TryParse(ivlist[pos + 0], out byte IV);
                int index = Array.IndexOf(StatNames, ivlist[pos + 1]);
                if (valid && index > -1)
                    IVs[index] = IV;
                else
                    InvalidLines.Add($"Unknown IV Type input: {ivlist[pos]}");
            }
            IVs = IVsSpeedFirst;
        }

        private static string ConvertFormToShowdown(string form, int spec)
        {
            if (string.IsNullOrWhiteSpace(form))
            {
                if (spec == 774) // Minior
                    form = "Meteor";
                return form;
            }

            switch (spec)
            {
                case 550 when form == "Blue":
                    return "Blue-Striped";
                case 666 when form == "Poké Ball":
                    return "Pokeball"; // Vivillon
                case 718: // Zygarde
                    form = form.Replace("-C", string.Empty);
                    return form.Replace("50%", string.Empty);
                case 774: // Minior
                    if (form.StartsWith("M-"))
                        return "Meteor";
                    return form.Replace("C-", string.Empty);
                case 800 when form == "Dusk": // Necrozma
                    return $"{form}-Mane";
                case 800 when form == "Dawn": // Necrozma
                    return $"{form}-Wings";

                case 676: // Furfrou
                case 658: // Greninja
                case 744: // Rockruff
                    return string.Empty;
                default:
                    if (Legal.Totem_USUM.Contains(spec) && form == "Large")
                        return Legal.Totem_Alolan.Contains(spec) && spec != 778 ? "Alola-Totem" : "Totem";
                    return form.Replace(" ", "-");
            }
        }

        private static string ConvertFormFromShowdown(string form, int spec, int ability)
        {
            form = form?.Replace(" ", "-"); // inconsistencies are great
            switch (spec)
            {
                case 550 when form == "Blue-Striped": // Basculin
                    return "Blue";
                case 658 when ability == 210: // Greninja
                    return "Ash"; // Battle Bond
                case 666 when form == "Pokeball": // Vivillon
                    return "Poké Ball";

                // Zygarde
                case 718 when string.IsNullOrWhiteSpace(form) && ability == 211:
                    return "50%-C";
                case 718 when string.IsNullOrWhiteSpace(form):
                    return "50%";
                case 718 when form == "Complete":
                    return form;
                case 718 when ability == 211:
                    return "-C"; // Power Construct

                case 744 when ability == 020: // Rockruff-1
                    return "Dusk";

                // Minior
                case 774 when !string.IsNullOrWhiteSpace(form) && form != "Meteor":
                    return $"C-{form}";

                // Necrozma
                case 800 when form == "Dusk-Mane":
                    return "Dusk";
                case 800 when form == "Dawn-Wings":
                    return "Dawn";

                default:
                    if (Legal.Totem_USUM.Contains(spec) && form?.EndsWith("Totem") == true)
                        return "Large";
                    return form;
            }
        }

        private static string[] SplitLineStats(string line)
        {
            // Because people think they can type sets out...
            return line
                .Replace("SAtk", "SpA").Replace("Sp Atk", "SpA")
                .Replace("SDef", "SpD").Replace("Sp Def", "SpD")
                .Replace("Spd", "Spe").Replace("Speed", "Spe").Split(StatSplitters, StringSplitOptions.None);
        }

        private static string ReplaceAll(string original, string to, params string[] toBeReplaced)
        {
            return toBeReplaced.Aggregate(original, (current, v) => current.Replace(v, to));
        }

        /// <summary>
        /// Fetches <see cref="ShowdownSet"/> data from the input <see cref="lines"/>.
        /// </summary>
        /// <param name="lines">Raw lines containing numerous multi-line set data.</param>
        /// <returns><see cref="ShowdownSet"/> objects until <see cref="lines"/> is consumed.</returns>
        public static IEnumerable<ShowdownSet> GetShowdownSets(IEnumerable<string> lines)
        {
            var setLines = new List<string>(8);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    setLines.Add(line);
                    continue;
                }
                if (setLines.Count == 0)
                    continue;
                yield return new ShowdownSet(setLines);
                setLines.Clear();
            }
            if (setLines.Count != 0)
                yield return new ShowdownSet(setLines);
        }

        /// <summary>
        /// Fetches ShowdownSet lines from the input <see cref="PKM"/> data.
        /// </summary>
        /// <param name="data">Pokémon data to summarize.</param>
        /// <returns>Consumable list of <see cref="ShowdownSet.Text"/> lines.</returns>
        public static IEnumerable<string> GetShowdownSets(IEnumerable<PKM> data) => data.Where(p => p.Species != 0).Select(GetShowdownText);

        /// <summary>
        /// Fetches ShowdownSet lines from the input <see cref="PKM"/> data, and combines it into one string.
        /// </summary>
        /// <param name="data">Pokémon data to summarize.</param>
        /// <param name="separator">Splitter between each set.</param>
        /// <returns>Single string containing all <see cref="ShowdownSet.Text"/> lines.</returns>
        public static string GetShowdownSets(IEnumerable<PKM> data, string separator) => string.Join(separator, GetShowdownSets(data));
    }
}
