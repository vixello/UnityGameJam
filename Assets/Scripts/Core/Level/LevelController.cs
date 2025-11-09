using System;
using UnityEngine;
using Core;
using System.Linq;

namespace Core
{
    public class LevelController : MonoBehaviour, ISaveable
    {
        [SerializeField] private int _levelIndex = 0;
        [SerializeField] private Canvas _pauseMenu;
        [SerializeField] private Canvas _winMenu;
        [SerializeField] private Canvas _gameOver;
        [SerializeField] private Material _sky;
        [SerializeField] private float levelDuration = 3f; // 3 minutes

        private float _timer;
        private bool _isGameOver = false;

        private LevelInfo _saveData;

        private void Start()
        {
            _timer = levelDuration;
            _saveData.levelIndex = _levelIndex;

            EventBus.OnGamePause += OnPauseChanged;
            EventBus.OnSaveScore += UpdateSavedData;
        }

        private void Update()
        {
            if (_isGameOver) return;

            _timer -= Time.deltaTime;
            _timer = Mathf.Max(_timer, 0f);

            // Invoke the timer event
            EventBus.InvokeTimerUpdated(_timer);

            if (_timer <= 0f && !_isGameOver)
            {
                GameOver();
                _gameOver.gameObject.SetActive(true);
            }


            // Rotate sky and gradually brighten
            if (_sky != null)
            {
                // Rotate over time
                _sky.SetFloat("_Rotation", Time.time * 2f); // assuming your shader has _Rotation

                // Brighten color as timer decreases
                Color baseColor = Color.gray; // starting sky color
                Color targetColor = Color.white; // bright sky
                float t = 1f - (_timer / levelDuration);
                _sky.SetColor("_BaseColor", Color.Lerp(baseColor, targetColor, t)); // shader property
            }
        }

        private void GameOver()
        {
            _isGameOver = true;
            Debug.Log("Time's up! Game Over!");
            EventBus.InvokeChangeGameState(GameState.GameOver);
        }

        private void OnPauseChanged(bool isPaused)
        {
            if (_pauseMenu != null)
                _pauseMenu.gameObject.SetActive(isPaused);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EventBus.InvokeChangeGameState(GameState.LevelComplete);
                EventBus.InvokeLevelComplete();
                if (_winMenu != null) _winMenu.gameObject.SetActive(true);
            }
        }

        private void UpdateSavedData(int ghostCount)
        {
            _saveData.maxScore = _saveData.maxScore > ghostCount ? ghostCount : _saveData.maxScore;
            SaveSystem.Save();
        }

        public void SaveData(ref SaveSystem.SaveData data)
        {
            for (int i = 0; i < data.LevelSaveData.Levels.Count(); ++i)
            {
                data.LevelSaveData.Levels[i] = _saveData;
            }
        }

        public void LoadData(SaveSystem.SaveData data)
        {
            for (int i = 0; i < data.LevelSaveData.Levels.Count(); ++i)
            {
                if (data.LevelSaveData.Levels[i].levelIndex == _levelIndex)
                    _saveData = data.LevelSaveData.Levels[i];
            }
        }
    }
}
