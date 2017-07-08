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
    float followSharpness = 0.3f; //how quickly should follow player
    float disFromPlayer;
    GVRInteractiveItem m_InteractiveItem;

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
                playerOffset = new Vector3(UnityEngine.Random.Range(-2.0f, 2.0f),
                    UnityEngine.Random.Range(-3.0f, 2.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
                //lerp fish position to position close to player
                move_wPlayer = true;
                Debug.Log(this.gameObject.name + "follow player");
            }
        }
    }

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
                playerOffset = new Vector3(xAngle+1f, yAngle, zAngle);
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
