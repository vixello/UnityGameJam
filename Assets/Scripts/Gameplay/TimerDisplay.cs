namespace Gameplay
{
    using UnityEngine;
    using TMPro;
    using Core;

    public class TimerDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;

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
            if (timerText == null) return;

            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"Time - {minutes:00}:{seconds:00}";
        }
    }
}
