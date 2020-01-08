using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */
public class Darkness : MonoBehaviour {

    public enum AggresionRating {Attacking = 1, CatchingUp, Idling, Wandering}
    public AggresionRating agRatingCurrent, agRatingPrevious;
    public Dark_State previousState, currentState;
    public AI_Manager.NavigationTarget Target;
    public AIPath pather;
    public Seeker sekr;
    
    public int actionIdle, creationID;
    
    [HideInInspector]
    public int attackHash = Animator.StringToHash("Attack"),
                attackAfterHash = Animator.StringToHash("AfterAttack"),
                chaseHash = Animator.StringToHash("Chase"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death"),
                wanderHash = Animator.StringToHash("Wander");
    public bool updateStates;
    public float stateUpdateRate, attackInitiationRange, waitRange, stopDist, targetDist;
    //public AIDestinationSetter aIDestSet;
    public GameObject deathFX;
    public Dark_State DeathState;
    public Animator animeController;

    void Awake()
    {
        actionIdle = 3;
        attackInitiationRange = 3.5f;
        waitRange = 10f;
        stopDist = 1;
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
        //StartCoroutine(ExecuteCurrentState());
        //aIMovement.target = Target;
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
            previousState = currentState;
            currentState = nextState;
            currentState.InitializeState(this);
        }
    }

    public void DistanceEvaluation()
    {
        targetDist = Vector3.Distance(transform.position, Target.location.position);
    }

    public void AggressionChanged(AggresionRating agR)
    {
        if(agR != agRatingCurrent)
            agRatingPrevious = agRatingCurrent;
		agRatingCurrent = agR;
    }

    public bool TargetWithinDistance(float range)
    {
        if(Vector3.Distance(transform.position, Target.location.position) <= range)
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
            }
        }
        else if(collider.gameObject.CompareTag("Player"))
        {
            //Debug.LogWarning("Darkness collided with Player");
        }
    }
}