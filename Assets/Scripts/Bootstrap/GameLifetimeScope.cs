using Assets.Scripts.Gameplay;
using Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private UpdatePublisher _updatePublisher;
        [SerializeField] private AdditiveScenesManager _additiveScenesManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(_gameManager, Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_updatePublisher, Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_additiveScenesManager, Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<GameBootstrap>();
        }
    }
}
