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

    [HideInInspector]
    public Collider darkHitBox;
    public GameObject deathFX;
    public Dark_State DeathState;
    
    public bool moving, updateStates, attacked;
    public int creationID;
    public float playerDist, swtichDist, navTargetDist, stopDistance;

    public Vector3 wayPoint, pathPoint, direction;
    private bool reachedEndOfPath, wandering, targetMoved;
    
    public AI_Manager.NavigationTarget navTarget;
    private Seeker sekr;
    private AIPath aIPath;
    private Path navPath;
    private Rigidbody rigidbod;

    public Animator animeController;
    [HideInInspector]
    public int attackHash = Animator.StringToHash("Attack"),
                attackAfterHash = Animator.StringToHash("AfterAttack"),
                chaseHash = Animator.StringToHash("Chase"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death");

    void Awake()
    {
        //attackInitiationRange = 2.5f;
        stopDistance = 3;
        swtichDist = 3; 
        creationID = 0;
        navTargetDist = -1;
        updateStates = true;
        agRatingCurrent = agRatingPrevious = AggresionRating.Idling;
        attacked = moving = wandering = targetMoved = reachedEndOfPath = false;
    }

    void Start () {
        AI_Manager.OnDarknessAdded(this);
        currentState.InitializeState(this);
        sekr.pathCallback += PathComplete;
        animeController = GetComponentInChildren<Animator>();
        darkHitBox = GetComponent<CapsuleCollider>();
        sekr = GetComponent<Seeker>();
        aIPath = GetComponent<AIPath>();
        rigidbod = gameObject.GetComponentInChildren<Rigidbody>();
        darkHitBox.enabled = false;
        direction = new Vector3();
	}

    private void TargetChanged()
    {
        
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

    void FixedUpdate()
    {
        if(moving && navPath != null)
        {
            direction = Vector3.Normalize(navPath.vectorPath[1] - this.transform.position);
            rigidbod.AddForce(direction); //* speed);
            //rigidbod.MovePosition(direction * speed * Time.deltaTime);
        }
    }

    public void UpdatePath()
    {
        if(sekr.IsDone())
            CreatePath(navTarget.location.position);
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

    private void PathTargetUpdated(bool b)
    {
        targetMoved = b;
    }

    /*private void BlockPathNodes(Path p)
    {
        foreach(GraphNode n in p.path)
        {
            bProvider.blockedNodes.Add(n);
        }
        //Debug.Break();
    }*/

    public void CreatePath(Vector3 endPoint)
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
    }

    public IEnumerator AttackCooldown(float idleTime)
    {
        darkHitBox.enabled = true;
        //animeController.SetTrigger(animationID);
        yield return new WaitForSeconds(idleTime);
        attacked = false;
        darkHitBox.enabled = false;
    }

    public void PlayerDistanceEvaluation(Vector3 location)
    {
        playerDist = Vector3.Distance(transform.position, location);
        if(navTarget != null)
        {
            navTargetDist = Vector3.Distance(transform.position, navTarget.location.position);
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

    class Blocker : ITraversalProvider
    {
        public HashSet<GraphNode> blockedNodes = new HashSet<GraphNode>();
        public bool CanTraverse(Path path, GraphNode node)
        {
            return DefaultITraversalProvider.CanTraverse(path, node) && !blockedNodes.Contains(node);
        }

        public uint GetTraversalCost(Path path, GraphNode node)
        {
            return DefaultITraversalProvider.GetTraversalCost(path, node);
        }
    }
}