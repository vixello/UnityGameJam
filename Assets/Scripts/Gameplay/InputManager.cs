using Core;
using UnityEngine;

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

            _controls.BoatControls.Throttle.performed += ctx =>
            {
                Throttle = ctx.ReadValue<float>();
                EventBus.InvokeThrottleChanged(Throttle);
                Debug.Log("W pressed");
            };
            _controls.BoatControls.Throttle.canceled += ctx =>
            {
                Throttle = 0f;
                EventBus.InvokeThrottleChanged(Throttle);
            };

            _controls.BoatControls.Steering.performed += ctx =>
            {
                Steering = ctx.ReadValue<float>() * -1f;
                EventBus.InvokeSteeringChanged(Steering);
            };
            _controls.BoatControls.Steering.canceled += ctx =>
            {
                Steering = 0f;
                EventBus.InvokeSteeringChanged(Steering);
            };

            _controls.BoatControls.Sprint.performed += ctx =>
            {
                Sprint = true;
                EventBus.InvokeSprintChanged(true);
            };

            _controls.BoatControls.Sprint.canceled += ctx =>
            {
                Sprint = false;
                EventBus.InvokeSprintChanged(false);
            };



            // Pause toggle
            _controls.BoatControls.Pause.performed += ctx =>
            {
                EventBus.InvokeGamePauseToggle();
            };
        }

        private void OnEnable()
        {
            _controls.Enable(); 
        }

        private void OnDisable()
        {
            _controls.Disable();
        }
    }
}
