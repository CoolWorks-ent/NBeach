using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Pathfinding;

namespace Darkness
{
    public class DarknessMinion : MonoBehaviour 
    {
        public enum AggresionRating {Aggressive, Passive, Patrol}
        public AggresionRating agRatingCurrent;

        [HideInInspector]
        public Vector3 nextPosition;

        [HideInInspector]
        public Collider darkHitBox;
        [HideInInspector]
        public Quaternion nextRotation;
        
        public Dark_State DeathState, currentState;
        
        //public bool attackOnCooldown, attackActive, attackEnded, idleOnCooldown, idleActive, movementOnCooldown;
        public float switchTargetDistance, pathUpdateTime, approachDistance;

        [HideInInspector]
        public int creationID;

        [HideInInspector]
        public float targetDistance, patrolDistance, playerDist;
        [HideInInspector]
        public Vector3 playerDirection;
        public bool reachedEndOfPath {get{ return aIPath.reachedEndOfPath;}}
        public bool activeCooldownsComplete{get{return activeTimedActions.Count <= 0;}}

        [SerializeField]
        private bool moving;
        private Rigidbody rigidbod;
        private float stateTime, actionTimer;
        private Seeker sekr;
        private AIPath aIPath;
        private Path navPath;
        private Animator animeController;

        private Dictionary<Dark_Action.ActionType, Dark_Action.ActionCooldownInfo> actionsOnCooldown, activeTimedActions;

        [HideInInspector]
        public int attackHash, chaseHash, idleHash, deathHash;

        public enum NavTargetTag {Attack, Patrol, Player}
        public NavigationTarget navTarget;
        public GameObject deathFX;

        void Awake()
        {
            //attackInitiationRange = 2.5f;
            creationID = 0;
            actionTimer = 0;
            pathUpdateTime = 1.6f;
            patrolDistance = switchTargetDistance * 2;
            //agRatingCurrent = AggresionRating.Idling;
            animeController = GetComponentInChildren<Animator>();
            darkHitBox = GetComponent<CapsuleCollider>();
            sekr = GetComponent<Seeker>();
            aIPath = GetComponent<AIPath>();
            rigidbod = gameObject.GetComponentInChildren<Rigidbody>();
            activeTimedActions = new Dictionary<Dark_Action.ActionType, Dark_Action.ActionCooldownInfo>();
            actionsOnCooldown = new Dictionary<Dark_Action.ActionType, Dark_Action.ActionCooldownInfo>();
            ResetCooldowns();
            //idleActive = true;
            attackHash = Animator.StringToHash("Attack");
            chaseHash = Animator.StringToHash("Chase");
            idleHash = Animator.StringToHash("Idle");
            deathHash = Animator.StringToHash("Death");
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

        #region State Handling
        private void UpdateCurrentState()
        {
            currentState.UpdateState(this);
            //if(animeController.animation.)
        }

        public void ChangeState(Dark_State nextState)
        {
            currentState = nextState;
            ResetCooldowns();
            currentState.InitializeState(this);
        }
        #endregion

        #region Cooldown Handling

        public bool CheckTimedActions(Dark_Action.ActionType actType)
        {
            return activeTimedActions.ContainsKey(actType);
        }

        public bool CheckActionsOnCooldown(Dark_Action.ActionType actType)
        {
            return actionsOnCooldown.ContainsKey(actType);
        }
        
        public void ProcessActionCooldown(Dark_Action.ActionType actType, float durationTime, float coolDownTime) //TODO just return a bool so 
        {
            if(!activeTimedActions.ContainsKey(actType) && !actionsOnCooldown.ContainsKey(actType))
            {
                if(durationTime > 0)
                {
                    activeTimedActions.Add(actType, new Dark_Action.ActionCooldownInfo(durationTime, coolDownTime, StartCoroutine(ActiveActionTimer(actType))));
                }
                else
                {
                    actionsOnCooldown.Add(actType, new Dark_Action.ActionCooldownInfo(durationTime, coolDownTime, StartCoroutine(ActionCooldownTimer(actType))));
                } 
            }
        }

        private IEnumerator ActiveActionTimer(Dark_Action.ActionType actType)
        {
            Dark_Action.ActionCooldownInfo info;
            if(activeTimedActions.TryGetValue(actType, out info))
            {
                yield return new WaitForSeconds(info.durationTime);
                if(info.coolDownTime > 0 && !actionsOnCooldown.ContainsKey(actType))
                {
                    actionsOnCooldown.Add(actType, info);
                    info.activeCoroutine = StartCoroutine(ActionCooldownTimer(actType));
                }
            }
            yield break;
        }

        private IEnumerator ActionCooldownTimer(Dark_Action.ActionType actType)
        {
            Dark_Action.ActionCooldownInfo info;
            if(activeTimedActions.TryGetValue(actType, out info))
            {
                yield return new WaitForSeconds(info.coolDownTime);
                actionsOnCooldown.Remove(actType);
            }
            yield break;
        }

        private void ResetCooldowns()
        {
            //attackOnCooldown = moving = idleOnCooldown = movementOnCooldown = attackActive = attackEnded = idleActive = false;
            foreach(KeyValuePair<Dark_Action.ActionType, Dark_Action.ActionCooldownInfo> info in activeTimedActions)
            {
                StopCoroutine(info.Value.activeCoroutine);
            }
            StopAllCoroutines();
            //activeActionCooldowns.Clear();
        }
        #endregion

        #region Pathing

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

        private void PathComplete(Path p)
        {
            Debug.LogWarning("path callback complete");
            
            //p.Claim(this);
            //BlockPathNodes(p);
            if(!p.error)
            {
                //if(navPath != null) 
                    //navPath.Release(this);
                navPath = p;
            }
            else 
            {
                //p.Release(this);
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
        #endregion

        public void UpdateAnimator(int atType) 
        {
            //Ensure the correct animation is played in the animator. 
            animeController.SetTrigger(atType);
        }

        public void AggressionChanged(AggresionRating agR)
        {
            //if(agR != agRatingCurrent)
                //agRatingPrevious = agRatingCurrent;
            agRatingCurrent = agR;
        }

        ///<summary>Called once per tick by the Darkness_Manager. Updates the playerDist, attackNavTarget/patrolNavTarget targetDistance values. If either NavTarget is inactive the distance is set to -1</summary>
        public void UpdateDistanceEvaluation(Vector3 playerLocation)
        {
            playerDist = Vector3.Distance(transform.position, playerLocation);
            playerDirection = playerLocation - transform.position;  
        }

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
    }
}