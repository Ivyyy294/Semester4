using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetShapeGenerator
{
	PlanetShapeSetting m_shapeSetting;
    
	public PlanetShapeGenerator (PlanetShapeSetting shapeSetting)
	{
		m_shapeSetting = shapeSetting;
	}

	public Vector3 CalculatePointOnPlanet (Vector3 pointOnUnitSphere)
	{
		return pointOnUnitSphere * m_shapeSetting.m_radius;
	}
}
