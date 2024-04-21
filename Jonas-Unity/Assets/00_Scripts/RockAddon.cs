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
		if (!mesh)
			return;

		int rockIndex = 0;

		for (int i = 0; i < mesh.vertexCount; ++i)
		{
			Vector3 v = mesh.vertices[i];
			float angle = Mathf.Abs (Vector3.Angle(mesh.normals[i], Vector3.up));

			if (v.y >= levelMinMax.x && v.y <= levelMinMax.y && angle <= angleMax && chance >= Random.value)
			{

				if (rockIndex < rockObjList.Count)
				{
					rockObjList[rockIndex].SetActive (true);
					rockObjList[rockIndex].transform.position = v;
					rockObjList[rockIndex].transform.up = Random.insideUnitSphere;
				}
				else
				{
					var obj = Instantiate (rockPrefab, v, Quaternion.identity, transform);
					obj.transform.up = Random.insideUnitSphere;
					rockObjList.Add (obj);
				}
				rockIndex++;
			}
		}

		//Disable unused grass
		for (int i = rockObjList.Count - 1; i > rockIndex; --i)
			rockObjList[i].SetActive (false);

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
