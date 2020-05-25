﻿namespace PKHeX.Core
{
    public interface IDynamaxLevel
    {
        byte DynamaxLevel { get; set; }
    }

    public static class DynamaxLevelExtensions
    {
        public static bool CanHaveDynamaxLevel(this IDynamaxLevel _, PKM pkm)
        {
            if (pkm.IsEgg)
                return false;
            if (pkm.Species >= (int)Species.Zacian && pkm.Species <= (int)Species.Eternatus)
                return false;
            return true;
        }
    }
}
