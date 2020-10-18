using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(TestOverseer))]
public class DarknessTester : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		if(GUILayout.Button("Spawn Darkness"))
		{
			TestOverseer.Instance.SpawnDarkness();
		}

		if(GUILayout.Button("Delete Darkness"))
		{
			TestOverseer.Instance.KillDarkness();
		}

		EditorGUILayout.BeginHorizontal();

		if(GUILayout.Button("Move player Left"))
		{
			TestOverseer.Instance.MovePlayerObject(Vector3.left*5);
		}

		if(GUILayout.Button("Move player Right"))
		{
			TestOverseer.Instance.MovePlayerObject(Vector3.right*5);
		}
		EditorGUILayout.EndHorizontal();

		if(TestOverseer.Instance != null)
		{
			if(TestOverseer.Instance.showDebugInfo)
			{
				TestOverseer.Instance.DisplayDebugInfo();
			}
			else TestOverseer.Instance.DeactivateDebugInfo();
		}
    }

}
