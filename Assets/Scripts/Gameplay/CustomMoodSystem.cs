using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using UnityEngine.UI;
using System;

namespace Gameplay
{
    public class CustomerMoodSystem : MonoBehaviour
    {
        [Header("Mood Settings")]
        [SerializeField] private float maxMood = 100f;
        [SerializeField] private float minMood = 0f;
        [SerializeField] private float middleMood = 50f;
        [SerializeField] private float moodDecreasePerSecond = 10f; // while sprinting
        [SerializeField] private float moodIncreasePerSecond = 5f;  // while not sprinting
        [SerializeField] private float flashThreshold = 15f; // distance to minMood to start flashing
        [SerializeField] private float flashSpeed = 5f;      // how fast the flash loops

        [Header("UI")]
        [SerializeField] private Slider moodSlider;
        [SerializeField] private Color lowMoodColor = Color.red;
        [SerializeField] private Color midMoodColor = Color.yellow;
        [SerializeField] private Color highMoodColor = Color.green;

        private Image _fillImage;

        private float _currentMood;
        private int _currentCustomers = 0;

        private bool _isSprinting;

        private void Start()
        {
            _currentMood = maxMood;

            EventBus.OnSprintChanged += OnSprintChanged;
            EventBus.OnPointsUpdated += UpdateTotalCustomers;

            if (moodSlider != null)
            {
                moodSlider.minValue = 0;
                moodSlider.maxValue = maxMood;
                moodSlider.value = _currentMood;

                _fillImage = moodSlider.fillRect.GetComponent<Image>();
                UpdateMoodColor();
            }
        }

        private void OnDestroy()
        {
            EventBus.OnSprintChanged -= OnSprintChanged;
            EventBus.OnPointsUpdated -= UpdateTotalCustomers;
        }

        private void OnSprintChanged(bool isSprinting)
        {
            _isSprinting = isSprinting;
        }

        private void UpdateTotalCustomers(int currentCustomers)
        {
            _currentCustomers = currentCustomers;
        }

        private void Update()
        {
            if (_currentCustomers <= 0) return; // all customers lost

            // Update mood
            _currentMood += (_isSprinting ? -moodDecreasePerSecond : moodIncreasePerSecond) * Time.deltaTime;
            _currentMood = Mathf.Clamp(_currentMood, minMood, maxMood);

            // Check if mood reached minimum
            if (_currentMood <= minMood)
            {
                LoseCustomer();
            }

            // Update slider
            if (moodSlider != null)
            {
                moodSlider.value = _currentMood;
                UpdateMoodColor();
            }
        }

        private void LoseCustomer()
        {
            _currentCustomers--;
            Debug.Log($"Customer lost! Remaining: {_currentCustomers}");

            // Reset mood
            _currentMood = middleMood;

            EventBus.InvokeCustomerLost(_currentCustomers);
        }

        private void UpdateMoodColor()
        {
            if (_fillImage == null) return;

            float t = (_currentMood - minMood) / (maxMood - minMood);
            Color baseColor;

            if (t < 0.5f)
            {
                // Blend low → mid
                baseColor = Color.Lerp(lowMoodColor, midMoodColor, t * 2f);
            }
            else
            {
                // Blend mid → high
                baseColor = Color.Lerp(midMoodColor, highMoodColor, (t - 0.5f) * 2f);
            }

            // Flash effect when near minimum mood
            if (_currentMood <= minMood + flashThreshold)
            {
                float flash = (Mathf.Sin(Time.time * flashSpeed) * 0.5f) + 0.5f; // 0 → 1
                Color brightFlash = Color.Lerp(baseColor, Color.white, flash * 0.5f); // flash toward white
                _fillImage.color = brightFlash;
            }
            else
            {
                _fillImage.color = baseColor;
            }
        }

        public float GetCurrentMood() => _currentMood;
        public int GetCurrentCustomers() => _currentCustomers;
    }
}
