using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VRStandardAssets.Common;

public enum PlayerState { MOVING, SWIMMING, JUMPING, NOTMOVING }

public class PlayerController : MonoBehaviour {

    [SerializeField]
    SplineController splineControl;
    [SerializeField]
    float swimSpeed = 2f;
    [SerializeField]
    float myGravity = 9f;
    //friction for underwater movement
    [SerializeField]
    float fakeFriction = 0.86f;
    [SerializeField]
    float waterDrag = 1f;

    Camera mainCamera;
    UnderwaterScript underwaterScript;
    public PlayerState playerState { get; set; }
    bool insideWall = false;
    bool startMove = false;
    bool isUnderwater;
    Quaternion p_startRot;
    Vector3 p_startPos;
    float maxRotAngle = 45;
    float minRotAngle = -45;

    


    // Use this for initialization
    void Start () {
        p_startPos = transform.position;
        p_startRot = transform.rotation; //use local rotation?
	}

    private void Awake()
    {
        //by default player is not moving
        playerState = PlayerState.NOTMOVING;

        //check that swimSpeed was not set to be faster than the camera splines speed, if the on-rails movement is being used
        //if (swimSpeed > splineControl.Speed)
        //swimSpeed = splineControl.Speed;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //get variable to check if player underwater or not
        underwaterScript = GetComponent<UnderwaterScript>();
    }

	// Update is called once per frame
	void Update () {

        //enable underwater movement if player underwater
        if (underwaterScript.isUnderwater)
        {
                UnderwaterMovement();
        }

        //Raycast from player to check for PlayerContainer Collision
        Vector3 fwd = transform.TransformDirection(Vector3.forward) * 2;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //layer mask to only collide with PlayerOnly layer
        int layerMask = 1 << 8;

        Debug.DrawRay(transform.position, fwd, Color.red);
        if (Physics.Raycast(transform.position, fwd, out hit,fwd.magnitude,layerMask)) 
        {
            //make it so player can't move forward more when colliding with container
            if (hit.collider.tag == "PlayerWall")
            {
                Debug.Log("Wall is in front of the player!");
                if (insideWall == false)
                {
                    playerState = PlayerState.NOTMOVING;
                    insideWall = true;
                }
            }
        }
        //if player no longer colliding with wall, set player to move
        else if (insideWall == true && playerState == PlayerState.NOTMOVING)
        {
            insideWall = false;
            startMove = true;
        }

        //begin player movement again if no longer colliding with PlayerContainer
        if (startMove == true)
        {
            playerState = PlayerState.MOVING;
            startMove = false;
        }

        /*DEBUG*/
        //if "R" pressed, continue spline
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (playerState == PlayerState.NOTMOVING)
                playerState = PlayerState.MOVING;
            else if(playerState == PlayerState.MOVING)
                playerState = PlayerState.NOTMOVING;
        }

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
        //make it so player can't move forward more when colliding with container
        /*if(collision.collider.tag == "PlayerContainer")
        {
            playerState = PlayerState.NOTMOVING;
        }*/
    }

    void UnderwaterMovement()
    {
        //if player rotation is less than max rotation angle, then player can move in that direction
        //if player rotation is greater than angle, prevent movement in that forward direction
        float rotDiff = Mathf.Abs(transform.localRotation.y) - Mathf.Abs(p_startRot.y);
        Rigidbody rigidbody = this.GetComponent<Rigidbody>();

        //rigidbody.useGravity = false;
        rigidbody.drag = waterDrag;

        //Idle movement in water
        //apply gravity force?
        //rigidbody.AddForce(Vector3.up * -myGravity, ForceMode.Acceleration);
        //rigidbody.AddForce(Vector3.down * myGravity, ForceMode.Acceleration);

        //currently player can only move if state is Moving
        if (playerState != PlayerState.NOTMOVING)
        {
            if (Mathf.Abs(rotDiff) < maxRotAngle)
            {
                //get head rotation Method 1
                Quaternion headRotation = InputTracking.GetLocalRotation(VRNode.Head);


                //whatever player's velocity is, exert friction force onto the velocity while in water
                rigidbody.velocity = new Vector3(rigidbody.velocity.x * fakeFriction, rigidbody.velocity.y * fakeFriction, rigidbody.velocity.z * fakeFriction);
                Debug.Log(rigidbody.velocity);
                

                //move player in direction facing
                rigidbody.AddForce(transform.forward * swimSpeed);
                //transform.localPosition += transform.forward * swimSpeed * Time.deltaTime;
            }
            else
                Debug.Log("rotation limit reached");
        }
    }
}
