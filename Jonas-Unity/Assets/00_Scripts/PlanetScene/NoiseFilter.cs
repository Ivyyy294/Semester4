using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter
{
	NoiseSettings m_noiseSettings;
	Noise m_noise = new Noise();

	public NoiseFilter (NoiseSettings noiseSettings)
	{
		m_noiseSettings = noiseSettings;
	}

	public float Evaluate (Vector3 point)
	{
		float noiseValue = 0;
		float frequency = m_noiseSettings.m_baseRoughness;
		float amplitude = 1;

		for (int i = 0; i < m_noiseSettings.m_numLayer; ++i)
		{
			float v = m_noise.Evaluate (point * frequency + m_noiseSettings.m_centre);
			noiseValue += (v + 1) * 0.5f * amplitude;

			frequency *= m_noiseSettings.m_roughness;
			amplitude *= m_noiseSettings.persistance;
		}

		noiseValue = Mathf.Max (0, noiseValue - m_noiseSettings.m_minVal);

		return noiseValue * m_noiseSettings.m_strength;
	}
}
