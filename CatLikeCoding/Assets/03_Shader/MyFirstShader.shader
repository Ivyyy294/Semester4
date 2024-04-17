// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MyFirstShader"
{
	Properties{
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader{

		//Object gets rendered per pass
		Pass {
			CGPROGRAM
				#pragma vertex MyVertexProgram

				//coloring individual pixels that lie inside the mesh's triangles.
				#pragma fragment MyFragmentProgram

				#include "UnityCG.cginc"
				
				//Variables
				//Name exactly as in Properties
				float4 _Tint;
				sampler2D _MainTex;

				//Tilling and offset
				//Tilling is scale (xy)
				//Offset (zw)
				float4 _MainTex_ST; 

				struct VertexData
				{
					float4 position : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct Interpolators
				{
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				//Functions
				Interpolators MyVertexProgram (VertexData v)
				{
					Interpolators i;
					i.position = UnityObjectToClipPos (v.position);

					//i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
					//shortcut
					i.uv = TRANSFORM_TEX (v.uv, _MainTex);

					return i;
				}

				float4 MyFragmentProgram (Interpolators i) : SV_TARGET
				{
					return tex2D (_MainTex, i.uv) * _Tint;
				}

			ENDCG
		}
	}
}
