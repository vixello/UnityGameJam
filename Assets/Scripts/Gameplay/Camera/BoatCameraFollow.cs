using UnityEngine;

public class BoatCameraTarget : MonoBehaviour
{
    public Transform boat;           // Assign your boat
    public Vector3 offset = new Vector3(0, 2, -6); // Camera offset
    public float rotationSmooth = 5f;
    public float positionSmooth = 5f;

    void LateUpdate()
    {
        if (boat == null) return;

        // Only take boat's yaw, ignore pitch/roll
        Quaternion targetRotation = Quaternion.Euler(0, boat.eulerAngles.y, 0);

        // Smoothly rotate target
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmooth);

        // Position target behind the boat using the rotated offset
        Vector3 desiredPosition = boat.position + transform.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionSmooth);
    }
}
