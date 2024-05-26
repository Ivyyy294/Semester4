using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	static List <Room> roomList;

	Vector3[] m_wallDirections;
	GameObject[] m_wallObjs;

	// Start is called before the first frame update
	private void OnEnable()
	{
		if (roomList == null)
			roomList = new List<Room>();

		roomList.Add (this);
	}

	private void OnDisable()
	{
		if (roomList.Contains (this))
			roomList.Remove (this);
	}

	void Start()
    {
		Init();
	}

    // Update is called once per frame
    void Update()
    {
		UpdateWallVisibility();
	}

	void Init()
	{
		if (m_wallDirections == null)
			m_wallDirections = new Vector3[] { Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

		if (m_wallObjs == null)
		{
			m_wallObjs = new GameObject[m_wallDirections.Length];

			for (int i = 0; i < m_wallDirections.Length; ++i)
				m_wallObjs[i] = AddWall (m_wallDirections[i]);
		}
	}

	GameObject AddWall (Vector3 direction)
	{
		GameObject wallObj = new GameObject("mesh");
		wallObj.transform.parent = transform;
		wallObj.transform.localPosition = Vector3.zero;

		//Construct Mesh
		Mesh mesh = new Mesh();
		RoomwallGenerator roomFaceGenerator = new RoomwallGenerator(mesh, 2, direction);
		roomFaceGenerator.ConstructMesh();

		//Add components
		wallObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
		wallObj.AddComponent<MeshFilter>().sharedMesh = mesh;
		wallObj.AddComponent<MeshCollider>();

		return wallObj;
	}

	void UpdateWallVisibility()
	{
		//Skip floor
		for (int i = 1; i < m_wallDirections.Length; ++i)
			m_wallObjs[i].SetActive (!HasNeigbour(m_wallDirections[i]));
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
