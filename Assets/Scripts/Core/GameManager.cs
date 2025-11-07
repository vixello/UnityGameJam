using UnityEngine;

namespace Core
{
    public enum GameState
    {
        Pause,
        Loading,
        Start
    }
    public class GameManager : MonoBehaviour
    {
        private GameState _state;

        void Start()
        {
            _state = GameState.Loading;
        }
    }
}
