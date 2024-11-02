using UnityEngine;

public class SetPosRot : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float positionSmoothTime = 0.1f; // Adjust for smoother or snappier movement
    [SerializeField] private float rotationSmoothTime = 0.1f;

    private Vector3 positionVelocity;
    private Quaternion rotationVelocity;

    private void LateUpdate()
    {
        // Smoothly interpolate the target's position
        target.position = Vector3.SmoothDamp(target.position, transform.position, ref positionVelocity, positionSmoothTime);

        // Smoothly interpolate the target's rotation
        target.rotation = Quaternion.Slerp(target.rotation, transform.rotation, rotationSmoothTime);
    }
}
