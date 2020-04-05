﻿using System.Collections;
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

    [HideInInspector]
    public Collider darkHitBox;
    public GameObject deathFX;
    public Dark_State DeathState;
    
    public bool moving, updateStates, attacked;
    public int creationID;
    public float playerDist, swtichDist, navTargetDist, stopDistance, pathUpdateTime;

    public Vector3 wayPoint, pathPoint, playerDirection;
    private Vector3 prevPos;
    private bool reachedEndOfPath, wandering, targetMoved, lookAtPlayer;
    
    public AI_Manager.NavigationTarget navTarget;
    private Seeker sekr;
    private AIPath aIPath;
    private Path navPath;
    public Rigidbody rigidbod;

    public Animator animeController;
    [HideInInspector]
    public int attackHash = Animator.StringToHash("Attack"),
                chaseHash = Animator.StringToHash("Chase"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death");


    void Awake()
    {
        //attackInitiationRange = 2.5f;
        stopDistance = 3;
        swtichDist = 3; 
        creationID = 0;
        pathUpdateTime = 1.6f;
        navTargetDist = -1;
        updateStates = true;
        agRatingCurrent = agRatingPrevious = AggresionRating.Idling;
        attacked = moving = wandering = targetMoved = reachedEndOfPath = lookAtPlayer = false;
        animeController = GetComponentInChildren<Animator>();
        darkHitBox = GetComponent<CapsuleCollider>();
        sekr = GetComponent<Seeker>();
        aIPath = GetComponent<AIPath>();
        rigidbod = gameObject.GetComponentInChildren<Rigidbody>();
    }

    void Start () {
        AI_Manager.OnDarknessAdded(this);
        currentState.InitializeState(this);
        sekr.pathCallback += PathComplete;
        darkHitBox.enabled = false;
        playerDirection = new Vector3();
	}

    public void ChangeState(Dark_State nextState)
    {
        previousState = currentState;
        currentState = nextState;
        previousState.ExitState(this);            
        currentState.InitializeState(this);
    }

    void FixedUpdate()
    {
        if(lookAtPlayer)
        {
            //TODO Rotate rigidbody manually in the background to face the player
        }
        if(moving && navPath != null)
        {
            Vector3 nextPosition;
            Quaternion nextRotation;

            aIPath.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);

            aIPath.FinalizeMovement(nextPosition, nextRotation);
        }
    }

    ///<summary>Called in state update loop to update path</summary>
    public IEnumerator UpdatePath()
    {
        if(sekr.IsDone())
            CreatePath(navTarget.presence.position);
        yield return new WaitForSeconds(pathUpdateTime);
    }

    private void PathComplete(Path p)
    {
        Debug.LogWarning("path callback complete");
        p.Claim(this);
        //BlockPathNodes(p);
        if(!p.error)
        {
            if(navPath != null) 
                navPath.Release(this);
            navPath = p;
        }
        else 
        {
            p.Release(this);
            Debug.LogError("Path failed calculation for " + this + " because " + p.errorLog);
        }
    }

    /*private void BlockPathNodes(Path p)
    {
        foreach(GraphNode n in p.path)
        {
            bProvider.blockedNodes.Add(n);
        }
        //Debug.Break();
    }*/

    private void CreatePath(Vector3 endPoint)
    {
        //bProvider.blockedNodes.Clear();
        Path p = ABPath.Construct(transform.position, endPoint);
        //p.traversalProvider = bProvider;
        sekr.StartPath(p, null);
        p.BlockUntilCalculated();
    }

    public void EndMovement()
    {
        if(!sekr.IsDone())
            sekr.CancelCurrentPathRequest();
        moving = false;
        sekr.pathCallback -= PathComplete;
        StopCoroutine(UpdatePath());
    }

    public void UpdateAnimator()
    {
        //if()
    }

    public IEnumerator AttackCooldown(float idleTime)
    {
        darkHitBox.enabled = true;
        //animeController.SetTrigger(animationID);
        yield return new WaitForSeconds(idleTime);
        attacked = false;
        darkHitBox.enabled = false;
    }

    public void PlayerDistanceEvaluation(Vector3 playerLocation)
    {
        playerDist = Vector3.Distance(transform.position, playerLocation);
        playerDirection = playerLocation - transform.position;
        if(navTarget != null)
        {
            navTargetDist = Vector3.Distance(transform.position, navTarget.presence.position);
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