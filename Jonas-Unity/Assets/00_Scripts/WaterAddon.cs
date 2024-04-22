using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAddon : MonoBehaviour, ITerrainGeneratorShaderAddon
{
	[SerializeField] Color waterColor;
	[SerializeField] float waterLevelY = 1f;
	[SerializeField] float waveAmplitude = 1f;
	[SerializeField] float waveSpeed = 2f;
	[SerializeField] float wavesDirection = 30f;

	public void LoadDefaultValues(Material material)
	{		
		waterColor = material.GetColor ("_WaterColor");
		waterLevelY = material.GetFloat ("_WaterLevel");
		waveAmplitude = material.GetFloat ("_WaveHeight");
		waveSpeed = material.GetFloat ("_WaveSpeed");
		wavesDirection = material.GetFloat ("_WavesDirection");
	}

	public void SetMaterialProperties (MaterialPropertyBlock propertyBlock)
	{
		propertyBlock.SetColor ("_WaterColor", waterColor);
        propertyBlock.SetFloat ("_WaterLevel", waterLevelY);
		propertyBlock.SetFloat ("_WaveHeight", waveAmplitude);
		propertyBlock.SetFloat ("_WaveSpeed", waveSpeed);
		propertyBlock.SetFloat ("_WavesDirection", wavesDirection);
	}
}
