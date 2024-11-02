using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Setup")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform bodyTarget;
    [SerializeField] private float sensitivity = 5f;

    [Header("Bobbing Settings")]
    [SerializeField] private float walkBobFrequency = 15f;
    [SerializeField] private float walkBobAmplitude = 0.06f;
    [SerializeField] private float runBobFrequency = 20f;
    [SerializeField] private float runBobAmplitude = 0.2f;

    [Header("Idle Bobbing Settings")]
    [SerializeField] private float idleBobFrequency = 3f;
    [SerializeField] private float idleBobAmplitude = 0.03f;
    [SerializeField] private float idleSwayFrequency = 0.5f;
    [SerializeField] private float idleSwayAmplitude = 0.03f;
    [SerializeField] private float randomBobMagnitude = 0.11f;

    [Header("Tilt Settings")]
    [SerializeField] private float walkTiltAngle = 1f;
    [SerializeField] private float runTiltAngle = 2f;

    [Header("Lean Settings")]
    [SerializeField] private float leanAngle = -4f;
    [SerializeField] private float leanSpeed = 5f;

    private Vector3 initialCameraOffset;
    private float xRotation, yRotation;
    private float bobTimer;
    private bool isMoving, isRunning;
    private float currentBobFrequency, currentBobAmplitude;
    private Vector3 targetPositionOffset;
    private Quaternion bobRotation;
    private float leanTarget;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        initialCameraOffset = cameraTarget.localPosition;
    }

    private void Update()
    {
        HandleMouseInput();
        UpdateBobbingAndSway();
    }

    private void HandleMouseInput()
    {
        xRotation += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime * 100;
        yRotation = Mathf.Clamp(yRotation - Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime * 100, -90, 90);

        cameraTarget.localRotation = Quaternion.Euler(yRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0, xRotation, 0);

        isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        float targetFrequency = isRunning ? runBobFrequency : walkBobFrequency;
        float targetAmplitude = isRunning ? runBobAmplitude : walkBobAmplitude;

        currentBobFrequency = Mathf.Lerp(currentBobFrequency, targetFrequency, Time.deltaTime * 5f);
        currentBobAmplitude = Mathf.Lerp(currentBobAmplitude, targetAmplitude, Time.deltaTime * 5f);
    }

    private void UpdateBobbingAndSway()
    {
        bobTimer += Time.deltaTime * (isMoving ? currentBobFrequency : idleBobFrequency);

        if (isMoving)
        {
            float randomOffset = Random.Range(-randomBobMagnitude, randomBobMagnitude);
            float bobOffsetY = Mathf.Sin(bobTimer) * currentBobAmplitude + randomOffset;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * currentBobAmplitude * 0.5f;

            targetPositionOffset = initialCameraOffset + new Vector3(bobOffsetX, bobOffsetY, 0);
            bobRotation = Quaternion.Euler(Mathf.Sin(bobTimer) * (isRunning ? runTiltAngle : walkTiltAngle), 0, 0);

            float horizontalInput = Input.GetAxis("Horizontal");
            leanTarget = Mathf.Lerp(leanTarget, horizontalInput * leanAngle, Time.deltaTime * leanSpeed);
        }
        else
        {
            float idleBobOffsetY = Mathf.Sin(bobTimer) * idleBobAmplitude;
            float idleBobOffsetX = Mathf.Cos(bobTimer * 0.5f) * idleBobAmplitude * 0.5f;

            float swayOffsetX = Mathf.Sin(bobTimer * idleSwayFrequency) * idleSwayAmplitude;
            float swayOffsetY = Mathf.Cos(bobTimer * idleSwayFrequency) * idleSwayAmplitude;

            targetPositionOffset = initialCameraOffset + new Vector3(idleBobOffsetX + swayOffsetX, idleBobOffsetY + swayOffsetY, 0);
            bobRotation = Quaternion.identity;
            leanTarget = Mathf.Lerp(leanTarget, 0, Time.deltaTime * leanSpeed);
        }

        cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, targetPositionOffset, Time.deltaTime * 8f);

        cameraTarget.localRotation = Quaternion.Slerp(cameraTarget.localRotation,
            Quaternion.Euler(yRotation, 0, 0) * bobRotation, Time.deltaTime * 8f);

        cameraTarget.localRotation *= Quaternion.Euler(0, 0, leanTarget);
    }
}
