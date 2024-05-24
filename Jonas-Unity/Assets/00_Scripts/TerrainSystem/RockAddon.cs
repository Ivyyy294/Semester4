using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RockSettings
{
	public Vector2 levelMinMax;
	[Range (0f, 90f)] public float angleMax;
	[Range (0f, 1f)] public float chance;
}

public class RockAddon : MonoBehaviour, ITerrainGeneratorAddon
{
	public GameObject rockPrefab;
	public RockSettings properties;

	List <GameObject> rockObjList = new List<GameObject>();
	RockSettings previousSettings = new RockSettings();
	int rockIndex = 0;

	public bool Valid (Vector3 pos, Vector3 normal)
	{
		float angle = Mathf.Abs (Vector3.Angle(normal, Vector3.up));
		bool ok = pos.y >= properties.levelMinMax.x && pos.y <= properties.levelMinMax.y && angle <= properties.angleMax && properties.chance > rand (pos);
		return ok;
	}

	public void Spawn (Vector3 pos, Vector3 normal)
	{
		if (rockIndex < rockObjList.Count)
		{
			rockObjList[rockIndex].SetActive (true);
			rockObjList[rockIndex].transform.localPosition = pos;
		}
		else
		{
			var obj = Instantiate (rockPrefab, transform);
			obj.transform.localPosition = pos;
			obj.transform.up = Random.insideUnitSphere;
			obj.transform.localScale *= 0.75f + rand (pos);
			rockObjList.Add (obj);
		}
		rockIndex++;
	}

	public bool Dirty()
	{
		return (properties.levelMinMax != previousSettings.levelMinMax
			|| properties.angleMax != previousSettings.angleMax
			|| properties.chance != previousSettings.chance);
	}

	public void Reset()
	{
		rockIndex = 0;

		//Disable unused grass
		for (int i = 0; i < rockObjList.Count; ++i)
			rockObjList[i].SetActive (false);

		previousSettings = properties;
	}

	float rand (Vector3 co)
	{
		float dot = Vector3.Dot (co, new Vector3(12.9898f, 78.233f, 53.539f));

		return Mathf.Abs (Mathf.Sin (dot * 43758.5453f));
	}
}
