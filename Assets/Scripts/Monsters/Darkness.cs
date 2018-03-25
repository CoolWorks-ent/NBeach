using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */

public enum EnemyState { CHASING, IDLE }

public class Darkness : MonoBehaviour {

    [SerializeField]
    float mvmtSpeed = 6f;

    //enemy states
    bool chasing = false, idle = false;
    public EnemyState enemyState { get; set; }
    public EnemySpawner enemySpawner;
    private Transform target;

    // Use this for initialization
    void Start () {
        enemyState = EnemyState.CHASING;
        target = GameObject.FindObjectOfType<AI_Manager>().player;
        GetComponent<Pathfinding.AIDestinationSetter>().target = target;
	}
	
	// Update is called once per frame
	void Update () {

        //StartCoroutine(ApproachPlayer());

        //MOVE THIS TO COROUTINE ONCE Time.DeltaTime is fixed
        float startTime;
        float curTime;
        float maxTime = 2f;

        //velocity based mvmt
        /*Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 velocity = Vector3.forward * mvmtSpeed; //move forward at constant speed

        if (enemyState == EnemyState.CHASING)
        {
            //TO DO - Create New code to chase player based upon player position

            //new Vector3(rigidbody.velocity.x * mvmtSpeed, rigidbody.velocity.y * mvmtSpeed, rigidbody.velocity.z * mvmtSpeed);
            rigidbody.MovePosition(rigidbody.position + Vector3.forward * mvmtSpeed * Time.deltaTime);
            //rigidbody.AddForce(velocity, ForceMode.Force);
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Projectile")
        {
            if (collision.collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
            {
                Debug.Log("Darkness Destroyed");
                this.enemySpawner.enemyList.Remove(this);
                Destroy(this.gameObject);
                //EventManager.TriggerEvent("DarknessDeath", gameObject.name);
            }
        }
            //EventManager.TriggerEvent("DarknessDeath", gameObject.);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            Debug.Log("Darkness collided with Player");
        }
    }

    /*IEnumerator ApproachPlayer()
    {
        float startTime ;
        float curTime;
        float maxTime = 2f;

        //velocity based mvmt
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 velocity = Vector3.forward * mvmtSpeed; //move forward at constant speed

        while (enemyState == EnemyState.CHASING)
        {
            //TO DO - Create New code to chase player based upon player position

            //new Vector3(rigidbody.velocity.x * mvmtSpeed, rigidbody.velocity.y * mvmtSpeed, rigidbody.velocity.z * mvmtSpeed);
            rigidbody.MovePosition(rigidbody.position + Vector3.forward * mvmtSpeed);
            //rigidbody.AddForce(velocity, ForceMode.Force);
            yield return null;
        }
        yield return 0;
    }*/
}
