using UnityEngine;

public class SetPosRot : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float positionSmoothTime = 0.01f;
    [SerializeField] private float rotationSmoothTime = 1;

    private Vector3 positionVelocity;
    private Quaternion rotationVelocity;

    private void LateUpdate()
    {
        target.position = Vector3.SmoothDamp(target.position, transform.position, ref positionVelocity, positionSmoothTime);
        target.rotation = Quaternion.Slerp(target.rotation, transform.rotation, rotationSmoothTime);
    }
}
