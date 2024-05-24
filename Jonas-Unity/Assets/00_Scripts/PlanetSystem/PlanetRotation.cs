using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
	[SerializeField] float m_rotationSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey (KeyCode.D))
			transform.Rotate (Vector3.up, m_rotationSpeed, Space.World);
		if (Input.GetKey(KeyCode.A))
			transform.Rotate(Vector3.up, -m_rotationSpeed, Space.World);
		if (Input.GetKey(KeyCode.W))
			transform.Rotate(Vector3.right, m_rotationSpeed, Space.World);
		if (Input.GetKey(KeyCode.S))
			transform.Rotate(Vector3.right, -m_rotationSpeed, Space.World);
	}
}
