namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.XD"/>.
/// </summary>
public sealed record EncounterSlot3XD(EncounterArea3XD Parent, ushort Species, byte LevelMin, byte LevelMax, byte SlotNumber)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<XK3>, INumberedSlot, IFatefulEncounterReadOnly, IRandomCorrelation
{
    public int Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public bool FatefulEncounter => true;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.None;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;

    public byte Form => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;
    public SlotType Type => Parent.Type;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public XK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public XK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pk = new XK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OT_Friendship = PersonalTable.E[Species].BaseFriendship,
            FatefulEncounter = FatefulEncounter,
            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)GameVersion.CXD,
            Ball = (byte)Ball.Poke,

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = 0,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria);
        EncounterUtil1.SetEncounterMoves(pk, GameVersion.XD, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(XK3 pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(-1, pi);
        int nature = (int)criteria.GetNature(Nature.Random);
        int ability = criteria.GetAbilityFromNumber(0);
        PIDGenerator.SetRandomPokeSpotPID(pk, nature, gender, ability, SlotNumber);
    }

    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo) => true; // Handled by Area
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;
    #endregion

    public bool IsCompatible(PIDType val, PKM pk) => val == PIDType.PokeSpot;
    public PIDType GetSuggestedCorrelation() => PIDType.PokeSpot;
}
