using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
	//Editor
	[Header ("Quad Settings")]
	[SerializeField] float quadWidth = 1f;
	[SerializeField] int quadRows = 1;
	[SerializeField] int quadColumns = 1;

	[Space]
	[SerializeField] bool useComputeShader = false;
	[SerializeField] ComputeShader computeShader;

	//Mesh values
	MeshFilter meshFilter;
	Mesh mesh;
	Vector3[] vertices;
	int[] triangles;

	//Wave animation
	[Header ("Quad Wave Settings")]
	[SerializeField] float quadWaveHeight = 1f;
	float waveAnimationTimer = 0f;
	ComputeBuffer verticeBuffer;


    // Start is called before the first frame update
    void Start()
    {
		//Create Mesh
        mesh = new Mesh();

		if (useComputeShader)
		{
			CreateQuadGPU();
			InitQuadWaveAnimationGPU();
		}
		else
			CreateQuad();

		UpdateMesh();

		//Set Mesh to MeshFilter
		meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		//Center camera in z and place it with 50% spacing behind the Mesh
		Camera.main.transform.position = new Vector3 (quadColumns * quadWidth, 10, quadRows * quadWidth / 2);

		//Focus camera at center vertice of Mesh
		Camera.main.transform.LookAt (vertices[vertices.Length / 2]);
	}

	//private void OnDrawGizmos()
	//{
	//	if (vertices == null)
	//		return;

	//	for (int i = 0; i < vertices.Length; i++)
	//		Gizmos.DrawSphere(vertices[i], 0.1f);
	//}

	void UpdateMesh()
	{
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}

    // Update is called once per frame
    void Update()
    {
		if (useComputeShader)
			UpdateQuadWaveAnimationGPU();
		else
			UpdateQuadWaveAnimation();

		UpdateMesh();
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

		computeShader.Dispatch (0, quadColumns + 1, 1, quadRows + 1);

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

		computeShader.Dispatch (1, quadRows, 1, quadColumns);

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
		computeShader.SetFloat ("WaveHeight", quadWaveHeight);
		computeShader.SetFloat ("WaveAnimationTimer", waveAnimationTimer);
		computeShader.Dispatch (2, quadColumns + 1, 1, quadRows + 1);

		verticeBuffer.GetData (vertices);

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
				vertices[i] = new Vector3 (x * quadWidth, 0, z * quadWidth);
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
				triangles[currentVertice++] = vBase + columnOffset;
				triangles[currentVertice++] = vBase + 1;

				triangles[currentVertice++] = vBase + 1;
				triangles[currentVertice++] = vBase + columnOffset;
				triangles[currentVertice++] = vBase + columnOffset + 1;

				//int vBase = z + (x * (quadColumns + 1)); 
				//int columnOffset = quadColumns + 1;

				//triangles[currentVertice++]	= vBase;
				//triangles[currentVertice++] = vBase + 1;
				//triangles[currentVertice++] = vBase + columnOffset;

				//triangles[currentVertice++] = vBase + 1;
				//triangles[currentVertice++] = vBase + columnOffset + 1;
				//triangles[currentVertice++] = vBase + columnOffset;
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
