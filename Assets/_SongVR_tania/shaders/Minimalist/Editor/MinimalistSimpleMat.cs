using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MinimalistSimpleMat : ShaderGUI {

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		MaterialProperty _MainTexture = ShaderGUI.FindProperty("_MainTexture", properties);
		MaterialProperty _MainTexturePower = ShaderGUI.FindProperty ("_MainTexturePower", properties);

		MaterialProperty _Color_F = ShaderGUI.FindProperty("_Color_F", properties);

		MaterialProperty _Color_B = ShaderGUI.FindProperty("_Color_B", properties);

		MaterialProperty _Color_L = ShaderGUI.FindProperty("_Color_L", properties);

		MaterialProperty _Color_R = ShaderGUI.FindProperty("_Color_R", properties);

		MaterialProperty _Color_T = ShaderGUI.FindProperty("_Color_T", properties);
		MaterialProperty _Color_D = ShaderGUI.FindProperty("_Color_D", properties);

		MaterialProperty _AmbientColor = ShaderGUI.FindProperty("_AmbientColor", properties);
		MaterialProperty _AmbientPower = ShaderGUI.FindProperty("_AmbientPower", properties);

		MaterialProperty _DontMix = ShaderGUI.FindProperty("_DontMix", properties);

		MaterialProperty _LM = ShaderGUI.FindProperty("_LM", properties);
		MaterialProperty _LMColor = ShaderGUI.FindProperty("_LMColor", properties);
		MaterialProperty _LMPower = ShaderGUI.FindProperty("_LMPower", properties);

		//Displaying Properties.....................

		materialEditor.ShaderProperty(_MainTexture, _MainTexture.displayName);
		materialEditor.ShaderProperty(_MainTexturePower, _MainTexturePower.displayName);

		if (_MainTexture.textureValue == null) {
			_MainTexturePower.floatValue = 0f;
		}

		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Color of 6 sides", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		materialEditor.ShaderProperty (_Color_F, "Forward");
		materialEditor.ShaderProperty (_Color_B, "Backward");
		materialEditor.ShaderProperty (_Color_L, "Left");
		materialEditor.ShaderProperty (_Color_R, "Right");
		materialEditor.ShaderProperty (_Color_T, "Top");
		materialEditor.ShaderProperty (_Color_D, "Bottom");

		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();


		//Ambient
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Ambient Color", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		materialEditor.ShaderProperty (_AmbientColor, "Color");
		materialEditor.ShaderProperty (_AmbientPower, "Power");

		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space();
		materialEditor.ShaderProperty (_DontMix, "Don't mix color of sides");
		EditorGUILayout.Space();

		//LightMap
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Lightmap Settings", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		materialEditor.ShaderProperty (_LM, "Enable");
		if (_LM.floatValue != 0) {
			materialEditor.ShaderProperty (_LMColor, "Color");
			materialEditor.ShaderProperty (_LMPower, "Power");
		}

		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();
	}
}
