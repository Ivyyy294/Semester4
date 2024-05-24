using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
		float velocity = Input.mouseScrollDelta.y;

		if (velocity != 0f)
		{
			Vector3 pos = transform.position;
			pos.z += velocity;
			transform.position = pos;
		}
	}
}
