using UnityEditor;
using DarknessMinion;

[CustomEditor(typeof(DarkState))]
public class DarkStateEditor : Editor 
{
	private DarkState[] darkStates;

	void OnSceneGUI()
	{
		
	}

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
		darkStates = UnityEngine.Resources.LoadAll<DarkState>("States");
		
		foreach (DarkState d in darkStates)
		{
			d.Startup();
		}
    }
}
