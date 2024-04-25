using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AddonContainer
{
	public WaterAddon water;
	public GrassAddon grass;
	public RockAddon rock;
}

public class InfiniteTerrain : MonoBehaviour
{
	[SerializeField] GameObject prefab;

	public int Width;
    public int Depth;
    public int ChunkDimention;

	[Header ("Addon settings")]
	[SerializeField] WaterProperties waterProperties;
	[SerializeField] GrassProperties grassProperties;
	[SerializeField] RockSettings rockProperties;

	List <AddonContainer> addons = new List<AddonContainer>();

	bool initDone = false;

	// Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateTerrain());

        Camera.main.transform.position = new Vector3(Width / 2f * ChunkDimention, 50, Depth / 10f * ChunkDimention);
        Camera.main.transform.LookAt(new Vector3(Width / 2f * ChunkDimention, 0, Depth / 2f * ChunkDimention));
    }

	IEnumerator GenerateTerrain()
	{
		for(int z = 0; z < Depth; z++)
        {
            for (int x = 0; x <Width; x++)
            {
                GameObject go = Instantiate (prefab, transform);
				go.name = "Chunk_" + x + "_" + z;
                go.transform.localPosition = new Vector3(x * ChunkDimention, 0, z * ChunkDimention);
                go.GetComponent<TerrainMeshGenerator>().Init(ChunkDimention, x, z);

				AddonContainer addon = new AddonContainer();
				addon.water = go.GetComponent <WaterAddon>();
				addon.grass = go.GetComponent<GrassAddon>();
				addon.rock = go.GetComponent<RockAddon>();

				addons.Add (addon);
                yield return new WaitForEndOfFrame();
            }
        }

        yield return true;
	}

	private void Update()
	{
		if (!initDone && addons.Count > 0)
		{
			waterProperties = addons[0].water.properties;
			grassProperties = addons[0].grass.properties;
			rockProperties = addons[0].rock.properties;
			initDone = true;
		}

		foreach (var i in addons)
		{
			i.water.properties = waterProperties;
			i.grass.properties = grassProperties;
			i.rock.properties = rockProperties;
		}
	}
}
