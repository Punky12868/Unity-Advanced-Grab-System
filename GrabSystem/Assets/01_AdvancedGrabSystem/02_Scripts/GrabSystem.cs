using UnityEngine;

public class GrabSystem : MonoBehaviour
{
    private GrabJoint _grabJoint;
    private DrawGrabLine _grabLine;

    [SerializeField] private bool drawGrabLine = true;
    [SerializeField] private bool dragPointIsOnHit = true;

    [SerializeField] private GameObject grabPoint;
    [SerializeField] private float grabRange = 5f;
    [SerializeField] private float maxDragDistance = 6f;

    [SerializeField] private float baseForce = 600f;
    [SerializeField] private float damping = 6f;

    [SerializeField] private float heavyObjectThreshold = 50f;
    [SerializeField] private float dragDifficultyFactor = 10f;

    [SerializeField] private float scrollSpeed = 2.0f;
    [SerializeField] private float minScroll = 2f;

    [SerializeField] private float zLinePoint;
    [SerializeField] private LayerMask GrabMask;

    private Vector3 localHitPoint;
    private Transform grabJoint, target, cameraPos;
    private float grabDistance;
    private float massBasedForce;

    private void Start()
    {
        _grabJoint = gameObject.AddComponent<GrabJoint>();
        cameraPos = Camera.main.transform;

        if (drawGrabLine && !GetComponent<DrawGrabLine>()) _grabLine = gameObject.AddComponent<DrawGrabLine>();
        else if (drawGrabLine && GetComponent<DrawGrabLine>()) _grabLine = GetComponent<DrawGrabLine>();
    }

    private void Update()
    {
        HandleInput();

        if (target != null)
        {
            CheckTargetDistance();
            grabJoint.position = grabPoint.transform.position;
            AdjustGrabPointDistance();
            DrawLine();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && target == null) GrabObject();
        if (Input.GetKeyUp(KeyCode.Mouse0) && target != null) DropObject();
    }

    private void CheckTargetDistance()
    {
        float distanceToTarget = Vector3.Distance(cameraPos.position, target.position);
        if (distanceToTarget > maxDragDistance) DropObject();
    }

    private void GrabObject()
    {
        if (Physics.Raycast(cameraPos.position, cameraPos.forward, out RaycastHit hit, grabRange, GrabMask))
        {
            target = hit.transform;
            localHitPoint = target.InverseTransformPoint(hit.point);
            grabDistance = Vector3.Distance(cameraPos.position, hit.point);
            grabPoint.transform.position = hit.point;

            Rigidbody hitRigidbody = hit.rigidbody;
            massBasedForce = CalculateMassBasedForce(hitRigidbody);

            if (hitRigidbody != null) GetComponent<FirstPersonCharacterController>().SetDraggingState(true, hitRigidbody.mass);

            _grabLine.AddLine(grabPoint);
            grabJoint = _grabJoint.AttachJoint(hitRigidbody,
                dragPointIsOnHit ? hit.point : hit.transform.position,
                massBasedForce,
                damping);
        }
    }

    private float CalculateMassBasedForce(Rigidbody hitRigidbody)
    {
        if (hitRigidbody == null) return baseForce;
        float mass = hitRigidbody.mass;
        if (mass > heavyObjectThreshold) return baseForce * (heavyObjectThreshold / mass) * dragDifficultyFactor;
        return baseForce;
    }

    private void DropObject()
    {
        target = null;
        GetComponent<FirstPersonCharacterController>().SetDraggingState(false);
        _grabJoint.RemoveJoint(grabPoint);
        _grabLine.RemoveLine(grabPoint);
    }

    private void AdjustGrabPointDistance()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            grabDistance = Mathf.Clamp(grabDistance + scrollInput * scrollSpeed, minScroll, grabRange);
            grabPoint.transform.position = cameraPos.position + cameraPos.forward * grabDistance;
        }

        float currentDistance = Vector3.Distance(cameraPos.position, grabPoint.transform.position);
        if (currentDistance < minScroll)
        {
            grabDistance = minScroll;
            grabPoint.transform.position = cameraPos.position + cameraPos.forward * grabDistance;
        }
    }

    private void DrawLine()
    {
        if (_grabLine != null)
        {
            Vector3 forwardDirection = cameraPos.forward;
            Vector3 adjustedPosition = cameraPos.position + forwardDirection * zLinePoint;
            if (target != null) _grabLine.DrawLine(adjustedPosition, !dragPointIsOnHit ? target.position : target.TransformPoint(localHitPoint));
        }
    }
}
