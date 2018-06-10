using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */
 [RequireComponent(typeof(DarkStateController))]
public class Darkness : MonoBehaviour {

    public Transform target;
    public Pathfinding.AIDestinationSetter aIDestSetter;
    public Pathfinding.RichAI aIRichPath;
    public DarkStateController dsController;
    public int attackRange;

    // Use this for initialization
    void Start () {
        aIDestSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        aIRichPath = GetComponent<Pathfinding.RichAI>();
        dsController = GetComponent<DarkStateController>();
        dsController.ChangeState(EnemyState.IDLE, this);
        aIDestSetter.target = target;
	}
	
	// Update is called once per frame
	void Update () {
        dsController.ExecuteCurrentState();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            if (collision.collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
            {
                Debug.Log("Darkness Destroyed");
                //AI_Manager.Instance.AddtoDarknessList(this);
                Destroy(this.gameObject);
                //EventManager.TriggerEvent("DarknessDeath", gameObject.name);
            }
        }
            //EventManager.TriggerEvent("DarknessDeath", gameObject.);
    }
}
