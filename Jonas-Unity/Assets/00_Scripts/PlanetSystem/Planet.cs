using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	[Range (2, 256)]
	public int m_resolution = 2;

	public PlanetShapeSetting m_shapeSetting;
	public PlanetColorSetting m_colorSetting;
	public PlanetGrassSetting m_grassSetting;
	public PlanetWaterSetting m_waterSetting;
	public PlanetRockSetting m_rockSetting;

	[HideInInspector]
	public bool shapeSettingsFoldout;
	[HideInInspector]
	public bool colorSettingsFoldout;
	[HideInInspector]
	public bool grassSettingsFoldout;
	[HideInInspector]
	public bool waterSettingsFoldout;
	[HideInInspector]
	public bool rockSettingsFoldout;

	PlanetShapeGenerator m_shapeGenerator;

	[SerializeField, HideInInspector]
	MeshFilter[] m_meshFilters;
	PlanetTerrainFace[] m_planetFaces;

	//Public Methods

	public void OnShapeSettingsUpdated()
	{
		Init();
		GenerateMesh();
	}
	
	public void OnShaderSettingsUpdated()
	{
		Init();
		SetShaderProperties();
	}

	public void OnTerrainObjectSettingsUpdated()
	{
		GenerateTerrainObjects();
	}

	public void GeneratePlanet()
	{
		Init();
		GenerateMesh();
		SetShaderProperties();
		GenerateTerrainObjects();
	}

	//Private Methods
	private void Start()
	{
		GeneratePlanet();
	}

	private void Init()
	{
		m_shapeGenerator = new PlanetShapeGenerator (m_shapeSetting);

		if (m_meshFilters == null || m_meshFilters.Length == 0)
			m_meshFilters = new MeshFilter[6];
		
		m_planetFaces = new PlanetTerrainFace[6];

		Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

		for (int i = 0; i < 6; ++i)
		{
			if (m_meshFilters[i] == null)
			{
				GameObject meshObj = new GameObject ("mesh");
				meshObj.transform.parent = transform;

				meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find ("PlanetShader"));
				m_meshFilters[i] = meshObj.AddComponent<MeshFilter>();
				m_meshFilters[i].sharedMesh = new Mesh();
			}
				
			m_planetFaces[i] = new PlanetTerrainFace (m_shapeGenerator, m_meshFilters[i].sharedMesh, m_resolution, directions[i]);
		}
	}

	void GenerateMesh()
	{
		foreach (PlanetTerrainFace face in m_planetFaces)
			face.ConstructMesh();
	}

	void SetShaderProperties()
	{
		MaterialPropertyBlock mp = new MaterialPropertyBlock();
		m_colorSetting.SetMaterialProperties (mp);
		m_grassSetting.SetMaterialProperties (mp);
		m_waterSetting.SetMaterialProperties (mp);

		foreach (MeshFilter m in m_meshFilters)
			m.GetComponent<MeshRenderer>().SetPropertyBlock(mp);
	}

	void GenerateTerrainObjects()
	{
		StopAllCoroutines();
		StartCoroutine (GenerateTerrainObjectsIntern());
	}

	IEnumerator GenerateTerrainObjectsIntern()
	{
		for (int i = 0; i < m_planetFaces.Length; ++i)
		{
			m_planetFaces[i].GenerateTerrainObjects (m_rockSetting, m_meshFilters[i].transform);
			yield return new WaitForEndOfFrame();
		}
	}
}
