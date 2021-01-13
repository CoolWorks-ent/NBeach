using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessMinion
{
    static class DarkEventManager
    {
		public delegate void DarkEvent();
		public delegate void DarkEvent<T>(T obj);
		public static event DarkEvent<Darkness> AddDarkness;
		public static event DarkEvent<Darkness> RemoveDarkness;
		public static event DarkEvent<Vector3> UpdateDarknessDistance;
		public static event DarkEvent UpdateDarknessStates;

		public static void OnDarknessAdded(Darkness d)
		{
			if (AddDarkness != null)
				AddDarkness(d);
		}

		public static void OnDarknessRemoved(Darkness d)
		{
			if (RemoveDarkness != null)
				RemoveDarkness(d);
		}

		public static void OnUpdateDarknessDistance(Vector3 pos)
		{
			if (UpdateDarknessDistance != null)
				UpdateDarknessDistance(pos);
		}

		public static void OnUpdateDarknessStates()
		{
			if (UpdateDarknessStates != null)
				UpdateDarknessStates();
		}
	}
}