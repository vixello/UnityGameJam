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
                float fakeDuration = 1f;
                float elapsed = 0f;
                while (elapsed < fakeDuration)
                {
                    elapsed += Time.deltaTime;
                    float progress = Mathf.Lerp(0f, 0.5f, elapsed / fakeDuration);
                    loadingScreenDisposable.SetLoadingBarPercent(progress);
                    await Task.Yield(); // let Unity update the UI each frame
                }
                await CreateObjects();
                fakeDuration = 0.5f;
                elapsed = 0f;
                while (elapsed < fakeDuration)
                {
                    elapsed += Time.deltaTime;
                    float progress = Mathf.Lerp(0.5f, 1f, elapsed / fakeDuration);
                    loadingScreenDisposable.SetLoadingBarPercent(progress);
                    await Task.Yield();
                }
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
