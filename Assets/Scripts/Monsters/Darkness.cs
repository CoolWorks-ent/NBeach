using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */
public class Darkness : MonoBehaviour {


    public enum AggresionRating {Attacking = 1, CatchingUp, Idling, Wandering}
    [HideInInspector]
    public AggresionRating agRatingCurrent, agRatingPrevious;
    public Dark_State previousState, currentState;
    public AI_Manager.NavigationTarget Target;

    [HideInInspector]
    public AIPath pather;

    [HideInInspector]
    public Seeker sekr;
    
    public GameObject deathFX;
    public Dark_State DeathState;
    public Animator animeController;

    [HideInInspector]
    public int attackHash = Animator.StringToHash("Attack"),
                attackAfterHash = Animator.StringToHash("AfterAttack"),
                chaseHash = Animator.StringToHash("Chase"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death"),
                wanderHash = Animator.StringToHash("Wander");
    public bool updateStates;
    public int actionIdle, creationID;
    public float stateUpdateRate, attackInitiationRange, waitRange, stopDist, playerDist, swtichDist, navTargetDist;

    void Awake()
    {
        actionIdle = 3;
        attackInitiationRange = 3.5f;
        waitRange = 10f;
        stopDist = 1;
        swtichDist = 4.25f;
        creationID = 0;
        navTargetDist = -1;
        updateStates = true;
        stateUpdateRate = 0.5f;
        agRatingCurrent = agRatingPrevious = AggresionRating.Idling;
    }

    void Start () {
        animeController = GetComponentInChildren<Animator>();
        pather = GetComponent<AIPath>();
        sekr = GetComponent<Seeker>();
        AI_Manager.OnDarknessAdded(this);
        //aIMovement = GetComponent<AI_Movement>();
        currentState.InitializeState(this);
        //aIMovement.target = Target;
	}

    public IEnumerator StateTransition(Dark_State nextState)
    {
        //if(nextState.stateType != currentState.stateType)
        if(nextState != currentState)
        {
            previousState = currentState;
            currentState = nextState;
            yield return new WaitForSeconds(previousState.transitionTime);
            previousState.ExitState(this);            
            currentState.InitializeState(this);
        }
        yield return null;
    }

    public void ChangeState(Dark_State nextState)
    {
        previousState = currentState;
        currentState = nextState;
        previousState.ExitState(this);            
        currentState.InitializeState(this);
        /*if(nextState.stateType == Dark_State.StateType.DEATH)
        {
            previousState = currentState;
            currentState.ExitState(this);
            currentState = nextState;
            currentState.InitializeState(this);
        } 
        //else if (nextState.stateType == Dark_State.StateType.ATTACK && currentState.stateType == Dark_State.StateType.CHASING)
        else StartCoroutine(StateTransition(nextState));*/
        
    }

    public void PlayerDistanceEvaluation(Vector3 location)
    {
        playerDist = Vector3.Distance(transform.position, location);
        if(Target != null)
        {
            TargetDistanceEvaluation();
        }
        else navTargetDist = -1;
    }

    private void TargetDistanceEvaluation()
    {
        navTargetDist = Vector3.Distance(transform.position, Target.location.position);
    }

    public void AggressionChanged(AggresionRating agR)
    {
        if(agR != agRatingCurrent)
            agRatingPrevious = agRatingCurrent;
		agRatingCurrent = agR;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Projectile"))
        {
            if (collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
            {
                Debug.LogWarning("Darkness Destroyed");
                ChangeState(DeathState);
            }
        }
        else if(collider.gameObject.CompareTag("Player"))
        {
            //Debug.LogWarning("Darkness collided with Player");
        }
    }
}