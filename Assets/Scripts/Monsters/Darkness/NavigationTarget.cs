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
		}

		public void UpdateOriginLocation(Vector3 updatedloc)
		{
			origin = new Vector3(updatedloc.x, groundElavation, updatedloc.z);
		}

		public void UpdateOffset(Vector3 updatedOff)
        {
			positionOffset = new Vector3(updatedOff.x, groundElavation, updatedOff.z);
        }
	}
}