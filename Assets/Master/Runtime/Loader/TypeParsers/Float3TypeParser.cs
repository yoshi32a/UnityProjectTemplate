#if UNITY_EDITOR
using System;
using System.Globalization;
using Master.Exceptions;
using Unity.Mathematics;

namespace Master.TypeParsers;

/// <summary>
/// float3型のパーサー
/// </summary>
public class Float3TypeParser : ITypeParser
{
    public bool CanParse(Type type) => type == typeof(float3);

    public object Parse(string rawValue, Type targetType, ParseContext context)
    {
        if (string.IsNullOrEmpty(rawValue))
        {
            throw new MasterParseException(
                "float3 value cannot be empty",
                context.TableName, context.RowNumber, context.ColumnName);
        }

        // "x,y,z" or "[x,y,z]" or "\"x,y,z\"" 形式をサポート
        var cleaned = rawValue.Trim('"', '[', ']', ' ');
        var parts = cleaned.Split(',');

        if (parts.Length != 3)
        {
            throw new MasterParseException(
                $"float3 requires exactly 3 components, got {parts.Length}: '{rawValue}'",
                context.TableName, context.RowNumber, context.ColumnName);
        }

        if (!float.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var x) ||
            !float.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var y) ||
            !float.TryParse(parts[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
        {
            throw new MasterParseException(
                $"Failed to parse float3 components: '{rawValue}'",
                context.TableName, context.RowNumber, context.ColumnName);
        }

        return new float3(x, y, z);
    }
}
#endif
