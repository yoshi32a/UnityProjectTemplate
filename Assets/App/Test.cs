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

        var value = "foo";
        logger.ZLogInformation($"Hello, {value}!");

        SampleAsync().Forget();

        if (File.Exists(Utility.BinPath))
        {
            IMasterLoader masterLoader = new CsvMasterLoader();
            MemoryDatabase db = masterLoader.Load();

            Debug.Log("load master success");
        }
    }

    async UniTask SampleAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        logger.ZLogInformation($"async");

        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => logger.ZLogInformation($"async2")).AddTo(this);
    }
}
