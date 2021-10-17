using UnityEngine;

namespace DarknessMinion
{
	[System.Serializable]
	public class NavigationTarget  
	{
		[SerializeField]
		private Vector3 origin, positionOffset;
		private float groundElevation;
		public Vector3 navPosition { get { return origin + positionOffset; } } 

		public NavigationTarget(Vector3 start, Vector3 offset, float elevation)
		{
			origin = start;
			groundElevation = elevation;
			positionOffset = offset;
			DarkEventManager.UpdateZoneLocation += UpdateOffsetLocation;
		}

		~NavigationTarget()
		{
			DarkEventManager.UpdateZoneLocation -= UpdateOffsetLocation;
		}

		private void UpdateOffsetLocation(Vector3 updatedloc)
		{
			positionOffset = new Vector3(updatedloc.x, groundElevation, updatedloc.z);
		}
	}
}