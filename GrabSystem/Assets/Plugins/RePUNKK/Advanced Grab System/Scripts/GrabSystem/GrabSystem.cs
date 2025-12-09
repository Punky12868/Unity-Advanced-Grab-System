using UnityEngine;

namespace RePunkk.GrabSystem
{
    public class GrabSystem : MonoBehaviour
    {
        [SerializeField] private bool drawGrabLine = true;
        [SerializeField] private bool enableScrollDistance = true;
        [SerializeField] private bool enableThrow = true;
        [SerializeField] private bool enableManualRotation = false;

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float grabRange = 5f;
        [SerializeField] private float maxDragDistance = 6f;
        [SerializeField] private bool dragPointIsOnHit = true;
        [SerializeField] private LayerMask grabMask = -1;

        [SerializeField] private Vector3 grabPointOffset = new Vector3(0f, 0f, 3f);
        [SerializeField] private float scrollSpeed = 2f;
        [SerializeField] private float minGrabDistance = 2f;

        [SerializeField] private float baseForce = 600f;
        [SerializeField] private float damping = 6f;
        [SerializeField] private float heavyObjectThreshold = 50f;
        [SerializeField] private float dragDifficultyFactor = 10f;
        [SerializeField] private float lightObjectMass = 1f;
        [SerializeField] private float heavyObjectMass = 50f;

        [SerializeField] private KeyCode throwChargeKey = KeyCode.Mouse1;
        [SerializeField] private float throwChargeSpeed = 3f;
        [SerializeField] private float minThrowDistanceOffset = 2f;
        [SerializeField] private float throwForceMultiplier = 25f;

        [SerializeField] private KeyCode rotateKey = KeyCode.R;
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private float maxAngularVelocity = 5f;

        [SerializeField] private float lineWidth = 0.01f;
        [SerializeField] private int lineVertices = 10;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private bool usingColor = true;
        [SerializeField] private Color lineColor = Color.white;
        [SerializeField] private float lineStartOffset = 0.5f;

        [SerializeField] private bool showGizmo = true;
        [SerializeField] private Color gizmoColor = Color.yellow;
        [SerializeField] private float gizmoSize = 0.1f;
        [SerializeField] private Color minDistanceGizmoColor = Color.red;
        [SerializeField] private float minDistanceGizmoSize = 0.25f;

        private GrabJoint _grabJoint;
        private DrawGrabLine _grabLine;
        private GameObject _grabPointObject;
        private Transform _cameraTransform;
        private Transform _target;
        private Transform _grabJointTransform;
        private Vector3 _localHitPoint;
        private float _grabDistance;
        private float _massBasedForce;
        private float _throwChargeAmount;
        private bool _isChargingThrow;
        private bool _isRotating;
        private Rigidbody _targetRigidbody;
        private float _originalDrag;
        private float _originalAngularDrag;
        private Quaternion _localRotationOffset;

        public bool IsGrabbing => _target != null;
        public bool IsChargingThrow => _isChargingThrow;
        public bool IsRotating => _isRotating;

        private void Start()
        {
            _grabJoint = gameObject.AddComponent<GrabJoint>();

            if (cameraTransform == null)
            {
                if (Camera.main != null)
                {
                    _cameraTransform = Camera.main.transform;
                    Debug.LogWarning("GrabSystem: Camera Transform not assigned, using Camera.main");
                }
                else
                {
                    _cameraTransform = transform;
                    Debug.LogWarning("GrabSystem: No Camera found, using own transform");
                }
            }
            else
            {
                _cameraTransform = cameraTransform;
            }

            _grabPointObject = new GameObject("GrabPoint");
            _grabPointObject.hideFlags = HideFlags.HideInHierarchy;
            _grabPointObject.transform.SetParent(transform);

            if (drawGrabLine)
            {
                _grabLine = gameObject.AddComponent<DrawGrabLine>();
                _grabLine.Initialize(lineWidth, lineVertices, lineMaterial, usingColor, lineColor);
            }
        }

        private void Update()
        {
            HandleInput();

            if (_target != null)
            {
                CheckTargetDistance();
                UpdateGrabPoint();
                HandleRotation();
                HandleThrowCharge();
                UpdateObjectRotation();
                DrawLine();
            }
        }

