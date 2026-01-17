#if UNITY_EDITOR
using System;
using Master.Exceptions;

namespace Master.TypeParsers;

/// <summary>
/// Content型のパーサー
/// </summary>
public class ContentTypeParser : ITypeParser
{
    public bool CanParse(Type type) => type == typeof(Content);

    public object Parse(string rawValue, Type targetType, ParseContext context)
    {
        if (string.IsNullOrEmpty(rawValue))
        {
            throw new MasterParseException(
                "Content value cannot be empty",
                context.TableName, context.RowNumber, context.ColumnName);
        }

        var parts = rawValue.Split(',');
        if (parts.Length != 3)
        {
            throw new MasterParseException(
                $"Content requires exactly 3 components (Type,Id,Count), got {parts.Length}: '{rawValue}'",
                context.TableName, context.RowNumber, context.ColumnName);
        }

        try
        {
            return new Content
            {
                Type = Enum.Parse<ContentType>(parts[0].Trim()),
                Id = int.Parse(parts[1].Trim()),
                Count = int.Parse(parts[2].Trim())
            };
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or OverflowException)
        {
            throw new MasterParseException(
                $"Failed to parse Content from '{rawValue}'. Expected format: 'ContentType,Id,Count'",
                context.TableName, context.RowNumber, context.ColumnName, ex);
        }
    }
}
#endif
