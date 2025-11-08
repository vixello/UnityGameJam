using UnityEngine;

/// <summary>
/// Simple buoyancy script: makes an object float on a defined water surface level.
/// Attach this to any Rigidbody with a Collider.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BuoyancyObject: MonoBehaviour
{
    [Header("Water Settings")]
    [Tooltip("Y height of the water surface.")]
    public float waterHeight = 0f;

    [Tooltip("How strongly the object is pushed upwards when submerged.")]
    public float buoyancyForce = 10f;

    [Tooltip("How much drag is applied when the object is underwater.")]
    public float waterDrag = 3f;

    [Tooltip("How much angular drag is applied underwater.")]
    public float waterAngularDrag = 1f;

    [Header("Extra Motion")]
    [Tooltip("Simulates gentle waves. Affects buoyant position slightly.")]
    public bool enableWaves = true;

    [Tooltip("Amplitude of the wave motion.")]
    public float waveAmplitude = 0.25f;

    [Tooltip("Frequency of the wave motion.")]
    public float waveFrequency = 0.5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplyBuoyancy();
    }

    private void ApplyBuoyancy()
    {
        // Optionally simulate waves
        float effectiveWaterHeight = waterHeight;
        if (enableWaves)
        {
            effectiveWaterHeight += Mathf.Sin(Time.time * waveFrequency + transform.position.x) * waveAmplitude;
        }

        float depth = effectiveWaterHeight - transform.position.y;

        if (depth > 0f)
        {
            // Apply buoyancy proportional to submersion
            float force = buoyancyForce * depth;
            rb.AddForce(Vector3.up * force, ForceMode.Acceleration);

            // Apply water drag for damping
            rb.linearDamping = waterDrag;
            rb.angularDamping = waterAngularDrag;
        }
        else
        {
            // Normal drag in air
            rb.linearDamping = 0.1f;
            rb.angularDamping = 0.05f;
        }
    }
}