        private void UpdateObjectRotation()
        {
            if (_target != null && _targetRigidbody != null)
            {
                float mass = _targetRigidbody.mass;
                float stabilizationStrength = 1f - Mathf.InverseLerp(lightObjectMass, heavyObjectMass, mass);

                Quaternion targetRotation = _cameraTransform.rotation * _localRotationOffset;
                Quaternion currentRotation = _target.rotation;
                Quaternion rotationDelta = targetRotation * Quaternion.Inverse(currentRotation);

                rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);
                if (angle > 180f) angle -= 360f;

                if (Mathf.Abs(angle) > 0.1f)
                {
                    float rotationForce = Mathf.Lerp(1f, 10f, stabilizationStrength);
                    Vector3 angularVelocity = axis.normalized * (angle * Mathf.Deg2Rad * rotationForce);
                    _targetRigidbody.angularVelocity = Vector3.Lerp(_targetRigidbody.angularVelocity, angularVelocity, stabilizationStrength);
                }
            }
        }

        private void HandleRotation()
        {
            if (!enableManualRotation) return;

            bool rotateKeyPressed = Input.GetKey(rotateKey);

            if (rotateKeyPressed && !_isRotating)
            {
                _isRotating = true;
            }
            else if (!rotateKeyPressed && _isRotating)
            {
                _isRotating = false;
                _localRotationOffset = Quaternion.Inverse(_cameraTransform.rotation) * _target.rotation;
                if (_targetRigidbody != null) _targetRigidbody.angularVelocity = Vector3.zero;
            }

            if (_isRotating && _targetRigidbody != null)
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

                if (Mathf.Abs(mouseX) > 0.001f || Mathf.Abs(mouseY) > 0.001f)
                {
                    Vector3 torqueHorizontal = _cameraTransform.up * mouseX;
                    Vector3 torqueVertical = _cameraTransform.right * -mouseY;
                    Vector3 totalTorque = (torqueHorizontal + torqueVertical);

                    _targetRigidbody.AddTorque(totalTorque, ForceMode.Force);

                    if (_targetRigidbody.angularVelocity.magnitude > maxAngularVelocity) _targetRigidbody.angularVelocity = _targetRigidbody.angularVelocity.normalized * maxAngularVelocity;

                    _localRotationOffset = Quaternion.Inverse(_cameraTransform.rotation) * _target.rotation;
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && _target == null) GrabObject();
            if (Input.GetKeyUp(KeyCode.Mouse0) && _target != null && !_isChargingThrow) DropObject();
            if (Input.GetKeyUp(throwChargeKey) && _target != null && _isChargingThrow) ThrowObject();
        }

        private void CheckTargetDistance()
        {
            float distanceToTarget = Vector3.Distance(_cameraTransform.position, _target.position);
            if (distanceToTarget > maxDragDistance) DropObject();
        }

