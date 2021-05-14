using UnityEngine;
using UnityEditor;
using DarknessMinion;

[CustomEditor(typeof(Darkness))]
public class DarknessEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		if(EditorApplication.isPlaying)
		{
			foreach(GameObject gObject in Selection.gameObjects)
			{
				//DarknessMovement dM = (DarknessMovement)gObject;
			}
		}
    }
}
