using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Master.Editor
{
    /// <summary>
    /// CSVファイルからテーブル定義クラスを自動生成するエディタツール
    /// </summary>
    public class TableDefinitionGenerator : EditorWindow
    {
        string selectedCsvPath;
        GeneratedTableInfo preview;
        Vector2 scrollPosition;
        bool useInitAccessor = true;
        int sampleRowCount = 10;

        [MenuItem("Tools/Master/Generate Table Definition from CSV")]
        public static void ShowWindow()
        {
            GetWindow<TableDefinitionGenerator>("Table Generator");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("CSV to C# Table Definition Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // オプション
            useInitAccessor = EditorGUILayout.Toggle("Use { get; init; }", useInitAccessor);
            sampleRowCount = EditorGUILayout.IntSlider("Sample Rows for Type Detection", sampleRowCount, 1, 100);

            EditorGUILayout.Space();

            // CSV選択
            if (GUILayout.Button("Select CSV File", GUILayout.Height(30)))
            {
                var initialPath = "Assets/Master/CSV";
                selectedCsvPath = EditorUtility.OpenFilePanel("Select CSV", initialPath, "csv");
                if (!string.IsNullOrEmpty(selectedCsvPath))
                {
                    preview = AnalyzeCsv(selectedCsvPath);
                }
            }

            if (!string.IsNullOrEmpty(selectedCsvPath))
            {
                EditorGUILayout.LabelField("Selected:", selectedCsvPath);
            }

            EditorGUILayout.Space();

            if (preview != null)
            {
                DrawPreview();

                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Generate C# File", GUILayout.Height(30)))
                    {
                        GenerateTableClass(preview);
                    }

                    if (GUILayout.Button("Copy to Clipboard", GUILayout.Height(30)))
                    {
                        var code = GenerateCode(preview);
                        EditorGUIUtility.systemCopyBuffer = code;
                        Debug.Log("Generated code copied to clipboard.");
                    }
                }
            }
        }

        void DrawPreview()
        {
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"Table Name: {preview.TableName}");
                EditorGUILayout.LabelField($"Class Name: {preview.ClassName}");
                EditorGUILayout.LabelField($"Properties: {preview.Properties.Count}");

                EditorGUILayout.Space();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(200));

                foreach (var prop in preview.Properties)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var keyLabel = prop.IsPrimaryKey ? "[PK] " : "";
                        EditorGUILayout.LabelField($"{keyLabel}{prop.PropertyName}", GUILayout.Width(150));
                        prop.TypeName = EditorGUILayout.TextField(prop.TypeName, GUILayout.Width(100));
                        prop.IsPrimaryKey = EditorGUILayout.Toggle("PK", prop.IsPrimaryKey, GUILayout.Width(40));
                    }
                }

                EditorGUILayout.EndScrollView();
            }
        }

        GeneratedTableInfo AnalyzeCsv(string path)
        {
            var tableName = Path.GetFileNameWithoutExtension(path);
            var className = ToPascalCase(tableName);

            var lines = File.ReadAllLines(path);
            if (lines.Length == 0)
            {
                Debug.LogError("CSV file is empty.");
                return null;
            }

            var headers = ParseCsvLine(lines[0]);
            var sampleRows = new List<string[]>();

            for (var i = 1; i < Math.Min(lines.Length, sampleRowCount + 1); i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    sampleRows.Add(ParseCsvLine(lines[i]));
                }
            }

            var properties = new List<GeneratedPropertyInfo>();
            var hasFloat3 = false;

            for (var i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                var propName = ToPascalCase(header);

                var sampleValues = sampleRows
                    .Where(r => i < r.Length)
                    .Select(r => r[i])
                    .ToList();

                var typeName = InferType(sampleValues, out var isFloat3);
                if (isFloat3)
                {
                    hasFloat3 = true;
                }

                var isPk = header.ToLower() == "id" ||
                           (i == 0 && header.ToLower().EndsWith("_id"));

                properties.Add(new GeneratedPropertyInfo
                {
                    ColumnName = header,
                    PropertyName = propName,
                    TypeName = typeName,
                    IsPrimaryKey = isPk,
                    KeyOrder = isPk ? properties.Count(p => p.IsPrimaryKey) : 0
                });
            }

            return new GeneratedTableInfo
            {
                TableName = tableName,
                ClassName = className,
                Properties = properties,
                HasCompositeKey = properties.Count(p => p.IsPrimaryKey) > 1,
                UsesFloat3 = hasFloat3
            };
        }

        string InferType(List<string> values, out bool isFloat3)
        {
            isFloat3 = false;

            if (values.Count == 0 || values.All(string.IsNullOrEmpty))
            {
                return "string";
            }

            var nonEmptyValues = values.Where(v => !string.IsNullOrEmpty(v)).ToList();

            // float3 チェック (x,y,z 形式)
            if (nonEmptyValues.All(v => IsFloat3Format(v)))
            {
                isFloat3 = true;
                return "float3";
            }

            // int[] チェック (カンマ区切りの整数)
            if (nonEmptyValues.All(v => IsIntArrayFormat(v)))
            {
                return "int[]";
            }

            // float[] チェック (カンマ区切りの小数)
            if (nonEmptyValues.All(v => IsFloatArrayFormat(v)))
            {
                return "float[]";
            }

            // bool チェック
            if (nonEmptyValues.All(v => v == "0" || v == "1" ||
                                        v.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                        v.Equals("false", StringComparison.OrdinalIgnoreCase)))
            {
                return "bool";
            }

            // int チェック
            if (nonEmptyValues.All(v => int.TryParse(v, out _)))
            {
                return "int";
            }

            // float チェック
            if (nonEmptyValues.All(v => float.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out _)))
            {
                return "float";
            }

            return "string";
        }

        bool IsFloat3Format(string value)
        {
            var cleaned = value.Trim('"', '[', ']', ' ');
            var parts = cleaned.Split(',');
            if (parts.Length != 3)
            {
                return false;
            }

            return parts.All(p => float.TryParse(p.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out _));
        }

        bool IsIntArrayFormat(string value)
        {
            if (!value.Contains(','))
            {
                return false;
            }

            var parts = value.Split(',');
            // 3つの要素でfloat3形式の場合は除外
            if (parts.Length == 3 && IsFloat3Format(value))
            {
                return false;
            }

            return parts.All(p => int.TryParse(p.Trim(), out _));
        }

        bool IsFloatArrayFormat(string value)
        {
            if (!value.Contains(','))
            {
                return false;
            }

            var parts = value.Split(',');
            // 3つの要素でfloat3形式の場合は除外
            if (parts.Length == 3 && IsFloat3Format(value))
            {
                return false;
            }

            return parts.All(p => float.TryParse(p.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out _));
        }

        void GenerateTableClass(GeneratedTableInfo info)
        {
            var code = GenerateCode(info);
            var outputPath = $"Assets/Master/Scripts/{info.ClassName}.cs";

            // 既存ファイルがある場合は確認
            if (File.Exists(outputPath))
            {
                if (!EditorUtility.DisplayDialog("Overwrite?",
                    $"File already exists: {outputPath}\nOverwrite?", "Yes", "No"))
                {
                    return;
                }
            }

            File.WriteAllText(outputPath, code, Encoding.UTF8);
            AssetDatabase.Refresh();
            Debug.Log($"Generated: {outputPath}");
        }

        string GenerateCode(GeneratedTableInfo info)
        {
            var sb = new StringBuilder();
            var accessor = useInitAccessor ? "init" : "private set";

            sb.AppendLine("using MasterMemory;");
            sb.AppendLine("using MessagePack;");
            if (info.UsesFloat3)
            {
                sb.AppendLine("using Unity.Mathematics;");
            }

            sb.AppendLine();
            sb.AppendLine("namespace Master");
            sb.AppendLine("{");
            sb.AppendLine($"    [MemoryTable(\"{info.TableName}\"), MessagePackObject]");
            sb.AppendLine($"    public sealed class {info.ClassName}");
            sb.AppendLine("    {");

            var keyIndex = 0;
            foreach (var prop in info.Properties)
            {
                var attributes = new List<string>();

                if (prop.IsPrimaryKey)
                {
                    if (info.HasCompositeKey)
                    {
                        attributes.Add($"PrimaryKey(keyOrder: {prop.KeyOrder})");
                    }
                    else
                    {
                        attributes.Add("PrimaryKey");
                    }
                }

                attributes.Add($"Key({keyIndex++})");

                sb.AppendLine($"        [{string.Join("] [", attributes)}]");
                sb.AppendLine($"        public {prop.TypeName} {prop.PropertyName} {{ get; {accessor}; }}");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        static string ToPascalCase(string snakeCase)
        {
            return string.Join("", snakeCase.Split('_')
                .Select(s => char.ToUpper(s[0]) + s.Substring(1).ToLower()));
        }

        static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var inQuotes = false;
            var current = new StringBuilder();

            foreach (var c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString().Trim());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString().Trim());
            return result.ToArray();
        }
    }
}
