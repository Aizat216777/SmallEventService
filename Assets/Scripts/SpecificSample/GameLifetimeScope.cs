using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Project 
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private GameConfig gameConfig;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SmallEventService>(Lifetime.Singleton)
                .As<IEventService>();
            builder.Register<DummyServer>(Lifetime.Singleton)
                .As<IServer>();
            builder.RegisterInstance(gameConfig.serviceConfig);
        }
    }
}

