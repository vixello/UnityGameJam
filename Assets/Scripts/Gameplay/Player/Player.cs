using System;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Gameplay
{
    public class Player : MonoBehaviour, ISaveable
    {
        private int _colletedGhosts = 0;
        private PlayerSaveData _playerSaveData;

        private void Start()
        {
            EventBus.OnCollectGhosts += UpdateCollectedGhostCount;
            EventBus.OnLevelComplete += ReactToLevelComplete;
        }

        private void ReactToLevelComplete()
        {
            EventBus.InvokeSaveScore(_colletedGhosts);
        }

        private void UpdateCollectedGhostCount(int ghostConnt)
        {
            _colletedGhosts += ghostConnt;
        }

        public void LoadData(SaveSystem.SaveData data)
        {
            throw new NotImplementedException();
        }

        public void SaveData(ref SaveSystem.SaveData data)
        {
            _playerSaveData = data.PlayerSaveData;
        }
    }
}
