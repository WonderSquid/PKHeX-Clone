namespace PKHeX.Core
{
    /// <summary>
    /// Information wrapper used for Bulk Editing to apply suggested values.
    /// </summary>
    internal sealed class PKMInfo
    {
        internal PKM pkm { get; }
        internal PKMInfo(PKM pk) { pkm = pk; }

        private LegalityAnalysis la;
        internal LegalityAnalysis Legality => la ?? (la = new LegalityAnalysis(pkm));

        public bool Legal => Legality.Valid;
        internal int[] SuggestedRelearn => Legality.GetSuggestedRelearn();
        internal EncounterStatic SuggestedEncounter => Legality.GetSuggestedMetInfo();
    }
}