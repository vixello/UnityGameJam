using Core;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class BoatCameraTarget : MonoBehaviour
{
    [SerializeField] private bool _lockPositionY = false;

    [SerializeField] private int _currentLevel = 0;
    [SerializeField] private Transform[] _boatPerLevel = new Transform[3];           
    
    [Header("How to follow the target per level")]
    [SerializeField] private Vector3[] _offsetPositionForLevels = new Vector3[3]; // Camera _offsetPositionForLevels

    [Header("How to rotate the camera per level")]
    [SerializeField] private float[] _cameraRotationPerLevelX;

    [SerializeField] private float rotationSmooth = 5f;
    [SerializeField] private float positionSmooth = 5f;

    [SerializeField] private CinemachineCamera _cmCamera;
    [SerializeField] private float sprintFOV = 75f;

    private bool _isSprinting;
    private float _fieldOfView;
    private Coroutine _sprintCoroutine;
    private delegate Vector3 PositioningStrategy() ;
    private PositioningStrategy positioningStrategy;

    private void Start()
    {
        _fieldOfView = _cmCamera.Lens.FieldOfView;

        EventBus.OnSprintChanged += OnSprintChanged;
        positioningStrategy += _lockPositionY ? StrategyLockY : StategyDontLockY;
    }

    private void OnDisable()
    {
        EventBus.OnSprintChanged -= OnSprintChanged;
    }

    private void OnSprintChanged(bool isSprinting)
    {
        _isSprinting = isSprinting;

        if (_sprintCoroutine != null)
        {
            StopCoroutine(_sprintCoroutine);
        }
        _sprintCoroutine = StartCoroutine(ChangeCameraFieldOfView());
    }

    private Vector3 StrategyLockY()
    {
        Vector3 targetXZ = _boatPerLevel[_currentLevel].position + transform.rotation * _offsetPositionForLevels[_currentLevel];
        targetXZ.y = transform.position.y; // Keep current Y
        return targetXZ;
    }

    private Vector3 StategyDontLockY()
    {
        return _boatPerLevel[_currentLevel].position + transform.rotation * _offsetPositionForLevels[_currentLevel];
    }

    private IEnumerator ChangeCameraFieldOfView()
    {
        float elapsed = 0f;
        float duration = 1.2f;
        float startFOV = _cmCamera.Lens.FieldOfView;
        float targetFOV = _isSprinting ? sprintFOV : _fieldOfView;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            _cmCamera.Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        // Ensure it lands exactly on the target value
        _cmCamera.Lens.FieldOfView = targetFOV;
    }


    void LateUpdate()
    {
        if (_boatPerLevel == null) return;

        // Only take _boatPerLevel's yaw, ignore pitch/roll
        Quaternion targetRotation = Quaternion.Euler(_cameraRotationPerLevelX[_currentLevel], _boatPerLevel[_currentLevel].eulerAngles.y, 0);

        // Smoothly rotate target
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmooth);

        // Position target behind the _boatPerLevel using the rotated _offsetPositionForLevels
        Vector3 desiredPosition = positioningStrategy();

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionSmooth);
    }
}
