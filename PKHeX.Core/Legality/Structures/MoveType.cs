﻿namespace PKHeX.Core
{
    public enum MoveType
    {
        Any = -1,
        Normal,
        Fighting,
        Flying,
        Poison,
        Ground,
        Rock,
        Bug,
        Ghost,
        Steel,
        Fire,
        Water,
        Grass,
        Electric,
        Psychic,
        Ice,
        Dragon,
        Dark,
        Fairy,
    }

    public static partial class Extensions
    {
        public static MoveType GetMoveTypeGeneration(this MoveType type, int generation)
        {
            if (generation <= 2)
                return GetMoveTypeFromG12(type);
            return type;
        }
        private static MoveType GetMoveTypeFromG12(this MoveType type)
        {
            if (type <= MoveType.Rock)
                return type;
            type -= 1; // Bird
            if (type <= MoveType.Steel)
                return type;
            type -= 10; // 10 Normal duplicates
            return type;
        }
    }
}
