﻿Shader "Custom/EVRshader_mobileDiffuse" {
	// Simplified Diffuse shader. Differences from regular Diffuse one:
	// - with color picker
	// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

	Properties{
		_Color("Color",COLOR) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 150

		CGPROGRAM
#pragma surface surf Lambert noforwardadd

		sampler2D _MainTex;
		fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}

		Fallback "Mobile/VertexLit"
}