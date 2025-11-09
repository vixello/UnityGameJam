using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class LevelController : MonoBehaviour, ISaveable
    {
        [SerializeField] int _levelIndex = 0;
        [SerializeField] private Canvas _pauseMenu;
        [SerializeField] private Canvas _winMenu;
        [SerializeField] private Material _sky;
        private LevelInfo _saveData;

        private void Start()
        {
            _saveData.levelIndex = _levelIndex;
            EventBus.OnGamePause += OnPauseChanged;
            EventBus.OnSaveScore += UpdateSavedData;
        }

        private void OnPauseChanged(bool isPaused)
        {
            _pauseMenu.gameObject.SetActive(isPaused);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EventBus.InvokeChangeGameState(GameState.LevelComplete);
                EventBus.InvokeLevelComplete();
                _winMenu.gameObject.SetActive(true);
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
