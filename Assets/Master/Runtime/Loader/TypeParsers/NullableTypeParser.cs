using System;

namespace Master.TypeParsers;

/// <summary>
/// Nullable型のパーサー
/// </summary>
public class NullableTypeParser : ITypeParser
{
    readonly TypeParserRegistry registry;

    public NullableTypeParser(TypeParserRegistry registry)
    {
        this.registry = registry;
    }

    public bool CanParse(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    public object Parse(string rawValue, Type targetType, ParseContext context)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return null;
        }

        var underlyingType = targetType.GenericTypeArguments[0];
        return registry.Parse(underlyingType, rawValue, context);
    }
}
