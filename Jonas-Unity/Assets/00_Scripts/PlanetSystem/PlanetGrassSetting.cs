using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlanetGrassSetting : ScriptableObject, IShaderSetting
{
	[Range(0f, 1000f)] public float m_minGrassLevel;
	[Range(0f, 1000f)] public float m_maxGrassLevel;
	public Color m_topColor;
	public Color m_bottomColor;
	[Range(0.01f, 1f)] public float m_bladeWidth;
	[Range(0.01f, 1f)] public float m_bladeHeight;
	[Range(0f, 1)] public float m_bendRotationRandom;
	[Range(0f, 1)] public float m_bladeWidthRandom;
	[Range(0f, 1)] public float m_bladeHeightRandom;
	[Range(1, 64)] public float m_tessellationUniform;

	public void SetMaterialProperties(MaterialPropertyBlock propertyBlock)
	{
		propertyBlock.SetFloat("_MinGrassLevel", m_minGrassLevel);
		propertyBlock.SetFloat("_MaxGrassLevel", m_maxGrassLevel);
		propertyBlock.SetColor("_TopColor", m_topColor);
		propertyBlock.SetColor("_BottomColor", m_bottomColor);
		propertyBlock.SetFloat("_BladeWidth", m_bladeWidth);
		propertyBlock.SetFloat("_BladeHeight", m_bladeHeight);
		propertyBlock.SetFloat("_BendRotationRandom", m_bendRotationRandom);
		propertyBlock.SetFloat("_BladeWidthRandom", m_bladeWidthRandom);
		propertyBlock.SetFloat("_BladeHeightRandom", m_bladeHeightRandom);
		propertyBlock.SetFloat("_TessellationUniform", m_tessellationUniform);
	}
}
