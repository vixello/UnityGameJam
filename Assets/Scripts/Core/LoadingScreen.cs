using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Slider _loadingBar;

        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
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
    }
}