        private void GrabObject()
        {
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, grabRange, grabMask))
            {
                _target = hit.transform;
                _localHitPoint = _target.InverseTransformPoint(hit.point);
                _grabDistance = Vector3.Distance(_cameraTransform.position, hit.point);
                _throwChargeAmount = 0f;
                _isChargingThrow = false;

                Vector3 grabPointPosition = CalculateGrabPointPosition();
                _grabPointObject.transform.position = grabPointPosition;

                _targetRigidbody = hit.rigidbody;
                _massBasedForce = CalculateMassBasedForce(_targetRigidbody);

                if (_targetRigidbody != null)
                {
                    _originalDrag = _targetRigidbody.drag;
                    _originalAngularDrag = _targetRigidbody.angularDrag;

                    float mass = _targetRigidbody.mass;
                    float stabilizationStrength = 1f - Mathf.InverseLerp(lightObjectMass, heavyObjectMass, mass);

                    _targetRigidbody.drag = Mathf.Lerp(_originalDrag, 5f, stabilizationStrength);
                    _targetRigidbody.angularDrag = Mathf.Lerp(_originalAngularDrag, 5f, stabilizationStrength);

                    _localRotationOffset = Quaternion.Inverse(_cameraTransform.rotation) * _target.rotation;
                }

                if (_grabLine != null) _grabLine.AddLine(_grabPointObject);

                _grabJointTransform = _grabJoint.AttachJoint(_targetRigidbody, dragPointIsOnHit ? hit.point : hit.transform.position, _massBasedForce, damping);
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
            if (_targetRigidbody != null)
            {
                _targetRigidbody.drag = _originalDrag;
                _targetRigidbody.angularDrag = _originalAngularDrag;
            }

            _target = null;
            _targetRigidbody = null;
            _throwChargeAmount = 0f;
            _isChargingThrow = false;
            _isRotating = false;
            _grabJoint.RemoveJoint(_grabPointObject);

            if (_grabLine != null) _grabLine.RemoveLine(_grabPointObject);
        }

        private void UpdateGrabPoint()
        {
            if (enableScrollDistance)
            {
                float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                if (Mathf.Abs(scrollInput) > 0.01f) _grabDistance = Mathf.Clamp(_grabDistance + scrollInput * scrollSpeed, minGrabDistance, grabRange);
            }

            _grabDistance = Mathf.Max(_grabDistance, minGrabDistance);
            Vector3 targetPosition = CalculateGrabPointPosition();
            _grabPointObject.transform.position = targetPosition;
            _grabJointTransform.position = targetPosition;
        }

        private Vector3 CalculateGrabPointPosition()
        {
            float effectiveDistance = _grabDistance - _throwChargeAmount;
            float minDistance = Mathf.Max(minGrabDistance, minThrowDistanceOffset);
            effectiveDistance = Mathf.Max(effectiveDistance, minDistance);

            Vector3 offset = _cameraTransform.TransformDirection(grabPointOffset.normalized * effectiveDistance);
            return _cameraTransform.position + offset;
        }

        private float GetMinThrowDistance()
        {
            return _grabDistance - minThrowDistanceOffset;
        }

        private void HandleThrowCharge()
        {
            if (!enableThrow) return;

            if (Input.GetKey(throwChargeKey))
            {
                _isChargingThrow = true;
                _throwChargeAmount += throwChargeSpeed * Time.deltaTime;
                float maxCharge = _grabDistance - Mathf.Max(minGrabDistance, minThrowDistanceOffset);
                _throwChargeAmount = Mathf.Clamp(_throwChargeAmount, 0f, maxCharge);
            }
        }

        private void ThrowObject()
        {
            if (_target != null && _target.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                Vector3 throwDirection = _cameraTransform.forward;
                float throwForce = _throwChargeAmount * throwForceMultiplier;

                DropObject();

                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }
            else DropObject();
        }

        private void DrawLine()
        {
            if (_grabLine != null && _target != null)
            {
                Vector3 startPosition = _cameraTransform.position + _cameraTransform.forward * lineStartOffset;
                Vector3 endPosition = dragPointIsOnHit ? _target.TransformPoint(_localHitPoint) : _target.position;
                _grabLine.DrawLine(startPosition, endPosition);
            }
        }

        private void OnDrawGizmos()
        {
            if (!showGizmo) return;

            Transform cam = _cameraTransform != null ? _cameraTransform : Camera.main?.transform;
            if (cam == null) return;

            float currentDistance = _target != null ? _grabDistance - _throwChargeAmount : grabPointOffset.magnitude;
            Vector3 gizmoPosition = cam.position + cam.TransformDirection(grabPointOffset.normalized * currentDistance);

            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(gizmoPosition, gizmoSize);

            float minDistance = _target != null ? GetMinThrowDistance() : minThrowDistanceOffset;
            Vector3 minDistancePosition = cam.position + cam.TransformDirection(grabPointOffset.normalized * minDistance);

            Gizmos.color = minDistanceGizmoColor;
            Vector3 planeNormal = cam.forward;
            DrawGizmoPlane(minDistancePosition, planeNormal, minDistanceGizmoSize);
        }

        private void DrawGizmoPlane(Vector3 position, Vector3 normal, float size)
        {
            Vector3 right = Vector3.Cross(normal, Vector3.up).normalized;
            if (right.magnitude < 0.1f) right = Vector3.Cross(normal, Vector3.right).normalized;
            Vector3 up = Vector3.Cross(right, normal).normalized;

            Vector3 topLeft = position - right * size + up * size;
            Vector3 topRight = position + right * size + up * size;
            Vector3 bottomLeft = position - right * size - up * size;
            Vector3 bottomRight = position + right * size - up * size;

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, bottomRight);
            Gizmos.DrawLine(topRight, bottomLeft);
        }

        private void OnDestroy()
        {
            if (_grabPointObject != null) Destroy(_grabPointObject);
        }
    }
}

