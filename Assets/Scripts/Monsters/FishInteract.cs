using UnityEngine;
using System.Collections;
using System;
using VRStandardAssets.Utils;


public class FishInteract : MonoBehaviour {

    Vector3 fishLocalPos;
    bool move_wPlayer;
    GameObject player;
    PlayerController pController;
    Vector3 playerDiff;
    Vector3 playerOffset;
    float followSharpness = 0.3f; //how quickly should follow player
    float disFromPlayer;
    GVRInteractiveItem m_InteractiveItem;
    private Vector3 velocity = Vector3.zero;

    int[] offsetRange = new int[] { 1, 2, 3, };
    int[] ZoffsetRange = new int[] { -3,-2,3 };

    // Use this for initialization
    void Start () {
        //player is the camera
        player = GameObject.FindGameObjectWithTag("Player");
        pController = GameController.instance.playerControl;
        m_InteractiveItem = GetComponent<GVRInteractiveItem>();
        m_InteractiveItem.OnClick += OnTouch;
    }

    //used for smoother movement while following player. should help to reduce jitter due to the player's update needing to happen first
    private void LateUpdate()
    {
        //if player interacts with fish, fish should follow player
        if (move_wPlayer)
        {
            Vector3 targetPos = player.transform.position + playerOffset;
            //smooth follow?
            //transform.position += (targetPos - transform.position) * followSharpness;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, followSharpness);
            //set to face same direction as player container (because the player can look anywhere)
            transform.rotation = pController.transform.rotation;
        }
        
    }

    // Update is called once per frame
    void Update () {
	}

    //function for clicking on fish
    public void OnTouch()
    {

    }

    
    private void OnTriggerEnter(Collider other)
    {
        //make fish begin swimming with player if player interacts with it & is fish is in camera viewport
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        
        //viewport space is within 0-1 you are in the cameras frustum
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1.5 && screenPoint.y > 0 && screenPoint.y < 1.5;
        if (other.tag == "Player") //check if fish is also "onScreen"
        {
            //if fish not already moving with player, start moving with player
            if (move_wPlayer == false)
            {
                StartCoroutine(fishCaptureAnim());
                /*
                fishLocalPos = other.transform.position;
                playerDiff = fishLocalPos - player.transform.position;
                float sign = (player.transform.position.y < fishLocalPos.y ? -1.0f : 1.0f);
                float angleDiff = Vector2.Angle(Vector3.right, playerDiff) * sign;

                float currentAngle = Vector3.Angle(fishLocalPos, player.transform.position);

                //LATER: based upon angle, place fish in particular quadrant around player
                //for now, just place in random position
                playerOffset = new Vector3(GetRandom("x"), GetRandom("y"), GetRandom("z"));
                
                Debug.Log(this.gameObject.name + "follow player");

                //for 1st second of collision, play animation, reduce the followSpeed, and make the transition smooth before going to Update
                float t = 0;
                float animTime_a = 2f;
                float animTime = 2f;
                bool anim1Finished = false;
                float tempZ = 0;
                //smooth follow?
                float followVal = 0.1f;
                float followVal_a = 1f;
                while (t < animTime_a)
                {
                    //float pos = animTime_a * GameController.instance.pathControl.pathSpeed;

                    tempZ = transform.position.z + 100;
                    Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, tempZ);

                    //transform.position += (targetPos - transform.position) * followVal;
                    transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, followVal_a);
                    t += Time.deltaTime;
                }

                t = 0;
                tempZ = 0;
                while (t < animTime && anim1Finished == true)
                {
                    //this will make the z-transform positive before adding to the offset, and then revert the z-transform back to the original value.
                    //ensures that the z-transform and playerOffset are being added together with the same "sign" value, + or -
                    if (player.transform.position.z < 0) tempZ = -1 * ((player.transform.position.z) * -1 - playerOffset.z);

                    Vector3 targetPos = new Vector3(player.transform.position.x + playerOffset.x,
                        player.transform.position.y + playerOffset.y, tempZ);
                    
                    transform.position += (targetPos - transform.position) * followVal;
                    t += Time.deltaTime;
                }
                //lerp fish position to position close to player
                move_wPlayer = true;
                */
            }
            
        }
    }

    IEnumerator fishCaptureAnim()
    {
        //LATER: based upon angle, place fish in particular quadrant around player
        //for now, just place in random position
        playerOffset = new Vector3(GetRandom("x"), GetRandom("y"), GetRandom("z"));

        Debug.Log(this.gameObject.name + "follow player");

        //for 1st second of collision, play animation, reduce the followSpeed, and make the transition smooth before going to Update
        float t = 0;
        float animTime_a = 1f;
        float animTime = 2f;
        float tempZ = 0;
        //smooth follow?
        float followVal = 0.1f;
        float followVal_a = 1f;
        //1st, move the fish in front of the player
        //2nd, move the fish beside the player
        while (t < animTime_a)
        {
            tempZ = transform.position.z + 20;
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, tempZ);

            //transform.position += (targetPos - transform.position) * followVal;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, followVal_a);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        tempZ = 0;
        while (t < animTime)
        {
            //this will make the z-transform positive before adding to the offset, and then revert the z-transform back to the original value.
            //ensures that the z-transform and playerOffset are being added together with the same "sign" value, + or -
            if (player.transform.position.z < 0) tempZ = -1 * ((player.transform.position.z) * -1 - playerOffset.z);

            Vector3 targetPos = new Vector3(player.transform.position.x + playerOffset.x,
                player.transform.position.y + playerOffset.y, tempZ);

            transform.position += (targetPos - transform.position) * followVal;
            t += Time.deltaTime;
            yield return null;
        }
        //lerp fish position to position close to player
        move_wPlayer = true;


        yield return 0;
    }

    int GetRandom(string val)
    {
        if(val == "x")
            return offsetRange[UnityEngine.Random.Range(0, offsetRange.Length)];
        else if (val == "y")
            return offsetRange[UnityEngine.Random.Range(0, offsetRange.Length)];
        else if(val =="z")
            return ZoffsetRange[UnityEngine.Random.Range(0, ZoffsetRange.Length)];
        else return offsetRange[UnityEngine.Random.Range(0, offsetRange.Length)];
    }

/*not using collider for this anymore*/
private void OnCollisionEnter(Collision collision)
    {
        //make fish begin swimming with player if player interacts with it & is fish is in camera viewport
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        //viewport space is within 0-1 you are in the cameras frustum
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1.5 && screenPoint.y > 0 && screenPoint.y < 1.5;
        if (collision.collider.tag == "PlayerCube" && onScreen == true)
        { 
            //if fish not already moving with player, start moving with player
            if (move_wPlayer == false)
            {
                fishLocalPos = collision.transform.position;
                playerDiff = fishLocalPos - player.transform.position;
                float currentAngle = Vector3.Angle(fishLocalPos, player.transform.position);
                float zAngle = Mathf.Sin(currentAngle);
                float xAngle = Mathf.Cos(currentAngle);
                float yAngle = Mathf.Tan(currentAngle);
                //based upon angle, add or subtract a number offset amt
                playerOffset = new Vector3(xAngle+1f, yAngle, zAngle+10f);
                //lerp fish position to position close to player
                move_wPlayer = true;
                Debug.Log(this.gameObject.name + "follow player");
            }
        }
        //if fish collides with another fish, pause fish 1 movement, move to outside of fish 2 collider
    }

    public IEnumerator moveToPlayer(Vector3 curPos, Vector3 newPos)
    {
        //transform.position = Vector3.Lerp()
        yield return null;
    }
}
