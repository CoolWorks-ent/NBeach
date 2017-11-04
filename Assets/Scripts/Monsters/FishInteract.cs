using UnityEngine;
using System.Collections;
using System;
using VRStandardAssets.Utils;


public class FishInteract : MonoBehaviour {

    Vector3 fishLocalPos;
    bool move_wPlayer;
    GameObject player;
    Vector3 playerDiff;
    Vector3 playerOffset;
    float followSharpness = 0.4f; //how quickly should follow player
    float disFromPlayer;
    GVRInteractiveItem m_InteractiveItem;

    int[] offsetRange = new int[] { 1, 2, 3, };
    int[] ZoffsetRange = new int[] { -3,-2,3 };

    // Use this for initialization
    void Start () {
        //player is the camera
        player = GameObject.FindGameObjectWithTag("Player");
        m_InteractiveItem = GetComponent<GVRInteractiveItem>();
        m_InteractiveItem.OnClick += OnTouch;
    }
	
	// Update is called once per frame
	void Update () {
        //if player interacts with fish, fish should follow player
        if (move_wPlayer)
        {
            Vector3 targetPos = player.transform.position + playerOffset;
            //smooth follow?
           transform.position += (targetPos - transform.position) * followSharpness;
           //set to face same direction as player
           transform.rotation = player.transform.rotation;
        }
	}

    //fucntion for clicking on fish
    public void OnTouch()
    {

    }

    
    private void OnTriggerEnter(Collider other)
    {
        //make fish begin swimming with player if player interacts with it & is fish is in camera viewport
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        
        //viewport space is within 0-1 you are in the cameras frustum
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1.5 && screenPoint.y > 0 && screenPoint.y < 1.5;
        if (other.tag == "PlayerCube")
        {
            //if fish not already moving with player, start moving with player
            if (move_wPlayer == false)
            {
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
                float animTime = 2f;
                float tempZ = 0;
                //smooth follow?
                float followVal = 0.1f;

                while (t < animTime)
                {
                    /*New Animation IDEA!?
                     * maybe also make the fish move forward before joining the player, so the player can recognize what the fish is doing
                     * 
                     * */

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
            }
        }
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
