using UnityEngine;
using UnityEngine.UIElements;

public class BoatCameraTarget : MonoBehaviour
{
    [SerializeField] private bool _lockPositionY = false;
    [SerializeField] private bool _lockRotationXZ = false;
    [SerializeField] private Transform boat;           // Assign your boat
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -6); // Camera offset

    [SerializeField] private float rotationSmooth = 5f;
    [SerializeField] private float positionSmooth = 5f;

    private delegate Vector3 PositioningStrategy() ;
    private PositioningStrategy positioningStrategy;

    private delegate Quaternion RotatingStrategy() ;
    private RotatingStrategy rotatingStrategy;
    private void Start()
    {
        positioningStrategy += _lockPositionY ? StrategyLockY : StategyDontLockY;
    }
    private Vector3 StrategyLockY()
    {
        Vector3 targetXZ = boat.position + transform.rotation * offset;
        targetXZ.y = transform.position.y; // Keep current Y
        return targetXZ;
    }

    private Vector3 StategyDontLockY()
    {
        return boat.position + transform.rotation * offset;
    }


    void LateUpdate()
    {
        if (boat == null) return;

        // Only take boat's yaw, ignore pitch/roll
        Quaternion targetRotation = Quaternion.Euler(0, boat.eulerAngles.y, 0);

        // Smoothly rotate target
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmooth);

        // Position target behind the boat using the rotated offset
        Vector3 desiredPosition = positioningStrategy();

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionSmooth);
    }
}
