using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Logic to check if things can learn specific moves.
/// </summary>
public static class EncounterLearn
{
    static EncounterLearn()
    {
        if (!EncounterEvent.Initialized)
            EncounterEvent.RefreshMGDB();
    }

    /// <summary>
    /// Default response if there are no matches.
    /// </summary>
    public const string NoMatches = "None";

    /// <summary>
    /// Checks if a <see cref="species"/> can learn all input <see cref="moves"/>.
    /// </summary>
    public static bool CanLearn(string species, IEnumerable<string> moves, int form = 0, string lang = GameLanguage.DefaultLanguage)
    {
        var encounters = GetLearn(species, moves, form, lang);
        return encounters.Any();
    }

    /// <summary>
    /// Gets a summary of all encounters where a <see cref="species"/> can learn all input <see cref="moves"/>.
    /// </summary>
    public static IEnumerable<string> GetLearnSummary(string species, IEnumerable<string> moves, int form = 0, string lang = GameLanguage.DefaultLanguage)
    {
        var encounters = GetLearn(species, moves, form, lang);
        var msg = Summarize(encounters).ToList();
        if (msg.Count == 0)
            msg.Add(NoMatches);
        return msg;
    }

    /// <summary>
    /// Gets all encounters where a <see cref="species"/> can learn all input <see cref="moves"/>.
    /// </summary>
    public static IEnumerable<IEncounterable> GetLearn(string species, IEnumerable<string> moves, int form = 0, string lang = GameLanguage.DefaultLanguage)
    {
        var str = GameInfo.GetStrings(lang);

        var speciesID = StringUtil.FindIndexIgnoreCase(str.specieslist, species);
        var moveIDs = StringUtil.GetIndexes(str.movelist, moves.ToList());
        if (Array.Exists(moveIDs, static z => z <= 0))
            return Array.Empty<IEncounterable>();

        var span = new ushort[moveIDs.Length];
        for (int i = 0; i < moveIDs.Length; i++)
            span[i] = (ushort)moveIDs[i];

        return GetLearn(speciesID, span, form);
    }

    /// <summary>
    /// Gets all encounters where a <see cref="species"/> can learn all input <see cref="moves"/>.
    /// </summary>
    public static IEnumerable<IEncounterable> GetLearn(int species, ushort[] moves, int form = 0)
    {
        if (species <= 0)
            return Array.Empty<IEncounterable>();

        var blank = EntityBlank.GetIdealBlank(species, form);
        blank.Species = species;
        blank.Form = form;

        var vers = GameUtil.GameVersions;
        return EncounterMovesetGenerator.GenerateEncounters(blank, moves, vers);
    }

    /// <summary>
    /// Summarizes all encounters by grouping by <see cref="IEncounterable.Name"/>.
    /// </summary>
    public static IEnumerable<string> Summarize(IEnumerable<IEncounterable> encounters, bool advanced = false)
    {
        var types = encounters.GroupBy(z => z.Name);
        return Summarize(types, advanced);
    }

    /// <summary>
    /// Summarizes groups of encounters.
    /// </summary>
    public static IEnumerable<string> Summarize(IEnumerable<IGrouping<string, IEncounterable>> types, bool advanced = false)
    {
        return types.SelectMany(g => EncounterSummary.SummarizeGroup(g, g.Key, advanced));
    }
}
