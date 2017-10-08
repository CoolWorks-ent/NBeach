// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

shader "Minimalist/2 Color Gradient/Advanced" {
	Properties{
		_MainTexture ("Main Texture", 2D) = "white" {}
		_MainTexturePower ("Main Texture Power", Range(0, 10.0)) = 1

		//Forward
		[HideInInspector][MaterialToggle] _UseGradient_F ("Use Gradient", Float ) = 0
		_Color1_F("Forward Color 1",  Color) = (0, 1, 0, 1)
		_Color2_F("Forward Color 2",  Color) = (1, 0, 0, 1)
		_GradientYStartPos_F ("Forward Gradient start Y", Float) = 0
		_GradientHeight_F("Forward Gradient Height", Float) = 10

		//Backward
		[HideInInspector][MaterialToggle] _UseGradient_B ("Use Gradient", Float ) = 0
		_Color1_B("Backward Color 1", Color) = (0, 1, 0, 1)
		_Color2_B("Backward Color 2", Color) = (1, 0, 0, 1)
		_GradientYStartPos_B ("Backward Gradient start Y", Float) = 0
		_GradientHeight_B("Backward Gradient Height", Float) = 10

		//Left
		[HideInInspector][MaterialToggle] _UseGradient_L ("Use Gradient", Float ) = 0
		_Color1_L("Left Color 1",     Color) = (0, 1, 0, 1)
		_Color2_L("Left Color 2",     Color) = (1, 0, 0, 1)
		_GradientYStartPos_L ("Left Gradient start Y", Float) = 0
		_GradientHeight_L("Left Gradient Height", Float) = 10

		//Right
		[HideInInspector][MaterialToggle] _UseGradient_R ("Use Gradient", Float ) = 0
		_Color1_R("Right Color 1",    Color) = (0, 1, 0, 1)
		_Color2_R("Right Color 2",    Color) = (1, 0, 0, 1)
		_GradientYStartPos_R ("Right Gradient start Y", Float) = 0
		_GradientHeight_R("Right Gradient Height", Float) = 10

		//Top Bottom
		_Color_T ("Top Color",        Color) = (0, 1, 0, 1)
		_Color_D ("Bottom Color",     Color) = (1, 0, 0, 1)

		_AmbientColor("Ambient Color",Color) = (1, 1, 1, 1)
		_AmbientPower("Ambient Power", Range(0, 2.0)) = 0

		[MaterialToggle] _LocalSpace ("Local Space", Float ) = 0
		[MaterialToggle] _DontMix ("Don't Mix Color", Float ) = 0

		[MaterialToggle] _LM ("Enable Lightmap", Float ) = 0
		_LMColor ("LightMap Color", Color) = (1, 1, 1, 1)
		_LMPower ("LightMap Power", Range(0, 5.0)) = 0
	}

	SubShader{
		Tags { "QUEUE"="Geometry" "RenderType"="Opaque" "LIGHTMODE"="ForwardBase"}

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase_fullshadows

			#include "UnityCG.cginc"

			uniform half3 _Color1_F;
			uniform half3 _Color2_F;
			uniform float _GradientYStartPos_F;
			uniform float _GradientHeight_F;

			uniform half3 _Color1_B;
			uniform half3 _Color2_B;
			uniform float _GradientYStartPos_B;
			uniform float _GradientHeight_B;

			uniform half3 _Color1_L;
			uniform half3 _Color2_L;
			uniform float _GradientYStartPos_L;
			uniform float _GradientHeight_L;

			uniform half3 _Color1_R;
			uniform half3 _Color2_R;
			uniform float _GradientYStartPos_R;
			uniform float _GradientHeight_R;

			uniform half3 _Color_T;
			uniform half3 _Color_D;

			uniform half3 _AmbientColor;
			uniform fixed _AmbientPower;

			uniform fixed _LocalSpace;
			uniform fixed _DontMix;

			uniform fixed _LM;
			uniform fixed3 _LMColor;
			uniform fixed _LMPower;

			uniform sampler2D _MainTexture; uniform fixed4 _MainTexture_ST;
			uniform fixed _MainTexturePower;


			static const half3 FrontDir = half3(0, 0, 1);
			static const half3 BackDir = half3(0, 0, -1);
			static const half3 LeftDir = half3(1, 0, 0);
			static const half3 RightDir = half3(-1, 0, 0);
			static const half3 TopDir = half3(0, 1, 0);
			static const half3 BottomDir = half3(0, -1, 0);
			static const half3 whiteColor = half3(1, 1, 1);

			struct vertexInput{
				float4 vertex : POSITION;
				half3 normal : NORMAL;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
			};

			struct vertexOutput{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float2 lightmapUV : TEXCOORD1;
				float3 color : TEXCOORD2;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
				//o.pos = UnityObjectToClipPos(v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv0;
				half3 normal = normalize(mul(unity_ObjectToWorld, half4(v.normal, 0))).xyz;

				fixed dirFront  = max(dot(normal, FrontDir),  0.0);
				fixed dirBack   = max(dot(normal, BackDir),   0.0);
				fixed dirLeft   = max(dot(normal, LeftDir),   0.0);
				fixed dirRight  = max(dot(normal, RightDir),  0.0);
				fixed dirTop    = max(dot(normal, TopDir),    0.0);
				fixed dirBottom = max(dot(normal, BottomDir), 0.0);

				float4 GradientPosition = mul(unity_ObjectToWorld, v.vertex);
				fixed GradientSpace = (GradientPosition.y * (1 - _LocalSpace)) + (v.vertex.y * _LocalSpace);

				fixed GradientFactor_F = saturate(lerp(_GradientHeight_F, _GradientYStartPos_F, GradientSpace));
				fixed GradientFactor_B = saturate(lerp(_GradientHeight_B, _GradientYStartPos_B, GradientSpace));
				fixed GradientFactor_L = saturate(lerp(_GradientHeight_L, _GradientYStartPos_L, GradientSpace));
				fixed GradientFactor_R = saturate(lerp(_GradientHeight_R, _GradientYStartPos_R, GradientSpace));

				fixed3 colorFront = lerp(_Color1_F, _Color2_F, GradientFactor_F);
				fixed3 colorBack = lerp(_Color1_B, _Color2_B, GradientFactor_B);
				fixed3 colorLeft = lerp(_Color1_L, _Color2_L, GradientFactor_L);
				fixed3 colorRight = lerp(_Color1_R, _Color2_R, GradientFactor_R);
				fixed3 colorTop = _Color_T;
				fixed3 colorDown = _Color_D;

				fixed3 AdditiveColor = colorFront * dirFront + colorBack * dirBack + colorLeft * dirLeft + colorRight * dirRight + colorTop * dirTop + colorDown * dirBottom;
				fixed3 MultipliedColor = lerp(colorFront, whiteColor, 1-dirFront) * lerp(colorBack, whiteColor, 1-dirBack) * lerp(colorLeft, whiteColor, 1-dirLeft) * lerp(colorRight, whiteColor, 1-dirRight) * lerp(colorTop, whiteColor, 1-dirTop) * lerp(colorDown, whiteColor, 1-dirBottom);

				fixed3 Maincolor = lerp(MultipliedColor, AdditiveColor, _DontMix);
				o.color = Maincolor + (_AmbientColor * _AmbientPower);

				//Lightmap
				o.lightmapUV = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				return o;
			}

			fixed4 frag(vertexOutput i) : COLOR
			{
				 fixed4 mainColor = fixed4(i.color, 1);
				 fixed4 _MainTexture_var = fixed4(tex2D(_MainTexture,i.uv.rg).rgb, 1)   * (_MainTexturePower + 1);

				 half4 lmColor = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV);
                 half4 lmPower = lerp(fixed4(1,1,1,1), half4(DecodeLightmap(lmColor), 0), _LMPower);
                 fixed4 LightData = lerp(fixed4(_LMColor, 0), fixed4(1,1,1,1) , lmPower);

                 fixed4 finalColor = mainColor * _MainTexture_var * ((LightData * _LM) + ( 1 -_LM ));

				 return  finalColor;
			}

			ENDCG
		}
	}

	FallBack "Standard"
    CustomEditor "MinimalistAdvancedMat"
}