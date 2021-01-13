using UnityEngine;

namespace DarknessMinion
{
	[System.Serializable]
	public class NavigationTarget  
	{
		public enum NavTargetTag { Attack, Patrol, Neutral, Null, AttackStandby}
		
		[SerializeField]
		private Vector3 position, positionOffset;
		private float groundElavation;

		public NavTargetTag navTargetTag { get; private set; }
		public Vector3 navPosition { get { return position + positionOffset; } } 
		public Vector3 navPositionSource { get { return position; } }

		public NavigationTarget(Vector3 loc, Vector3 offset, float elavation, NavTargetTag ntTag)
		{
			position = loc;
			groundElavation = elavation;
			positionOffset = offset;
			navTargetTag = ntTag;
		}

		public void UpdateLocation(Vector3 loc)
		{
			position = new Vector3(loc.x, groundElavation, loc.z);
		}
	}
}