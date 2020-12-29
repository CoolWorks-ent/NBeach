using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessMinion
{
	[System.Serializable]
	public class NavigationTarget  
	{
		public enum NavTargetTag { Attack, Patrol, Neutral, Null, AttackStandby}
		
		[SerializeField]
		private bool claimed;
		[SerializeField]
		private Vector3 position, positionOffset, positionCloseToSource;

		private int claimedID;
		private float groundElavation;
		
		public Transform navTransform { get; private set; }
		public bool navTargetClaimed {  get { return claimed; } }
		public NavTargetTag navTargetTag { get; private set; }
		public Transform attackTransform { get; private set; } //TODO make private probably
		//public Vector3 navPosition { get { return position + positionOffset; } } 
		//public Vector3 closeToSrcPosition { get { return position + positionCloseToSource; } }
		//public Vector3 transformPosition { get { return objectTransform.position; } }

		/*///<param name="iD">Used in AI_Manager to keep track of the Attack points. Arbitrary for the Patrol points.</param>
		///<param name="offset">Only used on targets that will be used for attacking. If non-attack point set to Vector3.Zero</param>
		public NavigationTarget(Vector3 loc, Vector3 offset, Vector3 srcOffset, float elavation, NavTargetTag ntTag)//, bool act)
		{
			position = loc;
			groundElavation = elavation;
			positionOffset = offset;
			positionCloseToSource = srcOffset;
			navTargetTag = ntTag;
			claimed = false;
			claimedID = 0;
		}*/

		public NavigationTarget(Transform nTransform, Transform aTransform, NavTargetTag ntTag)
		{
			navTransform = nTransform;
			attackTransform = aTransform;
			navTargetTag = ntTag;
            position = this.navTransform.position;
		}

		public NavigationTarget(Vector3 loc, float elavation, NavTargetTag ntTag)
		{
			position = loc;
			groundElavation = elavation;
			positionOffset = Vector3.zero;
			positionCloseToSource = Vector3.zero;
			navTargetTag = ntTag;
			claimed = false;
			claimedID = 0;
		}

		public Vector3 GetNavPosition()
		{
			if(navTransform != null)
				return navTransform.position; 
			else return position + positionOffset; 
		}

		public Vector3 GetAttackPosition()
		{
			return attackTransform.position;
		}

		public void ClaimTarget(int cID)
		{
			claimedID = cID;
			claimed = true;
		}

		public void ReleaseTarget()
		{
			claimedID = -1;
			claimed = false;
		}

		public void UpdateLocation(Vector3 loc)
		{
			position = new Vector3(loc.x, groundElavation, loc.z);
		}
	}
}