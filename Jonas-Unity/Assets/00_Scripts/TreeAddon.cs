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
	int treeIndex = 0;

    public void Apply (Mesh mesh)
	{
		if (!mesh || !enabled)
			return;

		StopAllCoroutines();

		//Disable unused grass
		for (int i = 0; i < treeObjList.Count; ++i)
			treeObjList[i].SetActive (false);

		treeIndex = 0;

		StartCoroutine (ApplyIntern (mesh));

		previousSettings = properties;
		previousMesh = mesh;
	}

	private bool Valid (Vector3 pos, Vector3 normal)
	{
		float angle = Mathf.Abs (Vector3.Angle(normal, Vector3.up));
		bool ok = pos.y >= properties.levelMinMax.x && pos.y <= properties.levelMinMax.y && angle <= properties.angleMax && properties.chance > rand (pos);
		return ok;
	}

	private void Spawn (Vector3 pos)
	{
		if (treeIndex < treeObjList.Count)
		{
			treeObjList[treeIndex].SetActive (true);
			treeObjList[treeIndex].transform.localPosition = pos;
		}
		else
		{
			var obj = Instantiate (treePrefab, transform);
			obj.transform.localPosition = pos;
			obj.transform.localScale *= 0.5f + rand (pos);
			obj.transform.Rotate (Vector3.up, Random.value * 360f);
			treeObjList.Add (obj);
		}
		treeIndex++;
	}

	IEnumerator ApplyIntern (Mesh mesh)
	{
		for (int i = 0; i < mesh.vertices.Length; ++i)
		{
			Vector3 v = mesh.vertices[i];

			if (Valid (v, mesh.normals[i]))
			{
				Spawn (v);

				if (i % 8 == 0)
					yield return null;
			}
		}

		yield return true;
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
