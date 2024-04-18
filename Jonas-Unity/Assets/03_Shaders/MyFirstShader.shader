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
					float yLevel : TEST;
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				//Functions
				Interpolators MyVertexProgram (VertexData v)
				{
					Interpolators i;
					i.position = UnityObjectToClipPos (v.position);
					i.uv = TRANSFORM_TEX (v.uv, _MainTex);
					return i;
				}

				float4 MyFragmentProgram (Interpolators i) : SV_TARGET
				{
					return tex2D (_MainTex, i.uv) * _Tint;
				}

			ENDCG
		}

		//Render Water
		Pass{
			CGPROGRAM
				#pragma vertex MyVertexProgram
				//coloring individual pixels that lie inside the mesh's triangles.
				#pragma fragment MyFragmentProgram

				#include "UnityCG.cginc"

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
					float2 uv : TEXCOORD0;
				};

				struct Interpolators
				{
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				//Functions
				float3 GetDirectionVec (float angle)
				{
					const float Deg2Rad = PI / 180.f;
					return float3(cos (angle * Deg2Rad), 0.f, sin (angle * Deg2Rad));
				}

				float4 WaveGerstner (float4 p)
				{
					float a = _WaveHeight;
					float w = _WavesFrequency;
					float x = p.x;
					float z = p.z;

					for (int i = 1; i <= 256; ++i)
					{
						float3 direction = GetDirectionVec (_WavesDirection * i * i * i);
						float f = w * ((x * direction.x + z * direction.z) + _Time.y * _WaveSpeed);

						p.x += direction.x * (a * cos (f));
						p.z += direction.z * (a * cos (f));
						p.y += a * sin (f);

						w *= 0.82;
						a *= 0.82;
					}

					return p;
				}

				float4 WaterAnimation (float4 position)
				{
					position.y = _WaterLevel;

					return WaveGerstner (position);
				}

				Interpolators MyVertexProgram (VertexData v)
				{
					Interpolators i;

					if (v.position.y < _WaterLevel)
						v.position = WaterAnimation (v.position);
					else
						v.position.y = _WaterLevel;

					i.position = UnityObjectToClipPos (v.position);

					return i;
				}

				float4 MyFragmentProgram (Interpolators i) : SV_TARGET
				{
					return _WaterColor;
				}
			ENDCG
		}
	}
}
