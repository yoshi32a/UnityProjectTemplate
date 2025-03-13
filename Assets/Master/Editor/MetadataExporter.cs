using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

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

                var file = $"{CsvPath}{table.TableName}.csv";

                // ファイルがあったら一旦作らない
                if (File.Exists(file))
                {
                    Debug.Log($"CSVファイルを作りませんでした {file}");
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
                Debug.Log($"csvファイルを作りました {file}");
            }
        }

        [MenuItem("Tools/Master/CsvToBin")]
        public static void CsvToBin()
        {
            var loader = new CsvMasterLoader();
            var database = loader.Load();

            if (database == null)
            {
                return;
            }

            var bin = database.ToDatabaseBuilder().Build();
            File.WriteAllBytes($"{Utility.BinPath}", bin);
            Debug.Log("[Completed] Master.bin exported.");

            AssetDatabase.ImportAsset(Utility.BinPath);
        }
    }
}
