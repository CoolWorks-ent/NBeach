using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Shell : MonoBehaviour {

    float airTime = 2f; //m/s
    float speed = 7f;
    float startTime;
    float elapsedTime = 0f;
    int maxRange = 5; //max distance
    Vector3 fwd;
	// Use this for initialization
	void Start ()
    {
        startTime = 0;
        //temporary setting of fwd for testing purposes. 
        fwd = Camera.main.transform.forward;
        Debug.Log("New projectile created");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (elapsedTime <= airTime)
        {
            //move forward until max range or collision
            transform.localPosition += fwd * speed*Time.deltaTime;
            elapsedTime += Time.deltaTime;
        }
        else
            Destroy(this.gameObject);
	}

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
