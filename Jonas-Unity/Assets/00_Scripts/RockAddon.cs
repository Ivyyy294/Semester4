using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RockSettings
{
	public Vector2 levelMinMax;
	public float angleMax = 45f;
	public float chance;
}

public class RockAddon : MonoBehaviour, ITerrainGeneratorAddon
{
	public GameObject rockPrefab;
	public Vector2 levelMinMax;

	[Range (0f, 90f)]
	public float angleMax;

	[Range (0f, 1f)]
	public float chance = 1f;

	List <GameObject> rockObjList = new List<GameObject>();
	RockSettings previousSettings = new RockSettings();
	Mesh previousMesh;

    public void Apply (Mesh mesh)
	{
		if (!mesh || !enabled)
			return;

		int rockIndex = 0;

		//Disable unused grass
		for (int i = 0; i < rockObjList.Count; ++i)
			rockObjList[i].SetActive (false);

		for (int i = 0; i < mesh.vertexCount; ++i)
		{
			Vector3 v = mesh.vertices[i];
			float angle = Mathf.Abs (Vector3.Angle(mesh.normals[i], Vector3.up));

			if (v.y >= levelMinMax.x && v.y <= levelMinMax.y && angle <= angleMax && chance > rand (v))
			{
				if (rockIndex < rockObjList.Count)
				{
					rockObjList[rockIndex].SetActive (true);
					rockObjList[rockIndex].transform.localPosition = v;
				}
				else
				{
					var obj = Instantiate (rockPrefab, transform);
					obj.transform.localPosition = v;
					obj.transform.up = Random.insideUnitSphere;
					rockObjList.Add (obj);
				}
				rockIndex++;
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
