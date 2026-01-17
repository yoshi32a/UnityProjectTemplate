using Master;
using MasterMemory;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using UnityEngine;

// Optional: Unity can't load default namespace to Source Generator
// If not specified, 'MasterMemory' will be used by default,
// but you can use this attribute if you want to specify a different namespace.
[assembly: MasterMemoryGeneratorOptions(
    Namespace = "Master",
    IsReturnNullIfKeyNotFound = false)]

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
        // Custom resolvers
        var customResolvers = new IMessagePackFormatter[]
        {
            new Master.Formatters.Float3Formatter()
        };

        // Create CompositeResolver
        var resolver = MessagePack.Resolvers.CompositeResolver.Create(
            customResolvers, // custom formatters
            new[] { MasterMemoryResolver.Instance, StandardResolver.Instance } // resolvers
        );

        // Create options with resolver
        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

        // Optional: as default.
        MessagePackSerializer.DefaultOptions = options;
    }
}
