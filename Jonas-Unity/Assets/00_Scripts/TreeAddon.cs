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
	int treeIndex = 0;

	public bool Valid (Vector3 pos, Vector3 normal)
	{
		float angle = Mathf.Abs (Vector3.Angle(normal, Vector3.up));
		bool ok = pos.y >= properties.levelMinMax.x && pos.y <= properties.levelMinMax.y && angle <= properties.angleMax && properties.chance > rand (pos);
		return ok;
	}

	public void Spawn (Vector3 pos, Vector3 normal)
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

	public bool Dirty()
	{
		return (properties.levelMinMax != previousSettings.levelMinMax
			|| properties.angleMax != previousSettings.angleMax
			|| properties.chance != previousSettings.chance);
	}

	public void Reset()
	{
		treeIndex = 0;

		//Disable unused grass
		for (int i = 0; i < treeObjList.Count; ++i)
			treeObjList[i].SetActive (false);

		previousSettings = properties;
	}

	float rand (Vector3 co)
	{
		float dot = Vector3.Dot (co, new Vector3(12.9898f, 78.233f, 53.539f));

		return Mathf.Abs (Mathf.Sin (dot * 43758.5453f));
	}
}
