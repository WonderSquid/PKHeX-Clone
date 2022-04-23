﻿namespace PKHeX.Core;

public readonly record struct EvoCriteria : IDexLevel
{
    public ushort Species { get; init; }
    public byte Form { get; init; }
    public byte LevelUpRequired { get; init; }
    public byte LevelMax { get; init; }
    public byte LevelMin { get; init; }

    public EvolutionType Method { get; init; }

    public bool RequiresLvlUp => LevelUpRequired != 0;

    public override string ToString() => $"{(Species) Species}{(Form != 0 ? $"-{Form}" : "")}}} [{LevelMin},{LevelMax}] via {Method}";
}
