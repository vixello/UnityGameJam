using Assets.Scripts.Gameplay;
using Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Bindable Objects")]
        [SerializeField] List<Object> _bindables;
        [SerializeField] LoadingScreen _loadingScreen;

        [Inject] private readonly GameManager _gameManager;
        [Inject] private readonly UpdatePublisher _updatePublisher;
        [Inject] private readonly AdditiveScenesManager _additiveScenesManager;

        private async void Start()
        {
            BindObjects();

            using (var loadingScreenDisposable =
                new ShowLoadingScreenDisposable(_loadingScreen))
            {
                loadingScreenDisposable.SetLoadingBarPercent(0f);
                await InitializeObjects();
                loadingScreenDisposable.SetLoadingBarPercent(0.33f);
                await CreateObjects();
                loadingScreenDisposable.SetLoadingBarPercent(0.66f);
                await PrepareGame();
                loadingScreenDisposable.SetLoadingBarPercent(1.0f);
            }

            await BeginGame();
        }

        private void BindObjects()
        {
            foreach(IBindableObject bindable in _bindables)
            {
                bindable.BindObject();
            }
        }

        private async Task InitializeObjects()
        {

        }

        private async Task CreateObjects()
        {

        }

        private async Task PrepareGame()
        {

        }

        private async Task BeginGame()
        {
            if (_additiveScenesManager != null)
            {
                await _additiveScenesManager.StartGame();
            }
            else
            {
                Debug.LogError("AdditiveScenesManager is not initialized.");
            }

        }
    }

    public interface IInitializable
    {
        System.Collections.IEnumerator InitializeCoroutine();
    }

    public interface IInjectable
    {
        //void Inject(GameContext context);
    }
}
