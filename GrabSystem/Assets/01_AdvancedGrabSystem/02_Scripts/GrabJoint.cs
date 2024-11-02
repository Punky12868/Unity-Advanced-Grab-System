using UnityEngine;

public class GrabJoint : MonoBehaviour
{
    private GameObject objPoint;

    public Transform AttachJoint(Rigidbody rb, Vector3 attachmentPoint, float force, float damping)
    {
        objPoint = new GameObject("Attachment Point");
        objPoint.hideFlags = HideFlags.HideInHierarchy;
        objPoint.transform.position = attachmentPoint;

        var newRb = objPoint.AddComponent<Rigidbody>();
        newRb.isKinematic = true;

        var joint = objPoint.AddComponent<ConfigurableJoint>();
        joint.connectedBody = rb;
        joint.configuredInWorldSpace = true;

        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        joint.xDrive = NewJointDrive(force, damping);
        joint.yDrive = NewJointDrive(force, damping);
        joint.zDrive = NewJointDrive(force, damping);

        joint.slerpDrive = new JointDrive { positionSpring = 0, positionDamper = 0, maximumForce = 0 };
        joint.rotationDriveMode = RotationDriveMode.Slerp;

        return objPoint.transform;
    }

    public JointDrive NewJointDrive(float force, float damping)
    {
        JointDrive drive = new JointDrive();
        drive.mode = JointDriveMode.Position;
        drive.positionSpring = force;
        drive.positionDamper = damping;
        drive.maximumForce = Mathf.Infinity;
        return drive;
    }

    public void RemoveJoint(GameObject attachmentPoint)
    {
        Destroy(attachmentPoint.GetComponent<ConfigurableJoint>());
        Destroy(attachmentPoint.GetComponent<Rigidbody>());
        Destroy(objPoint);
    }
}
