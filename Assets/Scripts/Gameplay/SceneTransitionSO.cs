using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    [CreateAssetMenu(fileName = "SceneTransition", menuName = "ScriptableObjects/SceneTransition", order = 1)]
    public class SceneTransitionSO : ScriptableObject
    {
        [Header("Scenes To Load")]
        public SceneField[] ScenesToLoad;
        [Header("Scenes To Unload")]
        public SceneField[] ScenesToUnload;
    }
}
