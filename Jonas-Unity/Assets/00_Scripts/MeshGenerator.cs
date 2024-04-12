using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	//Editor
	[Header ("Quad Settings")]
	[SerializeField] protected float quadWidth = 1f;
	[Range(1, 255)][SerializeField] protected int quadRows = 1;
	[Range(1, 255)][SerializeField] protected int quadColumns = 1;

	[Header ("Shader Settings")]
	[SerializeField] protected ComputeShader quadGridComputeShader;

	//Mesh values
	protected MeshFilter meshFilter;
	protected Mesh mesh;
	protected Vector3[] vertices;
	protected int[] triangles;
	protected Vector2[] uv;
	protected Color[] colors;

	//Shader values
	protected ComputeBuffer verticeBuffer;

	protected void InitMesh()
	{
		//Init Mesh
        mesh = new Mesh();
		UpdateMesh();

		//Set Mesh to MeshFilter
		meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;
	}

	protected void UpdateMesh()
	{
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.RecalculateNormals();
	}

	protected void CreateQuad()
	{
		//We need 1 additional vertice per row and colums
		vertices = new Vector3[(1 + quadRows) * ( 1 + quadColumns)];

		verticeBuffer = new ComputeBuffer (vertices.Length, (sizeof (float) * 3));
		verticeBuffer.SetData (vertices);

		quadGridComputeShader.SetBuffer (0, "Vertices", verticeBuffer);
		quadGridComputeShader.SetFloat ("QuadSize", quadWidth);
		quadGridComputeShader.SetFloat ("Columns", quadColumns);
		quadGridComputeShader.SetFloat ("Rows", quadRows);

		int threadCountX = Mathf.CeilToInt ((quadColumns + 1f) / 8f);
		int threadCountZ = Mathf.CeilToInt ((quadRows + 1f) / 8f);

		quadGridComputeShader.Dispatch (0, threadCountX, 1, threadCountZ);

		verticeBuffer.GetData (vertices);

		CreateTriangles();
		CreateUV();
	}

	protected void CreateTriangles()
	{
		//Each quad has 6 vertices
		triangles = new int[6 * quadRows * quadColumns];

		ComputeBuffer triangleBuffer = new ComputeBuffer (triangles.Length, sizeof (int));
		triangleBuffer.SetData (triangles);

		quadGridComputeShader.SetBuffer (1, "Triangles", triangleBuffer);
		quadGridComputeShader.SetFloat ("Columns", quadColumns);

		int threadCountX = Mathf.CeilToInt (quadColumns / 8f);
		int threadCountZ = Mathf.CeilToInt (quadRows / 8f);

		quadGridComputeShader.Dispatch (1, threadCountX, 1, threadCountZ);

		triangleBuffer.GetData (triangles);
		triangleBuffer.Dispose();
	}

	protected void CreateUV()
	{
		uv = new Vector2[vertices.Length];
		ComputeBuffer uvBuffer = new ComputeBuffer (uv.Length, sizeof (float) * 2);
		uvBuffer.SetData (uv);

		quadGridComputeShader.SetBuffer (2, "UV", uvBuffer);

		int threadCountX = Mathf.CeilToInt ((quadColumns + 1f) / 8f);
		int threadCountZ = Mathf.CeilToInt ((quadRows + 1f) / 8f);

		quadGridComputeShader.Dispatch (2, threadCountX, 1, threadCountZ);

		uvBuffer.GetData (uv);
		uvBuffer.Dispose();
	}

	protected virtual void OnDestroy()
	{
		if (verticeBuffer != null)
			verticeBuffer.Dispose();
	}
}
