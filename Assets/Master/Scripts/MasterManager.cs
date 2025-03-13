using VContainer;

namespace Master;

public class MasterManager
{
    public MemoryDatabase DB { get; }

    [Inject]
    public MasterManager(IMasterLoader loader)
    {
        DB = loader.Load();
    }
}
