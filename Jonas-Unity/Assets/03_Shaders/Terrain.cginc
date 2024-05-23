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

float rand (float3 co)
{
	return frac (sin (dot (co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
}

//Functions
Interpolators TerrainVertexProgram (VertexData v)
{
	Interpolators i;
	i.worldPos = mul (unity_ObjectToWorld, v.position);
	i.position = UnityObjectToClipPos (v.position);
	i.uv = TRANSFORM_TEX (v.uv, _MainTex);
	i.normal = UnityObjectToWorldNormal (v.normal);
	i.y = 0;
	return i;
}

float4 TerrainFragmentProgram (Interpolators i) : SV_TARGET
{
	i.normal = normalize (i.normal);
	float3 lightDir = _WorldSpaceLightPos0.xyz;
	float3 viewDir = normalize (_WorldSpaceCameraPos - i.worldPos);
	float3 lightColor = _LightColor0.rgb;

	bool beach = i.worldPos.y <= _WaterLevel + (1.f * rand (i.worldPos));

	float3 albedo = beach ? _BeachColor : tex2D (_MainTex, i.uv) * _Tint;
	float3 diffuse = albedo * lightColor * DotClamped (lightDir, i.normal);

	return float4 (diffuse, 1);
}