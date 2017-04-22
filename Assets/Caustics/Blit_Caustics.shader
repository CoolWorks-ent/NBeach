Shader "Custom/Blit_Caustics" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainTiling ("Distort Tiling", Float) = 1
		_Mask ("Base (RGB)", 2D) = "white" {}
		_DistortTex ("Distortion Texture (RG)", 2D) = "white" {}
		_DistortTiling ("Distort Tiling", Float) = 1
	}
	
	CGINCLUDE	

	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
	sampler2D _DistortTex;
	sampler2D _Mask;
	
	float _MainTiling;
	float _DistortTiling;
	 
	struct appdata_t {
	    float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	 
	struct v2f {
	    float4 pos : POSITION;
	    float2 uv : TEXCOORD0;
	};
	 
	v2f vert(appdata_img v){
	    v2f o;
	    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);      // compute transformed vertex position
		o.uv = v.texcoord;   // compute the texcoords of the noise
	    return o;
	}
	 
	half4 frag( v2f i ) : COLOR
	{  
	    half4 distort = tex2D( _DistortTex, i.uv * _DistortTiling + float2( 0.0, 0.1 ) * _Time.y * 0.5 );
		half4 mask = tex2D( _Mask, i.uv.xy + ( ( distort.xy - 0.5 ) * 0.1 ) );
	    half4 caustics1 = tex2D( _MainTex, i.uv.xy * _MainTiling * 3.1 + float2( 0.13, 0.0 ) * _Time.y + ( ( distort.xy - 0.5 ) * 0.2 ) );
	    half4 caustics2 = tex2D( _MainTex, i.uv.yx * _MainTiling * 2.7 + float2( -0.10, 0.0 ) * _Time.y + ( ( distort.xy - 0.5 ) * 0.15 ) );
	    
	    caustics1 = lerp( caustics1, caustics2, smoothstep( 0.3, 0.7, distort.w ) );
	    caustics1 *= mask.w;

	    return caustics1;
	    //return float4(1,1,1,1);
	}
	
	ENDCG
	
	SubShader {
	  
		// 0
		Pass {         
			ZTest Always
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		} 
	}
}
