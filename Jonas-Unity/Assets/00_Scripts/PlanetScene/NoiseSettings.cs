using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
	[Range (1,8)]
	public int m_numLayer = 1;

	[Min (0)]
	public float m_strength = 1f;

	[Min(0)]
	public float m_baseRoughness = 1f;
	[Min(0)]
	public float m_roughness = 1f;
	[Min (0f)]
	public float persistance = 0.5f;
	public Vector3 m_centre;
	[Min (0)]
	public float m_minVal = 0f;
}
