using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GrabSystem _grabSystem;

    [Header("Follow Settings")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.6f, 0f);

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    private float _pitch;
    private float _yaw;
    private bool _lookEnabled = true;

    private void Start()
    {
        _grabSystem = FindObjectOfType<GrabSystem>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (_grabSystem)
        {
            if (_grabSystem.IsRotating && _lookEnabled) SetLookEnabled(false);
            else if (!_grabSystem.IsRotating && !_lookEnabled) SetLookEnabled(true);
        }
    }

    private void LateUpdate()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position + offset;
            followTarget.rotation = Quaternion.Euler(0f, _yaw, 0f);
        }
    }

    private void HandleLook()
    {
        if (!_lookEnabled) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _yaw += mouseX;
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, -maxLookAngle, maxLookAngle);

        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    public void SetLookEnabled(bool enabled)
    {
        _lookEnabled = enabled;
    }
}
