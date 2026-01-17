using System;
using System.Text;

namespace Master.Exceptions;

/// <summary>
/// マスタデータ処理の基底例外
/// </summary>
public class MasterDataException : Exception
{
    public string TableName { get; }
    public int? RowNumber { get; }
    public string ColumnName { get; }

    public MasterDataException(string message, string tableName = null,
        int? rowNumber = null, string columnName = null, Exception innerException = null)
        : base(FormatMessage(message, tableName, rowNumber, columnName), innerException)
    {
        TableName = tableName;
        RowNumber = rowNumber;
        ColumnName = columnName;
    }

    static string FormatMessage(string message, string tableName, int? rowNumber, string columnName)
    {
        var sb = new StringBuilder(message);
        if (tableName != null) sb.Append($" [Table: {tableName}]");
        if (rowNumber != null) sb.Append($" [Row: {rowNumber}]");
        if (columnName != null) sb.Append($" [Column: {columnName}]");
        return sb.ToString();
    }
}
