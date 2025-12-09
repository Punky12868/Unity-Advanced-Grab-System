using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float maxVelocity = 10f;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = (transform.right * horizontal + transform.forward * vertical).normalized;
        float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * sprintMultiplier : moveSpeed;
        Vector3 targetVelocity = inputDirection * speed;

        Vector3 currentVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        Vector3 velocityChange = targetVelocity - currentVelocity;

        velocityChange = Vector3.ClampMagnitude(velocityChange, maxVelocity);
        _rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (inputDirection.magnitude < 0.01f)
        {
            _rb.velocity = new Vector3(_rb.velocity.x * 0.9f, _rb.velocity.y, _rb.velocity.z * 0.9f);
        }
    }
}
