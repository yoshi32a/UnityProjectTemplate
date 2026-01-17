namespace Master.TypeParsers;

/// <summary>
/// パース時のコンテキスト情報
/// </summary>
public readonly struct ParseContext
{
    public string TableName { get; init; }
    public int RowNumber { get; init; }
    public string ColumnName { get; init; }

    public ParseContext(string tableName, int rowNumber, string columnName)
    {
        TableName = tableName;
        RowNumber = rowNumber;
        ColumnName = columnName;
    }
}