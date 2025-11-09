using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class PointsCounter : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI pointsText;

        private int _points;

        private void Awake()
        {
            EventBus.OnPointsUpdated += UpdatePointsText;
        }

        private void OnDisable()
        {
            EventBus.OnPointsUpdated -= UpdatePointsText;
        }

        private void UpdatePointsText(int collectedGhosts)
        {
            _points = collectedGhosts;
            if (pointsText != null)
                pointsText.text = $"{_points}";
        }
    }
}
