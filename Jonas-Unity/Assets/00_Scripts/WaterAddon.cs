using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAddon : MonoBehaviour
{
	[SerializeField] Color waterColor;
	[SerializeField] float waterLevelY = 1f;
	[SerializeField] float waveAmplitude = 1f;
	[SerializeField] float waveSpeed = 2f;
	[SerializeField] float wavesDirection = 30f;
	//[SerializeField] float waveLength = 25f;
	//float timer = 0f;

	MeshRenderer meshRenderer;
	MaterialPropertyBlock propertyBlock;
	// Start is called before the first frame update
	void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
		LoadDefaultValues();
		propertyBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
		SetMaterialProperties();
    }

	void LoadDefaultValues()
	{		
		waterColor = meshRenderer.material.GetColor ("_WaterColor");
		waterLevelY = meshRenderer.material.GetFloat ("_WaterLevel");
		waveAmplitude = meshRenderer.material.GetFloat ("_WaveHeight");
		waveSpeed = meshRenderer.material.GetFloat ("_WaveSpeed");
		wavesDirection = meshRenderer.material.GetFloat ("_WavesDirection");
	}

	void SetMaterialProperties()
	{
		propertyBlock.SetColor ("_WaterColor", waterColor);
        propertyBlock.SetFloat ("_WaterLevel", waterLevelY);
		propertyBlock.SetFloat ("_WaveHeight", waveAmplitude);
		propertyBlock.SetFloat ("_WaveSpeed", waveSpeed);
		propertyBlock.SetFloat ("_WavesDirection", wavesDirection);
		meshRenderer.SetPropertyBlock (propertyBlock);
	}
}
