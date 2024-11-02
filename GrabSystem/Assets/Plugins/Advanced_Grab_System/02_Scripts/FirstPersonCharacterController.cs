using UnityEngine;

public class FirstPersonCharacterController : MonoBehaviour
{
    [SerializeField] private float normalSpeed = 5;
    [SerializeField] private float runSpeed = 10;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float gravityModifier = 1.5f;

    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    [SerializeField] private float groundSphereRadius = 0.28f;
    [SerializeField] private bool usingGroundMask = false;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private bool debugMode;

    private Rigidbody rb;
    private float speed, horizontal, vertical;
    private bool isDragging;
    private float objectMass;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Update()
    {
        DirectionInput();
        RunInput();
        JumpInput();
    }

    private void DirectionInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void RunInput()
    {
        speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : normalSpeed;

        if (isDragging) speed *= Mathf.Clamp(1f / (objectMass / 10f), 0.5f, 1f);
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        moveDirection = transform.TransformDirection(moveDirection) * speed;

        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(moveDirection.x, currentVelocity.y, moveDirection.z);

        rb.velocity = Vector3.Lerp(currentVelocity, targetVelocity, (moveDirection.magnitude > 0 ? acceleration : deceleration) * Time.deltaTime);
    }

    private void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (!IsGrounded() && rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityModifier - 1) * Time.deltaTime;
    }

    private bool IsGrounded()
    {
        if(!usingGroundMask)
        {
            Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundSphereRadius);
            foreach (Collider collider in colliders)
                if (collider.gameObject != gameObject) return true;
            return false;
        }
        else
            return Physics.CheckSphere(groundCheck.position, groundSphereRadius, groundMask);
    }

    private void OnDrawGizmos()
    {
        if (debugMode)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, groundSphereRadius);
        }
    }

    public void SetDraggingState(bool dragging, float mass = 0f)
    {
        isDragging = dragging;
        objectMass = mass;
    }
}
