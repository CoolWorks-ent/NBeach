using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Pathfinding;

namespace Darkness
{
    public class DarknessMinion : MonoBehaviour 
    {
        public enum AggresionRating {Aggressive, Passive}
        
        public Vector3 nextPosition;
        public Quaternion nextRotation;
        public AggresionRating agRatingCurrent, agRatingPrevious;

        [HideInInspector]
        public Collider darkHitBox;
        
        public Dark_State DeathState, currentState;
        
        public bool updateStates, attackOnCooldown, attackActive, attackEnded, idleOnCooldown, movementOnCooldown;
        public float switchTargetDistance, pathUpdateTime, approachDistance;

        [HideInInspector]
        public int creationID;

        [HideInInspector]
        public float targetDistance, patrolDistance, playerDist;

        public Vector3 playerDirection;
        public bool reachedEndOfPath {get{ return aIPath.reachedEndOfPath;}}
        public bool activeCooldownsComplete{get{return activeActionCooldowns.Count <= 0;}}

        [SerializeField]
        private bool moving;
        private Rigidbody rigidbod;
        private float stateTime, actionTimer;
        private Seeker sekr;
        private AIPath aIPath;
        private Path navPath;
        private Animator animeController;
        private List<int> actionCooldownList;
        private string timedActionStatus;

        private List<Dark_Action.ActionCooldownInfo> activeActionCooldowns;

        //private Dictionary<Dark_State, int> StateSelectionHistory; //Used to weigh selection of a new state. If a state has been used a lot try to pick one used less often

        [HideInInspector]
        public int attackHash = Animator.StringToHash("Attack"),
                    chaseHash = Animator.StringToHash("Chase"),
                    idleHash = Animator.StringToHash("Idle"),
                    deathHash = Animator.StringToHash("Death");

        public enum NavTargetTag {Attack, Patrol, Player}
        public NavigationTarget navTarget;

        ///<summary>NavigationTarget is used by Darkness for pathfinding purposes. </summary>
        public struct NavigationTarget
        { 
            public int weight;
            //public bool active;
            private float groundElavation;

            public Vector3 position;
            private Vector3 positionOffset;
            //public Transform locationInfo { 
            //	get {return transform; }}

            private NavTargetTag targetTag;
            public NavTargetTag navTargetTag { get{ return targetTag; }}

            ///<param name="iD">Used in AI_Manager to keep track of the Attack points. Arbitrary for the Patrol points.</param>
            ///<parem name="offset">Only used on targets that will be used for attacking. If non-attack point set to Vector3.Zero</param>
            public NavigationTarget(Vector3 loc, Vector3 offset, float elavation, NavTargetTag ntTag)//, bool act)
            {
                position = loc;
                groundElavation = elavation;
                //if(parent != null)
                //	transform.parent = parent;
                positionOffset = offset;
                targetTag = ntTag;
                weight = 0;
                //active = false;
                //assignedDarknessIDs = new int[assignmentLimit];
            }

            public void UpdateLocation(Vector3 loc)
            {
                //if(!applyOffset)
                //	position = new Vector3(loc.x, groundElavation, loc.y);
                //else 
                position = new Vector3(loc.x, groundElavation, loc.y) + positionOffset;
            }
        }

        void Awake()
        {
            //attackInitiationRange = 2.5f;
            actionCooldownList = new List<int>();
            creationID = 0;
            actionTimer = 0;
            pathUpdateTime = 1.6f;
            patrolDistance = switchTargetDistance * 2;
            updateStates = true;
            //agRatingCurrent = AggresionRating.Idling;
            animeController = GetComponentInChildren<Animator>();
            darkHitBox = GetComponent<CapsuleCollider>();
            sekr = GetComponent<Seeker>();
            aIPath = GetComponent<AIPath>();
            rigidbod = gameObject.GetComponentInChildren<Rigidbody>();
            activeActionCooldowns = new List<Dark_Action.ActionCooldownInfo>();
            ResetCooldowns();
        }

