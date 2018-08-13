using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */
public class Darkness : AI_StateController {

    public Transform target;
    
    ///<summary>Sets the initiation range for darkness units. Units will be qeued for attack in this range</summary>
    public int attackInitiationRange, actionIdle, queueID, stateUpdateRate;

    public GameObject deathFX;

    public Pathfinding.RichAI aIRichPath;

    public Animator animeController;
    public int attackHash = Animator.StringToHash("Attack"),
                attackAfterHash = Animator.StringToHash("AfterAttack"),
                chaseHash = Animator.StringToHash("Chase"),
                wanderHash = Animator.StringToHash("Wander"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death");
    public Pathfinding.IAstarAI ai;

    public AI_State previousState, currentState;

    public bool canAttack, idleFinished, updateStates;

    void Start () {
        actionIdle = 3;
        canAttack = idleFinished = false;
        queueID = 0;
        animeController = GetComponentInChildren<Animator>();
        ai = GetComponent<Pathfinding.IAstarAI>();
        aIRichPath = GetComponent<Pathfinding.RichAI>();
	}
	
	// Update is called once per frame
	void Update () {
        currentState.UpdateState(this);
    }

    public override void InitializeAI()
    {
        updateStates = true;
        //StartCoroutine(UpdateState());
    }

    public override void TransitionToState(AI_State nextState)
    {
        if(nextState != currentState)
        {
            currentState = nextState;
        }
    }
    
    /*protected override IEnumerator UpdateState()
    {
        while(updateStates)
            currentState.UpdateState(this);

        yield return new WaitForSeconds(stateUpdateRate);
    }*/

    public override void OnExitState()
    {
        
    }


    public void SetMovementParameters(bool canMove, float repathRate, float maxSpeed)
    {
        aIRichPath.canMove = canMove;
        aIRichPath.repathRate = repathRate;
        aIRichPath.maxSpeed = maxSpeed;
    }

    public void RevertMovementParameters()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
      //EventManager.TriggerEvent("DarknessDeath", gameObject.);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            Debug.Log("Darkness collided with Player");
        }
        else if (collider.gameObject.tag == "Projectile")
        {
            if (collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
            {
                Debug.Log("Darkness Destroyed");

                GameObject newFX = Instantiate(deathFX.gameObject, transform.position, Quaternion.identity) as GameObject;
                //gameObject.GetComponent<MeshRenderer>().material.SetColor(Color.white);
                
                //change darkness back to idle to state to prevent moving & set to Kinematic to prevent any Physics effects
                gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
                StartCoroutine(deathRoutine());

                //EventManager.TriggerEvent("DarknessDeath", gameObject.name);
            }
        }
    }

    IEnumerator deathRoutine()
    {
        float fxTime = 1;
        //Slowly increase texture power over the FX lifetime to show the Darkness "Glowing" and explode!
        int maxPower = 10;
        MeshRenderer renderer = gameObject.GetComponentInChildren<MeshRenderer>();
        float curPower = renderer.material.GetFloat("_MainTexturePower");
        float curTime = 0;
        while(curTime < fxTime)
        {
            curPower = curTime * maxPower;
            renderer.material.SetFloat("_MainTexturePower", curPower);
            curTime += Time.deltaTime;
            yield return 0;
        }
       
        //yield return new WaitForSeconds(fxTime);
        Destroy(this.gameObject);
        yield return 0;
    }

}