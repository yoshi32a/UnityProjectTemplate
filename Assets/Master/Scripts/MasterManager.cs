using System;
using Microsoft.Extensions.Logging;
using VContainer;

namespace Master;

public class MasterManager
{
    public static MemoryDatabase DB { get; private set; }
    public static ILogger<MasterManager> Logger { get; private set; }

    [Inject]
    public MasterManager(IMasterLoader loader, ILoggerFactory factory)
    {
        DB = loader.Load();
        Logger = factory.CreateLogger<MasterManager>();
    }

    public static void Rebuild()
    {

    }
}
