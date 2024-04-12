using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class TerrainMeshGenerator : MeshGenerator
{
	// Start is called before the first frame update
    void Start()
    {
        //Generate Quad Grid
		CreateQuad();

		InitMesh();

		//Center camera in z and place it with 50% spacing behind the Mesh
		Camera.main.transform.position = new Vector3 (quadColumns * quadWidth, 20, quadRows * quadWidth / 2);

		//Focus camera at center vertice of Mesh
		Camera.main.transform.LookAt (new Vector3 (quadColumns * quadWidth / 2f, 0f, quadRows * quadWidth / 2));
    }
}
