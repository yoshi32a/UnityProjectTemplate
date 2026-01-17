using System;

namespace Master.Exceptions;

/// <summary>
/// CSV値のパースに失敗した場合の例外
/// </summary>
public class MasterParseException : MasterDataException
{
    public MasterParseException(string message, string tableName = null,
        int? rowNumber = null, string columnName = null, Exception innerException = null)
        : base(message, tableName, rowNumber, columnName, innerException)
    {
    }
}