using System.IO;
using UnityEngine.Scripting;

namespace Master;

/// <summary>本番用マスタ読み込み</summary>
[Preserve]
public class MasterLoader : IMasterLoader
{
    public MemoryDatabase Load()
    {
        var db = new MemoryDatabase(File.ReadAllBytes(Utility.BinPath), maxDegreeOfParallelism: 6);
        return db;
    }
}
