using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgidNoiseFilter : INoiseFilter
{
	NoiseSettings m_noiseSettings;
	Noise m_noise = new Noise();

	public RidgidNoiseFilter(NoiseSettings noiseSettings)
	{
		m_noiseSettings = noiseSettings;
	}

	public float Evaluate(Vector3 point)
	{
		float noiseValue = 0;
		float frequency = m_noiseSettings.m_baseRoughness;
		float amplitude = 1;
		float weight = 1;

		for (int i = 0; i < m_noiseSettings.m_numLayer; ++i)
		{
			float v = 1 - Mathf.Abs (m_noise.Evaluate(point * frequency + m_noiseSettings.m_centre));
			v *= v;
			v *= weight;
			weight = v;

			noiseValue += v * amplitude;
			frequency *= m_noiseSettings.m_roughness;
			amplitude *= m_noiseSettings.persistance;
		}

		noiseValue = Mathf.Max(0, noiseValue - m_noiseSettings.m_minVal);

		return noiseValue * m_noiseSettings.m_strength;
	}
}
