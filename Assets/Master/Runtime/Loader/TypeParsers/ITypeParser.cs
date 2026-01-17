#if UNITY_EDITOR
using System;

namespace Master.TypeParsers;

/// <summary>
/// CSV値のパース処理インターフェース
/// </summary>
public interface ITypeParser
{
    /// <summary>この型を処理できるか判定</summary>
    bool CanParse(Type type);

    /// <summary>文字列から値をパース</summary>
    object Parse(string rawValue, Type targetType, ParseContext context);
}
#endif
