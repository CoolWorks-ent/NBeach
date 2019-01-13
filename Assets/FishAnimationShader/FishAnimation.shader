// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FishAnimation"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		_Speed("Speed", Range( 0 , 10)) = 0
		_Scale("Scale", Range( 0 , 1)) = 0.33
		_Yaw("Yaw", Float) = 0.5
		_Roll("Roll", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 2.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float _Speed;
		uniform float _Yaw;
		uniform float _Roll;
		uniform float _Scale;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			v.vertex.xyz += ( ( sin( ( ( _Time.w * _Speed ) + ( ase_vertex3Pos.z * _Yaw ) + ( ase_vertex3Pos.y * _Roll ) ) ) * _Scale ) * float3(1,0,0) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord;
			o.Albedo = tex2D( _MainTex, uv_MainTex ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=12101
7;29;1906;1004;1804.755;310.8004;1;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;12;-1304.3,120.0002;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;14;-1275.3,302.0001;Float;False;Property;_Yaw;Yaw;3;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;18;-1288.755,416.1996;Float;False;Property;_Roll;Roll;4;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;2;-1177,-294;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-1254,2;Float;False;Property;_Speed;Speed;1;0;0;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-916,-128;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-907.7546,274.1996;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-890.3,109.0002;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-582.3,-95.99985;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;5;-377,-72;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;-555,240;Float;False;Property;_Scale;Scale;2;0;0.33;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;11;-125.3,277.0002;Float;False;Constant;_Direction;Direction;2;0;1,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-165,122;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;16;-7.300049,-194.9998;Float;True;Property;_MainTex;MainTex;0;1;[NoScaleOffset];None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;66.69995,158.0002;Float;False;2;2;0;FLOAT;0.0,0,0;False;1;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;376,-8;Float;False;True;0;Float;;0;Standard;FishAnimation;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;2;4
WireConnection;4;1;3;0
WireConnection;17;0;12;2
WireConnection;17;1;18;0
WireConnection;13;0;12;3
WireConnection;13;1;14;0
WireConnection;15;0;4;0
WireConnection;15;1;13;0
WireConnection;15;2;17;0
WireConnection;5;0;15;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;9;0;6;0
WireConnection;9;1;11;0
WireConnection;0;0;16;0
WireConnection;0;11;9;0
ASEEND*/
//CHKSM=CBADF381CD032712CA9FA2D427A7D2A5E1BCAC45