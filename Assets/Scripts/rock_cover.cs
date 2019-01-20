using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rock_cover : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "RockSmashAttack")
        {
            //play animation to destroy rock
            Debug.Log("cover smashed");
            AstarPath.active.UpdateGraphs(GetComponent<Collider>().bounds);
            Destroy(this.gameObject);
        }
    }
}
