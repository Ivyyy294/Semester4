using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockAddon : MonoBehaviour, ITerrainGeneratorAddon
{
	public GameObject rockPrefab;
	public Vector2 levelMinMax;

	List <GameObject> rockObjList = new List<GameObject>();
	Vector2 previousSettings;
	Mesh previousMesh;

    public void Apply (Mesh mesh)
	{
		if (!mesh)
			return;

		int grassIndex = 0;

		for (int i = 0; i < mesh.vertexCount; ++i)
		{
			Vector3 v = mesh.vertices[i];

			if (v.y >= levelMinMax.x && v.y <= levelMinMax.y)
			{
				if (grassIndex < rockObjList.Count)
				{
					rockObjList[grassIndex].SetActive (true);
					rockObjList[grassIndex].transform.position = v;
					rockObjList[grassIndex].transform.up = mesh.normals[i];
				}
				else
				{
					var obj = Instantiate (rockPrefab, v, Quaternion.identity, transform);
					obj.transform.up = mesh.normals[i];
					rockObjList.Add (obj);
				}
				grassIndex++;
			}
		}

		//Disable unused grass
		for (int i = rockObjList.Count - 1; i > grassIndex; --i)
			rockObjList[i].SetActive (false);

		previousSettings = levelMinMax;
		previousMesh = mesh;
	}

	private void Update()
	{
		if (levelMinMax != previousSettings)
			Apply(previousMesh);
	}
}
