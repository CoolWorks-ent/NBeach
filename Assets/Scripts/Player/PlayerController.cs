using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VRStandardAssets.Common;

public enum PlayerState { MOVING, SWIMMING, JUMPING, NOTMOVING }
public enum PLAYER_STATUS { ALIVE, HURT, DEAD }

public class PlayerController : MonoBehaviour {

    [SerializeField]
    float swimSpeed = 2f;
    [SerializeField]
    float myGravity = 9f;
    //friction for underwater movement
    [SerializeField]
    float fakeFriction = 0.86f;
    [SerializeField]
    float waterDrag = 1f;
    [SerializeField]
    public GameObject playerContainer;
    [SerializeField]
    public SimpleAnimator2D animator_EyeBlink;

    public Camera mainCamera;
    public Camera UICamera;
    GameObject player;
    GameController gameController;
    UnderwaterScript underwaterScript;
    public PlayerState playerState { get; set; }
    public PLAYER_STATUS playerStatus { get; set; }
    public bool CanMove = true; //is player allowed to move or not?

    Rigidbody rigidbody;
    bool insideWall = false;
    bool startMove = false;
    
    bool isUnderwater;
    Quaternion p_startRot;
    Quaternion p_rotLimitMin, p_rotLimitMax;
    Vector3 p_startPos;
    float maxRotAngle = 90;
    float minRotAngle = -45;
    Vector3 movementVector;
    Vector3 undesiredMvmtVector;

    //playerInteractableItems + UI specific variables
    SpeedBoost speedBoost;
    SpeedEffectAnimator speedAnimator;
    

    // Use this for initialization
    void Start () {
        p_startPos = transform.position;
        p_startRot = transform.rotation; //use local rotation?
        //Rotation 
        p_rotLimitMin = Quaternion.Euler(180,180,180);
        p_rotLimitMax = Quaternion.Euler(359, 359, 359);
	}

    private void Awake()
    {

        EventManager.StartListening("Player_SpeedBoost", OnSpeedBoost);
        //by default player is not moving
        playerState = PlayerState.NOTMOVING;

        //check that swimSpeed was not set to be faster than the camera splines speed, if the on-rails movement is being used
        //if (swimSpeed > splineControl.Speed)
        //swimSpeed = splineControl.Speed;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        rigidbody = this.GetComponent<Rigidbody>();
        //get variable to check if player underwater or not
        underwaterScript = mainCamera.GetComponent<UnderwaterScript>();

        //Effects that affect player
        //speedAnimator = mainCamera.GetComponent<SpeedEffectAnimator>();
    }

    //reset player to default state
    public void Reset()
    {
        EventManager.TriggerEvent("CancelPowerUps", "cancelPowerUps");
    }


	// Update is called once per frame
	void Update () {

            //enable underwater movement if player underwater
            if (underwaterScript.isUnderwater)
        {
                UnderwaterMovement();
        }

        //set rotation of player character to same as camera
        player.transform.rotation = mainCamera.transform.rotation;

        //Raycast from player to check for PlayerContainer Collision
        Vector3 fwd = mainCamera.transform.TransformDirection(Vector3.forward) * 2;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //layer mask to only collide with PlayerOnly layer
        int layerMask = 1 << 8;

        Debug.DrawRay(mainCamera.transform.position, fwd, Color.red);
        if (Physics.Raycast(mainCamera.transform.position, fwd, out hit, fwd.magnitude, layerMask))
        {
            //make it so player can't move forward more when colliding with container
            if (hit.collider.tag == "PlayerWall")
            {
                
                if (playerState == PlayerState.MOVING)
                {
                    Debug.Log("Wall is in front of the player!");
                    CanMove = false;
                    insideWall = true;
                    //project vector of motion onto the plane's normal (via raycast) 
                    undesiredMvmtVector = hit.normal * Vector3.Dot(mainCamera.transform.forward, hit.normal);
                    movementVector = mainCamera.transform.forward - undesiredMvmtVector;
                }
            }
        }
        
        //if player no longer colliding with wall, set player to move
        else if (insideWall == true && playerState == PlayerState.NOTMOVING)
        {
            insideWall = false;
            CanMove = true;
            startMove = true;
        }
        else //if raycast hits nothing, then player is inside container but too deep
            //insideWall = true;

        //begin player movement again if no longer colliding with PlayerContainer
        if (startMove == true && CanMove==true)
        {
            Debug.Log("player can move");
            playerState = PlayerState.MOVING;
            startMove = false;
        }
        else if(CanMove == false)
        {
            playerState = PlayerState.NOTMOVING;
        }


        /*DEBUG*/
        //if "R" pressed, continue player mvmt
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (playerState == PlayerState.NOTMOVING)
            {
                CanMove = true;
                startMove = true;
            }
            else if (playerState == PlayerState.MOVING)
                playerState = PlayerState.NOTMOVING;
        }

