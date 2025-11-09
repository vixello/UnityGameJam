using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Assets.Scripts.Gameplay.NPCs
{
    internal class GhostBehaviour : MonoBehaviour
    {
        [SerializeField] private float _duration = 2.2f;   
        [SerializeField] private float _waveAmplitude = 0.5f; 
        [SerializeField] private float _waveFrequency = 2f;
        [SerializeField] private Material _ghostMaterial;

        private int _moveSpeed = 1;
        private Animator _animator;
        private Renderer _renderer;

        [SerializeField] private Color[] _colorsToChooseFrom;
        private bool _wasPickedUp = false;


        private void Start()
        {
            // Randomize color and scale
            _moveSpeed = UnityEngine.Random.Range(1, 3);

            int randomColorIndex = UnityEngine.Random.Range(0, _colorsToChooseFrom.Length);
            float randomScale = UnityEngine.Random.Range(0.8f, 1.2f);

            var mpb = new MaterialPropertyBlock();
            Color baseColor = _colorsToChooseFrom[randomColorIndex];

            _renderer = GetComponentInChildren<Renderer>();
            _renderer.GetPropertyBlock(mpb);

            mpb.SetColor("_Color", baseColor * 2.1f);
            _renderer.SetPropertyBlock(mpb);

            _animator = GetComponent<Animator>();
            if(!_animator) _animator.speed = randomScale;
            transform.localScale *= randomScale;
        }

        public void MoveToTarget(Transform target)
        {
            Debug.Log("Move to target");
            if (!_wasPickedUp) StartCoroutine(MoveCoroutine(target));
        }

        private IEnumerator MoveCoroutine(Transform target)
        {
            float elapsed = 0f;
            float threshold = 0.1f;
            Vector3 start = transform.position;

            while (true)
            {
                elapsed += _moveSpeed * Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _duration);

                Vector3 basePos = Vector3.Lerp(start, target.position, t);

                // Wave _offsetPositionForLevels, decaying as we approach the target
                float waveEase = Mathf.Sin((1 - t) * Mathf.PI * 0.5f);
                basePos.y += Mathf.Sin(t * Mathf.PI * _waveAmplitude) * _waveFrequency * waveEase;

                transform.position = basePos;

                // Check if we are close enough
                if ((transform.position - target.position).sqrMagnitude <= threshold * threshold)
                    break;

                // Also stop if duration exceeded
                if (t >= 1f)
                    break;

                yield return null;
            }

            // Ensure exact final position
            transform.position = target.position;

            StartCoroutine(Deactivate());
            _wasPickedUp = true;
        }


        private IEnumerator Deactivate()
        {
            _animator.Play("GhostHide");
            StartCoroutine(FadeOutMaterial());

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float animationLength = stateInfo.length;
            yield return new WaitForSeconds(animationLength);

            Destroy(gameObject);
        }

        private IEnumerator FadeOutMaterial()
        {
            var mpb = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(mpb);
            Color originalColor = mpb.GetColor("_Color");
            float elapsed = 1f;

            while (elapsed > 0f)
            {
                elapsed -= Time.deltaTime; 
                float t = Mathf.Clamp01(elapsed);

                mpb.SetColor("_Color", originalColor * t); 
                _renderer.SetPropertyBlock(mpb);

                yield return null;
            }

            mpb.SetColor("_Color", originalColor * 0f);
            _renderer.SetPropertyBlock(mpb);
        }
    }
}
