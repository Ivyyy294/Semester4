using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class TerrainMeshGenerator : MeshGenerator
{
	[Header ("Terrain settings")]
	[SerializeField] float scale = 20.0f;
	[SerializeField] float perlinAmplitude = 6.0f;
	[SerializeField] float perlinOctave = 3.0f;

	//controlls increase in frequency octaves
	[Min (1f)]
	[SerializeField] float lacunarity = 2.0f;

	//controlls decrease in amplitude of octaves
	[Range (0f, 1f)]
	[SerializeField] float persistance = 0.5f;

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

	private void Update()
	{
		AddHeightPerlin();
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}

	void AddHeightPerlin()
	{
		for (int z = 0, i = 0; z <= quadColumns; ++z)
		{
			for (int x = 0; x <= quadRows; ++x, i++)
			{
				vertices[i].y = GetHeight (x, z, scale, perlinOctave, lacunarity, persistance) * perlinAmplitude;
			}
		}
	}

	float GetHeight (float x, float z, float scale, float octaves, float lacunarity, float persistance)
	{
		if (scale <= 0)
			scale = 0.0001f;

		float height = 0f;
		float frequency = 1f;
		float amplitude = 1f;

		for (int i = 0; i < octaves; ++i)
		{
			float perlinValue = Mathf.PerlinNoise ((x / scale) * frequency, (z / scale) * frequency);
			height += perlinValue * amplitude;
			frequency *= lacunarity;
			amplitude *= persistance;
		}

		return height;
	}
}
