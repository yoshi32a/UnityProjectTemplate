using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Master;
using Microsoft.Extensions.Logging;
using R3;
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


        if (File.Exists(Utility.BinPath))
        {
            MemoryDatabase db = new MemoryDatabase(File.ReadAllBytes(Utility.BinPath), maxDegreeOfParallelism: 6);
        }
    }

    async UniTask SampleAsync()
    {
        var name = "foo";
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        logger.ZLogInformation($"async");

        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => logger.ZLogInformation($"async2")).AddTo(this);
    }
}
