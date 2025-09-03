using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody playerRB;

    [SerializeField] ConfigurableJoint playerJoint;

    [SerializeField] Animator playerAnim;

    //Input:
    Vector2 moveInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;

    //Controller settings:
    float maxSpeed = 3;

    //States
    bool isGrounded = false;

    //Raycasts
    RaycastHit[] raycastHits = new RaycastHit[10];

    //syncing of the physics objects
    SyncPhysicsObject[] syncPhysicsObjects;

    //Fall speed
    [SerializeField] private int fallSpeed = 20;

    //Settings values
    //inverting movement, defaulted to normal WASD
    public int invertXAxis = 1;
    public int invertYAxis = -1;

    void Awake()
    {
        syncPhysicsObjects = GetComponentsInChildren<SyncPhysicsObject>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Move input
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpButtonPressed = true;
        }
    }

    private void FixedUpdate()
    {
        //assuming not grounded
        isGrounded = false;

        //if grounded
        int numberOfHits = Physics.SphereCastNonAlloc(playerRB.position, 0.1f, transform.up * -1, raycastHits, 0.5f);

        //check for valid results
        for (int i = 0; i < numberOfHits; i++)
        {
            //ignore self hits
            if (raycastHits[i].transform.root == transform)
            {
                continue;
            }
            isGrounded = true;

            break;
        }

        //applying extra force to fall less floaty
        if (!isGrounded)
        {
            playerRB.AddForce(Vector3.down * fallSpeed);
        }

        float inputMagnitude = moveInputVector.magnitude;

        Vector3 localVelocityVsForward = transform.forward * Vector3.Dot(transform.forward, playerRB.linearVelocity);

        float localForwardVelocity = localVelocityVsForward.magnitude;

        if (inputMagnitude != 0)
        {
            Quaternion desiredDirection = Quaternion.LookRotation(new Vector3(moveInputVector.x * invertXAxis, 0, 
                moveInputVector.y * invertYAxis), transform.up);

            //rotate towards direction
            playerJoint.targetRotation = Quaternion.RotateTowards(playerJoint.targetRotation, desiredDirection, Time.fixedDeltaTime * 300);

            if (localForwardVelocity < maxSpeed)
            {
                //moving the player in the direction it is facing
                playerRB.AddForce(transform.forward * inputMagnitude * 30);
            }
        }

        if (isGrounded && isJumpButtonPressed)
        {
            playerRB.AddForce(Vector3.up * 20, ForceMode.Impulse);

            isJumpButtonPressed = false;
        }

        playerAnim.SetFloat("movementSpeed", localForwardVelocity * 0.4f);

        //animation

        //updating the joints rotation based on the animations
        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            syncPhysicsObjects[i].UpdateJointFromAnimation();
        }
    }
}
