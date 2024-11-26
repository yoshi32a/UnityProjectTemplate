using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using ZLogger.Unity;

public class Test : MonoBehaviour
{
    ILogger<Test> logger;
    void Start()
    {
        var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddZLoggerUnityDebug(); // log to UnityDebug
        });

        logger = loggerFactory.CreateLogger<Test>();

        var name = "foo";
        logger.ZLogInformation($"Hello, {name}!");
        
        SampleAsync().Forget();
        
    }

    async UniTask SampleAsync()
    {
        var name = "foo";
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        logger.ZLogInformation($"async");
    }
    
}
