#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Master.Editor;
using Master.Exceptions;
using Master.TypeParsers;
using MasterMemory;
using MasterMemory.Meta;
using UnityEditor;
using UnityEngine;

namespace Master;

/// <summary>
/// エディタ用 CSVからマスタを読み込むローダー
/// </summary>
public class CsvMasterLoader : IMasterLoader
{
    readonly TypeParserRegistry parserRegistry = new();

    /// <summary>
    /// マスタデータを読み込む
    /// </summary>
    public MemoryDatabase Load()
    {
        var builder = new DatabaseBuilder();

        var csvGuids = AssetDatabase.FindAssets("t:TextAsset", new[]
        {
            "Assets/Master/CSV/"
        });

        foreach (var guid in csvGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            // Skip non-CSV files (e.g., .md files)
            if (!path.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            ProcessCsvFile(builder, path);
        }

        var bin = builder.Build();
        var database = new MemoryDatabase(bin, maxDegreeOfParallelism: Environment.ProcessorCount);

        ValidateDatabase(database);

        return database;
    }

    /// <summary>
    /// 1つのCSVファイルを処理してビルダーに追加
    /// </summary>
    void ProcessCsvFile(DatabaseBuilder builder, string path)
    {
        var tableName = Path.GetFileNameWithoutExtension(path);
        var csvAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        var csv = csvAsset.text;

        var meta = MemoryDatabase.GetMetaDatabase();
        var table = meta.GetTableInfo(tableName);

        if (table == null)
        {
            throw new MasterTableNotFoundException(
                $"Table definition not found for CSV. Did you forget to create the [MemoryTable] class?",
                tableName);
        }

        var tableData = ParseCsvToObjects(csv, table, tableName);
        builder.AppendDynamic(table.DataType, tableData);
    }

    /// <summary>
    /// CSV文字列をオブジェクトのリストに変換
    /// </summary>
    List<object> ParseCsvToObjects(string csv, MetaTable table, string tableName)
    {
        var tableData = new List<object>();
        int rowNumber = 1; // 1-indexed (header is row 1)

        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        using var sr = new StreamReader(ms, Encoding.UTF8);
        using var reader = new TinyCsvReader(sr);

        while (reader.ReadValuesWithHeader() is { } values)
        {
            rowNumber++;
            var data = FormatterServices.GetUninitializedObject(table.DataType);

            foreach (var prop in table.Properties)
            {
                if (!values.TryGetValue(prop.NameSnakeCase, out var rawValue))
                {
                    throw new MasterParseException(
                        $"Column '{prop.NameSnakeCase}' not found in CSV header",
                        tableName, rowNumber, prop.NameSnakeCase);
                }

                var context = new ParseContext(tableName, rowNumber, prop.NameSnakeCase);

                object value;
                try
                {
                    value = parserRegistry.Parse(prop.PropertyInfo.PropertyType, rawValue, context);
                }
                catch (MasterDataException)
                {
                    throw; // 既にコンテキスト情報が含まれている
                }
                catch (Exception ex)
                {
                    throw new MasterParseException(
                        $"Unexpected error parsing value '{rawValue}'",
                        tableName, rowNumber, prop.NameSnakeCase, ex);
                }

                if (prop.PropertyInfo.SetMethod == null)
                {
                    throw new MasterParseException(
                        $"Property '{prop.PropertyInfo.Name}' has no setter. Use {{ get; private set; }} or {{ get; init; }}",
                        tableName, rowNumber, prop.NameSnakeCase);
                }

                prop.PropertyInfo.SetValue(data, value);
            }

            tableData.Add(data);
        }

        return tableData;
    }

    /// <summary>
    /// データベースのバリデーションを実行
    /// </summary>
    static void ValidateDatabase(MemoryDatabase database)
    {
        var validateResult = database.Validate();
        if (validateResult.IsValidationFailed)
        {
            throw new MasterValidationException(
                $"Validation failed:\n{validateResult.FormatFailedResults()}");
        }
    }
}
#endif
