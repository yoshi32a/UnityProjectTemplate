using System;
using Microsoft.Extensions.Logging;
using VContainer;

namespace Master;

public class MasterManager
{
    readonly IMasterLoader loader;

    public static MemoryDatabase DB { get; private set; }
    public static ILogger<MasterManager> Logger { get; private set; }

    [Inject]
    public MasterManager(IMasterLoader loader, ILoggerFactory factory)
    {
        this.loader = loader;
        DB = loader.Load();
        Logger = factory.CreateLogger<MasterManager>();
    }

    /// <summary>
    /// マスタデータを再読み込みする（エディタ専用）
    /// </summary>
    public void Rebuild()
    {
#if UNITY_EDITOR
        try
        {
            Logger?.LogInformation("Rebuilding master data...");
            var newDb = loader.Load();
            if (newDb != null)
            {
                DB = newDb;
                Logger?.LogInformation("Master data rebuilt successfully.");
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to rebuild master data");
            throw;
        }
#else
            Logger?.LogWarning("Rebuild is only available in Editor mode.");
#endif
    }
}
