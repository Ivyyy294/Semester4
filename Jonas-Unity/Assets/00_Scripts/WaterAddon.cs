using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaterProperties
{
	public Color waterColor;
	public float waterLevelY;
	[Min (1)] public int waveCount;
	[Range(0, 1)] public float waveSteepness;
	public float waveLength;
	public float waveSpeed;
	public float wavesDirection;
}

public class WaterAddon : MonoBehaviour, ITerrainGeneratorShaderAddon
{
	public WaterProperties properties;

	public void LoadDefaultValues(Material material)
	{		
		properties.waterColor = material.GetColor ("_WaterColor");
		properties.waterLevelY = material.GetFloat ("_WaterLevel");
		properties.waveCount = material.GetInt ("_WaveCount");
		properties.waveSteepness = material.GetFloat ("_WaveSteepness");
		properties.waveSpeed = material.GetFloat ("_WaveSpeed");
		properties.wavesDirection = material.GetFloat ("_WavesDirection");
		properties.waveLength = material.GetFloat ("_WavesLength");
	}

	public void SetMaterialProperties (MaterialPropertyBlock propertyBlock)
	{
		propertyBlock.SetColor ("_WaterColor", properties.waterColor);
        propertyBlock.SetFloat ("_WaterLevel", properties.waterLevelY);
		propertyBlock.SetInt ("_WaveCount", properties.waveCount);
		propertyBlock.SetFloat ("_WaveSteepness", properties.waveSteepness);
		propertyBlock.SetFloat ("_WaveSpeed", properties.waveSpeed);
		propertyBlock.SetFloat ("_WavesDirection", properties.wavesDirection);
		propertyBlock.SetFloat ("_WavesLength", properties.waveLength);
	}
}
