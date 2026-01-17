using System.Globalization;

namespace Master.TypeParsers;

/// <summary>
/// float配列パーサー
/// </summary>
public class FloatArrayTypeParser : ArrayTypeParserBase<float>
{
    protected override bool TryParseElement(string value, out float result)
    {
        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
    }
}