#if UNITY_EDITOR
using System;

namespace Master.TypeParsers;

/// <summary>
/// 文字列型のパーサー
/// </summary>
public class StringTypeParser : ITypeParser
{
    public bool CanParse(Type type) => type == typeof(string);

    public object Parse(string rawValue, Type targetType, ParseContext context)
    {
        return rawValue;
    }
}
#endif
