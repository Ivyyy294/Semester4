using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassAddon : MonoBehaviour, ITerrainGeneratorAddon
{
	public Vector2 levelMinMax;

	public GameObject grassPrefab;

	List <GameObject> grassObjList = new List<GameObject>();

	Vector2 previousSettings;
	Mesh previousMesh;

	public void Apply (Mesh mesh)
	{
		int grassIndex = 0;

		for (int i = 0; i < mesh.vertexCount; ++i)
		{
			Vector3 v = mesh.vertices[i];

			if (v.y >= levelMinMax.x && v.y <= levelMinMax.y)
			{
				if (grassIndex < grassObjList.Count)
				{
					grassObjList[grassIndex].SetActive (true);
					grassObjList[grassIndex].transform.position = v;
					grassObjList[grassIndex].transform.up = mesh.normals[i];
				}
				else
				{
					var obj = Instantiate (grassPrefab, v, Quaternion.identity, transform);
					obj.transform.up = mesh.normals[i];
					grassObjList.Add (obj);
				}
				grassIndex++;
			}
		}

		//Disable unused grass
		for (int i = grassObjList.Count - 1; i > grassIndex; --i)
			grassObjList[i].SetActive (false);

		previousSettings = levelMinMax;
		previousMesh = mesh;
	}

	private void Update()
	{
		if (levelMinMax != previousSettings)
			Apply(previousMesh);
	}
}
