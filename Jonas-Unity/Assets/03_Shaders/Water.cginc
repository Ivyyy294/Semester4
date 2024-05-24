struct Interpolators
{
	float4 position : SV_POSITION;
	float3 worldPos : TEXCOORD2;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
	float y : TEXCOORD3;
};

struct VertexData
{
	float4 position : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
};

//Variables
//Name exactly as in Properties

//Generall
float _OffsetX;
float _OffsetZ;

//Water
float4 _WaterColor;
float4 _WaterPeakColor;
float _WaterPeakArea;
float _WaterPeakIntensity;
float _WaterLevel;
float _WaterSmoothness;

//Foam
float4 _WaterFoamColor;

//Wave
int _WaveCount;
float _WaveSteepness;
float _WaveSpeed;
float _WavesDirection;
float _WavesLength;

//Help Functions
float3 GetDirectionVec (float angle)
{
	const float Deg2Rad = PI / 180.f;
	return float3(cos (angle * Deg2Rad), 0.f, sin (angle * Deg2Rad));
}

Interpolators PlanetWaveGerstner (float4 p)
{
	float dis = distance (float3(0, 0, 0), p);
	p *= _WaterLevel / dis;

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

float3 GetWaterColor (float3 pos, float y)
{
	float3 color = _WaterColor;
	float threshold = _WaterLevel + _WaveSteepness / (2 * UNITY_PI / _WavesLength); // _WaterLevel + _WaveSteepness;
	threshold *= 1 - (_WaterPeakArea / 10);

	//Peak color
	color = lerp (color, _WaterPeakColor, saturate ((pos.y - threshold) * _WaterPeakIntensity));

	return color;
}

float3 PlanetGetWaterColor (float3 pos, float y)
{
	float3 color = _WaterColor;
	float threshold = _WaterLevel + _WaveSteepness / (2 * UNITY_PI / _WavesLength); // _WaterLevel + _WaveSteepness;
	threshold *= 1 - (_WaterPeakArea / 10);

	//Peak color
	float dis = distance (float3(0, 0, 0), pos);
	color = lerp (color, _WaterPeakColor, saturate ((dis - threshold) * _WaterPeakIntensity));

	return color;
}

float GetDepthFactor (float3 pos, float y)
{
	float threshold = 2.5f;
	float depth = pos.y - y;

	if (pos.y > _WaterLevel)
		depth += pos.y - _WaterLevel;

	if (depth <= threshold)
	{
		float x = depth / threshold;
		float f = 1 - pow (1 - x, 3);
		return (f);
	}

	return 1;
}

float PlanetGetDepthFactor (float3 pos, float y)
{
	float threshold = 50.5f;
	float depth = distance (float3(0,0,0), pos);

	if (pos.y > _WaterLevel)
		depth += pos.y - _WaterLevel;

	if (depth <= threshold)
	{
		float x = depth / threshold;
		float f = 1 - pow (1 - x, 3);
		return (f);
	}

	return 1;
}

//Water kernel
Interpolators WaterVertexProgram (VertexData v)
{
	Interpolators i;

	i = WaveGerstner (v.position);
	i.normal = UnityObjectToWorldNormal (i.normal);
	i.worldPos = mul (unity_ObjectToWorld, i.position);
	i.position = UnityObjectToClipPos (i.position);
	i.y = v.position.y;
	return i;
}

float4 WaterFragmentProgram (Interpolators i) : SV_TARGET
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

Interpolators PlanetWaterVertexProgram (VertexData v)
{
	Interpolators i;

	i = PlanetWaveGerstner (v.position);
	i.normal = UnityObjectToWorldNormal (i.normal);
	i.worldPos = mul (unity_ObjectToWorld, i.position);
	i.position = UnityObjectToClipPos (i.position);
	i.y = v.position.y;
	return i;
}

float4 PlanetWaterFragmentProgram (Interpolators i) : SV_TARGET
{
	i.normal = normalize (i.normal);
	float3 lightDir = _WorldSpaceLightPos0.xyz;
	float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
	float3 lightColor = _LightColor0.rgb;
	float3 albedo = PlanetGetWaterColor (i.worldPos, i.y);
	float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);
	float3 reflectionDir = reflect (-lightDir, i.normal);
	float3 halfVector = normalize (lightDir + viewDir);
	float3 specular = lightColor * pow (DotClamped (halfVector, i.normal), _WaterSmoothness * 10);
	albedo *= 1 - specular;

	float test = pow (DotClamped (halfVector, i.normal), _WaterSmoothness * 10);

	return float4 (diffuse + specular, 0.9);
}

//Foam kernel
Interpolators FoamVertexProgram (VertexData v)
{
	Interpolators i;

	i = WaveGerstner (v.position);
	i.normal = UnityObjectToWorldNormal (i.normal);
	i.worldPos = mul (unity_ObjectToWorld, i.position);
	i.position = UnityObjectToClipPos (i.position);
	i.y = v.position.y;
	return i;
}

float4 FoamFragmentProgram (Interpolators i) : SV_TARGET
{
	float alpha = 1 - GetDepthFactor (i.worldPos, i.y);

	//Discard pixel with zero alpha
	if (alpha <= 0.f)
		discard;

	i.normal = normalize (i.normal);
	float3 lightDir = _WorldSpaceLightPos0.xyz;
	float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
	float3 lightColor = _LightColor0.rgb;
	float3 albedo = _WaterFoamColor;
	float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);

	return float4 (diffuse, alpha);
}

Interpolators PlanetFoamVertexProgram (VertexData v)
{
	Interpolators i;

	i = PlanetWaveGerstner (v.position);
	i.normal = UnityObjectToWorldNormal (i.normal);
	i.worldPos = mul (unity_ObjectToWorld, i.position);
	i.position = UnityObjectToClipPos (i.position);
	i.y = v.position.y;
	return i;
}

float4 PlanetFoamFragmentProgram (Interpolators i) : SV_TARGET
{
	float alpha = 1 - PlanetGetDepthFactor (i.worldPos, i.y);

	//Discard pixel with zero alpha
	if (alpha <= 0.f)
		discard;

	i.normal = normalize (i.normal);
	float3 lightDir = _WorldSpaceLightPos0.xyz;
	float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
	float3 lightColor = _LightColor0.rgb;
	float3 albedo = _WaterFoamColor;
	float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);

	return float4 (diffuse, alpha);
}
