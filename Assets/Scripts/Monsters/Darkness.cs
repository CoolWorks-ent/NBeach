using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */

public class Darkness : MonoBehaviour {

    public Transform target;
    public Pathfinding.AIDestinationSetter aIDestSetter;
    public Pathfinding.AIPath aIPath;
    public DarkStateController dsController;


    // Use this for initialization
    void Start () {
        aIDestSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        aIPath = GetComponent<Pathfinding.AIPath>();
        dsController.ChangeState(EnemyState.IDLE, this);

	}
	
	// Update is called once per frame
	void Update () {
        dsController.ExecuteCurrentState(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
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
