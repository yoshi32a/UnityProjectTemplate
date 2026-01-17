using MasterMemory;
using MessagePack;

namespace Master;

public enum Gender
{
    Male,
    Female,
    Unknown
}

// table definition marked by MemoryTableAttribute.
// database-table must be serializable by MessagePack-CSsharp
[MemoryTable("person"), MessagePackObject]
public sealed class Person : IValidatable<Person>
{
    // index definition by attributes.
    [PrimaryKey] [Key(0)] public int PersonId { get; init; }

    // secondary index can add multiple(discriminated by index-number).
    [SecondaryKey(0), NonUnique]
    [SecondaryKey(1, keyOrder: 1), NonUnique]
    [Key(1)]
    public int Age { get; init; }

    [SecondaryKey(2), NonUnique]
    [SecondaryKey(1, keyOrder: 1), NonUnique]
    [Key(2)]
    public Gender Gender { get; init; }

    [Key(3)] public int NameId { get; init; }

    void IValidatable<Person>.Validate(IValidator<Person> validator)
    {
        var localize = validator.GetReferenceSet<Localize>();
        localize.Exists(x => x.NameId, x => x.Key);
    }

    public Localize Localize()
    {
        return MasterManager.DB.LocalizeTable.FindByKey(NameId);
    }
}

[MemoryTable("superman"), MessagePackObject]
public sealed class Superman
{
    [PrimaryKey, Key(0)] public int GroupId { get; init; }

    [Key(1)] public int[] Members { get; init; }
    [Key(2)] public int[] Scores { get; init; }
}