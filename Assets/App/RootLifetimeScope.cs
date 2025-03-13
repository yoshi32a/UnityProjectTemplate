using Master;
using Microsoft.Extensions.Logging;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ZLogger;
using ZLogger.Unity;

namespace App;

public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
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

        var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddZLoggerUnityDebug(option =>
            {
                option.UsePlainTextFormatter(formatter =>
                {
                    formatter.SetPrefixFormatter($"[{0}]", (in MessageTemplate template, in LogInfo info) => template.Format(info.Category));
                });
                option.PrettyStacktrace = true;
                option.IsFormatLogImmediatelyInStandardLog = true;
            }); // log to UnityDebug
        });

        builder.RegisterInstance(loggerFactory);
        builder.RegisterInstance(loggerFactory.CreateLogger("default"));
    }
}
