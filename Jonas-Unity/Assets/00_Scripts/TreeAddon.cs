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
				
		//Disable unused grass
		for (int i = 0; i < treeObjList.Count; ++i)
			treeObjList[i].SetActive (false);

		int treeIndex = 0;

		for (int i = 0; i < mesh.vertexCount; ++i)
		{
			Vector3 v = mesh.vertices[i];
			float angle = Mathf.Abs (Vector3.Angle(mesh.normals[i], Vector3.up));

			if (v.y >= levelMinMax.x && v.y <= levelMinMax.y && angle <= angleMax && chance > rand (v))
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

	float rand (Vector3 co)
	{
		float dot = Vector3.Dot (co, new Vector3(12.9898f, 78.233f, 53.539f));

		return Mathf.Abs (Mathf.Sin (dot * 43758.5453f));
	}
}
