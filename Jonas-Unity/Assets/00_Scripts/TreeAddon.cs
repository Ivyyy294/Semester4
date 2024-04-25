using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TreeSettings
{
	public Vector2 levelMinMax;
	[Range (0f, 90f)] public float angleMax;
	[Range (0f, 1f)] public float chance;
}

class TreeAddon : MonoBehaviour, ITerrainGeneratorAddon
{
	public GameObject treePrefab;
	public TreeSettings properties;

	List <GameObject> treeObjList = new List<GameObject>();
	TreeSettings previousSettings = new TreeSettings();
	Mesh previousMesh;

    public void Apply (Mesh mesh)
	{
		if (!mesh || !enabled)
			return;
				
		//Disable unused grass
		for (int i = 0; i < treeObjList.Count; ++i)
			treeObjList[i].SetActive (false);

		int treeIndex = 0;

		for (int i = 0; i < mesh.vertexCount; ++i)
		{
			Vector3 v = mesh.vertices[i];
			float angle = Mathf.Abs (Vector3.Angle(mesh.normals[i], Vector3.up));

			if (v.y >= properties.levelMinMax.x && v.y <= properties.levelMinMax.y && angle <= properties.angleMax && properties.chance > rand (v))
			{
				if (treeIndex < treeObjList.Count)
				{
					treeObjList[treeIndex].SetActive (true);
					treeObjList[treeIndex].transform.localPosition = v;
					treeObjList[treeIndex].transform.up = Vector3.up;
				}
				else
				{
					var obj = Instantiate (treePrefab, transform);
					obj.transform.localPosition = v;
					obj.transform.up = Vector3.up;
					treeObjList.Add (obj);
				}
				treeIndex++;
			}
		}

		previousSettings = properties;
		previousMesh = mesh;
	}

	private void Update()
	{
		if (properties.levelMinMax != previousSettings.levelMinMax
			|| properties.angleMax != previousSettings.angleMax
			|| properties.chance != previousSettings.chance)
			Apply(previousMesh);
	}

	float rand (Vector3 co)
	{
		float dot = Vector3.Dot (co, new Vector3(12.9898f, 78.233f, 53.539f));

		return Mathf.Abs (Mathf.Sin (dot * 43758.5453f));
	}
}
