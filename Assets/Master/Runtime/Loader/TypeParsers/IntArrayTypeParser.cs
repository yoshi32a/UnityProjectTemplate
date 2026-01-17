using System.Globalization;

namespace Master.TypeParsers;

/// <summary>
/// int配列パーサー
/// </summary>
public class IntArrayTypeParser : ArrayTypeParserBase<int>
{
    protected override bool TryParseElement(string value, out int result)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
    }
}