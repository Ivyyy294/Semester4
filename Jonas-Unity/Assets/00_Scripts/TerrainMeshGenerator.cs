using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class TerrainMeshGenerator : MeshGenerator
{
	[Header ("Terrain settings")]
	[SerializeField] float scale = 1.0f;
	[SerializeField] float perlinAmplitude = 1.0f;
	// Start is called before the first frame update
    void Start()
    {
        //Generate Quad Grid
		CreateQuad();

		AddHeightPerlin();

		InitMesh();

		//Center camera in z and place it with 50% spacing behind the Mesh
		Camera.main.transform.position = new Vector3 (quadColumns * quadWidth, 20, quadRows * quadWidth / 2);

		//Focus camera at center vertice of Mesh
		Camera.main.transform.LookAt (new Vector3 (quadColumns * quadWidth / 2f, 0f, quadRows * quadWidth / 2));
    }

	void AddHeightPerlin()
	{
		for (int z = 0, i = 0; z <= quadColumns; ++z)
		{
			for (int x = 0; x <= quadRows; ++x, i++)
			{
				vertices[i].y = GetHeight (x, z, scale) * perlinAmplitude;
			}
		}
	}

	float GetHeight (float x, float z, float scale)
	{
		if (scale <= 0)
			scale = 0.0001f;

		float height = 0f;

		height = Mathf.PerlinNoise (x / scale, z / scale);

		return height;
	}
}
