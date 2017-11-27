Shader "2D/Texture Only"
{
	Properties
	{
		_MainTex("Texture", 2D) = ""
	}

		SubShader
	{
		ZWrite On // "Off" might make more sense in very specific games
		Cull Off
		Pass
	{
		SetTexture[_MainTex]
	}
	}
}