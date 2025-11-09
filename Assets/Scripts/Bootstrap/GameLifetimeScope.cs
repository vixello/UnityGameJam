using Assets.Scripts.Gameplay;
using Core;
using Gameplay;
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
        [SerializeField] private InputManager _inputManagerBoat;
        [SerializeField] private AudioManager _audioManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(_gameManager, Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_updatePublisher, Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_additiveScenesManager, Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_inputManagerBoat, Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_audioManager, Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<GameBootstrap>();
        }
    }
}
