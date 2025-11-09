using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private Transform motorPosition;   // Where thrust is applied (back of boat)
        [SerializeField] private Transform rudderPosition;  // Where turning force is applied
        [SerializeField] private float motorForce = 50f;
        [SerializeField] private float rudderForce = 10f;

        [SerializeField] private Material waterFx;

        private Rigidbody rb;
        private InputControlsBoat _controls;
        private float _throttle = 0f;
        private float _steering = 0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = Vector3.down * 0.5f;

            _controls = new InputControlsBoat();
            _controls.BoatControls.Throttle.performed += ctx => _throttle = ctx.ReadValue<float>();
            _controls.BoatControls.Throttle.canceled += ctx => _throttle = 0f;

            _controls.BoatControls.Steering.performed += ctx => _steering = ctx.ReadValue<float>()*-1;
            _controls.BoatControls.Steering.canceled += ctx => _steering = 0f;
        }

        private void OnEnable() => _controls.Enable();
        private void OnDisable() => _controls.Disable();

        private void FixedUpdate()
        {
            ApplyBuoyancy();
            ApplyMotor();
            ApplyRudder();
            ApplyDamping();

            waterFx.SetFloat("_Speed", rb.linearVelocity.magnitude);
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
            // Motor always pushes along boat's forward direction
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
