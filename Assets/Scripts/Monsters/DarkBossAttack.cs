using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBossAttack : MonoBehaviour {

    public GameObject Target;
    public string attackType; //Ball, RockSmash, Smash

    Vector3 targetPos;
    float moveTime = 5f; //higher this value, the faster the attack moves and the longer it lasts...
    float attackSpeed = 3f;
    float curTime = 0;
    bool move;
    
    //Constructor
    /*public DarkBossAttack(string atkType, GameObject target)
    {
        Target = target;
        attackType = atkType;
    }*/

	// Use this for initialization
	void Start () {
        move = false;
        targetPos = Target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (Target != null)
        {
            if (move == true)
            { 
                if (curTime < moveTime)
                {
                    //move attacck towards player's last known position
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveTime * Time.deltaTime * attackSpeed);
                    curTime += Time.deltaTime;
                }
                else
                {
                    //!!CHANGE CODE to DESTROY gameobject when outside of the battle boundary!!
                    Destroy(this.gameObject);
                }
            }
        }
	}

    public void moveAttack()
    {
        targetPos = Target.transform.position;
        move = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[Dark Boss] Attack hit!");
        //only destroy rock cover is the boss used the smash attack
        if (collision.gameObject.tag == "player cover" && attackType == "RockSmash")
        {
            Debug.Log("Dark Boss Attack collided with player rock cover");
            //event call for player damaged
            EventManager.TriggerEvent("Player_Cover_Destroyed", "Player_Cover_Destroyed");
            //kick off destruction animation for rock cover
            Destroy(collision.gameObject);
            //Destroy(this.gameObject);

        }

        else if (collision.gameObject.tag == "PlayerCube" && attackType == "Ball")
        {
            Debug.Log("Dark Boss Attack collided with Player");
            //event call for player damaged
            Destroy(this.gameObject);
        }

        //destroy attack if it hits a NORMAL environment object
        if(collision.gameObject.layer == LayerMask.NameToLayer("Environment") && attackType == "Ball")
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "PlayerCube" && attackType == "Ball")
        {
            Debug.Log("Dark Boss Attack collided with Player");
            //event call for player damaged
            Destroy(this.gameObject);
        }

        if (collider.gameObject.tag == "player cover" && attackType == "RockSmash")
        {
            Debug.Log("Dark Boss Attack collided with player rock cover");
            //event call for player damaged
            EventManager.TriggerEvent("Player_Cover_Destroyed", "Player_Cover_Destroyed");
            //kick off destruction animation for rock cover
            AstarPath.active.UpdateGraphs(collider.bounds);
            Destroy(collider.gameObject);
            //Destroy(this.gameObject);

        }
    }
}
