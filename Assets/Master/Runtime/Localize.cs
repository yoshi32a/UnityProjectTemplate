using MasterMemory;
using MessagePack;

namespace Master;

[MemoryTable("localize"), MessagePackObject]
public class Localize : IValidatable<Localize>
{
    [PrimaryKey, Key(0)] public int Key { get; init; }

    [Key(1)] public string Value { get; init; }

    public void Validate(IValidator<Localize> validator)
    {
    }
}