// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MyFirstShader"
{
	Properties{
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_WaterColor ("Water Color", Color) = (0, 0, 1, 1)
		_WaterLevel ("Water Level", Float) = 0
		_WaveHeight ("Water Height", Float) = 1
		_WaveSpeed ("Water Speed", Float) = 1
		_WavesDirection ("Water Direction", Float) = 30
		_WavesFrequency ("Water Frequency", Float) = 1
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

				#include "UnityStandardBRDF.cginc"
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
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct Interpolators
				{
					float yLevel : TEST;
					float4 position : SV_POSITION;
					float3 worldPos : TEXCOORD2;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				//Functions
				Interpolators MyVertexProgram (VertexData v)
				{
					Interpolators i;
					i.worldPos = mul (unity_ObjectToWorld, v.position);
					i.position = UnityObjectToClipPos (v.position);
					i.uv = TRANSFORM_TEX (v.uv, _MainTex);
					i.normal = UnityObjectToWorldNormal (v.normal);
					return i;
				}

				float4 MyFragmentProgram (Interpolators i) : SV_TARGET
				{
					i.normal = normalize (i.normal);
					float3 lightDir = _WorldSpaceLightPos0.xyz;
					float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
					float3 lightColor = _LightColor0.rgb;
					float3 albedo = tex2D (_MainTex, i.uv) * _Tint;
					float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);
					//float3 reflectionDir = reflect (-lightDir, i.normal);

					return float4 (diffuse, 1); // +DotClamped (viewDir, reflectionDir);
				}

			ENDCG
		}

		//Render Water
		Pass{
			Tags {
				"LightMode" = "ForwardBase"
			}
			CGPROGRAM
				#pragma vertex MyVertexProgram
				//coloring individual pixels that lie inside the mesh's triangles.
				#pragma fragment MyFragmentProgram

				#include "UnityStandardBRDF.cginc"

				#define PI 3.14159265358979323846

				//Variables
				//Name exactly as in Properties
				float4 _WaterColor;
				float _WaterLevel;
				float _WaveHeight;
				float _WaveSpeed;
				float _WavesDirection;
				float _WavesFrequency;

				struct VertexData
				{
					float4 position : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct Interpolators
				{
					float4 position : SV_POSITION;
					float3 worldPos : TEXCOORD2;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				//Functions
				float3 GetDirectionVec (float angle)
				{
					const float Deg2Rad = PI / 180.f;
					return float3(cos (angle * Deg2Rad), 0.f, sin (angle * Deg2Rad));
				}

				Interpolators WaveGerstner (float4 p)
				{
					p.y = _WaterLevel;
					float a = _WaveHeight;
					float w = _WavesFrequency;
					float x = p.x;
					float z = p.z;

					float3 tangent = float3(1, 0, 0);
					float3 binormal = float3(0, 0, 1);

					for (int i = 1; i <= 256; ++i)
					{
						float3 direction = GetDirectionVec (_WavesDirection * i * i * i);
						float f = w * ((x * direction.x + z * direction.z) + _Time.y * _WaveSpeed);

						p.x += direction.x * (a * cos (f));
						p.z += direction.z * (a * cos (f));
						p.y += a * sin (f);

						tangent += float3(
							-direction.x * direction.x * (a * sin (f)),
							direction.x * (a * cos (f)),
							-direction.x * direction.y * (a * sin (f))
							);
						binormal += float3(
							-direction.x * direction.y * (a * sin (f)),
							direction.y * (a * cos (f)),
							-direction.y * direction.y * (a * sin (f))
							);

						w *= 0.82;
						a *= 0.82;
					}

					Interpolators inter;
					inter.position = p;
					inter.normal = normalize (cross (binormal, tangent));

					return inter;
				}

				Interpolators MyVertexProgram (VertexData v)
				{
					Interpolators i;

					if (v.position.y <= _WaterLevel)
						i = WaveGerstner (v.position);
					else
					{
						i.position = v.position;
						i.position.y = _WaterLevel;
						i.normal = float3 (0, 1, 0);
					}

					i.normal = UnityObjectToWorldNormal (i.normal);
					i.worldPos = mul (unity_ObjectToWorld, i.position);
					i.position = UnityObjectToClipPos (i.position);

					return i;
				}

				float4 MyFragmentProgram (Interpolators i) : SV_TARGET
				{
					i.normal = normalize (i.normal);
					float3 lightDir = _WorldSpaceLightPos0.xyz;
					float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
					float3 lightColor = _LightColor0.rgb;
					float3 albedo = _WaterColor;
					float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);
					float3 reflectionDir = reflect (-lightDir, i.normal);

					return float4 (diffuse, 1) + DotClamped (viewDir, reflectionDir) * 0.5;
				}
			ENDCG
		}
	}
}
