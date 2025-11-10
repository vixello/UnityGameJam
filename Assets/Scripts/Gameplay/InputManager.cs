using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class InputManager : MonoBehaviour
    {
        private InputControlsBoat _controls;
        public static InputManager Instance { get; private set; }

        public float Throttle { get; private set; }
        public float Steering { get; private set; }
        public bool Sprint { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _controls = new InputControlsBoat();

            // Register named callbacks
            _controls.BoatControls.Throttle.performed += OnThrottlePerformed;
            _controls.BoatControls.Throttle.canceled += OnThrottleCanceled;

            _controls.BoatControls.Steering.performed += OnSteeringPerformed;
            _controls.BoatControls.Steering.canceled += OnSteeringCanceled;

            _controls.BoatControls.Sprint.performed += OnSprintPerformed;
            _controls.BoatControls.Sprint.canceled += OnSprintCanceled;

            _controls.BoatControls.Pause.performed += OnPausePerformed;
        }

        private void OnEnable() => _controls.Enable();

        private void OnDisable()
        {
            if (_controls == null)
                return; // Nothing to clean up

            // 🔥 Unsubscribe properly before disabling controls
            _controls.BoatControls.Throttle.performed -= OnThrottlePerformed;
            _controls.BoatControls.Throttle.canceled -= OnThrottleCanceled;

            _controls.BoatControls.Steering.performed -= OnSteeringPerformed;
            _controls.BoatControls.Steering.canceled -= OnSteeringCanceled;

            _controls.BoatControls.Sprint.performed -= OnSprintPerformed;
            _controls.BoatControls.Sprint.canceled -= OnSprintCanceled;

            _controls.BoatControls.Pause.performed -= OnPausePerformed;

            _controls.Disable();
        }

        // ----- Input Callbacks -----
        private void OnThrottlePerformed(InputAction.CallbackContext ctx)
        {
            Throttle = ctx.ReadValue<float>();
            EventBus.InvokeThrottleChanged(Throttle);
        }

        private void OnThrottleCanceled(InputAction.CallbackContext ctx)
        {
            Throttle = 0f;
            EventBus.InvokeThrottleChanged(Throttle);
        }

        private void OnSteeringPerformed(InputAction.CallbackContext ctx)
        {
            Steering = ctx.ReadValue<float>() * -1f;
            EventBus.InvokeSteeringChanged(Steering);
        }

        private void OnSteeringCanceled(InputAction.CallbackContext ctx)
        {
            Steering = 0f;
            EventBus.InvokeSteeringChanged(Steering);
        }

        private void OnSprintPerformed(InputAction.CallbackContext ctx)
        {
            Sprint = true;
            EventBus.InvokeSprintChanged(true);
        }

        private void OnSprintCanceled(InputAction.CallbackContext ctx)
        {
            Sprint = false;
            EventBus.InvokeSprintChanged(false);
        }

        private void OnPausePerformed(InputAction.CallbackContext ctx)
        {
            EventBus.InvokeGamePauseToggle();
        }
    }
}
