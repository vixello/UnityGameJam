using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Slider _loadingBar;
        [SerializeField] private Animator _animator;

        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            Deactivate();
        }

        public void SetBarPercent(float percent)
        {
            StartCoroutine(AnimateBar(percent));
        }

        private IEnumerator AnimateBar(float targetPercent)
        {
            float duration = 0.5f;
            float time = 0f;
            float initialValue = _loadingBar.value;

            while (time < duration)
            {
                _loadingBar.value = Mathf.Lerp(initialValue, targetPercent, time / duration);
                time += Time.deltaTime;
                yield return null; 
            }

            _loadingBar.value = targetPercent; 
        }

        private IEnumerator Deactivate()
        {
            _animator.Play("HideLoadingScreen");

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float animationLength = stateInfo.length;
            yield return new WaitForSeconds(animationLength);

            gameObject.SetActive(false);
        }

    }
}