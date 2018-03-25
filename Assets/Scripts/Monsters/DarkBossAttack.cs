using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBossAttack : MonoBehaviour {

    public GameObject Target;
    float moveTime = 5f; //higher this value, the faster the attack moves and the longer it lasts...
    float attackSpeed = 3f;
    float curTime = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Target != null)
        {
            if (curTime < moveTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, moveTime * Time.deltaTime * attackSpeed);
                curTime += Time.deltaTime;
            }
            else
            {
                //!!CHANGE CODE to DESTROY gameobject when outside of the battle boundary!!
                Destroy(this.gameObject);
            }
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[Dark Boss] Attack hit!");

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Debug.Log("Dark Boss Attack collided with Player");
            Destroy(this.gameObject);
        }
    }
}
