using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TreeSettings
{
	public Vector2 levelMinMax;
	public float angleMax = 45f;
	public float chance;
}

class TreeAddon : MonoBehaviour, ITerrainGeneratorAddon
{
	public GameObject treePrefab;
	public Vector2 levelMinMax;

	[Range (0f, 90f)]
	public float angleMax;

	[Range (0f, 1f)]
	public float chance = 1f;

	List <GameObject> treeObjList = new List<GameObject>();
	TreeSettings previousSettings = new TreeSettings();
	Mesh previousMesh;

    public void Apply (Mesh mesh)
	{
		if (!mesh || !enabled)
			return;

		int treeIndex = 0;

		for (int i = 0; i < mesh.vertexCount; ++i)
		{
			Vector3 v = mesh.vertices[i];
			float angle = Mathf.Abs (Vector3.Angle(mesh.normals[i], Vector3.up));

			if (v.y >= levelMinMax.x && v.y <= levelMinMax.y && angle <= angleMax && chance >= Random.value)
			{
				if (treeIndex < treeObjList.Count)
				{
					treeObjList[treeIndex].SetActive (true);
					treeObjList[treeIndex].transform.position = v;
					treeObjList[treeIndex].transform.up = Vector3.up;
				}
				else
				{
					var obj = Instantiate (treePrefab, v, Quaternion.identity, transform);
					obj.transform.up = Vector3.up;
					treeObjList.Add (obj);
				}
				treeIndex++;
			}
		}

		//Disable unused grass
		for (int i = treeObjList.Count - 1; i > treeIndex; --i)
			treeObjList[i].SetActive (false);

		previousSettings.levelMinMax = levelMinMax;
		previousSettings.angleMax = angleMax;
		previousSettings.chance = chance;
		previousMesh = mesh;
	}

	private void Update()
	{
		if (levelMinMax != previousSettings.levelMinMax
			|| angleMax != previousSettings.angleMax
			|| chance != previousSettings.chance)
			Apply(previousMesh);
	}
}
