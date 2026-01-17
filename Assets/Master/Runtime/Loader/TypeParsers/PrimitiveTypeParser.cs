using System;
using System.Globalization;
using Master.Exceptions;

namespace Master.TypeParsers;

/// <summary>
/// プリミティブ型のパーサー（int, float, bool等）
/// </summary>
public class PrimitiveTypeParser : ITypeParser
{
    public bool CanParse(Type type)
    {
        var typeCode = Type.GetTypeCode(type);
        return typeCode != TypeCode.Object && typeCode != TypeCode.Empty && typeCode != TypeCode.DBNull;
    }

    public object Parse(string rawValue, Type targetType, ParseContext context)
    {
        try
        {
            return Type.GetTypeCode(targetType) switch
            {
                TypeCode.Boolean => ParseBoolean(rawValue),
                TypeCode.Char => char.Parse(rawValue),
                TypeCode.SByte => sbyte.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.Byte => byte.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.Int16 => short.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.UInt16 => ushort.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.Int32 => int.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.UInt32 => uint.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.Int64 => long.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.UInt64 => ulong.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.Single => float.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.Double => double.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.Decimal => decimal.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.DateTime => DateTime.Parse(rawValue, CultureInfo.InvariantCulture),
                TypeCode.String => rawValue,
                _ => ParseSpecialTypes(rawValue, targetType, context)
            };
        }
        catch (FormatException ex)
        {
            throw new MasterParseException(
                $"Failed to parse '{rawValue}' as {targetType.Name}",
                context.TableName, context.RowNumber, context.ColumnName, ex);
        }
        catch (OverflowException ex)
        {
            throw new MasterParseException(
                $"Value '{rawValue}' is out of range for {targetType.Name}",
                context.TableName, context.RowNumber, context.ColumnName, ex);
        }
    }

    static bool ParseBoolean(string rawValue)
    {
        // "True"/"False" or "0"/"1" をサポート
        if (int.TryParse(rawValue, out var intBool))
        {
            return Convert.ToBoolean(intBool);
        }

        return bool.Parse(rawValue);
    }

    static object ParseSpecialTypes(string rawValue, Type targetType, ParseContext context)
    {
        if (targetType == typeof(DateTimeOffset))
        {
            return DateTimeOffset.Parse(rawValue, CultureInfo.InvariantCulture);
        }

        if (targetType == typeof(TimeSpan))
        {
            return TimeSpan.Parse(rawValue, CultureInfo.InvariantCulture);
        }

        if (targetType == typeof(Guid))
        {
            return Guid.Parse(rawValue);
        }

        throw new MasterParseException(
            $"Unsupported primitive type: {targetType.FullName}",
            context.TableName, context.RowNumber, context.ColumnName);
    }
}