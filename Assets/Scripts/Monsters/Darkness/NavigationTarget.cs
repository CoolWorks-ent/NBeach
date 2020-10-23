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
		private Vector3 position, positionOffset;

		private int claimedID;
		private float groundElavation;
		private readonly NavTargetTag targetTag;
		

		public bool navTargetClaimed {  get { return claimed; } }
		public NavTargetTag navTargetTag { get { return targetTag; } }
		public Vector3 navPosition { get { return position + positionOffset; } }

		public Vector3 srcPosition { get { return position; } }

		///<param name="iD">Used in AI_Manager to keep track of the Attack points. Arbitrary for the Patrol points.</param>
		///<param name="offset">Only used on targets that will be used for attacking. If non-attack point set to Vector3.Zero</param>
		public NavigationTarget(Vector3 loc, Vector3 offset, float elavation, NavTargetTag ntTag)//, bool act)
		{
			position = loc;
			groundElavation = elavation;
			//if(parent != null)
			//	transform.parent = parent;
			positionOffset = offset;
			targetTag = ntTag;
			claimed = false;
			claimedID = 0;
			//active = false;
			//assignedDarknessIDs = new int[assignmentLimit];
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