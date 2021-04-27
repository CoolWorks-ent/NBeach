using UnityEngine;

namespace DarknessMinion
{
	[System.Serializable]
	public class NavigationTarget  
	{
		[SerializeField]
		private Vector3 origin, positionOffset;
		private float groundElavation;
		public Vector3 navPosition { get { return origin + positionOffset; } } 

		public NavigationTarget(Vector3 start, Vector3 offset, float elavation)
		{
			origin = start;
			groundElavation = elavation;
			positionOffset = offset;
			DarkEventManager.UpdateZoneLocation += UpdateOffsetLocation;
		}

		~NavigationTarget()
		{
			DarkEventManager.UpdateZoneLocation -= UpdateOffsetLocation;
		}

		private void UpdateOffsetLocation(Vector3 updatedloc)
		{
			positionOffset = new Vector3(updatedloc.x, groundElavation, updatedloc.z);
		}
	}
}