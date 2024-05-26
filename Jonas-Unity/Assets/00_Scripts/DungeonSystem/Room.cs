using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	static List <Room> roomList;

	// Start is called before the first frame update
	private void Awake()
	{
		if (roomList == null)
			roomList = new List<Room>();

		roomList.Add (this);
	}

	void Start()
    {
		GenerateMesh();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	void GenerateMesh()
	{
		Stack<Vector3> directions = GetWallDirections();

		while (directions.Count > 0)
			AddFaceMesh (directions.Pop());
	}

	void AddFaceMesh (Vector3 direction)
	{
		GameObject meshObj = new GameObject("mesh");
		meshObj.transform.parent = transform;
		meshObj.transform.localPosition = Vector3.zero;

		//Construct Mesh
		Mesh mesh = new Mesh();
		RoomFaceGenerator roomFaceGenerator = new RoomFaceGenerator(mesh, 2, direction);
		roomFaceGenerator.ConstructMesh();

		//Add components
		meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
		meshObj.AddComponent<MeshFilter>().sharedMesh = mesh;
		meshObj.AddComponent<MeshCollider>();
	}

	Stack <Vector3> GetWallDirections()
	{
		Stack<Vector3> directions = new Stack<Vector3>();
		directions.Push (Vector3.down);

		Vector3[] wallDirections = {Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

		for (int i = 0; i < wallDirections.Length; ++i)
		{
			if (!HasNeigbour (wallDirections[i]))
				directions.Push (wallDirections[i]);
		}

		return directions;
	}

	Vector2 GetGridPos()
	{
		Vector3 worldPos = transform.position;
		return new Vector2 (worldPos.x * 0.5f, worldPos.z * 0.5f);
	}

	bool HasNeigbour (Vector3 direction)
	{
		Vector2 gridPos = GetGridPos();

		foreach (Room r in roomList)
		{
			Vector2 wallPos = gridPos + new Vector2(direction.x, direction.z);
			
			if (r.GetGridPos() == wallPos)
				return true;
		}

		return false;
	}
}
