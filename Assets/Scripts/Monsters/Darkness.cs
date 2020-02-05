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
    public Collider darkHitBox;
    
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
    public bool updateStates, attacked;
    public int creationID;
    public float attackInitiationRange, playerDist, swtichDist, navTargetDist, stopDistance;

    void Awake()
    {
        attackInitiationRange = 2.5f;
        stopDistance = 3;
        swtichDist = attackInitiationRange*1.75f;
        creationID = 0;
        navTargetDist = -1;
        updateStates = true;
        agRatingCurrent = agRatingPrevious = AggresionRating.Idling;
        attacked = false;
    }

    void Start () {
        animeController = GetComponentInChildren<Animator>();
        pather = GetComponent<AIPath>();
        sekr = GetComponent<Seeker>();
        darkHitBox = GetComponent<CapsuleCollider>();
        AI_Manager.OnDarknessAdded(this);
        //aIMovement = GetComponent<AI_Movement>();
        currentState.InitializeState(this);
        darkHitBox.enabled = false;
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

    public IEnumerator AttackCooldown(float idleTime, int animationID)
    {
        darkHitBox.enabled = true;
        if(!attacked)
            attacked = true;
        animeController.SetTrigger(animationID);
        yield return new WaitForSeconds(idleTime);
        attacked = false;
        darkHitBox.enabled = false;
    }

    public void PlayerDistanceEvaluation(Vector3 location)
    {
        playerDist = Vector3.Distance(transform.position, location);
        if(Target != null)
        {
            navTargetDist = Vector3.Distance(transform.position, Target.location.position);
        }
        else navTargetDist = -1;
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