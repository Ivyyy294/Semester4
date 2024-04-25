using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour
{
	[SerializeField] GameObject prefab;
 
	public int Width;
    public int Depth;
    public int ChunkDimention;

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
                yield return new WaitForEndOfFrame();
            }
        }

        yield return true;
	}
}
