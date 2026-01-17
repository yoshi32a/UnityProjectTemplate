#if UNITY_EDITOR
using System;
using Master.Exceptions;
using UnityEngine.Pool;

namespace Master.TypeParsers;

/// <summary>
/// 配列パース用基底クラス
/// </summary>
public abstract class ArrayTypeParserBase<T> : ITypeParser
{
    public bool CanParse(Type type) => type == typeof(T[]);

    public object Parse(string rawValue, Type targetType, ParseContext context)
    {
        if (string.IsNullOrEmpty(rawValue))
        {
            return Array.Empty<T>();
        }

        var parts = rawValue.Split(',');

        using (ListPool<T>.Get(out var pool))
        {
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                if (!TryParseElement(part, out var element))
                {
                    throw new MasterParseException(
                        $"Failed to parse '{part}' as {typeof(T).Name} in array (index {i})",
                        context.TableName, context.RowNumber, context.ColumnName);
                }

                pool.Add(element);
            }

            return pool.ToArray();
        }
    }

    protected abstract bool TryParseElement(string value, out T result);
}
#endif
