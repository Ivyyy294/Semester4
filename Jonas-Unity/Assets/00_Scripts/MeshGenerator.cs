using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
	//Editor
	[Header ("Quad Settings")]
	[SerializeField] float quadWidth = 1f;
	[Range(1, 255)][SerializeField] int quadRows = 1;
	[Range(1, 255)][SerializeField] int quadColumns = 1;

	[Header ("Shader Settings")]
	[SerializeField] bool useComputeShader = false;
	[SerializeField] ComputeShader computeShader;

	//Mesh values
	MeshFilter meshFilter;
	Mesh mesh;
	Vector3[] vertices;
	int[] triangles;

	//Wave animation
	[System.Serializable]
	enum WaveAnimationFunc
	{
		SinCos = 0,
		Sin = 1,
		SumSin = 2,
		Experimental
	}

	[Header ("Quad Wave Settings")]
	[SerializeField] WaveAnimationFunc waveTyp;
	[SerializeField] float quadWaveHeight = 1f;
	[SerializeField] float quadWaveSpeed = 1f;
	[SerializeField] float quadWavesFrequency = 1f;
	[SerializeField] float quadWavesDirection = 0f;
	float waveAnimationTimer = 0f;
	ComputeBuffer verticeBuffer;

    // Start is called before the first frame update
    void Start()
    {
		//Generate Quad Grid
		if (useComputeShader)
		{
			CreateQuadGPU();
			InitQuadWaveAnimationGPU();
		}
		else
			CreateQuad();

		//Init Mesh
        mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;

		//Set Mesh to MeshFilter
		meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		//Center camera in z and place it with 50% spacing behind the Mesh
		Camera.main.transform.position = new Vector3 (quadColumns * quadWidth, 20, quadRows * quadWidth / 2);

		//Focus camera at center vertice of Mesh
		Camera.main.transform.LookAt (new Vector3 (quadColumns * quadWidth / 2f, 0f, quadRows * quadWidth / 2));
	}

	void Update()
    {
		if (useComputeShader)
			UpdateQuadWaveAnimationGPU();
		else
			UpdateQuadWaveAnimation();

		//Update Mesh
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}

	private void OnDestroy()
	{
		if (verticeBuffer != null)
			verticeBuffer.Dispose();
	}

	//GPU
	void CreateQuadGPU()
	{
		//We need 1 additional vertice per row and colums
		vertices = new Vector3[(1 + quadRows) * ( 1 + quadColumns)];

		verticeBuffer = new ComputeBuffer (vertices.Length, (sizeof (float) * 3));
		verticeBuffer.SetData (vertices);

		computeShader.SetBuffer (0, "Vertices", verticeBuffer);
		computeShader.SetFloat ("QuadSize", quadWidth);
		computeShader.SetFloat ("Columns", quadColumns);
		computeShader.SetFloat ("Rows", quadRows);

		int threadCountX = Mathf.CeilToInt ((quadColumns + 1f) / 8f);
		int threadCountZ = Mathf.CeilToInt ((quadRows + 1f) / 8f);

		computeShader.Dispatch (0, threadCountX, 1, threadCountZ);

		verticeBuffer.GetData (vertices);

		CreateTrianglesGPU();
	}

	void CreateTrianglesGPU()
	{
		//Each quad has 6 vertices
		triangles = new int[6 * quadRows * quadColumns];

		ComputeBuffer triangleBuffer = new ComputeBuffer (triangles.Length, sizeof (int));
		triangleBuffer.SetData (triangles);

		computeShader.SetBuffer (1, "Triangles", triangleBuffer);
		computeShader.SetFloat ("Columns", quadColumns);

		int threadCountX = Mathf.CeilToInt (quadColumns / 8f);
		int threadCountZ = Mathf.CeilToInt (quadRows / 8f);

		computeShader.Dispatch (1, threadCountX, 1, threadCountZ);

		triangleBuffer.GetData (triangles);
		triangleBuffer.Dispose();
	}

	void InitQuadWaveAnimationGPU()
	{
		computeShader.SetBuffer (2, "Vertices", verticeBuffer);
		computeShader.SetFloat ("Columns", quadColumns);
	}

	void UpdateQuadWaveAnimationGPU()
	{
		computeShader.SetInt("WaveTyp", (int)waveTyp);
		computeShader.SetFloat("WaveHeight", quadWaveHeight);
		computeShader.SetFloat("WaveAnimationTimer", waveAnimationTimer);
		computeShader.SetFloat("WaveSpeed", quadWaveSpeed);
		computeShader.SetFloat("WavesDirection", quadWavesDirection);
		computeShader.SetFloat("WavesFrequency", quadWavesFrequency);

		int threadCountX = Mathf.CeilToInt((quadColumns + 1f) / 8f);
		int threadCountZ = Mathf.CeilToInt((quadRows + 1f) / 8f);

		computeShader.Dispatch(2, threadCountX, 1, threadCountZ);

		verticeBuffer.GetData(vertices);

		waveAnimationTimer += Time.deltaTime;
	}

	//CPU
	void CreateQuad()
	{
		//We need 1 additional vertice per row and colums
		vertices = new Vector3[(1 + quadRows) * ( 1 + quadColumns)];

		//Each quad has 6 vertices
		triangles = new int[6 * quadRows * quadColumns];

		//Create Vertices
		//Patern:
		//0 1 2 3 4  5
		//6 7 8 9 10 11
		//...
		for (int z = 0, i = 0; z <= quadColumns; ++z)
		{
			for (int x = 0; x <= quadColumns; ++x, ++i)
				vertices[i] = new Vector3 (z * quadWidth, 0, x * quadWidth);
		}

		CreateTriangles();
	}

	void CreateTriangles()
	{
		//Create Triangles
		//Tell renderer which vertices build a rectangle
		//First rect is 0, 1, 6 in case we have a 5 column grid
		//Second rect is 1, 6, 7

		int currentVertice = 0;

		for (int x = 0; x < quadRows; x++)
		{
			for (int z = 0; z < quadColumns; z++)
			{
				int vBase = z + (x * (quadColumns + 1));
				int columnOffset = quadColumns + 1;

				triangles[currentVertice++] = vBase;
				triangles[currentVertice++] = vBase + 1;
				triangles[currentVertice++] = vBase + columnOffset;

				triangles[currentVertice++] = vBase + 1;
				triangles[currentVertice++] = vBase + columnOffset + 1;
				triangles[currentVertice++] = vBase + columnOffset;
			}
		}
	}

	void UpdateQuadWaveAnimation()
	{
		waveAnimationTimer += Time.deltaTime;

		for (int z = 0, i = 0; z <= quadColumns; ++z)
		{
			for (int x = 0; x <= quadColumns; ++x, ++i)
			{
				float baseHeight = Mathf.Sin(z + waveAnimationTimer) + Mathf.Cos (x + waveAnimationTimer);
				vertices[i].y = baseHeight * quadWaveHeight;
			}
		}
	}
}
