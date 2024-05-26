using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
	int gridSize = 6;
	int cellSize = 2;

	string mapData = "";
    // Start is called before the first frame update
    void Start()
    {
        mapData = "111011"
				+ "101011"
				+ "101010"
				+ "101010"
				+ "101111"
				+ "001111";

		SpawnRooms();
	}

	void SpawnRooms()
	{
		for (int i = 0; i < mapData.Length; ++i)
		{
			int x = i % gridSize;
			int z = Mathf.CeilToInt (i / gridSize);

			if (mapData[i] == '1')
				SpawnRoom (new Vector3 (x, 0, -z), "Room" + z + x);
		}
	}

	void SpawnRoom (Vector3 gridPos, string name)
	{
		GameObject wallObj = new GameObject(name);
		wallObj.transform.parent = transform;
		wallObj.transform.localPosition = gridPos * cellSize;
		wallObj.AddComponent<Room>();
	}
}
