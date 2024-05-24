// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PlanetShader"
{
	Properties{
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}

		[Header (Water)]
		_WaterColor ("Water Color", Color) = (0, 0, 1, 1)
		_WaterPeakColor ("Water Peak Color", Color) = (0, 0, 1, 1)
		_WaterFoamColor ("Water Foam Color", Color) = (0, 0, 1, 1)
		_BeachColor ("Beach Color", Color) = (0, 0, 1, 1)
		_WaterPeakIntensity ("Water Peak Intensity", Range (0, 1)) = 0.5
		_WaterPeakArea ("Water Peak Area", Range (0, 1)) = 0.95
		_WaterLevel ("Water Level", Float) = 0
		_WaterSmoothness ("Water Smoothness", Range (0, 1)) = 0.5
		_WaveCount ("Water Count", Int) = 1
		_WaveSteepness ("Steepness", Range (0, 1)) = 0.5
		_WaveSpeed ("Water Speed", Float) = 1
		_WavesDirection ("Water Direction", Float) = 30
		_WavesLength ("Wave Length", Float) = 1

		[Header (Grass)]
		_MinGrassLevel ("Min Grass Level", Range (0, 50)) = 1
		_MaxGrassLevel ("Max Grass Level", Range (0, 50)) = 2
		_TopColor ("Top Color", Color) = (1,1,1,1)
		_BottomColor ("Bottom Color", Color) = (1,1,1,1)
		_BladeWidth ("Blade Width", Range (0.01, 1)) = 0.05
		_BladeHeight ("Blade Height", Range (0.01, 1)) = 0.5
		//_TranslucentGain ("Translucent Gain", Range (0,1)) = 0.5
		_BendRotationRandom ("Bend Rotation Random", Range (0, 1)) = 0.2
		_BladeWidthRandom ("Blade Width Random", Range (0, 1)) = 0.02
		_BladeHeightRandom ("Blade Height Random", Range (0, 1)) = 0.3
		_TessellationUniform ("Tessellation Uniform", Range (1, 64)) = 1

		[Header (Wind)]
		_WindDistortionMap ("Wind Distortion Map", 2D) = "white" {}
		_WindFrequency ("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
		_WindStrength ("Wind Strength", Float) = 1
	}

		CGINCLUDE
		#include "UnityStandardBRDF.cginc"
		#define PI 3.14159265358979323846

			// Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
			// Extended discussion on this function can be found at the following link:
			// https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
			// Returns a number in the 0...1 range.
			ENDCG

			SubShader{

			//Terrain
			Pass {
				Tags {
					"LightMode" = "ForwardBase"
					"RenderType" = "Opaque"
				}
				CGPROGRAM
				#pragma vertex TerrainVertexProgram
			//coloring individual pixels that lie inside the mesh's triangles.
			#pragma fragment PlanetFragmentProgram
			#include "Terrain.cginc"

			float4 _PlanetColor;

			float4 PlanetFragmentProgram (Interpolators i) : SV_TARGET
			{
				i.normal = normalize (i.normal);
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
				float3 lightColor = _LightColor0.rgb;

				float dis = distance (float3(0, 0, 0), i.worldPos);
				bool beach = dis <= _WaterLevel + (1.f * rand (i.worldPos));

				float3 albedo = beach ? _BeachColor : tex2D (_MainTex, i.uv) * _Tint;
				float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);

				return float4 (diffuse, 1);
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
			#pragma fragment GrassFragmentProgramm
			#pragma geometry PlanetGrassGeoProgramm
			#pragma target 4.6
			#pragma hull hull
			#pragma domain domain

			#include "Grass.cginc"

			ENDCG
		}

		//Render Water
		Pass
		{
			Tags 
			{
				"LightMode" = "ForwardBase"
				"RenderType" = "Opaque"
			}
			CGPROGRAM
			#pragma vertex PlanetWaterVertexProgram
			#pragma fragment PlanetWaterFragmentProgram

			#include "Water.cginc"
			ENDCG
		}
		////Render Foam
		//Pass
		//{
		//	Tags {
		//		"LightMode" = "ForwardBase"
		//		"Queue" = "Transparent"
		//	}
		//	Blend SrcAlpha OneMinusSrcAlpha
		//	ZWrite Off
		//	CGPROGRAM
		//	#pragma vertex PlanetFoamVertexProgram
		//	//coloring individual pixels that lie inside the mesh's triangles.
		//	#pragma fragment PlanetFoamFragmentProgram

		//	#include "Water.cginc"

		//	ENDCG
		//}
	}
}
