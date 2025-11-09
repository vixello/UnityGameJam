using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class EventBus
    {
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
    }
}
