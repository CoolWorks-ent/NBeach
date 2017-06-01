Shader "Custom/GrahamTextShader" {
	Properties{

		_MainTex("Font Texture", 2D) = "white" {}

	_Color("Text Color", Color) = (1,0,0,1)

	}

		SubShader{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Lighting Off Cull Off ZWrite On Fog{ Mode Off }

		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		Color[_Color] // not used
		ColorMaterial AmbientAndDiffuse // use material colour which includes vertex colour

		SetTexture[_MainTex]{
		combine primary, texture * primary

	}
	}
	}
}