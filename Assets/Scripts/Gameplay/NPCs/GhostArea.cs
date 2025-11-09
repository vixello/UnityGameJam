using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.NPCs
{
    [DefaultExecutionOrder(-10)]
    public class GhostArea : MonoBehaviour
    {
        [Header("Area Information")]
        [SerializeField] int _areaIndex;
        [SerializeField] GameObject _ghostParent;

        private int _areaGhostCount = 0;
        private GhostBehaviour[] _ghostBehaviours;
        private Transform _targetPosition;

        private void Start()
        {
            List<GhostBehaviour> ghosts = new List<GhostBehaviour>();

            foreach (Transform child in _ghostParent.transform)
            {
                GhostBehaviour gb = child.GetComponent<GhostBehaviour>();
                if (gb != null)
                    ghosts.Add(gb);
            }

            _ghostBehaviours = ghosts.ToArray();
            _areaGhostCount = _ghostBehaviours.Length;

            EventBus.OnSendTargetData += AssignTargetPosition;
            EventBus.InvokeCreatedArea(_areaGhostCount);

            Debug.Log($"GhostArea {_areaIndex} found {_areaGhostCount} ghosts under {_ghostParent.name}");
        }

        
        private void AssignTargetPosition(Transform targetPosition)
        {
            Debug.Log("Assigned target position");
            _targetPosition = targetPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Call move to target");
                for (int i =0; i < _areaGhostCount; i++) 
                {
                    if(_ghostBehaviours[i] != null && _targetPosition != null)
                    _ghostBehaviours[i].MoveToTarget(_targetPosition);
                }

                EventBus.InvokeCollectGhosts(_areaGhostCount);
            }
        }

#if UNITY_EDITOR
        [System.Obsolete]
        private void OnValidate()
        {
            // Only run in the Editor, not in Play mode
            if (!Application.isPlaying)
            {
                GhostArea[] allAreas = FindObjectsOfType<GhostArea>(true);
                foreach (var area in allAreas)
                {
                    if (area != this && area._areaIndex == _areaIndex)
                    {
                        Debug.LogWarning($"Duplicate GhostArea index {_areaIndex} found on {name} and {area.name}.");
                        _areaIndex = GetUniqueIndex(allAreas);
                        break;
                    }
                }
            }
        }

        private int GetUniqueIndex(GhostArea[] allAreas)
        {
            int newIndex = 0;
            bool exists;
            do
            {
                exists = false;
                foreach (var area in allAreas)
                {
                    if (area != this && area._areaIndex == newIndex)
                    {
                        exists = true;
                        newIndex++;
                        break;
                    }
                }
            } while (exists);
            return newIndex;
        }
#endif
    }
}
