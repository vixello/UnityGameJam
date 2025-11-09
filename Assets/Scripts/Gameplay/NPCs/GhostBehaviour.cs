using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Assets.Scripts.Gameplay.NPCs
{
    internal class GhostBehaviour : MonoBehaviour
    {
        [SerializeField] private float _duration = 2f;   
        [SerializeField] private float _waveAmplitude = 0.5f; 
        [SerializeField] private float _waveFrequency = 2f;
        private bool _wasPickedUp = false;

        /*        private Vector3 _startPosition;
                private float _elapsed = 0f;
                private bool moving = false;

                private void Start()
                {
                    _startPosition = transform.position;
                }
        */

        public void MoveToTarget(Vector3 target)
        {
            if(!_wasPickedUp) StartCoroutine(MoveCoroutine(target));
        }

        private IEnumerator MoveCoroutine(Vector3 target)
        {
            Vector3 start = transform.position;
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _duration);

                Vector3 basePos = Vector3.Lerp(start, target, t);

                // Add sine wave on Y-axis
                basePos.y += Mathf.Sin(t * Mathf.PI * _waveAmplitude) * _waveFrequency;

                transform.position = basePos;

                yield return null; 
            }

            transform.position = target;
            _wasPickedUp = true;
        }
    }
}
