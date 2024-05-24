using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlanetColorSetting : ScriptableObject, IShaderSetting
{
    public Color m_beachColor;
	public Texture2D m_planetTexture;

	public void SetMaterialProperties(MaterialPropertyBlock propertyBlock)
	{
		propertyBlock.SetColor("_BeachColor", m_beachColor);
		propertyBlock.SetTexture("_MainTex", m_planetTexture);
	}
}
