namespace PKHeX.Core;

/// <summary>
/// Minimal Trainer Information necessary for generating a <see cref="PKM"/>.
/// </summary>
public interface ITrainerInfo : ITrainerID32
{
    string OT { get; }
    byte Gender { get; }
    GameVersion Version { get; }
    int Language { get; }

    byte Generation { get; }
    EntityContext Context { get; }
}

/// <summary>
/// Extension methods for <see cref="ITrainerInfo"/>.
/// </summary>
public static class TrainerInfoExtensions
{
    /// <summary>
    /// Copies the <see cref="ITrainerInfo"/> data to the <see cref="PKM"/> object.
    /// </summary>
    /// <param name="info">Trainer Information</param>
    /// <param name="pk">Pokémon to copy to</param>
    public static void ApplyTo(this ITrainerInfo info, PKM pk)
    {
        pk.OriginalTrainerName = info.OT;
        pk.TID16 = info.TID16;
        pk.SID16 = pk.Format < 3 || pk.VC ? default : info.SID16;
        pk.OriginalTrainerGender = info.Gender;
        pk.Language = info.Language;
        pk.Version = info.Version;

        if (pk is not IRegionOrigin tr)
            return;

        if (info is not IRegionOrigin o)
            return;
        o.CopyRegionOrigin(tr);
    }

    /// <summary>
    /// Copies the <see cref="ITrainerInfo"/> data to the <see cref="PKM"/> object's Handling Trainer data.
    /// </summary>
    /// <param name="sav">Trainer Information</param>
    /// <param name="pk">Pokémon to copy to</param>
    /// <param name="force">If true, will overwrite the Handling Trainer Data even if it has not been traded.</param>
    public static void ApplyHandlingTrainerInfo(this ITrainerInfo sav, PKM pk, bool force = false)
    {
        if (pk.Format == sav.Generation && !force)
            return;

        pk.HandlingTrainerName = sav.OT;
        pk.HandlingTrainerGender = sav.Gender;
        pk.HandlingTrainerFriendship = pk.OriginalTrainerFriendship;
        pk.CurrentHandler = 1;
        if (pk is IHandlerLanguage h)
            h.HandlingTrainerLanguage = (byte)sav.Language;

        if (pk is PK6 pk6 && sav is IRegionOrigin o)
        {
            pk6.Geo1_Country = o.Country;
            pk6.Geo1_Region = o.Region;
            pk6.SetTradeMemoryHT6(true);
        }
        else if (pk is PK8 pk8 && PersonalTable.SWSH.IsPresentInGame(pk.Species, pk.Form))
        {
            pk8.SetTradeMemoryHT8();
        }
    }

    /// <summary>
    /// Checks if the <see cref="ITrainerInfo"/> data matches the <see cref="PKM"/> object's Original Trainer data.
    /// </summary>
    /// <param name="tr">Trainer Information</param>
    /// <param name="pk">Pokémon to compare to</param>
    /// <returns>True if the data matches.</returns>
    public static bool IsFromTrainer(this ITrainerInfo tr, PKM pk)
    {
        if (pk.IsEgg)
            return tr.IsFromTrainerEgg(pk);

        if (tr.Version == GameVersion.Any)
            return true;

        if (!IsFromTrainerNoVersion(tr, pk))
            return false;

        return IsMatchVersion(tr, pk);
    }

    /// <summary>
    /// Checks if the <see cref="ITrainerInfo"/> data matches the <see cref="PKM"/> object's Original Trainer data, ignoring the version.
    /// </summary>
    /// <param name="tr">Trainer Information</param>
    /// <param name="pk">Pokémon to compare to</param>
    /// <returns>True if the data matches.</returns>
    public static bool IsFromTrainerNoVersion(ITrainerInfo tr, PKM pk)
    {
        if (tr.ID32 != pk.ID32)
            return false;
        if (tr.OT != pk.OriginalTrainerName)
            return false;

        if (pk.Format == 3)
            return true; // Generation 3 does not check ot gender nor pokemon version

        if (tr.Gender != pk.OriginalTrainerGender)
        {
            if (pk.Format == 2)
                return pk is ICaughtData2 { CaughtData: 0 };
            return false;
        }
        return true;
    }

    /// <summary>
    /// Only call this if it is still an egg.
    /// </summary>
    public static bool IsFromTrainerEgg(this ITrainerInfo tr, PKM pk)
    {
        System.Diagnostics.Debug.Assert(pk.IsEgg);

        if (tr.Context != pk.Context)
            return false;
        if (tr.ID32 != pk.ID32)
            return false;
        if (tr.Gender != pk.OriginalTrainerGender)
            return false;

        if (tr.Version != pk.Version)
        {
            // PK9 does not store version for Picnic eggs.
            if (pk is PK9 { Version: 0 }) { }
            else { return false; }
        }

        if (tr.OT != pk.OriginalTrainerName)
            return false;

        return true;
    }

    private static bool IsMatchVersion(ITrainerInfo tr, PKM pk)
    {
        if (tr.Version == pk.Version)
            return true;
        if (pk.GO_LGPE)
            return tr.Version is GameVersion.GP or GameVersion.GE;
        return false;
    }
}
