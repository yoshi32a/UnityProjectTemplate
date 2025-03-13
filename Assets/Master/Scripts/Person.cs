using MasterMemory;
using MessagePack;

namespace Master
{
    public enum Gender
    {
        Male,
        Female,
        Unknown
    }

    // table definition marked by MemoryTableAttribute.
    // database-table must be serializable by MessagePack-CSsharp
    [MemoryTable("person"), MessagePackObject]
    public class Person : IValidatable<Person>
    {
        // index definition by attributes.
        [PrimaryKey] [Key(0)] public int PersonId { get; set; }

        // secondary index can add multiple(discriminated by index-number).
        [SecondaryKey(0), NonUnique]
        [SecondaryKey(1, keyOrder: 1), NonUnique]
        [Key(1)]
        public int Age { get; set; }

        [SecondaryKey(2), NonUnique]
        [SecondaryKey(1, keyOrder: 0), NonUnique]
        [Key(2)]
        public Gender Gender { get; set; }

        [Key(3)] public int TextKey { get; set; }

        public void Validate(IValidator<Person> validator)
        {
            var localize = validator.GetReferenceSet<Localize>();
            localize.Exists(x => x.TextKey, x => x.Key);
        }
    }

    [MemoryTable("superman"), MessagePackObject]
    public class Superman
    {
        [PrimaryKey, Key(0)] public int GroupId { get; set; }

        [Key(1)] public int[] Members { get; set; }
        [Key(2)] public int[] Scores { get; set; }
    }
}
