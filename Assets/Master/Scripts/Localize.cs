using MasterMemory;
using MessagePack;

namespace Master
{
    [MemoryTable("localize"), MessagePackObject]
    public class Localize : IValidatable<Localize>
    {
        [PrimaryKey, Key(0)]
        public int Key { get; set; }

        [Key(1)]
        public string Value { get; set; }

        public void Validate(IValidator<Localize> validator)
        {

        }
    }
}
