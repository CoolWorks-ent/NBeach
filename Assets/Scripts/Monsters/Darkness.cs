using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */

public enum EnemyState { CHASING, IDLE, ATTACK }

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
        enemyState = EnemyState.IDLE;
        target = GameObject.FindObjectOfType<AI_Manager>().player;
        GetComponent<Pathfinding.AIDestinationSetter>().target = target;
        GetComponent<Pathfinding.AIPath>().repathRate = Random.Range(2.5f, 5.0f);
	}
	
	// Update is called once per frame
	void Update () {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Projectile")
        {
            if (collision.collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
            {
                Debug.Log("Darkness Destroyed");
                AI_Manager.Instance.AddtoDarknessList(this);
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

}
