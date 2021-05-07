Shader "Unlit/MovingWave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DisplaceTex("Displacement texutre", 2D) = "white" {}
        _DisplaceAmount("Displacement amount", Range(0, 0.1)) = 0.1
        _DisplaceSpeed ("Displacement Speed", Range (0, 5)) = 1
        _Color ("Tint", Color) = (1, 1, 1, 0.5)
        _WaveSpeed ("Wave speed", Range(0, 1)) = 0.5
        _WaveAmount ("Wave amount", Range(0, 1)) = 0.5
        _WaveHeight ("Wave height", Range(0, 1)) = 0.5
        _Foamline ("Foamline thickness", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 srcPos : TEXCOORD1;
            };

            sampler2D _MainTex, _DisplaceTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _DisplaceAmount, _WaveAmount, _WaveHeight, _WaveSpeed, _Foamline, _DisplaceSpeed;
            uniform sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                float4 tex = tex2Dlod(_DisplaceTex, float4(v.uv.xy, 0, 0));
                v.vertex.y += sin(_Time.z * _WaveSpeed + (v.vertex.x + v.vertex.z * _WaveAmount * tex)) * _WaveHeight;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.srcPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 changingUV = i.uv + _Time.x * _DisplaceSpeed;
                float2 displ = tex2D(_DisplaceTex, changingUV).xy;
                displ = ((displ * 2) - 1) * _DisplaceAmount;

                half4 col = tex2D(_MainTex, i.uv + displ) * _Color ;
                half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.srcPos)));
                half4 foam = 1 - saturate(_Foamline * (depth - i.srcPos.w));
                col += foam * _Color;
                return col;
            }
            ENDCG
        }
    }
}
