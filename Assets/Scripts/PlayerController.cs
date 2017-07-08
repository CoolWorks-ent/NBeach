﻿using UnityEngine;
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
    GameObject player;
    UnderwaterScript underwaterScript;
    public PlayerState playerState { get; set; }
    public bool CanMove = true; //is player allowed to move or not?

    Rigidbody rigidbody;
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
        player = GameObject.FindGameObjectWithTag("Player");
        rigidbody = this.GetComponent<Rigidbody>();
        //get variable to check if player underwater or not
        underwaterScript = mainCamera.GetComponent<UnderwaterScript>();
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
        if (Physics.Raycast(mainCamera.transform.position, fwd, out hit,fwd.magnitude,layerMask)) 
        {
            //make it so player can't move forward more when colliding with container
            if (hit.collider.tag == "PlayerWall")
            {
                if (insideWall == false)
                {
                    Debug.Log("Wall is in front of the player!");
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
        if (startMove == true && CanMove==true)
        {
            playerState = PlayerState.MOVING;
            startMove = false;
        }
        else if(CanMove == false)
        {
            
        }

        /*DEBUG*/
        //if "R" pressed, continue player mvmt
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (playerState == PlayerState.NOTMOVING)
                playerState = PlayerState.MOVING;
            else if(playerState == PlayerState.MOVING)
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
                Quaternion headRotation = mainCamera.transform.localRotation;//InputTracking.GetLocalRotation(VRNode.Head);


                //whatever player's velocity is, exert friction force onto the velocity while in water
                rigidbody.velocity = new Vector3(rigidbody.velocity.x * fakeFriction, rigidbody.velocity.y * fakeFriction, rigidbody.velocity.z * fakeFriction);

                //move player in direction camera facing
                rigidbody.AddForce(mainCamera.transform.forward * swimSpeed);
                //transform.localPosition += transform.forward * swimSpeed * Time.deltaTime;
            }
            else
                Debug.Log("rotation limit reached");
        }
        else { }
            //Debug.Log("player not moving");
    }
}
