using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlanetWaterSetting : ScriptableObject, IShaderSetting
{
	[Header("Water appearance")]
	public Color m_waterColor;
	public Color m_waterPeakColor;
	public Color m_waterFoamColor;
	[Range(0, 1)] public float m_waterSmoothness;
	[Range(0, 1)] public float m_waterPeakIntensity;
	[Range(0, 1)] public float m_waterPeakArea;

	[Header("Water settings")]
	public float m_waterLevelY;
	[Min(1)] public int m_waveCount;
	[Range(0, 1)] public float m_waveSteepness;
	public float m_waveLength;
	public float m_waveSpeed;
	public float m_wavesDirection;

	public void SetMaterialProperties(MaterialPropertyBlock propertyBlock)
	{
		propertyBlock.SetColor("_WaterColor", m_waterColor);
		propertyBlock.SetColor("_WaterPeakColor", m_waterPeakColor);
		propertyBlock.SetColor("_WaterFoamColor", m_waterFoamColor);

		propertyBlock.SetFloat("_WaterPeakIntensity", m_waterPeakIntensity);
		propertyBlock.SetFloat("_WaterPeakArea", m_waterPeakArea);

		propertyBlock.SetFloat("_WaterLevel", m_waterLevelY);
		propertyBlock.SetFloat("_WaterSmoothness", m_waterSmoothness);
		propertyBlock.SetInt("_WaveCount", m_waveCount);
		propertyBlock.SetFloat("_WaveSteepness", m_waveSteepness);
		propertyBlock.SetFloat("_WaveSpeed", m_waveSpeed);
		propertyBlock.SetFloat("_WavesDirection", m_wavesDirection);
		propertyBlock.SetFloat("_WavesLength", m_waveLength);
	}
}
