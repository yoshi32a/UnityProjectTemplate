#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Master.Exceptions;

namespace Master.TypeParsers;

/// <summary>
/// 型パーサーの登録と取得を管理するレジストリ
/// </summary>
public class TypeParserRegistry
{
    readonly List<ITypeParser> parsers = new();
    readonly Dictionary<Type, ITypeParser> cache = new();

    public TypeParserRegistry()
    {
        // 優先度順に登録（先に登録したものが優先）
        parsers.Add(new StringTypeParser());
        parsers.Add(new NullableTypeParser(this));
        parsers.Add(new EnumTypeParser());
        parsers.Add(new IntArrayTypeParser());
        parsers.Add(new FloatArrayTypeParser());
        parsers.Add(new Float3TypeParser());
        parsers.Add(new ContentTypeParser());
        parsers.Add(new PrimitiveTypeParser());
    }

    /// <summary>
    /// 指定した型の値をパース
    /// </summary>
    public object Parse(Type type, string rawValue, ParseContext context)
    {
        var parser = GetParser(type);
        if (parser == null)
        {
            throw new MasterParseException(
                $"No parser found for type: {type.FullName}",
                context.TableName, context.RowNumber, context.ColumnName);
        }

        return parser.Parse(rawValue, type, context);
    }

    ITypeParser GetParser(Type type)
    {
        if (cache.TryGetValue(type, out var cached))
        {
            return cached;
        }

        foreach (var parser in parsers)
        {
            if (parser.CanParse(type))
            {
                cache[type] = parser;
                return parser;
            }
        }

        return null;
    }
}
#endif
