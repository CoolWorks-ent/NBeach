Shader "Mage Fight/Water" {
Properties {
	_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_Refraction ("Refraction", Range (0.00, 100.0)) = 1.0
	_ReflToRefrExponent ("_ReflToRefrExponent", Range(0.00,4.00)) = 1.0
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
	_DepthColor ("Depth Color", Color) = (1,1,1,0.5)
	_BumpReflectionStr ("_BumpReflectionStr", Range(0.00,1.00)) = 0.5
	_ReflectionTex ("_ReflectionTex", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_FPOW("FPOW", Float) = 5.0
	_R0("R0", Float) = 0.05
}

SubShader 
{	
	
	Tags { "RenderType"="Transparent" }
	LOD 100
	
	GrabPass 
	{ 
		
	}
	
CGPROGRAM

#pragma surface surf BlinnPhong alpha
#pragma target 3.0

sampler2D _GrabTexture : register(s0);
sampler2D _BumpMap : register(s2);
sampler2D _ReflectionTex : register(s3);

sampler2D _CameraDepthTexture; // : register(s4);

float4 _ReflectColor;
float4 _DepthColor;
float _Shininess;
float _WeirdScale;
float _Refraction;
float _BumpReflectionStr;
float _ReflToRefrExponent;
float _FPOW;
float _R0;

float4 _GrabTexture_TexelSize;
float4 _CameraDepthTexture_TexelSize;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 worldRefl; 
	float4 screenPos;
	float3 viewDir;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutput o) 
{
	// shore blending
	float z1 = tex2Dproj(_CameraDepthTexture,  IN.screenPos); 
	z1 =  LinearEyeDepth(z1);	
	float z2 = (IN.screenPos.z);
	
	float beachAlpha = ( saturate(  abs(z2-z1) ) - 0.5 ) * 2.0;
	//float depthAlpha =  saturate( 0.5 * (abs(z2-z1)) );
	
	//Specular stuff
	o.Gloss = _SpecColor.a;
	o.Specular = _Shininess;
	
	//Normal stuff
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap - 33.0 * _Time.y * _GrabTexture_TexelSize.xy ));
	o.Normal += UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap - 100.0 * _Time.y * _GrabTexture_TexelSize.xy));
	o.Normal *= 0.5;

	float2 offset = o.Normal * _Refraction * beachAlpha * _GrabTexture_TexelSize.xy;
	IN.screenPos.xy = offset * IN.screenPos.z + IN.screenPos.xy;	
	
	float3 worldRefl = WorldReflectionVector(IN, o.Normal*half3(_BumpReflectionStr,_BumpReflectionStr,_BumpReflectionStr));
	worldRefl.y = -worldRefl.y;
	worldRefl.x = -worldRefl.x;
	
	half4 reflcol = tex2Dproj(_ReflectionTex, IN.screenPos);
	reflcol = reflcol * _ReflectColor;
	
	float3 refrColor = tex2Dproj(_GrabTexture, IN.screenPos);
	//refrColor = refrColor * ( 1.0 -  depthAlpha ) + _DepthColor * depthAlpha;
	refrColor = _DepthColor * refrColor ;
	
	//Beach blending
	o.Alpha = beachAlpha;
	
	//Freshel realisation
	half fresnel = saturate( 1.0 - dot(o.Normal, normalize(IN.viewDir)) );
	fresnel = pow(fresnel, _FPOW);
	fresnel =  _R0 + (1.0 - _R0) * fresnel;
	
	half4 resCol = reflcol * fresnel + half4( refrColor.xyz,1.0) * ( 1.0 - fresnel);	
	//half4 resCol = reflcol;	
	o.Emission = resCol;
	
	o.Albedo = o.Emission;

}
ENDCG
}

//Underwater HACK 
SubShader {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }
		
		LOD 50
		Cull Off

       CGPROGRAM

		//#pragma surface surf BlinnPhong alpha
		#pragma surface surf BlinnPhong 
		#pragma target 3.0

		sampler2D _BumpMap : register(s2);
		sampler2D _ReflectionTex : register(s3);
		
		float _Shininess;
		float4 _DepthColor;
		float _Refraction;
		float _BumpReflectionStr;
		float4 _GrabTexture_TexelSize;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldRefl; 
			float4 screenPos;
			float3 viewDir;
			INTERNAL_DATA
		};

void surf (Input IN, inout SurfaceOutput o) 
{
	
	//Normal stuff
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap - 33.0 * _Time.y * _GrabTexture_TexelSize.xy ));
	o.Normal += UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap - 100.0 * _Time.y * _GrabTexture_TexelSize.xy));
	o.Normal *= 0.5;

	//Specular stuff
	o.Gloss = _SpecColor.a;
	o.Specular = _Shininess;

	//Reflection stuff
	float2 offset = o.Normal * 10 * _Refraction * _GrabTexture_TexelSize.xy;
	IN.screenPos.xy = offset * IN.screenPos.z + IN.screenPos.xy;	
	
	float3 worldRefl = WorldReflectionVector(IN, o.Normal*half3(_BumpReflectionStr,_BumpReflectionStr,_BumpReflectionStr));
	worldRefl.y = -worldRefl.y;
	worldRefl.x = -worldRefl.x;
	
	half4 reflcol = tex2Dproj(_ReflectionTex, IN.screenPos);
	reflcol = reflcol * _DepthColor;

	//Alpha
	o.Alpha = 1.0;
		
	half4 resCol = reflcol;	
	o.Emission = resCol;
	
	o.Albedo = o.Emission;

}
ENDCG
		
}
	
FallBack "Reflective/Bumped Diffuse"
}
