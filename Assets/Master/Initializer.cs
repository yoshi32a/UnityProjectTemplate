using Master;
using MasterMemory;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

// Optional: Unity can't load default namespace to Source Generator
// If not specified, 'MasterMemory' will be used by default,
// but you can use this attribute if you want to specify a different namespace.
[assembly: MasterMemoryGeneratorOptions(Namespace = "Master")]

// Optional: If you want to use init keyword, copy-and-paste this.
namespace System.Runtime.CompilerServices
{
    internal sealed class IsExternalInit
    {
    }
}

public static class Initializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void SetupMessagePackResolver()
    {
        // Create CompositeResolver
        StaticCompositeResolver.Instance.Register(new[]
        {
            MasterMemoryResolver.Instance, // set MasterMemory generated resolver
            StandardResolver.Instance // set default MessagePack resolver
        });

        // Create options with resolver
        var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

        // Optional: as default.
        MessagePackSerializer.DefaultOptions = options;
    }
}
