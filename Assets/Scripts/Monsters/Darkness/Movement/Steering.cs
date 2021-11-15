using UnityEngine;

namespace DarknessMinion.Movement
{
    public class Steering
    {
        public void Seek(DirectionNode dNode, Vector2 startPos, Vector2 destination, float distanceThreshold, float playerDist)
        {
	        Vector2 direction = (destination - startPos).normalized;
        	float dotValue = Vector2.Dot(dNode.directionAtAngle, direction);
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

        public void Avoid(DirectionNode dNode, Vector2 startPos, float distanceThreshold, float raycastDistance, LayerMask avoidLayerMask)
        {
        	dNode.avoidWeight = 0;
        	RaycastHit rayHit;
        	float dotValue, distanceNorm;

        	if (Physics.SphereCast(startPos + dNode.directionAtAngle * 1.5f, 2, dNode.directionAtAngle, out rayHit, raycastDistance, avoidLayerMask, QueryTriggerInteraction.Collide))
            {
	            Vector2 dir = (rayHit.transform.position.ToVector2() - startPos).normalized;
        		dotValue = Vector2.Dot(dNode.directionAtAngle, dir);
        		distanceNorm = Vector2.Distance(startPos, rayHit.transform.position);
        		if (dotValue >= 0.6f)
        			dNode.avoidWeight += -1;
        		else if (dotValue <= 0.6f && dotValue > 0)
        			dNode.avoidWeight += -0.5f;

        		if (distanceNorm < distanceThreshold) 
        			dNode.avoidWeight += -1;
        	}
        }
    }
}