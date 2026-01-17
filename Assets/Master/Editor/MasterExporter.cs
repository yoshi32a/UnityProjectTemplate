using System;
using System.IO;
using System.Text;
using Master.Exceptions;
using UnityEditor;
using UnityEngine;

namespace Master.Editor
{
    public static class MasterExporter
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

                var file = $"{CsvPath}{table.TableName}.csv";

                // ファイルがあったら作らない
                if (File.Exists(file))
                {
                    Debug.Log($"CSV file already exists, skipping: {file}");
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

                File.WriteAllText($"{file}", sb.ToString(), new UTF8Encoding(false));
                Debug.Log($"Created CSV file: {file}");
            }
        }

        [MenuItem("Tools/Master/CsvToBin")]
        public static void ExportBinFromCsv()
        {
            try
            {
                Initializer.SetupMessagePackResolver();

                var loader = new CsvMasterLoader();
                var database = loader.Load();

                // CsvMasterLoader内でバリデーションが行われるため、ここでは不要
                // nullが返ることはなくなった（失敗時は例外が投げられる）

                var bin = database.ToDatabaseBuilder().Build();
                File.WriteAllBytes($"{Utility.BinPath}", bin);
                Debug.Log("[Completed] Master.bin exported successfully.");

                AssetDatabase.Refresh();
            }
            catch (MasterTableNotFoundException ex)
            {
                Debug.LogError($"[Master Export Failed] Table definition not found.\n" +
                               $"Table: {ex.TableName}\n" +
                               $"Please create the [MemoryTable] class for this CSV file.\n" +
                               $"Details: {ex.Message}");
            }
            catch (MasterParseException ex)
            {
                Debug.LogError($"[Master Export Failed] Failed to parse CSV data.\n" +
                               $"Table: {ex.TableName}, Row: {ex.RowNumber}, Column: {ex.ColumnName}\n" +
                               $"Details: {ex.Message}");
            }
            catch (MasterValidationException ex)
            {
                Debug.LogError($"[Master Export Failed] Validation failed.\n" +
                               $"Details: {ex.Message}");
            }
            catch (MasterDataException ex)
            {
                Debug.LogError($"[Master Export Failed] {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Master Export Failed] Unexpected error: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