        void Start () {
            //patrolNavTarget = new Darkness_Manager.NavigationTarget(this.transform.position, Vector3.zero, Darkness_Manager.Instance.ground, 10, Darkness_Manager.NavTargetTag.Patrol);
            Darkness_Manager.OnDarknessAdded(this); //Called in Start instead of at instantiation becuase the Darkness needs to be fully setup before the Manager approves behavior changes.
            Darkness_Manager.UpdateDarkStates += UpdateCurrentState;
            Darkness_Manager.DistanceUpdate += UpdateDistanceEvaluation;
            currentState.InitializeState(this);
            sekr.pathCallback += PathComplete;
            darkHitBox.enabled = false;
            playerDirection = new Vector3();
            //patrolNavTarget.UpdateLocation(Vector3.zero, false);
        }

        void FixedUpdate()
        {
            if(moving && navPath != null)
            {
                aIPath.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);

                aIPath.FinalizeMovement(nextPosition, nextRotation);
            }
        }   

        private void UpdateCurrentState()
        {
            if(activeActionCooldowns.Count > 0)
            {
                UpdateCooldowns();
            }
            currentState.UpdateState(this);
            //if(animeController.animation.)
        }

        public void ProcessActionCooldown(Dark_Action.ActionCooldownType actionType, float coolDownTime) //return true if the if succesful and false if the cooldown is already active
        {
            /*if(!activeActionCooldowns.ContainsKey(actionName))
            {
                activeActionCooldowns.Add(actionName, new Dark_Action.ActionCooldownInfo(Time.deltaTime, coolDownTime));
            }*/

            switch(actionType)
            {
                case Dark_Action.ActionCooldownType.Idle: //basically idle will request movement be on cooldown while idle. then set the idle cooldown after the idle duration
                    if(idleOnCooldown == false)
                    {
                        activeActionCooldowns.Add(new Dark_Action.ActionCooldownInfo(actionType, Time.deltaTime, coolDownTime));
                        idleOnCooldown = true;
                    }
                    break;
                case Dark_Action.ActionCooldownType.Attack:
                    if(attackOnCooldown == false)
                    {
                        activeActionCooldowns.Add(new Dark_Action.ActionCooldownInfo(actionType, Time.deltaTime, coolDownTime));
                        attackOnCooldown = true;
                    }
                    break;
                case Dark_Action.ActionCooldownType.Movement:
                    if(movementOnCooldown == false)
                    {
                        activeActionCooldowns.Add(new Dark_Action.ActionCooldownInfo(actionType, Time.deltaTime, coolDownTime));
                        movementOnCooldown = true;
                    }
                    break;
                case Dark_Action.ActionCooldownType.AttackActive:
                    if(attackActive == false)
                    {
                        activeActionCooldowns.Add(new Dark_Action.ActionCooldownInfo(actionType, Time.deltaTime, coolDownTime));
                        attackActive = true;
                        attackEnded = false;
                    }
                    break;
                default: 
                    return;
            }
            
            //TODO just make sure this isn't being set multiple times
            //start an ienumarator that will clear this status upon execution
            /*if(timedActionStatus == "")
            {
                timedActionStatus = actionName;
                StartCoroutine(WaitTimer(coolDownTime));
            }*/
        }

        private void UpdateCooldowns() //update all cooldown statuses 
        {
            foreach(Dark_Action.ActionCooldownInfo acInfo in activeActionCooldowns)
            {
                if(Time.deltaTime - acInfo.initialTime >= acInfo.coolDownTime)
                {
                    activeActionCooldowns.Remove(acInfo);
                    switch(acInfo.acType)
                    {
                        case Dark_Action.ActionCooldownType.Idle: 
                            idleOnCooldown = false;
                            break;
                        case Dark_Action.ActionCooldownType.Attack:
                            attackOnCooldown = false;
                            break;
                        case Dark_Action.ActionCooldownType.Movement:
                            movementOnCooldown = false;
                            break;
                        case Dark_Action.ActionCooldownType.AttackActive:
                            attackActive = false;
                            attackEnded = true;
                            break;
                    }
                }
            }
        }

