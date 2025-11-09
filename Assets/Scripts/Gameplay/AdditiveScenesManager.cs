using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;
using Core;
using System.Linq;

namespace Assets.Scripts.Gameplay
{

    public class AdditiveScenesManager : MonoBehaviour
    {
        [Header("Scenes To Load")]
        [SerializeField] private SceneField _maineMenu;
        [SerializeField] private SceneField _loadingScene;
        private LoadingScreen _loadingScreen;

        public async Task StartGame()
        {
            //SceneManager.LoadSceneAsync(_bootstrap);
            await SceneManager.LoadSceneAsync(_maineMenu, LoadSceneMode.Additive);
        }

        /*        private IEnumerator ProgressLoadingBar()
                {
                    float loadProgress = 0f;
                    for (int i = 0; i < ScenesToLoad.Count; ++i)
                    {
                        while (!ScenesToLoad[i].isDone)
                        {
                            loadProgress += ScenesToLoad[i].progress;
                            yield return null;
                        }
                    }
                }*/

        /*        private void OnTriggerEnter2D(Collider2D collision)
                {
                    if (collision.CompareTag("PLayer"))
                    {
                        LoadScenes();
                        UnloadScenes();
                    }
                }*/

        public void RunMenuToGameTransition(SceneTransitionSO transition)
        {
            // Fire and forget (Unity doesn’t await)
            _ = RunTransition(transition);
        }

        public async Task RunTransition(SceneTransitionSO transition)
        {

            if (_loadingScene != null && !IsSceneLoaded(_loadingScene.SceneName))
            {
                await SceneManager.LoadSceneAsync(_loadingScene.SceneName, LoadSceneMode.Additive);
                _loadingScreen = await GetLoadingScreen(_loadingScene);
            }

            using(var loadingScreenDisposable = new ShowLoadingScreenDisposable(_loadingScreen))
            {
                loadingScreenDisposable.SetLoadingBarPercent(0f);

                // 2. Unload old scenes
                await UnloadScenes(transition.ScenesToUnload);

                loadingScreenDisposable.SetLoadingBarPercent(0.5f);

                // 3. Load new scenes
                await LoadScenes(transition.ScenesToLoad);

                loadingScreenDisposable.SetLoadingBarPercent(1.0f);

                if (transition.ScenesToLoad[0].SceneName.Contains("Level"))
                {
                    EventBus.InvokeChangeGameState(GameState.InGame);
                }
            }

            if (_loadingScene != null && IsSceneLoaded(_loadingScene.SceneName))
            {
                await SceneManager.UnloadSceneAsync(_loadingScene.SceneName);
            }
        }

        private async Task<LoadingScreen> GetLoadingScreen(SceneField loadingScene)
        {
            // Load the scene additively if not loaded
            if (!IsSceneLoaded(loadingScene.SceneName))
                await SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);

            // Get a handle to that scene
            Scene scene = SceneManager.GetSceneByName(loadingScene.SceneName);

            // Iterate all root GameObjects
            foreach (var rootObj in scene.GetRootGameObjects())
            {
                if (rootObj.TryGetComponent<LoadingScreen>(out var screen))
                    return screen;
            }

            Debug.LogWarning($"LoadingScreen not found in scene {loadingScene.SceneName}");
            return null;
        }


        private bool IsSceneLoaded(string sceneName)
        {
            for(int i = 0; i < SceneManager.sceneCount; i++)
            {
                if(SceneManager.GetSceneAt(i).name == sceneName)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task LoadScenes(SceneField[] _scenesToLoad)
        {
            for (int i = 0; i < _scenesToLoad.Length; ++i)
            {
                bool isSceneLoaded = false;
                for (int j = 0; j < SceneManager.sceneCount; ++j)
                {
                    Scene loadedScene = SceneManager.GetSceneAt(j);
                    if (loadedScene.name == _scenesToLoad[i].SceneName)
                    {
                        isSceneLoaded = true;
                        break;
                    }
                }

                if (!isSceneLoaded)
                {
                    var asyncOp = SceneManager.LoadSceneAsync(_scenesToLoad[i].SceneName, LoadSceneMode.Additive);
                    Debug.Log($"Loading scene: {_scenesToLoad[i].SceneName}");
                    while (!asyncOp.isDone)
                        await Task.Yield();
                }
            }
        }

        public async Task UnloadScenes(SceneField[] _scenesToUnload)
        {
            for (int i = 0; i < _scenesToUnload.Length; ++i)
            {
                for (int j = 0; j < SceneManager.sceneCount; ++j)
                {
                    Scene loadedScene = SceneManager.GetSceneAt(j);
                    if (loadedScene.name == _scenesToUnload[i].SceneName)
                    {
                        var asyncOp = SceneManager.UnloadSceneAsync(_scenesToUnload[i].SceneName);
                        Debug.Log($"Unloading scene: {_scenesToUnload[i].SceneName}");
                        while (!asyncOp.isDone)
                            await Task.Yield();
                    }
                }
            }
        }
    }
}
