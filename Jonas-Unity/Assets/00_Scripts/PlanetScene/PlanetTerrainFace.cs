using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTerrainFace
{
	Mesh m_mesh;
	int m_resolution;
	Vector3 m_localUp;
	Vector3 m_axisA;
	Vector3 m_axisB;

	public PlanetTerrainFace (Mesh mesh, int resolution, Vector3 localUp)
	{
		m_mesh = mesh;
		m_resolution = resolution;
		m_localUp = localUp;

		m_axisA = new Vector3 (localUp.y, localUp.z, localUp.x);
		m_axisB = Vector3.Cross (localUp, m_axisA);
	}

	public void ConstructMesh()
	{
		Vector3[] vertices = new Vector3[m_resolution * m_resolution];
		int[] triangles = new int[(m_resolution -1) * (m_resolution -1) * 6];

		int triIndex = 0;

		for (int y = 0; y < m_resolution; ++y)
		{
			for (int x = 0; x < m_resolution; ++x)
			{
				int i = x + y * m_resolution;
				Vector2 percent = new Vector2 (x, y) / (m_resolution - 1);
				Vector3 pointOnUnitCubeAxisA = (percent.x - 0.5f) * 2 * m_axisA;
				Vector3 pointOnUnitCubeAxisB = (percent.y - 0.5f) * 2 * m_axisB;
				Vector3 pointOnUnitCube = m_localUp + pointOnUnitCubeAxisA + pointOnUnitCubeAxisB;
				Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
				vertices[i] = pointOnUnitSphere;

				if (x != m_resolution -1 && y != m_resolution -1)
				{
					triangles[triIndex] = i;
					triangles[triIndex + 1] = i + m_resolution + 1;
					triangles[triIndex + 2] = i + m_resolution;
					
					triangles[triIndex + 3] = i;
					triangles[triIndex + 4] = i + 1;
					triangles[triIndex + 5] = i + m_resolution + 1;

					triIndex += 6;
				}
			}
		}

		m_mesh.Clear();
		m_mesh.vertices = vertices;
		m_mesh.triangles = triangles;
		m_mesh.RecalculateNormals();
	}
}