        /**NotMoving PlayerState
         * Cancel all forces when not moving
         **/
        if(playerState == PlayerState.NOTMOVING)
        {
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
        }

    }

    void OnSpeedBoost(string str)
    {
        //StartCoroutine(SpeedBoostRoutine());
    }

    private IEnumerator SpeedBoostRoutine()
    {
        //increase speed of player 
        float speedTime = 4f;//speedBoost.speedTime;
        float initBoostTime = 2f;//speedBoost.initBoostTime;
        float boostAmt = 2f;//speedBoost.boostAmt;
        float timeElapsed = 0f;

        //plays the speed effect for x seconds
        while (timeElapsed < speedTime)
        {
            //increase speed instantly.  but need to set up a gradual increase instead.
            gameController.pathControl.pathSpeed += boostAmt;

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //stop speed boost
        EventManager.TriggerEvent("Player_SpeedBoostOff", "Player_SpeedBoost");   
    }


    private void FixedUpdate()
    {

    }

    void ContainerCollision(bool gazedAt)
    {
        bool collide;
        GvrReticlePointer reticle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //trigger speed boost if collide with the object
        if (collision.collider.tag == "SpeedBoost")
        {
            EventManager.TriggerEvent("Player_SpeedBoost", "Player_SpeedBoost");
            //get speedBoost object to use for variables
            speedBoost = collision.gameObject.GetComponent<SpeedBoost>();
        }
    }



    /// <summary>
    /// Controls Underwater movement for Player
    /// </summary>
    void UnderwaterMovement()
    {
        //if player rotation is less than max rotation angle, then player can move in that direction
        //if player rotation is greater than angle, prevent movement in that forward direction
        float rotDiff = Mathf.Abs(transform.localRotation.y) - Mathf.Abs(p_startRot.y);

        //rigidbody.useGravity = false;
        rigidbody.drag = waterDrag;

        //Idle movement in water
        //apply gravity force?
        //rigidbody.AddForce(Vector3.up * -myGravity, ForceMode.Acceleration);
        //rigidbody.AddForce(Vector3.down * myGravity, ForceMode.Acceleration);

        //currently player can only move if state is Moving
        if (playerState != PlayerState.NOTMOVING)
        {
            // get head rotation Method 1
                Quaternion headRotation = mainCamera.transform.localRotation;//InputTracking.GetLocalRotation(VRNode.Head);
                                                                             //get player's current rotation based on their starting rotation
            float curAngle = Quaternion.Angle(p_startRot, headRotation);
            float curVal = p_startRot.eulerAngles.y - headRotation.eulerAngles.y;

            //if player's rotation is not behind them, allow movement in the direction they are facing.  DO NOT ALLOW BACKWARDS MOVEMENT
            if(!(headRotation.eulerAngles.y > maxRotAngle && headRotation.eulerAngles.y < 270))
            {
                //whatever player's velocity is, exert friction force onto the velocity while in water
                rigidbody.velocity = new Vector3(rigidbody.velocity.x * fakeFriction, rigidbody.velocity.y * fakeFriction, rigidbody.velocity.z * fakeFriction);

                if (insideWall)
                {
                    /*old player mvmt code*/
                    //move player in direction of player's movement vector when colliding with wall.
                    //movementVector = mainCamera.transform.forward - undesiredMvmtVector;
                    //setting z to 0 to prevent player moving backward away from wall or forward into the wall
                    //movementVector = new Vector3(movementVector.normalized.x, movementVector.normalized.y, 0);
                    //rigidbody.velocity = movementVector * 10f;
                    //rigidbody.AddForce(movementVector * (swimSpeed));

                    /*new moveement code*/
                    float maxVelocityChange = 1;
                    movementVector = mainCamera.transform.forward - undesiredMvmtVector;
                    // Calculate how fast we should be moving
                    Vector3 targetVelocity = movementVector;
                    targetVelocity = transform.TransformDirection(targetVelocity);
                    targetVelocity *= swimSpeed;

                    // Apply a force that attempts to reach our target velocity
                    Vector3 velocity = rigidbody.velocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = 0;
                    rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
                }
                else
                {
                    /*old player mvmt code*/
                    //move player in direction camera facing, when player wall not being collided with
                    //rigidbody.AddForce(mainCamera.transform.forward * swimSpeed);

                    /*new moveement code*/
                    float maxVelocityChange = 1;
                    movementVector = mainCamera.transform.forward;
                    // Calculate how fast we should be moving
                    Vector3 targetVelocity = movementVector;
                    targetVelocity = transform.TransformDirection(targetVelocity);
                    targetVelocity *= swimSpeed;

                    // Apply a force that attempts to reach our target velocity
                    Vector3 velocity = rigidbody.velocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = 0;
                    rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

                }
            }
           /* if (Mathf.Abs(rotDiff) < maxRotAngle)
            {
                //get head rotation Method 1
                Quaternion headRotation = mainCamera.transform.localRotation;//InputTracking.GetLocalRotation(VRNode.Head);
                //get player's current rotation based on their starting rotation
                float curAngle = Quaternion.Angle(p_startRot, headRotation);
                Debug.Log("Rot = " + curAngle);
                //if player's rotation is not behind them, allow movement in the direction they are facing
                if (curAngle < ma360xRotAngle)
                {
                    //whatever player's velocity is, exert friction force onto the velocity while in water
                    rigidbody.velocity = new Vector3(rigidbody.velocity.x * fakeFriction, rigidbody.velocity.y * fakeFriction, rigidbody.velocity.z * fakeFriction);

                    //move player in direction camera facing
                    rigidbody.AddForce(mainCamera.transform.forward * swimSpeed);
                    //transform.localPosition += transform.forward * swimSpeed * Time.deltaTime;
                }

            }
            else
                Debug.Log("rotation limit reached");*/
        }
        else { }
            //Debug.Log("player not moving");
    }
}
