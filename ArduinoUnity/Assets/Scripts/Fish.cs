using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Fish")]
public class Fish : ScriptableObject
{
	public float maxDiff = 2f;
	public float padding = 1f;
	public float difficulty = 2;
	//public AnimationCurve animationCurve;
	//[Range (1, 10)] public int waveCount = 1;
	//public float bla = 1.5f;

	//public float GetVelocity (float time)
	//{
	//	float value = 0f;
	//	float frequency = 1f;

	//	for (int i = 0; i < waveCount; ++i)
	//	{
	//		value += Mathf.Sin (time * frequency);
	//		frequency *= bla;
	//	}

	//	return value;
	//}
}
