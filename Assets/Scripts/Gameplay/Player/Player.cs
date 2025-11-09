using System;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Gameplay
{
    public class Player : MonoBehaviour, ISaveable
    {
        [Header("Place To Move The Ghosts to")]
        [SerializeField] private Transform _ghostTargetPosition;

        private int _collectedGhosts = 0;
        private PlayerSaveData _playerSaveData;

        private void Start()
        {
            EventBus.OnCollectGhosts += UpdateCollectedGhostCount;
            EventBus.OnCustomerLost += ReactToCustomerLost;
            EventBus.OnLevelComplete += ReactToLevelComplete;
            EventBus.InvokeSendTargetData(_ghostTargetPosition);
        }

        private void OnDisable()
        {
            EventBus.OnCollectGhosts -= UpdateCollectedGhostCount;
            EventBus.OnCustomerLost -= ReactToCustomerLost;
            EventBus.OnLevelComplete -= ReactToLevelComplete;
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
