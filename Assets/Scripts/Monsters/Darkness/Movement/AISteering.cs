using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Darkness.Movement
{
    [RequireComponent(typeof(MovementController))]
    public class AISteering : MonoBehaviour, IInputInterpreter
    {
	    public Vector2 currentMovementDirection { get; private set; } 
	    public MovementController movementController { get; private set; }
	    public float targetDist { get; private set; }

	    public Transform Target
	    {
		    get
		    {
			    if (!target)
				    target = DarknessManager.Instance.playerTransform;
			    return target;
		    }
	    }

	    private Transform target;
        
	    [Header("Obstacle Avoidance Related")]
	    [Tooltip("How much difference between the next direction")]
        [SerializeField, Range(0.1f, 1)]
        private float changeDirectionThreshold;

        [SerializeField, Range(1, 3.5f)]
        private float agentBoundsScalar, lookAheadSpeedMod;
        
        [SerializeField] 
        private LayerMask obstacleLayerMask;
        
        private int bestDirectionIndex;
        private DirectionNode[] directionNodes;
        private HashSet<Avoidable> avoidableObstacles;
        
        private InputInfo inputInfo;
        private Collider colliderBounds;

        private void Awake()
        {
            currentMovementDirection = Vector2.zero;
            inputInfo = new InputInfo();
            CreateDirectionNodes(4);
            avoidableObstacles = new HashSet<Avoidable>(new AvoidableComparer());
            movementController = GetComponent<MovementController>();
            colliderBounds = GetComponent<Collider>();
        }
        
        void OnEnable() { DarkEventManager.UpdateDarknessDistance += DistanceEvaluation; }
        void OnDisable() { DarkEventManager.UpdateDarknessDistance -= DistanceEvaluation; }
        
        public void DistanceEvaluation()
        {
	        targetDist = Vector2.Distance(transform.position.ToVector2(), Target.position.ToVector2());
        }

        public void CreateDirectionNodes(int angleAmounts)
        {
            directionNodes = new DirectionNode[angleAmounts];
            float angle, dAngle = 0;
            for(int i = 0; i < directionNodes.Length; i++)
            {
                dAngle = ((360.0f / directionNodes.Length) * (float)i);
                angle = Mathf.Deg2Rad * dAngle;
                DirectionNode n = new DirectionNode(angle);
                directionNodes[i] = n;
            }
            bestDirectionIndex = 0;
        }

        public void ResetMovement()
        {
            currentMovementDirection = Vector2.zero;
            inputInfo = new InputInfo();
        }

        public void SetMovementDirection(params Vector2[] directions)
        {
	        Vector2 finalDir = Vector2.zero;
	        foreach (Vector2 dir in directions)
	        {
		        finalDir += dir;
	        }
            currentMovementDirection = finalDir;
        }
        
        public bool IsFacingTarget(float minimumValue)
        {
	        float closeness = Vector3.Dot(TargetDirection(), transform.forward);
	        return closeness >= minimumValue;
        }
        
        public Vector3 TargetDirection()
        {
	        return (Target.position - transform.position).normalized;
        }
        
        public InputInfo GetInputInfo()
        {
	        if (currentMovementDirection == Vector2.zero)
		        return new InputInfo();
	        inputInfo.inputDirection = currentMovementDirection;
            return inputInfo;
        }

        public Vector2 GetMovementDirection()
        {
            return currentMovementDirection;
        }

        public void SetInputInfo(Vector2 direction, float speedModifier = 0)
        {
	        inputInfo.inputDirection = direction;
	        inputInfo.maxSpeedModifier = speedModifier;
        }

        public Vector2 GetLocalNavDirection(Vector2 destination) 
        {
            foreach (DirectionNode dNode in directionNodes)
            {
                //ContextSeek(dNode, destination);
                //ContextAvoid(dNode);
            }
            
            bestDirectionIndex = 0;
            for (int i = 0; i < directionNodes.Length; i++)
            {
                if(i+1 <= directionNodes.Length-1)
                {
                    if (directionNodes[bestDirectionIndex].combinedWeight < directionNodes[i].combinedWeight)
                    {
                        float directionDifference = Mathf.Abs(directionNodes[bestDirectionIndex].combinedWeight -
                                                              directionNodes[i].combinedWeight);
                        if(directionDifference > changeDirectionThreshold)
                            bestDirectionIndex = i;
                    }
                }
            }

            return directionNodes[bestDirectionIndex].directionAtAngle; 
        }

        #region Steering functions
        public Vector2 Seek(Vector2 target)
	    {
		    return (target - movementController.GetPosition()).normalized;
	    }

	    public Vector2 Flee(Vector2 target)
	    {
		    return (movementController.GetPosition() - target).normalized;
	    }
	    
	    public Vector2 Arrive(Vector2 targetPos, float slowdownDistance = 2.5f)
	    {
		    float distanceTo = Vector2.Distance(targetPos, movementController.GetPosition());
		    if (distanceTo < slowdownDistance) //distance at 5
		    {
			    //use the distance to decrease max speed the closer we get to the target
			    float speedMod = Mathf.Min(movementController.movementSpeedMax / distanceTo, 
				    movementController.movementSpeedMax);
			    inputInfo.maxSpeedModifier = -speedMod;
		    }

		    return Seek(targetPos);
	    }

	    public Vector2 Pursuit(MovementController targetMovement)
	    {
		    //Get context target velocity normalized and current position
		    //check if the direction is the same 

		    Vector2 toTarget = targetMovement.GetPosition() - movementController.GetPosition();
		    float directtionAlignment = Vector2.Dot(toTarget, currentMovementDirection);
		    if (directtionAlignment > 0.85f)
			    return Seek(targetMovement.GetPosition());
		    float lookAheadTime = toTarget.magnitude / (movementController.movementSpeedMax + targetMovement.GetVelocity().magnitude);
		    
		    return Seek(targetMovement.GetPosition() + targetMovement.inputDirection * lookAheadTime);
	    }

	    public Vector2 Evade(MovementController pMovement)
	    {
		    Vector2 toPursuer = pMovement.GetPosition() - movementController.GetPosition();

		    float lookAheadTime = toPursuer.magnitude / (movementController.movementSpeedMax + pMovement.GetVelocity().magnitude);

		    return Flee(pMovement.GetPosition() + pMovement.GetVelocity() * lookAheadTime);
	    }

	    public Vector2 Wander(float wanderRadius, float wanderDistance, Vector2 wanderTarget, float wanderDisplacement = 0.5f)
	    {
		    //take a point on a circle
		    //displace that point slightly in one direction every frame
		    //place the new displaced point back onto the circle
		    Vector2 wTarget = wanderTarget;
			wTarget += new Vector2( Random.Range(-1, 1), Random.Range(-1, 1)) * wanderDisplacement; // small vector to displace the target by each frame
			wTarget = wTarget.normalized * wanderRadius;
			wanderTarget = wTarget;
			wanderTarget = movementController.GetPosition() + (wTarget * wanderDistance);

			return Seek(wanderTarget);
	    }

	    public Vector2 ObstacleAvoidance(float discardDistance = 2.5f, float avoidanceForce = 1.25f, float brakeWeight = 0.2f)
	    {
		    
		    Vector2 avoidanceVector = Vector2.zero;
		    float distanceScaled = Mathf.Max(1 ,(lookAheadSpeedMod * movementController.GetVelocity().magnitude));

		    RaycastHit hitInfo;
			//if I hit something with this boxcast store the obstacle, if already stored update the hitPosition
			if (Physics.BoxCast(transform.position, colliderBounds.bounds.size * agentBoundsScalar, transform.forward,
				out hitInfo, Quaternion.identity, distanceScaled, obstacleLayerMask))
			{
				int iD = hitInfo.transform.GetInstanceID();
				Bounds obstacleBounds = new Bounds(hitInfo.transform.position, hitInfo.collider.bounds.size * agentBoundsScalar);
				Avoidable a = new Avoidable(hitInfo.transform, obstacleBounds, hitInfo.point.ToVector2(), iD);
				if (avoidableObstacles.Contains(a))
				{
					foreach (Avoidable avoidable in avoidableObstacles)
					{
						if (avoidable.comparer.Equals(avoidable, a))
							avoidable.UpdateHitPosition(hitInfo.point.ToVector2());
					}
				}
				else avoidableObstacles.Add(a);
			}
			
			//for each obstacle in the list create a force away from those obstacles if they are within distance of forward direction 
			if (avoidableObstacles.Count > 0)
			{
				Vector3 forward = transform.forward;
				Vector3 position = transform.position;
				List<Avoidable> entriesToDiscard = new List<Avoidable>();
				foreach (Avoidable avoidable in avoidableObstacles)
				{
					//Check to see if the obstacle is behind us or too far away, if true -> mark for deletion, continue
					Vector3 obstaclePos = avoidable.transform.position;
					Vector3 obstacleDirection = (obstaclePos - transform.position).normalized;
					Vector2 hitPointXZ = (avoidable.hitPosition);
					Vector2 closestPoint = colliderBounds.ClosestPointOnBounds(obstaclePos).ToVector2();
					
					float distanceToObject = Vector2.Distance(hitPointXZ,  closestPoint); 
					float obstacleAgentDot = Vector3.Dot(obstacleDirection, transform.forward);
					if (obstacleAgentDot <= 0.15f || (distanceToObject > discardDistance * distanceScaled)) //TODO check to see if we are likely to collide still?
					{
						entriesToDiscard.Add(avoidable);
						continue;
					}
					
					//scale the amount of avoidance based on the distance to the obstacle point hit
					Vector2 obstacleGlobalXZ = obstaclePos.ToVector2();
					float lateralForce = (hitPointXZ.x - obstacleGlobalXZ.x);
					float brakingForce = (hitPointXZ.y - obstacleGlobalXZ.y); 
					float multiplierBase = 1;
					
					//check to see if we are head on to collide with the object 
					Bounds obstacleBounds = avoidable.GetBounds();
					Ray ray = new Ray(position, forward);
					if (obstacleBounds.IntersectRay(ray))
					{
						multiplierBase = avoidanceForce;
					}
					float forceMultiplier = multiplierBase + avoidanceForce / distanceToObject;
					lateralForce *= forceMultiplier;
					brakingForce *= (brakeWeight);
					avoidanceVector += new Vector2(lateralForce, brakingForce);
				}

				if (entriesToDiscard.Count > 0)
				{
					for (int i = 0; i <= entriesToDiscard.Count-1; i++)
					{
						avoidableObstacles.Remove(entriesToDiscard[i]);
					}
				}
			}
			return avoidanceVector.normalized;
	    }
	    


    #endregion

    #if UNITY_EDITOR
	    void OnDrawGizmosSelected()
	    {
		    if (Application.isPlaying)
		    {
			    Color col = Color.red;
			    Vector3 lineStart, lineEnd;

			    foreach (DirectionNode dir in directionNodes)
			    {
				    if (dir == directionNodes[bestDirectionIndex])
					    col = new Color(1f, 0.56f, 0.03f);
				    else if (dir.combinedWeight > 0.9f)
					    col = Color.green;
				    else if (dir.combinedWeight > 0.7f && dir.combinedWeight < 0.9f)
					    col = Color.cyan;
				    else if (dir.combinedWeight < 0.7f && dir.combinedWeight > 0.5f)
					    col = Color.yellow;
				    else if (dir.combinedWeight < 0.5f && dir.combinedWeight > 0)
					    col = Color.magenta;
				    else col = Color.white;
				    lineStart = transform.position + dir.directionAtAngle.ToVector3();
				    lineEnd = dir.directionAtAngle.ToVector3(1) * dir.combinedWeight + transform.position;
				    Debug.DrawLine(lineStart, lineEnd, col);
			    }
		    }
	    }
    #endif
    }
}