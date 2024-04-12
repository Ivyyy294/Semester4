using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class WaterMeshGenerator : MeshGenerator
{
	[Header ("Shader Settings")]
	[SerializeField] ComputeShader WaveComputeShader;

	//Wave animation
	[System.Serializable]
	enum WaveAnimationFunc
	{
		SinCos = 0,
		Sin = 1,
		SumSin = 2,
		SumSinCos = 3,
		Experimental = 4,
		GerstnerWave = 5,
		GerstnerWaveExperimental
	}

	[Header ("Quad Wave Settings")]
	[SerializeField] WaveAnimationFunc waveTyp;
	[SerializeField] float quadWaveHeight = 1f;
	[SerializeField] float quadWaveSpeed = 1f;
	[SerializeField] float quadWaveLength = 1f;
	[SerializeField] float quadWavesDirection = 0f;
	float waveAnimationTimer = 0f;
	ComputeBuffer verticeBaseBuffer;

    // Start is called before the first frame update
    void Start()
    {
		//Generate Quad Grid
		CreateQuad();
		InitQuadWaveAnimation();

		InitMesh();

		//Center camera in z and place it with 50% spacing behind the Mesh
		Camera.main.transform.position = new Vector3 (quadColumns * quadWidth, 20, quadRows * quadWidth / 2);

		//Focus camera at center vertice of Mesh
		Camera.main.transform.LookAt (new Vector3 (quadColumns * quadWidth / 2f, 0f, quadRows * quadWidth / 2));
	}

	void Update()
    {
		UpdateQuadWaveAnimation();

		//Update Mesh
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (verticeBaseBuffer != null)
			verticeBaseBuffer.Dispose();
	}

	void InitQuadWaveAnimation()
	{
		WaveComputeShader.SetBuffer (0, "Vertices", verticeBuffer);

		verticeBaseBuffer = new ComputeBuffer (vertices.Length, (sizeof (float) * 3));
		verticeBaseBuffer.SetData (vertices);
		WaveComputeShader.SetBuffer (0, "VerticesBase", verticeBaseBuffer);

		WaveComputeShader.SetFloat ("Columns", quadColumns);
		WaveComputeShader.SetFloat ("Rows", quadRows);
	}

	void UpdateQuadWaveAnimation()
	{
		WaveComputeShader.SetInt("WaveTyp", (int)waveTyp);
		WaveComputeShader.SetFloat("WaveHeight", quadWaveHeight);
		WaveComputeShader.SetFloat("WaveAnimationTimer", waveAnimationTimer);
		WaveComputeShader.SetFloat("WaveSpeed", quadWaveSpeed);
		WaveComputeShader.SetFloat("WavesDirection", quadWavesDirection);

		float waveFrequency = 2f * Mathf.PI / quadWaveLength;
		WaveComputeShader.SetFloat("WavesFrequency", waveFrequency);

		int threadCountX = Mathf.CeilToInt((quadColumns + 1f) / 8f);
		int threadCountZ = Mathf.CeilToInt((quadRows + 1f) / 8f);

		WaveComputeShader.Dispatch(0, threadCountX, 1, threadCountZ);
		verticeBuffer.GetData (vertices);

		waveAnimationTimer += Time.deltaTime;
	}
}
