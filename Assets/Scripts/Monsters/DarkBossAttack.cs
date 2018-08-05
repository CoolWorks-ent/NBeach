using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBossAttack : MonoBehaviour {

    public GameObject Target;
    public string attackType; //Ball, RockSmash, Smash

    Vector3 targetPos;
    float ballMoveTime = 5f; //higher this value, the faster the attack moves and the longer it lasts...
    public float attackSpeed = 20f;
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
                if (curTime < ballMoveTime)
                {
                    //move attack towards player's last known position

                    //transform.position = Vector3.MoveTowards(transform.position, targetPos, ballMoveTime * Time.deltaTime * attackSpeed);
                    Vector3 normalizeDirection = (targetPos - transform.position).normalized;
                    //normalizeDirection = new Vector3(normalizeDirection.x, normalizeDirection.y-.2f, normalizeDirection.z+2).normalized;
                    transform.position += (normalizeDirection * Time.deltaTime) * attackSpeed;
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

    public void moveAttack(Vector3 toPos)
    {
        if (toPos != Vector3.zero)
        {
            targetPos = toPos;
        }
        else
        {
            //Vector3 tempPos = new Vector3(Target.transform.position.x, Target.transform.position.y, Target.transform.position.z + 20);
            targetPos = Target.transform.position;
        }
        move = true;
    }

    public void moveAttack_3Ball()
    {

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
            //IF SMASH hits an environment object, then should play SFX & FX

        else if (collision.gameObject.tag == "PlayerCube" && attackType == "Ball")
        {
            Debug.Log("Dark Boss Attack collided with Player");
            //event call for player damaged
            Destroy(this.gameObject);
        }

        //destroy attack if it hits a NORMAL environment object
        if(collision.gameObject.tag == "player_cover" && (attackType == "Ball" || attackType == "Smash"))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "PlayerCube" && (attackType == "Ball" || attackType == "Smash"))
        {
            Debug.Log("Dark Boss Attack collided with Player");
            //event call for player damaged
            Destroy(this.gameObject);
        }
        //Check if attack hits the trigger wall behind player, if so, then destroy attack because it has already passed player
        else if(collider.gameObject.name == "p_attack_wall" && (attackType == "Ball" || attackType == "Smash"))
        {
            Debug.Log("Destroy dark ball attack");
            Destroy(this.gameObject);
        }
        else if (collider.gameObject.tag == "player cover" && (attackType == "Ball" || attackType == "Smash"))
        {
            Debug.Log("Destroy dark ball attack");
            Destroy(this.gameObject);
        }

        if (collider.gameObject.tag == "player cover" && attackType == "RockSmash")
            {
                //shake Screen with RockSmash collides with the game world
                Debug.Log("Dark Boss Attack collided with object");
                ScreenShake camShake = new ScreenShake();
                //camShake.Play(.7f, 1);
                StartCoroutine(camShake.Shake(.2f, .5f));

                Debug.Log("Dark Boss Attack collided with player rock cover");
                //event call for player damaged
                EventManager.TriggerEvent("Player_Cover_Destroyed", "Player_Cover_Destroyed");
                //kick off destruction animation for rock cover
                Destroy(collider.gameObject);
                //Destroy(this.gameObject);

            }

    }
}
