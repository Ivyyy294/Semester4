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

		CreateQuad();
		UpdateMesh();

		meshFilter.mesh = mesh;
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
