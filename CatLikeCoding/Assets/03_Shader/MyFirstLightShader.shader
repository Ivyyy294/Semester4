// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MyFirstLightShader"
{
	Properties{
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		//_Albedo ("Albedo", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo", 2D) = "white" {}
	}

	SubShader{

		//Object gets rendered per pass
		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}
			CGPROGRAM
				#pragma vertex MyVertexProgram

				//coloring individual pixels that lie inside the mesh's triangles.
				#pragma fragment MyFragmentProgram

				//#include "UnityCG.cginc"
				#include "UnityStandardBRDF.cginc" //includes UnityCG.cginc
				
				//Variables
				//Name exactly as in Properties
				float4 _Tint;
				//float4 _Albedo;
				sampler2D _MainTex;

				//Tilling and offset
				//Tilling is scale (xy)
				//Offset (zw)
				float4 _MainTex_ST; 

				struct VertexData
				{
					float4 position : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct Interpolators
				{
					float4 position : SV_POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
					float3 worldPos : TEXCOORD2;
				};

				//Functions
				Interpolators MyVertexProgram (VertexData v)
				{
					Interpolators i;

					//Position
					i.position = UnityObjectToClipPos (v.position);
					i.worldPos = mul (unity_ObjectToWorld, v.position);

					//Normal
					/*i.normal = mul (
						transpose ((float3x3)unity_WorldToObject),
						v.normal
					);
					i.normal = normalize (i.normal);*/

					i.normal = UnityObjectToWorldNormal (v.normal); //shortcut

					//UV
					//i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
					i.uv = TRANSFORM_TEX (v.uv, _MainTex); 	//shortcut

					return i;
				}

				float4 MyFragmentProgram (Interpolators i) : SV_TARGET
				{
					i.normal = normalize (i.normal);
					float3 lightDir = _WorldSpaceLightPos0.xyz;
					float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
					float3 lightColor = _LightColor0.rgb;
					float3 albedo = tex2D (_MainTex, i.uv).rgb * _Tint.rgb;
					float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);
					//clamp light a 0, there is no negative light
					//return max (0, dot (float3(0, 1, 0), i.normal));
					//return float4 (diffuse, 1); //Shortcut

					float3 reflectionDir = reflect (-lightDir, i.normal);

					return float4 (diffuse, 1) + DotClamped (viewDir, reflectionDir);
				}

			ENDCG
		}
	}
}
