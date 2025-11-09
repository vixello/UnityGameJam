using System;
using UnityEngine;

namespace Core
{
    public class EventBus
    {
        public delegate void GamePause(bool isPaused);
        public static event GamePause OnGamePause;
        private static bool _isPaused = false; // Track current pause state

        public static void InvokeGamePauseToggle()
        {
            _isPaused = !_isPaused;
            OnGamePause?.Invoke(_isPaused);
        }

        public delegate void ThrottleChanged(float value);
        public static event ThrottleChanged OnThrottleChanged;
        public static void InvokeThrottleChanged(float value) => OnThrottleChanged?.Invoke(value);

        public delegate void SteeringChanged(float value);
        public static event SteeringChanged OnSteeringChanged;
        public static void InvokeSteeringChanged(float value) => OnSteeringChanged?.Invoke(value);

        public delegate void SprintChanged(bool isSprinting);
        public static event SprintChanged OnSprintChanged;

        public static void InvokeSprintChanged(bool isSprinting) => OnSprintChanged?.Invoke(isSprinting);


        public delegate void SendTargetData(Transform targetPosition);
        public static event SendTargetData OnSendTargetData;
        public static void InvokeSendTargetData(Transform targetPosition)
        {
            OnSendTargetData?.Invoke(targetPosition);
        }

        public delegate void CollectGhosts(int ghostConnt);
        public static event CollectGhosts OnCollectGhosts;
        public static void InvokeCollectGhosts(int ghostCount)
        { 
            OnCollectGhosts?.Invoke(ghostCount);
        }

        public delegate void LevelComplete();
        public static event LevelComplete OnLevelComplete;
        public static void InvokeLevelComplete()
        {
            OnLevelComplete?.Invoke();
        }

        public delegate void SaveScore(int ghostCount);
        public static event SaveScore OnSaveScore;
        public static void InvokeSaveScore(int ghostCount)
        {
            OnSaveScore?.Invoke(ghostCount);
        }

        public delegate void ChangeGameState(GameState gameState);
        public static event ChangeGameState OnChangeGameState;

        public static void InvokeChangeGameState(GameState gameState)
        {
            OnChangeGameState?.Invoke(gameState);
        }

        public delegate void CustomerLost(int currentCustomers);
        public static event CustomerLost OnCustomerLost;

        public static void InvokeCustomerLost(int currentCustomers)
        {
            OnCustomerLost?.Invoke(currentCustomers);
        }
    }
}
