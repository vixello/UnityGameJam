using System;
using UnityEngine;

namespace Core
{
    public enum GameState
    {
        Pause,
        Loading,
        InGame,
        GameOver,
        LevelComplete
    }

    public class GameManager : MonoBehaviour
    {
        private GameState _state;

        void Start()
        {
            _state = GameState.Loading;

            EventBus.OnChangeGameState += ChangeGameState;
        }

        private void ChangeGameState(GameState gameState)
        {
            if (_state != gameState)
            {
                _state = gameState;
            }
        }
    }
}
