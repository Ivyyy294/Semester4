using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassAddon : MonoBehaviour, ITerrainGeneratorShaderAddon
{
	[SerializeField] [Range(0f, 50f)] float MinGrassLevel;
	[SerializeField] [Range(0f, 50f)] float MaxGrassLevel;
	[SerializeField] Color TopColor;
	[SerializeField] Color BottomColor;
	[SerializeField] [Range(0.01f, 1f)] float BladeWidth;
	[SerializeField] [Range(0.01f, 1f)] float BladeHeight;
	[SerializeField] [Range(0f, 1)] float BendRotationRandom;
	[SerializeField] [Range(0f, 1)] float BladeWidthRandom;
	[SerializeField] [Range(0f, 1)] float BladeHeightRandom;
	[SerializeField] [Range(1, 64)]float TessellationUniform;

	public void LoadDefaultValues(Material material)
	{		
		MinGrassLevel = material.GetFloat ("_MinGrassLevel");
		MaxGrassLevel = material.GetFloat ("_MaxGrassLevel");
		TopColor = material.GetColor ("_TopColor");
		BottomColor = material.GetColor ("_BottomColor");
		BladeWidth = material.GetFloat ("_BladeWidth");
		BladeHeight = material.GetFloat ("_BladeHeight");
		BendRotationRandom = material.GetFloat ("_BendRotationRandom");
		BladeWidthRandom = material.GetFloat ("_BladeWidthRandom");
		BladeHeightRandom = material.GetFloat ("_BladeHeightRandom");
		TessellationUniform = material.GetFloat ("_TessellationUniform");
		//waterLevelY = material.GetFloat ("_WaterLevel");
		//waveAmplitude = material.GetFloat ("_WaveHeight");
		//waveSpeed = material.GetFloat ("_WaveSpeed");
		//wavesDirection = material.GetFloat ("_WavesDirection");
	}

	public void SetMaterialProperties (MaterialPropertyBlock propertyBlock)
	{
		propertyBlock.SetFloat ("_MinGrassLevel", MinGrassLevel);
        propertyBlock.SetFloat ("_MaxGrassLevel", MaxGrassLevel);
		propertyBlock.SetColor ("_TopColor", TopColor);
        propertyBlock.SetColor ("_BottomColor", BottomColor);
		propertyBlock.SetFloat ("_BladeWidth", BladeWidth);
        propertyBlock.SetFloat ("_BladeHeight", BladeHeight);
		propertyBlock.SetFloat ("_BendRotationRandom", BendRotationRandom);
        propertyBlock.SetFloat ("_BladeWidthRandom", BladeWidthRandom);
		propertyBlock.SetFloat ("_BladeHeightRandom", BladeHeightRandom);
        propertyBlock.SetFloat ("_TessellationUniform", TessellationUniform);
	}
}
