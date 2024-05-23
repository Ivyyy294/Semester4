using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetShapeGenerator
{
	PlanetShapeSetting m_shapeSetting;
	NoiseFilter[] m_noiseFilters;

	public PlanetShapeGenerator (PlanetShapeSetting shapeSetting)
	{
		m_shapeSetting = shapeSetting;
		m_noiseFilters = new NoiseFilter[m_shapeSetting.m_noiseLayers.Length];
		
		for (int i = 0; i < m_shapeSetting.m_noiseLayers.Length; ++i)
			m_noiseFilters[i] = new NoiseFilter(m_shapeSetting.m_noiseLayers[i].m_noiseSettings);
	}

	public Vector3 CalculatePointOnPlanet (Vector3 pointOnUnitSphere)
	{
		float elevation = 0;
		float firstLayerValue = 0;

		if (m_shapeSetting.m_noiseLayers.Length > 0)
		{
			firstLayerValue = m_noiseFilters[0].Evaluate(pointOnUnitSphere);

			if (m_shapeSetting.m_noiseLayers[0].m_enabled)
				elevation = firstLayerValue;
		}

		for (int i = 1; i < m_noiseFilters.Length; ++i)
		{
			PlanetShapeSetting.NoiseLayer layer = m_shapeSetting.m_noiseLayers[i];

			if (layer.m_enabled)
			{
				float mask = layer.useFirstLayerAsMask ? firstLayerValue : 1;
				elevation += m_noiseFilters[i].Evaluate (pointOnUnitSphere) * mask;
			}

		}

		return pointOnUnitSphere * m_shapeSetting.m_radius * (1 + elevation);
	}
}
