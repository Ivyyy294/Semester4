using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlantSettings
{
	public Vector2 levelMinMax;
	[Range (0f, 90f)] public float angleMax;
	[Range (0f, 1f)] public float chance;
}

public class PlantAddon : MonoBehaviour, ITerrainGeneratorAddon
{
	public GameObject[] plantPrefab;
	public PlantSettings properties;

	List <GameObject> plantObjList = new List<GameObject>();
	PlantSettings previousSettings = new PlantSettings();
	int plantIndex = 0;

	public bool Valid (Vector3 pos, Vector3 normal)
	{
		float angle = Mathf.Abs (Vector3.Angle(normal, Vector3.up));
		bool ok = pos.y >= properties.levelMinMax.x && pos.y <= properties.levelMinMax.y && angle <= properties.angleMax && properties.chance > rand (pos);
		return ok;
	}

	public void Spawn (Vector3 pos, Vector3 normal)
	{
		if (plantIndex < plantObjList.Count)
		{
			plantObjList[plantIndex].SetActive (true);
			plantObjList[plantIndex].transform.localPosition = pos;
			plantObjList[plantIndex].transform.up = normal;
		}
		else
		{
			int index = Random.Range (0, plantPrefab.Length);
			var obj = Instantiate (plantPrefab[index], transform);
			obj.transform.localPosition = pos;
			obj.transform.up = normal;
			obj.transform.localScale *= 0.25f + rand (pos);
			plantObjList.Add (obj);
		}
		plantIndex++;
	}

	public bool Dirty()
	{
		return (properties.levelMinMax != previousSettings.levelMinMax
			|| properties.angleMax != previousSettings.angleMax
			|| properties.chance != previousSettings.chance);
	}

	public void Reset()
	{
		plantIndex = 0;

		//Disable unused grass
		for (int i = 0; i < plantObjList.Count; ++i)
			plantObjList[i].SetActive (false);

		previousSettings = properties;
	}

	float rand (Vector3 co)
	{
		float dot = Vector3.Dot (co, new Vector3(12.9898f, 78.233f, 53.539f));

		return Mathf.Abs (Mathf.Sin (dot * 43758.5453f));
	}
}
