using MessagePack;
using MessagePack.Formatters;
using Unity.Mathematics;

namespace Master.Formatters;

public class Float3Formatter : IMessagePackFormatter<float3>
{
    public void Serialize(ref MessagePackWriter writer, float3 value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(3);
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
    }

    public float3 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return default;
        }

        var count = reader.ReadArrayHeader();
        if (count != 3) throw new MessagePackSerializationException("Invalid float3 array length");

        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();

        return new float3(x, y, z);
    }
}
