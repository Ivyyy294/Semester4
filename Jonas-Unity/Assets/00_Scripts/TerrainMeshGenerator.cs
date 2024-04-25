using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ITerrainGeneratorAddon
{
	public void Apply(Mesh mesh);
}

interface ITerrainGeneratorShaderAddon
{
	public void LoadDefaultValues (Material m);
	public void SetMaterialProperties (MaterialPropertyBlock propertyBlock);
}

[System.Serializable]
class TerrainSettings
{
	public float scale = 20.0f;
	public float perlinAmplitude = 6.0f;
	public int perlinOctaves = 3;
	public float lacunarity = 2.0f;
	public float persistance = 0.5f;
}

[RequireComponent (typeof(MeshFilter))]
public class TerrainMeshGenerator : MeshGenerator
{
	[Header ("Terrain settings")]
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

	TerrainSettings terrain = new TerrainSettings();
	float minTerrainHeight = 0f;
	float maxTerrainHeight = 0f;
	MeshRenderer meshRenderer;
	float offsetX;
	float offsetZ;
	int chunkDimention;

	public void Init (int _chunkDimention, float x, float z)
	{
		chunkDimention = _chunkDimention;
		quadRows = chunkDimention;
		quadColumns = chunkDimention;
		offsetX = x;
		offsetZ = z;

        //Generate Quad Grid
		CreateQuad();

		AddHeightPerlin();

		InitMesh();

		//Init Shader Addons
		meshRenderer = GetComponent<MeshRenderer>();
		foreach(var i in GetComponents<ITerrainGeneratorShaderAddon>())
			i.LoadDefaultValues (meshRenderer.material);
	}

	private void Update()
	{
		if (UpdateTerrainSettings())
		{
			AddHeightPerlin();
			UpdateMesh();
			UpdateTerrainAddons();
		}

		UpdateTerrainShaderAddons();
	}

	void AddHeightPerlin()
	{
		for (int z = 0, i = 0; z <= quadColumns; ++z)
		{
			for (int x = 0; x <= quadRows; ++x, i++)
			{
				float h = GetHeight(x + (offsetZ * chunkDimention), z + (offsetX * chunkDimention), scale, perlinOctaves, lacunarity, persistance) * perlinAmplitude;

				vertices[i].y = h;

				//if (i == 0)
				//{
				//	minTerrainHeight = vertices[i].y;
				//	maxTerrainHeight = vertices[i].y;
				//}
				//else if (vertices[i].y < minTerrainHeight)
				//	minTerrainHeight = vertices[i].y;
				//else if (vertices[i].y > maxTerrainHeight)
				//	maxTerrainHeight = vertices[i].y;
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

	//Return true if settings were modified
	bool UpdateTerrainSettings()
	{
		bool val = terrain.scale != scale
			|| terrain.lacunarity != lacunarity
			|| terrain.perlinAmplitude != perlinAmplitude
			|| terrain.perlinOctaves != perlinOctaves
			|| terrain.persistance != persistance
			|| terrain.scale != scale;

		terrain.scale = scale;
		terrain.lacunarity = lacunarity;
		terrain.perlinAmplitude = perlinAmplitude;
		terrain.perlinOctaves = perlinOctaves;
		terrain.persistance = persistance;
		terrain.scale = scale;

		return val;
	}

	void UpdateTerrainAddons()
	{
		foreach(var i in GetComponents<ITerrainGeneratorAddon>())
			i.Apply (mesh);
	}

	void UpdateTerrainShaderAddons()
	{
		MaterialPropertyBlock mp = new MaterialPropertyBlock();

		mp.SetFloat ("_OffsetX", offsetX * chunkDimention);
		mp.SetFloat ("_OffsetZ", offsetZ * chunkDimention);

		foreach(var i in GetComponents<ITerrainGeneratorShaderAddon>())
			i.SetMaterialProperties (mp);

		meshRenderer.SetPropertyBlock (mp);
	}
}
