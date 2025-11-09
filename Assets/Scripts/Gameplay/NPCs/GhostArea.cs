using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Gameplay.NPCs
{
    public class GhostArea : MonoBehaviour
    {
        [Header("Place To Move The Ghosts to")]
        [SerializeField] private Transform _targetPosition;

        [Header("Area Information")]
        [SerializeField] int _areaIndex;
        [SerializeField] GameObject _ghostParent;
        private GhostBehaviour[] _ghostBehaviours;
        private int _areaGhostCount = 0;

        private void Start()
        {
            Transform[] allChildren = _ghostParent.GetComponentsInChildren<Transform>();

            for (int i = 0; i < allChildren.Count(); ++i)
            {
                _ghostBehaviours[i] = allChildren[i].GetComponent<GhostBehaviour>();
                _areaGhostCount++;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach(GhostBehaviour ghostBehaviour in _ghostBehaviours)
                {
                    if(ghostBehaviour != null && _targetPosition != null)
                    ghostBehaviour.MoveToTarget(_targetPosition.transform.position);
                }

                EventBus.InvokeCollectGhosts(_areaGhostCount);
            }
        }

#if UNITY_EDITOR
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
