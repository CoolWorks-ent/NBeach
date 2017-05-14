using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using VRStandardAssets.Common;

public class PlayerContainer : MonoBehaviour {

    PlayerController playerController;
    bool playerInside = false;
    Vector3 distanceToWall;
    // Use this for initialization
    void Start () {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {

        if (playerInside)
        {
            if (distanceToWall.z < 1f)
                playerController.playerState = PlayerState.NOTMOVING;
        }
	}

    /*
     * When EventTrigger calls OnPointerEnter, set bool variable for updatet
     * Use this bool variable to tell script to begin checking the distance from player to container wall
     * because the eventSystem won't trigger OnPointerEnter while player is inside playerContainer
     * */

    //must use BaseEventData to pass pointer event data to function
    public void OnPointerEnter(BaseEventData EventData)
    {
        //make BaseEventData info into PointerEventData
        PointerEventData eventData = EventData as PointerEventData;

        distanceToWall = eventData.pointerCurrentRaycast.worldPosition - Camera.main.transform.position;
        Debug.Log("Ray distance" + distanceToWall);
        playerInside = true;
    }

    public void OnPointerExit(BaseEventData EventData)
    {

    }

    public void setGazedAt(bool gaze)
    {

    }
}
