using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GrassProperties
{
	[Range(0f, 50f)] public float MinGrassLevel;
	[Range(0f, 50f)] public float MaxGrassLevel;
	public Color TopColor;
	public Color BottomColor;
	[Range(0.01f, 1f)] public float BladeWidth;
	[Range(0.01f, 1f)] public float BladeHeight;
	[Range(0f, 1)] public float BendRotationRandom;
	[Range(0f, 1)] public float BladeWidthRandom;
	[Range(0f, 1)] public float BladeHeightRandom;
	[Range(1, 64)] public float TessellationUniform;
}

public class GrassAddon : MonoBehaviour, ITerrainGeneratorShaderAddon, IShaderSetting
{
	public GrassProperties properties;

	public void LoadDefaultValues(Material material)
	{		
		properties.MinGrassLevel = material.GetFloat ("_MinGrassLevel");
		properties.MaxGrassLevel = material.GetFloat ("_MaxGrassLevel");
		properties.TopColor = material.GetColor ("_TopColor");
		properties.BottomColor = material.GetColor ("_BottomColor");
		properties.BladeWidth = material.GetFloat ("_BladeWidth");
		properties.BladeHeight = material.GetFloat ("_BladeHeight");
		properties.BendRotationRandom = material.GetFloat ("_BendRotationRandom");
		properties.BladeWidthRandom = material.GetFloat ("_BladeWidthRandom");
		properties.BladeHeightRandom = material.GetFloat ("_BladeHeightRandom");
		properties.TessellationUniform = material.GetFloat ("_TessellationUniform");
		//waterLevelY = material.GetFloat ("_WaterLevel");
		//waveAmplitude = material.GetFloat ("_WaveHeight");
		//waveSpeed = material.GetFloat ("_WaveSpeed");
		//wavesDirection = material.GetFloat ("_WavesDirection");
	}

	public void SetMaterialProperties (MaterialPropertyBlock propertyBlock)
	{
		propertyBlock.SetFloat ("_MinGrassLevel", properties.MinGrassLevel);
        propertyBlock.SetFloat ("_MaxGrassLevel", properties.MaxGrassLevel);
		propertyBlock.SetColor ("_TopColor", properties.TopColor);
        propertyBlock.SetColor ("_BottomColor", properties.BottomColor);
		propertyBlock.SetFloat ("_BladeWidth", properties.BladeWidth);
        propertyBlock.SetFloat ("_BladeHeight", properties.BladeHeight);
		propertyBlock.SetFloat ("_BendRotationRandom", properties.BendRotationRandom);
        propertyBlock.SetFloat ("_BladeWidthRandom", properties.BladeWidthRandom);
		propertyBlock.SetFloat ("_BladeHeightRandom", properties.BladeHeightRandom);
        propertyBlock.SetFloat ("_TessellationUniform", properties.TessellationUniform);
	}
}
