using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessMinion
{
    static class Dark_Event_Manager
    {
		public delegate void DarkEvent();
		public delegate void DarkEvent<T>(T obj);
		public static event DarkEvent<Darkness> AddDarkness;
		public static event DarkEvent<Darkness> RemoveDarkness;

		public static event DarkEvent<int> RequestNewTarget;
		public static event DarkEvent UpdateDarkness;

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

		public static void OnRequestNewTarget(int ID)
		{
			if (RequestNewTarget != null)
				RequestNewTarget(ID);
		}

		public static void OnUpdateDarkness()
		{
			if (UpdateDarkness != null)
				UpdateDarkness();
		}
	}
}