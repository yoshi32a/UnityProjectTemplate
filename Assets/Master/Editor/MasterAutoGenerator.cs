using UnityEditor;
using UnityEngine;

namespace Master.Editor
{
    public class MasterAutoGenerator : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var needsRebuild = false;

            foreach (var str in importedAssets)
            {
                if (!IsMasterCsv(str))
                {
                    continue;
                }

                needsRebuild = true;
                Debug.Log($"[MasterAutoGenerator] CSV Modified: {str}");
                break;
            }

            if (!needsRebuild)
            {
                foreach (var str in deletedAssets)
                {
                    if (!IsMasterCsv(str))
                    {
                        continue;
                    }

                    needsRebuild = true;
                    Debug.Log($"[MasterAutoGenerator] CSV Deleted: {str}");
                    break;
                }
            }

            switch (needsRebuild)
            {
                case false:
                    return;
                default:
                    Debug.Log("[MasterAutoGenerator] Rebuilding Master Memory Binary...");
                    MasterExporter.ExportBinFromCsv();
                    break;
            }
        }

        static bool IsMasterCsv(string path)
        {
            return path.Contains("Assets/Master/CSV/") && path.EndsWith(".csv");
        }
    }
}
