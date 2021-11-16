using UnityEditor;
using Darkness.States;

[CustomEditor(typeof(DarkState), true)]
public class DarkStateEditor : Editor 
{
	private DarkState[] darkStates;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		darkStates = UnityEngine.Resources.LoadAll<DarkState>("States");
		serializedObject.Update();

		DarkState dState = (DarkState)target;

		dState.SortTransitionsByPriority();
	
		foreach (DarkState d in darkStates)
		{
			d.UpdateReferences();
		}
    }
}
