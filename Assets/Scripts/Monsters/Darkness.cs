using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */
public class Darkness : MonoBehaviour {

    public Dark_State previousState, currentState;
    public Transform target;
    
    public int actionIdle, creationID, engIndex;
    public int attackHash = Animator.StringToHash("Attack"),
                attackAfterHash = Animator.StringToHash("AfterAttack"),
                chaseHash = Animator.StringToHash("Chase"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death"),
                wanderHash = Animator.StringToHash("Wander");
    public bool canAttack, updateStates, standBy;
    public float stateUpdateRate, attackInitiationRange, waitRange, stopDist;
    public Seeker sekr;
    public AIDestinationSetter aIDestSet;
    public GameObject deathFX;
    public Dark_State DeathState;
    public Animator animeController;
    
    //public AI_Movement aIMovement;
    public RichAI aIRichPath;

    void Awake()
    {
        actionIdle = 3;
        attackInitiationRange = 3.5f;
        waitRange = attackInitiationRange*2.5f;
        stopDist = 1;
        canAttack = standBy = false;
        creationID = 0;
        engIndex = 0;
        updateStates = true;
        stateUpdateRate = 0.5f;
    }

    void Start () {
        AI_Manager.OnDarknessAdded(this);
        animeController = GetComponentInChildren<Animator>();
        //aIMovement = GetComponent<AI_Movement>();
        aIRichPath = GetComponent<RichAI>();
        sekr = GetComponent<Seeker>();
        aIDestSet = GetComponent<AIDestinationSetter>();
        currentState.InitializeState(this);
        StartCoroutine(ExecuteCurrentState());
        aIDestSet.target = target;
        aIRichPath.endReachedDistance = stopDist;
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    // void FixedUpdate()
    // {
    //     if(!aIRichPath.canMove)
    //     {
    //         Vector3 dir = Vector3.RotateTowards(this.transform.position, target.position,0f,0f);
    //         transform.rotation = Quaternion.LookRotation(dir);
    //     }    
    // }

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
            currentState = nextState;
            currentState.InitializeState(this);
            previousState = currentState;
        }
    }

    public bool TargetWithinDistance(float range)
    {
        if(Vector3.Distance(transform.position, target.position) <= range)
        {
            return true;
        }
        else return false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Projectile"))
        {
            if (collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
            {
                Debug.LogWarning("Darkness Destroyed");
                ChangeState(DeathState);
                //ChangeState(DeathState);
                //EventManager.TriggerEvent("DarknessDeath", gameObject.name);
            }
        }
        else if(collider.gameObject.CompareTag("Player"))
        {
            //Debug.LogWarning("Darkness collided with Player");
        }
    }
}