        ///<summary>Called in state update loop to update path</summary>
        public void UpdatePath()
        {
            if(sekr.IsDone())
            {
                //if(navTarget.active)
                CreatePath(navTarget.position);
                //else if(patrolNavTarget.active)
                //    CreatePath(patrolNavTarget.position);
            }
            //yield return new WaitForSeconds(pathUpdateTime);
        }

        public void ChangeState(Dark_State nextState)
        {
            //currentState.ExitState(this);
            currentState = nextState;
            currentState.InitializeState(this);
            /*previousState = currentState;
            if(!timedState) //if the timer on a state is still active don't switch yet
            {
                if(queuedState != null && queuedState != nextState)
                {
                    if(nextState.statePriority > queuedState.statePriority)
                    {
                        currentState = nextState;
                        previousState.ExitState(this);
                        currentState.InitializeState(this);
                    }
                    else if(queuedState.statePriority > nextState.statePriority)
                    {
                        currentState = queuedState;
                        previousState.ExitState(this);
                        currentState.InitializeState(this);
                    }
                }
                else currentState = nextState;
            }
            else
            {
                if(!timedStateExiting)
                {
                    StartCoroutine(StateTransitionTimer(currentState.exitTime));
                    timedState = false;
                }            
            }  //Check to see if this state has initiated it's timer to exit bevavior*/
        }

        private void ResetCooldowns()
        {
            attackOnCooldown = moving = idleOnCooldown = movementOnCooldown = attackActive = attackEnded = false;
            activeActionCooldowns.Clear();
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

        public void ResumeMovement()
        {
            moving = true;
            sekr.pathCallback += PathComplete;
        }

        public void EndMovement()
        {
            if(!sekr.IsDone())
                sekr.CancelCurrentPathRequest();
            moving = false;
            sekr.pathCallback -= PathComplete;
        }

        public void UpdateAnimator(Dark_Action.AnimationType atType) 
        {
            //Ensure the correct animation is played in the animator. 
            switch(atType)
            {
                case Dark_Action.AnimationType.Attack:
                    animeController.SetTrigger(attackHash);
                    break;
                case Dark_Action.AnimationType.Chase:
                    animeController.SetTrigger(chaseHash);
                    break;
                case Dark_Action.AnimationType.Idle:
                    animeController.SetTrigger(idleHash);
                    break;
                /*case Dark_State.StateType.DEATH:
                    animeController.SetTrigger(deathHash);
                    break;*/
            }
        }

        public void AggressionChanged(AggresionRating agR)
        {
            if(agR != agRatingCurrent)
                agRatingPrevious = agRatingCurrent;
            agRatingCurrent = agR;
        }

        public IEnumerator AttackActivation(float idleTime)
        {
            attackOnCooldown = true;
            darkHitBox.enabled = true;
            //animeController.SetTrigger(animationID);
            yield return new WaitForSeconds(idleTime);
            attackOnCooldown = false;
            darkHitBox.enabled = false;
        }

        ///<summary>Called once per tick by the Darkness_Manager. Updates the playerDist, attackNavTarget/patrolNavTarget targetDistance values. If either NavTarget is inactive the distance is set to -1</summary>
        public void UpdateDistanceEvaluation(Vector3 playerLocation)
        {
            playerDist = Vector3.Distance(transform.position, playerLocation);
            playerDirection = playerLocation - transform.position;
            /*if(attackNavTarget.active)
            {
                attackNavTarget.targetDistance = Vector3.Distance(transform.position, attackNavTarget.position);
            }
            else attackNavTarget.targetDistance = -1;

            if(patrolNavTarget.active)
            {
                patrolNavTarget.targetDistance = Vector3.Distance(transform.position, patrolNavTarget.position);
            }
            else patrolNavTarget.targetDistance = -1;*/
        }


        /*public IEnumerator WaitTimer(float timer)
        {
            yield return new WaitForSeconds(timer);
            timedActionStatus = "";
            //cooldownActive = true;
        }*/

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag("Projectile"))
            {
                if (collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
                {
                    Debug.LogWarning("Darkness Destroyed");
                    //ChangeState(DeathState);
                }
            }
            else if(collider.gameObject.CompareTag("Player"))
            {
                //Debug.LogWarning("Darkness collided with Player");
            }
        }

    }
}