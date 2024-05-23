using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlanetShapeSetting : ScriptableObject
{
    public float m_radius = 1f;
	public NoiseLayer[] m_noiseLayers;

	[System.Serializable]
	public class NoiseLayer
	{
		public bool m_enabled = true;
		public bool useFirstLayerAsMask;
		public NoiseSettings m_noiseSettings;
	}
}
