using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class TerrainMeshGenerator : MeshGenerator
{
	[Header ("Terrain settings")]
	[SerializeField] ComputeShader shader;
	[SerializeField] float scale = 20.0f;
	[Min (1f)]
	[SerializeField] float perlinAmplitude = 6.0f;
	[Min (1)]
	[SerializeField] int perlinOctaves = 3;

	//controlls increase in frequency octaves
	[Min (1f)]
	[SerializeField] float lacunarity = 2.0f;

	//controlls decrease in amplitude of octaves
	[Range (0f, 1f)]
	[SerializeField] float persistance = 0.5f;

	[SerializeField] Gradient terrainColorGradient;

	[Header ("Water settings")]
	[SerializeField] Color waterColor;
	[SerializeField] float waterLevelY = 1f;
	[SerializeField] float waveAmplitude = 1f;
	[SerializeField] float waveSpeed = 2f;
	[SerializeField] float waveLength = 25f;
	[SerializeField] float wavesDirection = 30f;
	float timer = 0f;

	float minTerrainHeight = 0f;
	float maxTerrainHeight = 0f;

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
		AddColor();
		
		TerrainComputeShader();

		mesh.colors = colors;
		mesh.RecalculateNormals();
		
		//UpdateMesh();
	}

	void AddHeightPerlin()
	{
		for (int z = 0, i = 0; z <= quadColumns; ++z)
		{
			for (int x = 0; x <= quadRows; ++x, i++)
			{
				float h = GetHeight (x, z, scale, perlinOctaves, lacunarity, persistance) * perlinAmplitude;
				
				//Cap y at water level
				//if (h < waterLevelY)
				//	h = waterLevelY;

				vertices[i].y = h;

				if (i == 0)
				{
					minTerrainHeight = vertices[i].y;
					maxTerrainHeight = vertices[i].y;
				}
				else if (vertices[i].y < minTerrainHeight)
					minTerrainHeight = vertices[i].y;
				else if (vertices[i].y > maxTerrainHeight)
					maxTerrainHeight = vertices[i].y;
			}
		}
	}

	void AddColor ()
	{
		colors = new Color[vertices.Length];
		
		for (int z = 0, i = 0; z <= quadColumns; ++z)
		{
			for (int x = 0; x <= quadRows; ++x, i++)
			{
				if (vertices[i].y <= waterLevelY)
					colors[i] = waterColor;
				else
					colors[i] = terrainColorGradient.Evaluate(Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y));
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

	void TerrainComputeShader()
	{
		shader.SetFloat ("Width", quadColumns);
		shader.SetFloat ("Height", quadRows);
		shader.SetFloat ("WaterLevelY", waterLevelY);

		shader.SetFloat ("WaveAnimationTimer", timer);
		shader.SetFloat ("WaveHeight", waveAmplitude);
		shader.SetFloat ("WaveSpeed", waveSpeed);
		shader.SetFloat ("WavesDirection", wavesDirection);

		float waveFrequency = 2f * Mathf.PI / waveLength;
		shader.SetFloat ("WavesFrequency", waveFrequency);

		verticeBuffer.SetData (vertices);
		shader.SetBuffer (0, "Vertices", verticeBuffer);

		int threadCountX = Mathf.CeilToInt((quadColumns + 1f) / 8f);
		int threadCountZ = Mathf.CeilToInt((quadRows + 1f) / 8f);

		shader.Dispatch(0, threadCountX, 1, threadCountZ);

		Vector3[] tmpVertices = new Vector3[vertices.Length];
		verticeBuffer.GetData (tmpVertices);

		mesh.vertices = tmpVertices;

		timer += Time.deltaTime;
	}
}
