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
            EventBus.OnCustomerLost += ChangeCollectedGhostNumber;
            EventBus.OnLevelComplete += ReactToLevelComplete;
            EventBus.InvokeSendTargetData(_ghostTargetPosition);
        }

        private void OnDisable()
        {
            EventBus.OnCollectGhosts -= UpdateCollectedGhostCount;
            EventBus.OnCustomerLost -= ChangeCollectedGhostNumber;
            EventBus.OnLevelComplete -= ReactToLevelComplete;
        }

        private void ChangeCollectedGhostNumber(int currentCustomers)
        {
            _collectedGhosts = currentCustomers;
        }

        private void ReactToLevelComplete()
        {
            EventBus.InvokeSaveScore(_collectedGhosts);
        }

        private void UpdateCollectedGhostCount(int ghostCount)
        {
            _collectedGhosts += ghostCount;
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
