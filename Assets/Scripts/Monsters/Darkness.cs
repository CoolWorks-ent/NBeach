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
    public float stateUpdateRate, attackInitiationRange, waitRange, stopDist, playerDist, swtichDist;

    void Awake()
    {
        actionIdle = 3;
        attackInitiationRange = 3.5f;
        waitRange = 10f;
        stopDist = 1;
        swtichDist = 4.25f;
        creationID = 0;
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

    public IEnumerator ValidateStateChange(Dark_State nextState, float transitionTime)
    {
        //if(nextState.stateType != currentState.stateType)
            
        yield return new WaitForSeconds(1.25f);
        if(nextState != currentState)
        {
            previousState = currentState;
            currentState.ExitState(this);
            currentState = nextState;
            currentState.InitializeState(this);
        }
    }

    public void ChangeState(Dark_State nextState, float time)
    {
        if(nextState.stateType == Dark_State.StateType.DEATH)
        {
            previousState = currentState;
            currentState.ExitState(this);
            currentState = nextState;
            currentState.InitializeState(this);
        } 
        //else if (nextState.stateType == Dark_State.StateType.ATTACK && currentState.stateType == Dark_State.StateType.CHASING)
        else StartCoroutine(ValidateStateChange(nextState, time));
        
    }

    public void DistanceEvaluation(Vector3 location)
    {
        playerDist = Vector3.Distance(transform.position, location);
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
                ChangeState(DeathState, 0.1f);
            }
        }
        else if(collider.gameObject.CompareTag("Player"))
        {
            //Debug.LogWarning("Darkness collided with Player");
        }
    }
}