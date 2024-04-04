using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
	//Editor
	[Header ("Quad Settings")]
	[SerializeField] float quadWidth = 1f;
	[SerializeField] int rows = 1;
	[SerializeField] int columns = 1;

	Mesh mesh;
	Vector3[] vertices;
	int[] triangles;

	MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {

		meshFilter = GetComponent<MeshFilter>();

        mesh = new Mesh();

		CreateQuad2();
		UpdateMesh();

		meshFilter.mesh = mesh;

		Camera.main.transform.position = new Vector3 (columns * quadWidth / 2, 10, rows * quadWidth / 5);
		Camera.main.transform.LookAt (vertices[vertices.Length / 2]);
    }

	void CreateQuad2()
	{
		vertices = new Vector3[(1 + rows) * ( 1 + columns)];
		triangles = new int[6 * rows * columns];

		int indexVertice = 0;
		//int indexTriangle = 0;

		//Create Vertices
		for (int z = 0, i = 0; z <= columns; ++z)
		{
			for (int x = 0; x <= columns; ++x, ++i)
				vertices[indexVertice++] = new Vector3 (x * quadWidth, Mathf.Sin(z) + Mathf.Cos (x), z * quadWidth);
		}

		//Create Triangles
		int numberTriangles = 0;
		int currentVertice = 0;

		for (int x = 0; x < columns; x++)
		{
			for (int y = 0; y < rows; y++)
			{
				triangles[currentVertice] = numberTriangles + x;
				triangles[currentVertice + 1] = numberTriangles + rows + 1 + x;
				triangles[currentVertice + 2] = numberTriangles + 1 + x;
				triangles[currentVertice + 3] = numberTriangles + 1 + x;
				triangles[currentVertice + 4] = numberTriangles + rows + 1 + x;
				triangles[currentVertice + 5] = numberTriangles + rows + + 2 + x;

				numberTriangles++;
				currentVertice += 6;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (vertices == null)
			return;

		for (int i = 0; i < vertices.Length; i++)
			Gizmos.DrawSphere (vertices[i], 0.1f);
	}

	void CreateQuad()
	{
		vertices = new Vector3[4 * rows * columns];
		triangles = new int[vertices.Length * 3];

		int indexVertice = 0;
		int indexTriangle = 0;

		for (int iRow = 0; iRow < rows; iRow++)
		{
			for (int iColumn = 0; iColumn < columns; ++iColumn)
			{
				float xBase = quadWidth * iRow;
				float yBase = quadWidth * iColumn;

				vertices[indexVertice] = new Vector3 (xBase, 0, yBase);
				vertices[indexVertice + 1] = new Vector3 (xBase, 0, yBase + quadWidth);
				vertices[indexVertice + 2] = new Vector3 (xBase + quadWidth, 0, yBase);
				vertices[indexVertice + 3] = new Vector3 (xBase + quadWidth, 0, yBase + quadWidth);

				triangles[indexTriangle] = indexVertice;
				triangles[indexTriangle + 1] = indexVertice + 1;
				triangles[indexTriangle + 2] = indexVertice + 2;
				triangles[indexTriangle + 3] = indexVertice + 1;
				triangles[indexTriangle + 4] = indexVertice + 3;
				triangles[indexTriangle + 5] = indexVertice + 2;

				indexVertice += 4;
				indexTriangle += 6;
			}
		}

	}

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
        
    }
}
