using UnityEngine;
using Core;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoatController : MonoBehaviour
    {
        [Header("Buoyancy")]
        [SerializeField] private Transform[] buoyancyPoints;
        [SerializeField] private float buoyancyForce = 10f;
        [SerializeField] private float waterLevel = 0f;
        [SerializeField] private float damping = 0.9f;

        [Header("Boat Movement")]
        [SerializeField] private Transform motorPosition;   // Where thrust is applied (back of _boatPerLevel)
        [SerializeField] private Transform rudderPosition;  // Where turning force is applied
        [SerializeField] private float motorForce = 50f;
        [SerializeField] private float rudderForce = 10f;

        [SerializeField] private Material waterFx;

        private Rigidbody rb;
        private float _throttle = 0f;
        private float _steering = 0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = Vector3.down * 0.5f;
            rb.isKinematic = false;
        }

        private void OnEnable()
        {
            EventBus.OnLevelComplete += ReactToLevelComplete;
            EventBus.OnThrottleChanged += value => _throttle = value;
            EventBus.OnSteeringChanged += value => _steering = value;
            EventBus.OnSprintChanged += OnSprintChanged;
            EventBus.OnChangeGameState += React;
            EventBus.OnGamePause += OnPauseChanged;
        }

        private void OnDisable()
        {
            EventBus.OnLevelComplete -= ReactToLevelComplete;
            EventBus.OnThrottleChanged -= value => _throttle = value;
            EventBus.OnSteeringChanged -= value => _steering = value;
            EventBus.OnSprintChanged -= OnSprintChanged;
            EventBus.OnChangeGameState -= React;
            EventBus.OnGamePause -= OnPauseChanged;
        }

        private void FixedUpdate()
        {
            ApplyBuoyancy();
            ApplyMotor();
            ApplyRudder();
            ApplyDamping();

            waterFx.SetFloat("_Speed", rb.linearVelocity.magnitude);
        }

        private void ReactToLevelComplete()
        {
            rb.isKinematic = true;
        }
        private void React(GameState gameState)
        {
            if (gameState == GameState.GameOver)
            {
                rb.isKinematic = true;
            }
        }

        private void OnSprintChanged(bool isSprinting)
        {
            if (isSprinting)
                motorForce *= 1.2f; // or use a multiplier
            else
                motorForce /= 1.2f;

            Debug.Log($"Sprint: {isSprinting}");
        }

        private void OnPauseChanged(bool isPaused)
        {
            // Freeze physics, show UI, or whatever you want
            rb.isKinematic = isPaused;
            Debug.Log("Paused: " + isPaused);
        }

        private void ApplyBuoyancy()
        {
            foreach (var point in buoyancyPoints)
            {
                float depth = waterLevel - point.position.y;
                if (depth > 0f)
                {
                    rb.AddForceAtPosition(Vector3.up * buoyancyForce * depth, point.position);
                }
            }
        }

        private void ApplyMotor()
        {
            // Motor always pushes along _boatPerLevel's forward direction
            if (motorPosition != null)
            {
                Vector3 thrust = motorPosition.forward * _throttle * motorForce;
                rb.AddForceAtPosition(thrust, motorPosition.position, ForceMode.Force);
            }
        }

        private void ApplyRudder()
        {
            if (rudderPosition != null)
            {
                // Apply a sideways force at the rudder position
                Vector3 lateral = rudderPosition.right * _steering * rudderForce;
                rb.AddForceAtPosition(lateral, rudderPosition.position, ForceMode.Force);
            }
            else
            {
                // fallback: simple torque
                rb.AddRelativeTorque(Vector3.up * _steering * rudderForce, ForceMode.Force);
            }
        }

        private void ApplyDamping()
        {
            rb.linearVelocity *= damping;
            rb.angularVelocity *= damping;
        }

        private void OnDrawGizmosSelected()
        {
            if (buoyancyPoints != null)
            {
                Gizmos.color = Color.cyan;
                foreach (var point in buoyancyPoints)
                    Gizmos.DrawSphere(point.position, 0.1f);
            }

            if (motorPosition != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(motorPosition.position, 0.15f);
            }

            if (rudderPosition != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(rudderPosition.position, 0.15f);
            }
        }
    }
}
