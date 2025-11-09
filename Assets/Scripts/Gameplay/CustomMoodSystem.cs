using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Gameplay
{
    public class CustomerMoodSystem : MonoBehaviour
    {
        [Header("Mood Settings")]
        [SerializeField] private int totalCustomers = 5;
        [SerializeField] private float maxMood = 100f;
        [SerializeField] private float minMood = 0f;
        [SerializeField] private float middleMood = 50f;
        [SerializeField] private float moodDecreasePerSecond = 10f; // while sprinting
        [SerializeField] private float moodIncreasePerSecond = 5f;  // while not sprinting

        private float _currentMood;
        private int _currentCustomers;

        private bool _isSprinting;

        private void Start()
        {
            _currentMood = middleMood;
            _currentCustomers = totalCustomers;

            EventBus.OnSprintChanged += OnSprintChanged;
        }

        private void OnDestroy()
        {
            EventBus.OnSprintChanged -= OnSprintChanged;
        }

        private void OnSprintChanged(bool isSprinting)
        {
            _isSprinting = isSprinting;
        }

        private void Update()
        {
            if (_currentCustomers <= 0) return; // all customers lost, early exit

            if (_isSprinting)
            {
                _currentMood -= moodDecreasePerSecond * Time.deltaTime;
            }
            else
            {
                _currentMood += moodIncreasePerSecond * Time.deltaTime;
            }

            // Clamp mood between min and max
            _currentMood = Mathf.Clamp(_currentMood, minMood, maxMood);

            // Check if mood reached the bottom
            if (_currentMood <= minMood)
            {
                LoseCustomer();
            }
        }

        private void LoseCustomer()
        {
            _currentCustomers--;
            Debug.Log($"Customer lost! Remaining: {_currentCustomers}");

            // Reset mood to middle
            _currentMood = middleMood;

            // Optionally notify other systems
            EventBus.InvokeCustomerLost(_currentCustomers);
        }

        public float GetCurrentMood() => _currentMood;
        public int GetCurrentCustomers() => _currentCustomers;
    }
}
