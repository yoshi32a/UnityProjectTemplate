using System;

namespace Master.Exceptions;

/// <summary>
/// テーブル定義が見つからない場合の例外
/// </summary>
public class MasterTableNotFoundException : MasterDataException
{
    public MasterTableNotFoundException(string message, string tableName = null,
        int? rowNumber = null, string columnName = null, Exception innerException = null)
        : base(message, tableName, rowNumber, columnName, innerException)
    {
    }
}