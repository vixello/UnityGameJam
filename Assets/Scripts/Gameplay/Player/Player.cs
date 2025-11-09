using System;
using System.Collections.Generic;
using UnityEngine;
using Core;
using TMPro;

namespace Gameplay
{
    public class Player : MonoBehaviour, ISaveable
    {
        [Header("Place To Move The Ghosts to")]
        [SerializeField] private Transform _ghostTargetPosition;
        [SerializeField] private TextMeshProUGUI _updatefinish;

        private int _collectedGhosts = 0;
        private PlayerSaveData _playerSaveData;
        private int _ghostCountPerLevel = 0;

        private void Start()
        {
            EventBus.OnCollectGhosts += UpdateCollectedGhostCount;
            EventBus.OnCustomerLost += ReactToCustomerLost;
            EventBus.OnLevelComplete += ReactToLevelComplete;
            EventBus.InvokeSendTargetData(_ghostTargetPosition);
            EventBus.OnCreatedArea += UpdateGhostCountPerLevel;
            EventBus.OnChangeGameState += React;
        }

        private void React(GameState gameState)
        {
            if(gameState == GameState.GameOver)
            {
                _updatefinish.text = $"{_collectedGhosts}/{_ghostCountPerLevel}";
            }
        }

        private void OnDisable()
        {
            EventBus.OnCollectGhosts -= UpdateCollectedGhostCount;
            EventBus.OnCustomerLost -= ReactToCustomerLost;
            EventBus.OnLevelComplete -= ReactToLevelComplete;
            EventBus.OnCreatedArea -= UpdateGhostCountPerLevel;
        }
        private void UpdateGhostCountPerLevel(int ghostCount)
        {
            _ghostCountPerLevel += ghostCount;
            Debug.Log("Ghost count per level: " + _ghostCountPerLevel);
        }

        private void ReactToLevelComplete()
        {
            EventBus.InvokeSaveScore(_collectedGhosts);
        }

        private void UpdateCollectedGhostCount(int ghostCount)
        {
            _collectedGhosts += ghostCount;
            EventBus.InvokePointsUpdated(_collectedGhosts);
        }

        private void ReactToCustomerLost(int currentCustomers)
        {
            _collectedGhosts = currentCustomers;
            EventBus
                .InvokePointsUpdated(_collectedGhosts);
        }
        public void LoadData(SaveSystem.SaveData data)
        {
            data.PlayerSaveData  = _playerSaveData;
        }

        public void SaveData(ref SaveSystem.SaveData data)
        {
            _playerSaveData = data.PlayerSaveData;
        }
    }
}
