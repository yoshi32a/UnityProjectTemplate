using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Master;
using Microsoft.Extensions.Logging;
using R3;
using UnityEngine;
using VContainer;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public class Test : MonoBehaviour
{
    ILogger<Test> logger;
    [Inject]
    ILogger log;

    [Inject]
    public void Construct(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<Test>();
    }

    void Start()
    {
        var value = "foo";
        logger.ZLogInformation($"Hello, {value}!");
        log.ZLogInformation($"log hello");

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
