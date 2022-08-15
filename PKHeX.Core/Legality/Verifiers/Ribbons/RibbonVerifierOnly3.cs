using static PKHeX.Core.RibbonIndex3;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetOnly3"/>.
/// </summary>
public static class RibbonVerifierOnly3
{
    public static void Parse(this IRibbonSetOnly3 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        if (r.RibbonWorld)
            list.Add(RibbonIndex.World);

        var max = RibbonRules.IsAllowedContest3(args.History) ? 4 : 0;
        FlagContest(r, ref list, max);
    }

    private static void FlagContest(IRibbonSetOnly3 r, ref RibbonResultList list, int max = 4)
    {
        if (r.RibbonCountG3Cool > max)
            list.Add(Cool);
        if (r.RibbonCountG3Beauty > max)
            list.Add(Beauty);
        if (r.RibbonCountG3Cute > max)
            list.Add(Cute);
        if (r.RibbonCountG3Smart > max)
            list.Add(Smart);
        if (r.RibbonCountG3Tough > max)
            list.Add(Tough);
    }

    public static void ParseEgg(this IRibbonSetOnly3 r, ref RibbonResultList list)
    {
        if (r.RibbonWorld)
            list.Add(RibbonIndex.World);

        if (r.RibbonCountG3Cool != 0)
            list.Add(Cool);
        if (r.RibbonCountG3Beauty != 0)
            list.Add(Beauty);
        if (r.RibbonCountG3Cute != 0)
            list.Add(Cute);
        if (r.RibbonCountG3Smart != 0)
            list.Add(Smart);
        if (r.RibbonCountG3Tough != 0)
            list.Add(Tough);
    }
}
