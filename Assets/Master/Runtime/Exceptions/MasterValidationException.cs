using System;

namespace Master.Exceptions;

/// <summary>
/// マスタデータのバリデーションに失敗した場合の例外
/// </summary>
public class MasterValidationException : MasterDataException
{
    public MasterValidationException(string message, string tableName = null,
        int? rowNumber = null, string columnName = null, Exception innerException = null)
        : base(message, tableName, rowNumber, columnName, innerException)
    {
    }
}