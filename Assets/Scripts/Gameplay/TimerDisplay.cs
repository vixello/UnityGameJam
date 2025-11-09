namespace Gameplay
{
    using UnityEngine;
    using TMPro;
    using Core;

    public class TimerDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private Color _timerFlash;

        private void OnEnable()
        {
            EventBus.OnTimerUpdated += UpdateTimerText;
        }

        private void OnDisable()
        {
            EventBus.OnTimerUpdated -= UpdateTimerText;
        }

        private void UpdateTimerText(float timeRemaining)
        {
            if (_timerText == null) return;
            if (timeRemaining <= 30f)
            {
                _timerText.color = _timerFlash;
            }


            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            _timerText.text = $"Time - {minutes:00}:{seconds:00}";
        }
    }
}
