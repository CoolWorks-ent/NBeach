using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRing : MonoBehaviour {

    [SerializeField]
    Transform[] fishGroup;

	// Use this for initialization
	void Start () {
        fishGroup = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            fishGroup[i] = transform.GetChild(i);
            Debug.Log(fishGroup[i]);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //make fish begin swimming with player if player interacts with it & is fish is in camera viewport
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);

        //viewport space is within 0-1 you are in the cameras frustum
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1.5 && screenPoint.y > 0 && screenPoint.y < 1.5;
        if (other.tag == "PlayerCube") //check if fish is also "onScreen"
        {
            //StartCoroutine(fishCaptureAnim());
            foreach (Transform obj in fishGroup)
            {
                obj.GetComponent<FishInteract>().ForceFollow("");
            }

        }
    }
}
