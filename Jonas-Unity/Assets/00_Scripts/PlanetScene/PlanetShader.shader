Shader "Custom/PlanetShader"
{
	CGINCLUDE
		#include "UnityStandardBRDF.cginc"
		#define PI 3.14159265358979323846
	ENDCG
    SubShader
    {
        Tags 
			{ 
			 "LightMode" = "ForwardBase"
			 "RenderType"="Opaque" 
			}
        Pass
        {
            CGPROGRAM
            #pragma vertex PlanetVertexProgram
            #pragma fragment PlanetFragmentProgram

            #include "UnityCG.cginc"

				float4 _PlanetColor;

				struct Interpolators
				{
					float4 position : SV_POSITION;
					float3 worldPos : TEXCOORD2;
					float3 normal : NORMAL;
					float y : TEXCOORD3;
				};

				struct VertexData
				{
					float4 position : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				float rand (float3 co)
				{
					return frac (sin (dot (co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
				}

				//Functions
				Interpolators PlanetVertexProgram (VertexData v)
				{
					Interpolators i;
					i.worldPos = mul (unity_ObjectToWorld, v.position);
					i.position = UnityObjectToClipPos (v.position);
					i.normal = UnityObjectToWorldNormal (v.normal);
					i.y = 0;
					return i;
				}

				float4 PlanetFragmentProgram (Interpolators i) : SV_TARGET
				{
					i.normal = normalize (i.normal);
					float3 lightDir = _WorldSpaceLightPos0.xyz;
					float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
					float3 lightColor = _LightColor0.rgb;

					//bool beach = i.worldPos.y <= _WaterLevel + (1.f * rand (i.worldPos));

					float3 albedo = _PlanetColor;
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
				#pragma vertex PlanetVertexProgram
				#pragma fragment PlanetFragmentProgram

				#include "UnityCG.cginc"

				float _WaterLevel;

				struct Interpolators
				{
					float4 position : SV_POSITION;
					float3 worldPos : TEXCOORD2;
					float3 normal : NORMAL;
					float y : TEXCOORD3;
				};

				struct VertexData
				{
					float4 position : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				float rand (float3 co)
				{
					return frac (sin (dot (co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
				}

				//Functions
				Interpolators PlanetVertexProgram (VertexData v)
				{
					Interpolators i;

					float dis = distance (float3(0,0,0), v.position);

					//Clamp to water level
					v.position *= _WaterLevel / dis;
					v.position += 0.1f * sin (v.position.x + v.position.y + v.position.z +_Time.y);

					i.worldPos = mul (unity_ObjectToWorld, v.position);
					i.position = UnityObjectToClipPos (v.position);
					i.normal = UnityObjectToWorldNormal (normalize(v.position));
					i.y = 0;
					return i;
				}

				float4 PlanetFragmentProgram (Interpolators i) : SV_TARGET
				{
					i.normal = normalize (i.normal);
					float3 lightDir = _WorldSpaceLightPos0.xyz;
					float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
					float3 lightColor = _LightColor0.rgb;

					//bool beach = i.worldPos.y <= _WaterLevel + (1.f * rand (i.worldPos));

					float3 albedo = float3 (0,0,1);
					float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);

					return float4 (diffuse, 1);
				}
				ENDCG
			}
    }
}
