#include "Lighting.cginc"
#include "CustomTessellation.cginc"

struct Interpolators
{
	float4 position : SV_POSITION;
	float3 worldPos : TEXCOORD2;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
	float y : TEXCOORD3;
};

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

//Wind
sampler2D _WindDistortionMap;
float4 _WindDistortionMap_ST;
float2 _WindFrequency;
float _WindStrength;

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

float3x3 GetWindRotation (float3 pos)
{
	//Callculate uv
	float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;

	//Get value from distortion map and scale to -1 - 1
	float2 windSample = (tex2Dlod (_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
	float3 wind = normalize (float3(windSample.x, windSample.y, 0));

	//Calculate rotation matrix
	float3x3 windRotation = AngleAxis3x3 (UNITY_PI * windSample, wind);
	return windRotation;
}

float rand (float3 co)
{
	return frac (sin (dot (co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
}

//Shader
[maxvertexcount (6)]
void GrassGeoProgramm (triangle vertexOutput IN[3] : SV_POSITION, inout TriangleStream<Interpolators> triStream)
{
	float3 pos = IN[0].vertex;
	geometryOutput o;

	if (pos.y < _MinGrassLevel || pos.y > _MaxGrassLevel)
		return;

	float height = (rand (pos.zyx) * _BladeHeightRandom) + _BladeHeight;
	float width = ((rand (pos.xzy) * _BladeWidthRandom) + _BladeWidth) * 0.5f;


	//Local to tangent space matrix
	float3 vNormal = IN[0].normal;
	float4 vTangent = IN[0].tangent;
	float3 vBinormal = cross (vNormal, vTangent) * vTangent.w;

	float3x3 tangentToLocal = float3x3(
		vTangent.x, vNormal.x, vBinormal.x,
		vTangent.y, vNormal.y, vBinormal.y,
		vTangent.z, vNormal.z, vBinormal.z
		);

	float3x3 windRotationMatrix = GetWindRotation (pos);

	//Random rotation around y-axis
	float3x3 facingRotationMatrix = AngleAxis3x3 (rand (pos) * UNITY_TWO_PI, float3(0, 1, 0));

	//Random rotation around x-axis
	float3x3 bendRotationMatrix = AngleAxis3x3 (rand (pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));

	//Calculate tranformation matrix
	float3x3 transformationMatrix = mul (mul (mul (tangentToLocal, windRotationMatrix), facingRotationMatrix), bendRotationMatrix);
	float3x3 transformationMatrixBase = mul (mul (tangentToLocal, facingRotationMatrix), bendRotationMatrix);

	//Front Face
	triStream.Append (VertexOutput (pos + mul (transformationMatrixBase, float3(width, 0, 0)), float2(0, 0), vNormal));
	triStream.Append (VertexOutput (pos + mul (transformationMatrixBase, float3(-width, 0, 0)), float2(0, 0), vNormal));
	triStream.Append (VertexOutput (pos + mul (transformationMatrix, float3(0, height, 0)), float2(1, 1), vNormal));

	//Back Face
	triStream.Append (VertexOutput (pos + mul (transformationMatrixBase, float3(-width, 0, 0)), float2(0, 0), vNormal));
	triStream.Append (VertexOutput (pos + mul (transformationMatrixBase, float3(width, 0, 0)), float2(0, 0), vNormal));
	triStream.Append (VertexOutput (pos + mul (transformationMatrix, float3(0, height, 0)), float2(1, 1), vNormal));
}

[maxvertexcount (6)]
void PlanetGrassGeoProgramm (triangle vertexOutput IN[3] : SV_POSITION, inout TriangleStream<Interpolators> triStream)
{
	float3 pos = IN[0].vertex;
	geometryOutput o;

	float grassLevel = distance (float3(0, 0, 0), pos);

	if (grassLevel < _MinGrassLevel || grassLevel > _MaxGrassLevel)
		return;

	float height = (rand (pos.zyx) * _BladeHeightRandom) + _BladeHeight;
	float width = ((rand (pos.xzy) * _BladeWidthRandom) + _BladeWidth) * 0.5f;


	//Local to tangent space matrix
	float3 vNormal = IN[0].normal;
	float4 vTangent = IN[0].tangent;
	float3 vBinormal = cross (vNormal, vTangent) * vTangent.w;

	float3x3 tangentToLocal = float3x3(
		vTangent.x, vNormal.x, vBinormal.x,
		vTangent.y, vNormal.y, vBinormal.y,
		vTangent.z, vNormal.z, vBinormal.z
		);

	float3x3 windRotationMatrix = GetWindRotation (pos);

	//Random rotation around y-axis
	float3x3 facingRotationMatrix = AngleAxis3x3 (rand (pos) * UNITY_TWO_PI, float3(0, 1, 0));

	//Random rotation around x-axis
	float3x3 bendRotationMatrix = AngleAxis3x3 (rand (pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));

	//Calculate tranformation matrix
	float3x3 transformationMatrix = mul (mul (mul (tangentToLocal, windRotationMatrix), facingRotationMatrix), bendRotationMatrix);
	float3x3 transformationMatrixBase = mul (mul (tangentToLocal, facingRotationMatrix), bendRotationMatrix);

	//Front Face
	triStream.Append (VertexOutput (pos + mul (transformationMatrixBase, float3(width, 0, 0)), float2(0, 0), vNormal));
	triStream.Append (VertexOutput (pos + mul (transformationMatrixBase, float3(-width, 0, 0)), float2(0, 0), vNormal));
	triStream.Append (VertexOutput (pos + mul (transformationMatrix, float3(0, height, 0)), float2(1, 1), vNormal));

	//Back Face
	triStream.Append (VertexOutput (pos + mul (transformationMatrixBase, float3(-width, 0, 0)), float2(0, 0), vNormal));
	triStream.Append (VertexOutput (pos + mul (transformationMatrixBase, float3(width, 0, 0)), float2(0, 0), vNormal));
	triStream.Append (VertexOutput (pos + mul (transformationMatrix, float3(0, height, 0)), float2(1, 1), vNormal));
}

float4 GrassFragmentProgramm (Interpolators i, fixed facing : VFACE) : SV_Target
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