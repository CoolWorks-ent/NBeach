using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */
public class Darkness : MonoBehaviour {

    public Transform target;
    
    ///<summary>Sets the initiation range for darkness units. Units will be qeued for attack in this range</summary>
    public int attackInitiationRange, actionIdle, queueID;
    private float stateUpdateRate;
    public Seeker sekr;
    public AIDestinationSetter aIDestSet;

    public GameObject deathFX;

    //public Pathfinding.RichAI aIRichPath;

    public Animator animeController;
    public int attackHash = Animator.StringToHash("Attack"),
                attackAfterHash = Animator.StringToHash("AfterAttack"),
                chaseHash = Animator.StringToHash("Chase"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death");
    //public AI_Movement aIMovement;

    public Dark_State previousState, currentState;
    public RichAI aIRichPath;

    public bool canAttack, idleFinished, attackRequested, updateStates;

    void Start () {
        actionIdle = attackInitiationRange = 3;
        canAttack = idleFinished = attackRequested = false;
        queueID = 0;
        animeController = GetComponentInChildren<Animator>();
        //aIMovement = GetComponent<AI_Movement>();
        aIRichPath = GetComponent<RichAI>();
        aIDestSet = GetComponent<AIDestinationSetter>();
        currentState.InitializeState(this);
        updateStates = true;
        stateUpdateRate = 0.5f;
        StartCoroutine(ExecuteCurrentState());
        aIDestSet.target = target;
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public IEnumerator ExecuteCurrentState()
    {
        while(updateStates)
        {
            currentState.UpdateState(this);
            yield return new WaitForSeconds(stateUpdateRate);
        }
        yield return null;
    }

    public void ChangeState(Dark_State nextState)
    {
        if(currentState != nextState)
        {
            /*string memes = "Switching from current <color = red><b>" + currentState.stateType + "</b></color> state to next <color = green><b>" + nextState.stateType +  "</b></color> state";
            Debug.Log(memes);*/
            currentState = nextState;
            currentState.InitializeState(this);
            previousState = currentState;
        }
    }

    /* public void SetMovementParameters(bool canMove, float repathRate, float maxSpeed)
    {
        aIRichPath.canMove = canMove;
        aIRichPath.repathRate = repathRate;
        aIRichPath.maxSpeed = maxSpeed;
    }*/

    public bool TargetWithinDistance()
    {
        if(Vector3.Distance(transform.position, target.position) <= (float)attackInitiationRange && canAttack)
        {
            canAttack = false;
            return true;
        }
        else return false;
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
                //ChangeState(DeathState);
                //EventManager.TriggerEvent("DarknessDeath", gameObject.name);
            }
        }
    }
}