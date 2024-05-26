using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlanetRockSetting: ScriptableObject
{
	public GameObject rockPrefab;
	public Vector2 levelMinMax;
	[Range (0f, 90f)] public float angleMax;
	[Range (0f, 1f)] public float chance;

	List <GameObject> rockObjList = new List<GameObject>();
	int rockIndex = 0;

	public bool Valid (Vector3 pos, Vector3 normal)
	{
		float angle = Mathf.Abs (Vector3.Angle(normal, Vector3.up));
		float distance = Mathf.Abs (Vector3.Distance (Vector3.zero, pos));
		bool ok = distance >= levelMinMax.x && distance <= levelMinMax.y && angle <= angleMax && chance > rand (pos);
		return ok;
	}

	public void Spawn (Vector3 pos, Vector3 normal, Transform parent)
	{
		//if (rockIndex < rockObjList.Count)
		//{
		//	rockObjList[rockIndex].SetActive (true);
		//	rockObjList[rockIndex].transform.localPosition = pos;
		//}
		//else
		//{
			var obj = Instantiate (rockPrefab, parent);
			obj.transform.position = pos;
			obj.transform.up = Random.insideUnitSphere;
			obj.transform.localScale *= 0.75f + rand (pos);
			//rockObjList.Add (obj);
		//}
		//rockIndex++;
	}

	public void Reset()
	{
		//rockIndex = 0;

		////Disable unused grass
		//for (int i = 0; i < rockObjList.Count; ++i)
		//	rockObjList[i].SetActive (false);
	}

	float rand (Vector3 co)
	{
		float dot = Vector3.Dot (co, new Vector3(12.9898f, 78.233f, 53.539f));

		return Mathf.Abs (Mathf.Sin (dot * 43758.5453f));
	}
}
