using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MinimalistStandardMat : ShaderGUI {

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		MaterialProperty _MainTexture = ShaderGUI.FindProperty("_MainTexture", properties);
		MaterialProperty _MainTexturePower = ShaderGUI.FindProperty ("_MainTexturePower", properties);

		MaterialProperty _Color1_F = ShaderGUI.FindProperty("_Color1_F", properties);
		MaterialProperty _Color2_F = ShaderGUI.FindProperty("_Color2_F", properties);

		MaterialProperty _Color1_B = ShaderGUI.FindProperty("_Color1_B", properties);
		MaterialProperty _Color2_B = ShaderGUI.FindProperty("_Color2_B", properties);

		MaterialProperty _Color1_L = ShaderGUI.FindProperty("_Color1_L", properties);
		MaterialProperty _Color2_L = ShaderGUI.FindProperty("_Color2_L", properties);

		MaterialProperty _Color1_R = ShaderGUI.FindProperty("_Color1_R", properties);
		MaterialProperty _Color2_R = ShaderGUI.FindProperty("_Color2_R", properties);

		MaterialProperty _Color_T = ShaderGUI.FindProperty("_Color_T", properties);
		MaterialProperty _Color_D = ShaderGUI.FindProperty("_Color_D", properties);

		MaterialProperty _AmbientColor = ShaderGUI.FindProperty("_AmbientColor", properties);
		MaterialProperty _AmbientPower = ShaderGUI.FindProperty("_AmbientPower", properties);

		MaterialProperty _GradientYStartPos = ShaderGUI.FindProperty("_GradientYStartPos", properties);
		MaterialProperty _GradientHeight = ShaderGUI.FindProperty("_GradientHeight", properties);

		MaterialProperty _LocalSpace = ShaderGUI.FindProperty("_LocalSpace", properties);
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

		//forward Properties
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Forward", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		materialEditor.ShaderProperty (_Color1_F, "Color 1");
		materialEditor.ShaderProperty (_Color2_F, "Color 2");

		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();

		//Backward Properties
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Backward", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		materialEditor.ShaderProperty (_Color1_B, "Color 1");
		materialEditor.ShaderProperty (_Color2_B, "Color 2");

		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();

		//Backward Properties
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Left", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		materialEditor.ShaderProperty (_Color1_L, "Color 1");
		materialEditor.ShaderProperty (_Color2_L, "Color 2");

		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();

		//Right Properties
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Right", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		materialEditor.ShaderProperty (_Color1_R, "Color 1");
		materialEditor.ShaderProperty (_Color2_R, "Color 2");

		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();

		//Top Properties
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Top", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		materialEditor.ShaderProperty (_Color_T, "Color");

		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();

		//bottom Properties
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Bottom", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		materialEditor.ShaderProperty (_Color_D, "Color");

		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();

        //Gradient Settings
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Gradient Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        materialEditor.ShaderProperty(_GradientYStartPos, "Gradient Start Position");
        materialEditor.ShaderProperty(_GradientHeight, "Gradient Falloff");

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

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
		materialEditor.ShaderProperty (_LocalSpace, "Gradient in Local Space");
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
