using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	[Range (2, 256)]
	public int m_resolution;

	[SerializeField, HideInInspector]
	MeshFilter[] m_meshFilters;
	PlanetTerrainFace[] m_planetFaces;

	private void OnValidate()
	{
		Init();
		GenerateMesh();
	}

	private void Init()
	{
		if (m_meshFilters == null || m_meshFilters.Length == 0)
			m_meshFilters = new MeshFilter[6];
		
		m_planetFaces = new PlanetTerrainFace[6];

		Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

		for (int i = 0; i < 6; ++i)
		{
			if (m_meshFilters[i] == null)
			{
				GameObject meshObj = new GameObject ("mesh");
				meshObj.transform.parent = transform;

				meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find ("Standard"));
				m_meshFilters[i] = meshObj.AddComponent<MeshFilter>();
				m_meshFilters[i].sharedMesh = new Mesh();
			}
				
			m_planetFaces[i] = new PlanetTerrainFace (m_meshFilters[i].sharedMesh, m_resolution, directions[i]);
		}
	}

	void GenerateMesh()
	{
		foreach (PlanetTerrainFace face in m_planetFaces)
			face.ConstructMesh();
	}
}
