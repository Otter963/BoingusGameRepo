using UnityEngine;

public class SyncPhysicsObject : MonoBehaviour
{
    Rigidbody myRB;
    ConfigurableJoint joint;

    [SerializeField] Rigidbody animatedRigidBody;

    [SerializeField] bool syncAnimation = false;

    //keep track of the starting rotation
    Quaternion startLocalRotation;

    void Awake()
    {
        myRB = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();

        //storing the starting local rotation
        startLocalRotation = transform.localRotation;
    }

    public void UpdateJointFromAnimation()
    {
        if (!syncAnimation)
            return;

        ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedRigidBody.transform.localRotation, startLocalRotation);
    }
}
