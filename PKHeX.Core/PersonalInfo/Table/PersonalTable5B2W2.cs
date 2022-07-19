using System;

namespace PKHeX.Core;

public sealed class PersonalTable5B2W2 : IPersonalTable, IPersonalTable<PersonalInfo5B2W2>
{
    private readonly PersonalInfo5B2W2[] Table;
    private const int SIZE = PersonalInfo5B2W2.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_5;
    public int MaxSpeciesID => MaxSpecies;
    public int Count => Table.Length;

    public PersonalTable5B2W2(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo5B2W2[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo5B2W2(slice);
        }
    }

    public PersonalInfo5B2W2 this[int index] => Table[index];
    public PersonalInfo5B2W2 this[int species, int form] => Table[species];
    public PersonalInfo5B2W2 GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

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
            (int)Species.Unown => form < 26,
            (int)Species.Mothim => form < 3,
            _ => false,
        };
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);
}
