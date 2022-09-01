// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Eremite/ProjectorTexture"
{
	Properties
	{
		_LightTex("LightTex", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_EmissionColor("_EmissionColor", Color) = (0,0,0,0)
		_Color("_Color", Color) = (1,1,1,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float4 vertexToFrag11;
		};

		uniform float4 _Color;
		uniform sampler2D _LightTex;
		float4x4 unity_Projector;
		uniform float4 _EmissionColor;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_vertex4Pos = v.vertex;
			o.vertexToFrag11 = mul( unity_Projector, ase_vertex4Pos );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_output_20_0 = ( (i.vertexToFrag11).xy / (i.vertexToFrag11).w );
			float4 tex2DNode18 = tex2D( _LightTex, temp_output_20_0 );
			float4 appendResult25 = (float4(tex2DNode18.rgb , ( 1.0 - tex2DNode18.a )));
			float2 break44 = temp_output_20_0;
			float temp_output_49_0 = ( step( break44.x , 1.0 ) * step( 0.0 , break44.x ) * step( break44.y , 1.0 ) * step( 0.0 , break44.y ) );
			float4 temp_output_35_0 = ( appendResult25 * temp_output_49_0 );
			o.Albedo = ( _Color * temp_output_35_0 ).rgb;
			o.Emission = ( temp_output_35_0 * _EmissionColor ).xyz;
			o.Alpha = 1;
			clip( temp_output_49_0 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
1918;-8;1920;1005;2392.095;638.4153;1.810816;True;False
Node;AmplifyShaderEditor.UnityProjectorMatrixNode;8;-1628.802,81.28082;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.PosVertexDataNode;10;-1628.802,161.2809;Inherit;False;1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1420.802,81.28082;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexToFragmentNode;11;-1276.802,81.28082;Inherit;False;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;21;-1036.802,81.28082;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;22;-1036.802,161.2809;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;20;-796.8017,81.28082;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;18;-590.1979,21.97193;Inherit;True;Property;_LightTex;LightTex;0;0;Create;True;0;0;0;False;0;False;-1;None;4f03d62d1c248e048ba7a8afbabed761;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;44;-422.7511,350.3985;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.StepOpNode;46;-227.751,303.3985;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;9999999;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;45;-225.751,210.3985;Inherit;False;2;0;FLOAT;-999999;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;30;-254.1978,117.9719;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;47;-224.751,489.8986;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;9999999;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;48;-222.751,396.8985;Inherit;False;2;0;FLOAT;-999999;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-55.75101,325.3985;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;25;-78.19788,53.97193;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;185.5958,272.4614;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;53;157.5026,78.80701;Inherit;False;Property;_Color;_Color;3;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;51;171.5026,434.807;Inherit;False;Property;_EmissionColor;_EmissionColor;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;435.5026,176.807;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;427.5026,325.807;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;39;783,202;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Eremite/ProjectorTexture;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;8;0
WireConnection;12;1;10;0
WireConnection;11;0;12;0
WireConnection;21;0;11;0
WireConnection;22;0;11;0
WireConnection;20;0;21;0
WireConnection;20;1;22;0
WireConnection;18;1;20;0
WireConnection;44;0;20;0
WireConnection;46;1;44;0
WireConnection;45;0;44;0
WireConnection;30;0;18;4
WireConnection;47;1;44;1
WireConnection;48;0;44;1
WireConnection;49;0;45;0
WireConnection;49;1;46;0
WireConnection;49;2;48;0
WireConnection;49;3;47;0
WireConnection;25;0;18;0
WireConnection;25;3;30;0
WireConnection;35;0;25;0
WireConnection;35;1;49;0
WireConnection;54;0;53;0
WireConnection;54;1;35;0
WireConnection;52;0;35;0
WireConnection;52;1;51;0
WireConnection;39;0;54;0
WireConnection;39;2;52;0
WireConnection;39;10;49;0
ASEEND*/
//CHKSM=D7793D77F3B6BFB0AC42B9AB7FBA965275743F96