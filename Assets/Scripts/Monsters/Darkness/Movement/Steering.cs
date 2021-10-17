using UnityEngine;

namespace DarknessMinion.Movement
{
    public class Steering
    {
        public void Seek(DirectionNode dNode, Vector3 direction, float distanceThreshold, float playerDist)
        {
        	float dotValue = Vector3.Dot(dNode.directionAtAngle, direction);
        	dNode.SetDirLength(direction.magnitude);

        	if (playerDist > distanceThreshold)
        	{
        		if (dotValue > 0)
        			dNode.seekWeight = dotValue;
        		else if (dotValue == 0)
        			dNode.seekWeight = 0.1f;
        		else dNode.seekWeight = 0.05f;
        	}
        	else
        	{
        		if (dotValue >= 0.8f && dotValue <= 0.9f)
        			dNode.seekWeight = dotValue + 0.2f;
        		else if (dotValue < 0.8f && dotValue > 0.7f)
        			dNode.seekWeight = dotValue + 0.1f;
        		else if (dotValue > 0.9f)
        			dNode.seekWeight = dotValue - 0.1f;
        		else if (dotValue < 0.7f && dotValue > 0)
        			dNode.seekWeight = dotValue;
        		else dNode.seekWeight = 0.1f;
        	}
        }

        public void Avoid(DirectionNode dNode, Vector3 position, float distanceThreshold, float raycastDistance, LayerMask avoidLayerMask)
        {
        	dNode.avoidWeight = 0;
        	RaycastHit rayHit;
        	float dotValue, distanceNorm;

        	if (Physics.SphereCast(position + dNode.directionAtAngle * 1.5f, 2, dNode.directionAtAngle, out rayHit, raycastDistance, avoidLayerMask, QueryTriggerInteraction.Collide))
        	{
        		dotValue = Vector3.Dot(dNode.directionAtAngle, rayHit.transform.position.normalized);
        		distanceNorm = Vector3.Distance(position, rayHit.transform.position);
        		if (dotValue >= 0.6f)
        			dNode.avoidWeight += -1;
        		else if (dotValue <= 0.6f && dotValue > 0)
        			dNode.avoidWeight += -0.5f;

        		if (distanceNorm < distanceThreshold) //highPrecsion
        			dNode.avoidWeight += -1;
        	}
        }
    }
}