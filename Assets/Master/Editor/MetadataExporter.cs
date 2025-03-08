using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MasterMemory;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Master.Editor
{
    public static class MetadataExporter
    {
        static readonly string CsvPath = $"{Directory.GetCurrentDirectory()}/Assets/Master/CSV/";

        [MenuItem("Tools/Master/Create Metadata")]
        public static void ExportMetadata()
        {
            var metaDB = MemoryDatabase.GetMetaDatabase();

            var sb = new StringBuilder();
            foreach (var table in metaDB.GetTableInfos())
            {
                sb.Clear();

                // ファイルがあったら一旦作らない
                if (File.Exists($"{CsvPath}{table.TableName}.csv"))
                {
                    Debug.Log("continue");
                    continue;
                }

                foreach (var prop in table.Properties)
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(',');
                    }

                    sb.Append(prop.NameSnakeCase);
                }

                File.WriteAllText($"{CsvPath}{table.TableName}.csv", sb.ToString(), new UTF8Encoding(false));
                Debug.Log($"csvファイルを作りました {CsvPath}{table.TableName}.csv");
            }
        }

        [MenuItem("Tools/Master/CsvToBin")]
        public static void CsvToBin()
        {
            var builder = new DatabaseBuilder();

            var csvGuids = AssetDatabase.FindAssets("t:TextAsset", new[]
            {
                "Assets/Master/CSV/"
            });
            foreach (var guid in csvGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var csvAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                var tableName = Path.GetFileNameWithoutExtension(path);
                var csv = csvAsset.text;

                var meta = MemoryDatabase.GetMetaDatabase();
                var table = meta.GetTableInfo(tableName);

                var tableData = new List<object>();
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(csv)))
                using (var sr = new StreamReader(ms, Encoding.UTF8))
                using (var reader = new TinyCsvReader(sr))
                {
                    while (reader.ReadValuesWithHeader() is { } values)
                    {
                        // create data without call constructor
                        var data = FormatterServices.GetUninitializedObject(table.DataType);

                        foreach (var prop in table.Properties)
                        {
                            if (values.TryGetValue(prop.NameSnakeCase, out var rawValue))
                            {
                                var value = ParseValue(prop.PropertyInfo.PropertyType, rawValue);
                                if (prop.PropertyInfo.SetMethod == null)
                                {
                                    throw new Exception(
                                        "Target property does not exists set method. If you use {get;}, please change to { get; private set; }, Type:"
                                        + prop.PropertyInfo.DeclaringType
                                        + " Prop:"
                                        + prop.PropertyInfo.Name);
                                }

                                prop.PropertyInfo.SetValue(data, value);
                            }
                            else
                            {
                                throw new KeyNotFoundException($"Not found \"{prop.NameSnakeCase}\" in \"{tableName}.csv\" header.");
                            }
                        }

                        tableData.Add(data);
                    }
                }

                // add dynamic collection.
                builder.AppendDynamic(table.DataType, tableData);
            }

            var bin = builder.Build();
            var database = new MemoryDatabase(bin, maxDegreeOfParallelism: Environment.ProcessorCount);

            var validateResult = database.Validate();
            if (validateResult.IsValidationFailed)
            {
                // 文字列でエラー内容を出力するには FormatFailedResults() を使う
                Debug.LogError(validateResult.FormatFailedResults());
            }
            else
            {
                File.WriteAllBytes($"{Utility.BinPath}", bin);
                Debug.Log("[Completed] Master.bin exported.");

                AssetDatabase.ImportAsset(Utility.BinPath);
            }
        }

        static object ParseValue(Type type, string rawValue)
        {
            if (type == typeof(string))
            {
                return rawValue;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrWhiteSpace(rawValue))
                {
                    return null;
                }

                return ParseValue(type.GenericTypeArguments[0], rawValue);
            }

            if (type.IsEnum)
            {
                var value = Enum.Parse(type, rawValue);
                return value;
            }

            if (type.IsArray)
            {
                if (type == typeof(int[]))
                {
                    if (string.IsNullOrEmpty(rawValue))
                    {
                        return Array.Empty<int>();
                    }

                    using (ListPool<int>.Get(out var intPool))
                    {
                        foreach (var s in rawValue.Split(','))
                        {
                            if (int.TryParse(s, out var x))
                            {
                                intPool.Add(x);
                            }
                            else
                            {
                                Debug.LogError($"元データ{rawValue}の {s} が整数ではありません");
                            }
                        }

                        return intPool.ToArray();
                    }
                }
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    // True/False or 0,1
                    if (int.TryParse(rawValue, out var intBool))
                    {
                        return Convert.ToBoolean(intBool);
                    }

                    return bool.Parse(rawValue);
                case TypeCode.Char:
                    return char.Parse(rawValue);
                case TypeCode.SByte:
                    return sbyte.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.Byte:
                    return byte.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.Int16:
                    return short.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.UInt16:
                    return ushort.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.Int32:
                    return int.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.UInt32:
                    return uint.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.Int64:
                    return long.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.UInt64:
                    return ulong.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.Single:
                    return float.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.Double:
                    return double.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.Decimal:
                    return decimal.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.DateTime:
                    return DateTime.Parse(rawValue, CultureInfo.InvariantCulture);
                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.String:
                default:
                    if (type == typeof(DateTimeOffset))
                    {
                        return DateTimeOffset.Parse(rawValue, CultureInfo.InvariantCulture);
                    }
                    else if (type == typeof(TimeSpan))
                    {
                        return TimeSpan.Parse(rawValue, CultureInfo.InvariantCulture);
                    }
                    else if (type == typeof(Guid))
                    {
                        return Guid.Parse(rawValue);
                    }

                    // or other your custom parsing.
                    throw new NotSupportedException($"{type}はサポートしていない型です");
            }
        }
    }
}
