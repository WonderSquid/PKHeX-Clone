using System;

namespace PKHeX.Core;

public sealed class PersonalTable6XY : IPersonalTable, IPersonalTable<PersonalInfo6XY>
{
    private readonly PersonalInfo6XY[] Table;
    private const int SIZE = PersonalInfo6XY.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_6;
    public int MaxSpeciesID => MaxSpecies;
    public int Count => Table.Length;

    public PersonalTable6XY(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo6XY[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo6XY(slice);
        }
    }

    public PersonalInfo6XY this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo6XY this[int species, int form] => Table[GetFormIndex(species, form)];
    public PersonalInfo6XY GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(int species, int form)
    {
        if ((uint)species <= MaxSpecies)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(int species) => (uint)species <= MaxSpecies;
    public bool IsPresentInGame(int species, int form)
    {
        if (!IsSpeciesInGame(species))
            return false;
        if (form == 0)
            return true;
        if (Table[species].HasForm(form))
            return true;
        return species switch
        {
            (int)Species.Unown => form < 28,
            (int)Species.Mothim => form < 3,
            (int)Species.Arceus => form < 17,
            (int)Species.Genesect => form <= 4,
            (int)Species.Scatterbug or (int)Species.Spewpa => form <= 17,
            (int)Species.Vivillon => form <= 19,
            _ => false,
        };
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);
}
