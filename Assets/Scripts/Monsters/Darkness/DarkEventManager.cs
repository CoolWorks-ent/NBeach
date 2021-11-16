using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darkness
{
    static class DarkEventManager
    {
		public delegate void DarkEvent();
		public delegate void DarkEvent<T>(T obj);
		public static event DarkEvent<DarknessController> AddDarkness;
		public static event DarkEvent<DarknessController> RemoveDarkness;

		public static event DarkEvent<Vector3> UpdateZoneLocation;
		public static event DarkEvent UpdateDarknessDistance;
		public static event DarkEvent UpdateDarknessStates;

		public static void OnDarknessAdded(DarknessController d)
		{
			if (AddDarkness != null)
				AddDarkness(d);
		}

		public static void OnDarknessRemoved(DarknessController d)
		{
			if (RemoveDarkness != null)
				RemoveDarkness(d);
		}

		public static void OnUpdateZoneLocation(Vector3 loc)
		{
			if(UpdateZoneLocation != null)
				UpdateZoneLocation(loc);
		}

		public static void OnUpdateDarknessDistance()
		{
			if (UpdateDarknessDistance != null)
				UpdateDarknessDistance();
		}

		public static void OnUpdateDarknessStates()
		{
			if (UpdateDarknessStates != null)
				UpdateDarknessStates();
		}
	}
}