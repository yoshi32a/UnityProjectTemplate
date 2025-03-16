using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Master;
using Microsoft.Extensions.Logging;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using VContainer;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace App
{
public class Test : MonoBehaviour
{
    ILogger<Test> logger;
    [Inject] ILogger log;
    [SerializeField] AssetReference reference;

    [Inject]
    public void Construct(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<Test>();
        // reference.LoadAssetAsync<>()
        reference.ReleaseAsset();
    }

    void Start()
    {
        var value = "foo";
        logger.ZLogInformation($"Hello, {value}!");
        log.ZLogInformation($"log hello");

        logger.ZLogInformation($"{MasterManager.DB.ItemTable.FindByItemId(1).Content.Type}");

        var localize = MasterManager.DB.PersonTable.FindByPersonId(10).Localize();
        logger.ZLogDebug($"value={localize.Value}");

        SampleAsync().Forget();
    }

    async UniTask SampleAsync()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(reference);
        logger.ZLogDebug($"load start");
        var go = await handle.WithCancellation(destroyCancellationToken);
        logger.ZLogDebug($"load finish {go}");
        var result = await InstantiateAsync(go, 10).WithCancellation(destroyCancellationToken);
        logger.ZLogDebug($"InstantiateAsync finish {result.Length}");

        Addressables.ReleaseInstance(handle);

        await UniTask.Delay(TimeSpan.FromSeconds(2));
        logger.ZLogInformation($"async");

        Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => logger.ZLogInformation($"async2")).AddTo(this);
    }
}
}
