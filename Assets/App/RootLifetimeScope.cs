using Master;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App
{
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
        }
    }
}
