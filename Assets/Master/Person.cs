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
    [MemoryTable("person"), MessagePackObject(true)]
    public class Person
    {
        // index definition by attributes.
        [PrimaryKey]
        public int PersonId { get; set; }

        // secondary index can add multiple(discriminated by index-number).
        [SecondaryKey(0), NonUnique]
        [SecondaryKey(1, keyOrder: 1), NonUnique]
        public int Age { get; set; }

        [SecondaryKey(2), NonUnique]
        [SecondaryKey(1, keyOrder: 0), NonUnique]
        public Gender Gender { get; set; }

        public string Name { get; set; }
    }

    [MemoryTable("superman"), MessagePackObject(true)]
    public class Superman
    {
        [PrimaryKey, Key(0)]
        public int GroupId { get; set; }

        [Key(1)]
        public int[] Members { get; set; }
        [Key(2)]
        public int[] Scores { get; set; }



    }
}
