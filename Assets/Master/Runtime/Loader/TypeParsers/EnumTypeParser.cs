#if UNITY_EDITOR
using System;
using Master.Exceptions;

namespace Master.TypeParsers;

/// <summary>
/// Enum型の汎用パーサー
/// </summary>
public class EnumTypeParser : ITypeParser
{
    public bool CanParse(Type type) => type.IsEnum;

    public object Parse(string rawValue, Type targetType, ParseContext context)
    {
        try
        {
            // 数値とシンボル名の両方をサポート
            return Enum.Parse(targetType, rawValue, ignoreCase: true);
        }
        catch (ArgumentException ex)
        {
            throw new MasterParseException(
                $"Failed to parse '{rawValue}' as {targetType.Name}. " +
                $"Valid values: {string.Join(", ", Enum.GetNames(targetType))}",
                context.TableName, context.RowNumber, context.ColumnName, ex);
        }
    }
}
#endif
