using App;
using Master;
using Microsoft.Extensions.Logging;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ZLogger;
using ZLogger.Unity;

namespace App
{
public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        Application.targetFrameRate = 60;

#if UNITY_EDITOR
        if (Application.isEditor)
        {
            builder.Register<IMasterLoader, CsvMasterLoader>(Lifetime.Singleton);
        }
        else
#endif
        {
            builder.Register<IMasterLoader, MasterLoader>(Lifetime.Singleton);
        }

        builder.Register<MasterManager>(Lifetime.Singleton);

        var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddZLoggerUnityDebug(option =>
            {
                option.UsePlainTextFormatter(formatter =>
                {
                    formatter.SetPrefixFormatter($"[{0}]", (in MessageTemplate template, in LogInfo info) => template.Format(info.Category));
                });
            }); // log to UnityDebug
        });

        builder.RegisterInstance(loggerFactory);
        builder.RegisterInstance(loggerFactory.CreateLogger("default"));

        builder.RegisterBuildCallback(container =>
        {
            container.Resolve<MasterManager>();
        });
    }
}
}
