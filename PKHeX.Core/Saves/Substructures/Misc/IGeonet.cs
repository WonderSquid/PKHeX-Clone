using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 4/5 Geonet
/// </summary>
public interface IGeonet
{
    /// <summary>
    /// Lets the globe be panned outside Japan and zoomed out in Japanese games. Has no effect in non-Japanese games.
    /// Set once you register a location outside of Japan, even if it's just your own location in a non-Japanese game.
    /// </summary>
    public bool GlobalFlag { get; set; }

    /// <summary>
    /// Gets the number of subregions for the specified country.
    /// </summary>
    /// <param name="country">Country index</param>
    /// <returns>Number of subregions.</returns>
    public abstract static byte GetSubregionCount(byte country);

    /// <summary>
    /// Gets the point status for the specified country and subregion.
    /// </summary>
    /// <param name="country">Country index</param>
    /// <param name="subregion">Subregion index</param>
    /// <returns>Point status.</returns>
    public abstract GeonetPoint GetCountrySubregion(byte country, byte subregion);

    /// <summary>
    /// Sets the point status for the specified country and subregion.
    /// </summary>
    /// <param name="country">Country index</param>
    /// <param name="subregion">Subregion index</param>
    public abstract void SetCountrySubregion(byte country, byte subregion, GeonetPoint point);

    /// <summary>
    /// Sets all Geonet locations.
    /// </summary>
    public abstract void SetAll();

    /// <summary>
    /// Sets all legal Geonet locations.
    /// </summary>
    public abstract void SetAllLegal();

    /// <summary>
    /// Clear all Geonet locations.
    /// </summary>
    public abstract void ClearAll();

    /// <summary>
    /// Sets the point status for the registered Geonet location as the player's location.
    /// </summary>
    public abstract void SetSAVCountry();
}

public enum GeonetPoint
{
    /// <summary>
    /// Location that has never been communicated with.
    /// </summary>
    None = 0,

    /// <summary>
    /// Location that was first communicated with today.
    /// </summary>
    Blue = 1,

    /// <summary>
    /// Location that has already been communicated with.
    /// </summary>
    Yellow = 2,

    /// <summary>
    /// The player's own registered location.
    /// </summary>
    Red = 3,
}
