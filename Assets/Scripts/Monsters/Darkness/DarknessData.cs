using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darkness
{
	public class DarknessData : ScriptableObject
	{
		[SerializeField]
		public readonly Transform player;
		
		public Vector3 DirectionToPlayer(Vector3 pos)
		{
			return (player.position - pos).normalized;
		}

		public float PlayerDistance(Vector3 position)
		{
			return Vector3.Distance(position, player.position);
		}
	}
}