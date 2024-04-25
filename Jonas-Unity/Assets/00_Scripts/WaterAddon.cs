using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaterProperties
{
	public Color waterColor;
	public float waterLevelY;
	public float waveAmplitude;
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
		properties.waveAmplitude = material.GetFloat ("_WaveHeight");
		properties.waveSpeed = material.GetFloat ("_WaveSpeed");
		properties.wavesDirection = material.GetFloat ("_WavesDirection");
	}

	public void SetMaterialProperties (MaterialPropertyBlock propertyBlock)
	{
		propertyBlock.SetColor ("_WaterColor", properties.waterColor);
        propertyBlock.SetFloat ("_WaterLevel", properties.waterLevelY);
		propertyBlock.SetFloat ("_WaveHeight", properties.waveAmplitude);
		propertyBlock.SetFloat ("_WaveSpeed", properties.waveSpeed);
		propertyBlock.SetFloat ("_WavesDirection", properties.wavesDirection);
	}
}
