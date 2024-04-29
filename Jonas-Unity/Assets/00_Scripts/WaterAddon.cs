using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaterProperties
{
	[Header ("Water appearance")]
	public Color waterColor;
	public Color waterPeakColor;
	public Color waterFoamColor;
	[Range(0, 1)] public float waterSmoothness;
	[Range(0, 1)] public float waterPeakIntensity;
	[Range(0, 1)] public float waterPeakArea;

	[Header ("Water settings")]
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
		properties.waterPeakColor = material.GetColor ("_WaterPeakColor");
		properties.waterFoamColor = material.GetColor ("_WaterFoamColor");
		properties.waterPeakIntensity = material.GetFloat ("_WaterPeakIntensity");
		properties.waterPeakArea = material.GetFloat ("_WaterPeakArea");
		properties.waterLevelY = material.GetFloat ("_WaterLevel");
		properties.waterSmoothness = material.GetFloat ("_WaterSmoothness");
		properties.waveCount = material.GetInt ("_WaveCount");
		properties.waveSteepness = material.GetFloat ("_WaveSteepness");
		properties.waveSpeed = material.GetFloat ("_WaveSpeed");
		properties.wavesDirection = material.GetFloat ("_WavesDirection");
		properties.waveLength = material.GetFloat ("_WavesLength");
	}

	public void SetMaterialProperties (MaterialPropertyBlock propertyBlock)
	{
		propertyBlock.SetColor ("_WaterColor", properties.waterColor);
		propertyBlock.SetColor ("_WaterPeakColor", properties.waterPeakColor);
		propertyBlock.SetColor ("_WaterFoamColor", properties.waterFoamColor);

		propertyBlock.SetFloat ("_WaterPeakIntensity", properties.waterPeakIntensity);
		propertyBlock.SetFloat ("_WaterPeakArea", properties.waterPeakArea);

        propertyBlock.SetFloat ("_WaterLevel", properties.waterLevelY);
		propertyBlock.SetFloat ("_WaterSmoothness", properties.waterSmoothness);
		propertyBlock.SetInt ("_WaveCount", properties.waveCount);
		propertyBlock.SetFloat ("_WaveSteepness", properties.waveSteepness);
		propertyBlock.SetFloat ("_WaveSpeed", properties.waveSpeed);
		propertyBlock.SetFloat ("_WavesDirection", properties.wavesDirection);
		propertyBlock.SetFloat ("_WavesLength", properties.waveLength);
	}
}
