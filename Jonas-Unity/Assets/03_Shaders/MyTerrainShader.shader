// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MyFirstShader"
{
	Properties{
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}

		[Header(Water)]
		_WaterColor ("Water Color", Color) = (0, 0, 1, 1)
		_WaterPeakColor ("Water Peak Color", Color) = (0, 0, 1, 1)
		_WaterFoamColor ("Water Foam Color", Color) = (0, 0, 1, 1)
		_WaterPeakIntensity ("Water Peak Intensity", Range (0, 1)) = 0.5
		_WaterPeakArea ("Water Peak Area", Range (0, 1)) = 0.95
		_WaterLevel ("Water Level", Float) = 0
		_WaterSmoothness ("Water Smoothness", Range (0, 1)) = 0.5
		_WaveCount ("Water Count", Int) = 1
		_WaveSteepness ("Steepness", Range (0, 1)) = 0.5
		_WaveSpeed ("Water Speed", Float) = 1
		_WavesDirection ("Water Direction", Float) = 30
		_WavesLength ("Wave Length", Float) = 1

		[Header(Grass)]
		_MinGrassLevel ("Min Grass Level", Range (0, 50)) = 1
		_MaxGrassLevel ("Max Grass Level", Range (0, 50)) = 2
		_TopColor ("Top Color", Color) = (1,1,1,1)
		_BottomColor ("Bottom Color", Color) = (1,1,1,1)
		_BladeWidth ("Blade Width", Range(0.01, 1)) = 0.05
		_BladeHeight ("Blade Height", Range (0.01, 1)) = 0.5
		//_TranslucentGain ("Translucent Gain", Range (0,1)) = 0.5
		_BendRotationRandom ("Bend Rotation Random", Range (0, 1)) = 0.2
		_BladeWidthRandom ("Blade Width Random", Range (0, 1)) = 0.02
		_BladeHeightRandom ("Blade Height Random", Range (0, 1)) = 0.3
		_TessellationUniform ("Tessellation Uniform", Range (1, 64)) = 1

		[Header (Beach)]
		_BeachColor ("Beach Color", Color) = (0, 0, 1, 1)
	}

	CGINCLUDE
		#include "UnityStandardBRDF.cginc"
		#define PI 3.14159265358979323846

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
			float y : TEXCOORD3;
		};

		// Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
		// Extended discussion on this function can be found at the following link:
		// https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
		// Returns a number in the 0...1 range.
		float rand (float3 co)
		{
			return frac (sin (dot (co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
		}
	ENDCG

	SubShader{

		//Object gets rendered per pass
		Pass {
			Tags {
				"LightMode" = "ForwardBase"
				"RenderType" = "Opaque"
			}
			CGPROGRAM
				#pragma vertex MyVertexProgram
				//coloring individual pixels that lie inside the mesh's triangles.
				#pragma fragment MyFragmentProgram

				//Variables
				//Name exactly as in Properties
				float4 _Tint;
				sampler2D _MainTex;
				float4 _BeachColor;
				float _WaterLevel;
				//Tilling and offset
				//Tilling is scale (xy)
				//Offset (zw)
				float4 _MainTex_ST;

				//Functions
				Interpolators MyVertexProgram (VertexData v)
				{
					Interpolators i;
					i.worldPos = mul (unity_ObjectToWorld, v.position);
					i.position = UnityObjectToClipPos (v.position);
					i.uv = TRANSFORM_TEX (v.uv, _MainTex);
					i.normal = UnityObjectToWorldNormal (v.normal);
					i.y = 0;
					return i;
				}

				float4 MyFragmentProgram (Interpolators i) : SV_TARGET
				{
					i.normal = normalize (i.normal);
					float3 lightDir = _WorldSpaceLightPos0.xyz;
					float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
					float3 lightColor = _LightColor0.rgb;

					bool beach = i.worldPos.y <= _WaterLevel + (1.f * rand (i.worldPos));

					float3 albedo = beach ? _BeachColor : tex2D (_MainTex, i.uv) * _Tint;
					float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);
					//float3 reflectionDir = reflect (-lightDir, i.normal);

					return float4 (diffuse, 1); // +DotClamped (viewDir, reflectionDir);
				}

			ENDCG
		}

		//Grass
		Pass
		{
			Tags
			{
				"RenderType" = "Opaque"
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geo
			#pragma target 4.6
			#pragma hull hull
			#pragma domain domain
			
			#include "Lighting.cginc"
			#include "CustomTessellation.cginc"

			float4 _TopColor;
			float4 _BottomColor;
			float _TranslucentGain;
			float _BendRotationRandom;
			float _BladeHeight;
			float _BladeHeightRandom;
			float _BladeWidth;
			float _BladeWidthRandom;
			float _MinGrassLevel;
			float _MaxGrassLevel;

			struct geometryOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//Help funcs
			Interpolators VertexOutput (float3 pos, float uv, float3 normal)
			{
				Interpolators i;
				i.position = UnityObjectToClipPos (pos);
				i.worldPos = mul (unity_ObjectToWorld, pos);
				i.uv = uv;
				i.normal = UnityObjectToWorldNormal (normal);
				i.y = 0;
				return i;
			}

			// Construct a rotation matrix that rotates around the provided axis, sourced from:
			// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
			float3x3 AngleAxis3x3 (float angle, float3 axis)
			{
				float c, s;
				sincos (angle, s, c);

				float t = 1 - c;
				float x = axis.x;
				float y = axis.y;
				float z = axis.z;

				return float3x3(
					t * x * x + c, t * x * y - s * z, t * x * z + s * y,
					t * x * y + s * z, t * y * y + c, t * y * z - s * x,
					t * x * z - s * y, t * y * z + s * x, t * z * z + c
					);
			}

			//Shader
			[maxvertexcount (6)]
			void geo (triangle vertexOutput IN[3] : SV_POSITION, inout TriangleStream<Interpolators> triStream)
			{
				float3 pos = IN[0].vertex;
				geometryOutput o;

				if (pos.y < _MinGrassLevel || pos.y > _MaxGrassLevel)
					return;

				float height = (rand (pos.zyx) * _BladeHeightRandom) + _BladeHeight;
				float width = ((rand (pos.xzy)* _BladeWidthRandom) + _BladeWidth) * 0.5f;

				//Local to tangent space matrix
				float3 vNormal = IN[0].normal;
				float4 vTangent = IN[0].tangent;
				float3 vBinormal = cross (vNormal, vTangent) * vTangent.w;

				float3x3 tangentToLocal = float3x3(
					vTangent.x, vNormal.x, vBinormal.x,
					vTangent.y, vNormal.y, vBinormal.y,
					vTangent.z, vNormal.z, vBinormal.z
					);

				//Random rotation around y-axis
				float3x3 facingRotationMatrix = AngleAxis3x3 (rand (pos) * UNITY_TWO_PI, float3(0, 1, 0));

				//Random rotation around x-axis
				float3x3 bendRotationMatrix = AngleAxis3x3 (rand (pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));
				
				//Calculate tranformation matrix
				float3x3 transformationMatrix = mul (mul (tangentToLocal, facingRotationMatrix), bendRotationMatrix);

				//Front Face
				triStream.Append (VertexOutput (pos + mul (transformationMatrix, float3(width, 0, 0)), float2(0, 0), vNormal));
				triStream.Append (VertexOutput (pos + mul (transformationMatrix, float3(-width, 0, 0)), float2(0, 0), vNormal));
				triStream.Append (VertexOutput (pos + mul (transformationMatrix, float3(0, height, 0)), float2(1, 1), vNormal));

				//Back Face
				triStream.Append (VertexOutput (pos + mul (transformationMatrix, float3(-width, 0, 0)), float2(0, 0), vNormal));
				triStream.Append (VertexOutput (pos + mul (transformationMatrix, float3(width, 0, 0)), float2(0, 0), vNormal));
				triStream.Append (VertexOutput (pos + mul (transformationMatrix, float3(0, height, 0)), float2(1, 1), vNormal));
			}

			float4 frag (Interpolators i, fixed facing : VFACE) : SV_Target
			{
				i.normal = normalize (i.normal);
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
				float3 lightColor = _LightColor0.rgb;
				float3 albedo = lerp (_BottomColor, _TopColor, i.uv.y);
				float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);
				float3 reflectionDir = reflect (-lightDir, i.normal);

				return float4 (diffuse, 1) + DotClamped (viewDir, reflectionDir) * 0.15;
			}
			ENDCG
		}

		//Render Water
		Pass{
			Tags {
				"LightMode" = "ForwardBase"
				"RenderType" = "Opaque"
			}
			CGPROGRAM
				#pragma vertex MyVertexProgram
			//coloring individual pixels that lie inside the mesh's triangles.
			#pragma fragment MyFragmentProgram


			//Variables
			//Name exactly as in Properties
			float4 _WaterColor;
			float4 _WaterPeakColor;
			float4 _WaterFoamColor;
			float _BeachColor;
			float _WaterPeakArea;
			float _WaterPeakIntensity;
			float _WaterLevel;
			float _WaterSmoothness;
			int _WaveCount;
			float _WaveSteepness;
			float _WaveSpeed;
			float _WavesDirection;
			float _WavesLength;
			float _OffsetX;
			float _OffsetZ;

			//Functions
			float3 GetDirectionVec (float angle)
			{
				const float Deg2Rad = PI / 180.f;
				return float3(cos (angle * Deg2Rad), 0.f, sin (angle * Deg2Rad));
			}

			Interpolators WaveGerstner (float4 p)
			{
				p.y = _WaterLevel;

				float k = 2 * UNITY_PI / _WavesLength;
				float x = p.x + _OffsetX;
				float z = p.z + _OffsetZ;
				float steepness = _WaveSteepness;
				float3 tangent = float3(1, 0, 0);
				float3 binormal = float3(0, 0, 1);

				for (int i = 0; i < _WaveCount; ++i)
				{
					float c = sqrt (9.8 / k) * _WaveSpeed;
					float a = steepness / k;

					float3 direction = GetDirectionVec (_WavesDirection * i * i * i);
					float f = k * ((x * direction.x + z * direction.z) + _Time.y * c);

					p.x += direction.x * (a * cos (f));
					p.z += direction.z * (a * cos (f));
					p.y += a * sin (f);

					tangent += float3(
						-direction.x * direction.x * (steepness * sin (f)),
						direction.x * (steepness * cos (f)),
						-direction.x * direction.y * (steepness * sin (f))
						);
					binormal += float3(
						-direction.x * direction.y * (steepness * sin (f)),
						direction.y * (steepness * cos (f)),
						-direction.y * direction.y * (steepness * sin (f))
						);

					k *= 1.18;
					steepness *= 0.82;
				}

				Interpolators inter;
				inter.position = p;
				inter.normal = normalize (cross (binormal, tangent));

				return inter;
			}

			Interpolators MyVertexProgram (VertexData v)
			{
				Interpolators i;

				i = WaveGerstner (v.position);
				i.normal = UnityObjectToWorldNormal (i.normal);
				i.worldPos = mul (unity_ObjectToWorld, i.position);
				i.position = UnityObjectToClipPos (i.position);
				i.y = v.position.y;
				return i;
			}

			float GetDepthFactor (float3 pos, float y)
			{
				float threshold = 1.f;
				float depth = pos.y - y;

				if (depth <= threshold)
				{
					float x = depth / threshold;
					//float f = 1 - pow (1 - (x), 3);
					//float f = x * x * x;
					float f = 1 - pow (1 - x, 3);
					return (f);
				}

				return 1;
			}

			float3 GetWaterColor (float3 pos, float y)
			{
				float3 color = _WaterColor;
				float threshold = _WaterLevel + _WaveSteepness / (2 * UNITY_PI / _WavesLength); // _WaterLevel + _WaveSteepness;
				threshold *= 1 - (_WaterPeakArea / 10);

				//Peak color
				//color += (white * (pos.y - threshold) * _WaterPeakIntensity);
				color = lerp (color, _WaterPeakColor, saturate((pos.y - threshold) * _WaterPeakIntensity));

				//Shor Color
				color = lerp (color, _WaterFoamColor, 1 - GetDepthFactor (pos, y));

				return color;
			}

			float4 MyFragmentProgram (Interpolators i) : SV_TARGET
			{
				i.normal = normalize (i.normal);
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
				float3 lightColor = _LightColor0.rgb;
				float3 albedo = GetWaterColor (i.worldPos, i.y);
				float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);
				float3 reflectionDir = reflect (-lightDir, i.normal);
				float3 halfVector = normalize (lightDir + viewDir);
				float3 specular = lightColor * pow (DotClamped (halfVector, i.normal), _WaterSmoothness * 10);
				albedo *= 1 - specular;

				float test = pow (DotClamped (halfVector, i.normal), _WaterSmoothness * 10);

				return float4 (diffuse + specular, 0.9);
			}
		ENDCG
		}
	}
}
