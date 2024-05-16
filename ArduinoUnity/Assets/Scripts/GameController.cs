using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	[SerializeField] Fish fish;
	[SerializeField] [Min (0.01f)] float maxReelSpeed = 0.1f;
	[SerializeField] [Min (0.01f)] float progressPerSec = 20f;

	//[SerializeField] float stessCooldown = 1;

	[SerializeField] MeshRenderer reelingDebug;
	[SerializeField] MeshRenderer strugglingDebug;
	[SerializeField] MeshRenderer stressDebug;
	[SerializeField] MeshRenderer progressDebug;
	[SerializeField] Transform fishobj;
	[SerializeField] Transform playerObj;

	float timer = 0f;

	//Stress
	float progress = 0f;

	//float tickTimer;
	float lastFishY;
	float lastPlayerY;

	//bool isStruggling;
	//bool isReeling;

    // Start is called before the first frame update
    void Start()
    {
		lastPlayerY = Input.mousePosition.y;
		
		Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		float velocityFish = GetFishVelocity();
		float velocityPlayer = GetPlayerVelocity();

		bool fishStruggling = velocityFish < 0f;
		bool isReeling = velocityPlayer > 0f;

		//Stress
		bool reelToFast = velocityPlayer > maxReelSpeed;
		bool lineStress = velocityFish < 0f && velocityPlayer >= 0f;

		//Progress
		if (isReeling && progress < 100f)
			progress += progressPerSec * Time.deltaTime;

		//if (reelToFast || lineStress)
		//	stressTimer += Time.deltaTime;
		//else if (stressTimer > 0f)
		//	stressTimer -= Time.deltaTime;
		//else
		//	stressTimer = 0f;

		//Debug
		Vector3 tmp = fishobj.localPosition;
		tmp.y += velocityFish;
		fishobj.localPosition = tmp;

		tmp = playerObj.localPosition;
		tmp.y += velocityPlayer;
		playerObj.localPosition = tmp;

		stressDebug.material.color = reelToFast || lineStress ? Color.yellow : Color.white;
		reelingDebug.material.color = isReeling ? Color.green : Color.white;
		strugglingDebug.material.color = fishStruggling ? Color.red : Color.white;
		progressDebug.material.color = Color.green * (progress / 100f);
    }

	float GetFishVelocity()
	{
		float y = 0f;
		float f = 1f;

		for (int i = 0; i < fish.difficulty; ++i)
		{
			y += Mathf.Sin (timer * f);
			f *= 1.5f;
		}

		float velocity = y - lastFishY;

		lastFishY = y;
		timer += Time.deltaTime;
		return velocity;
	}

	float GetPlayerVelocity()
	{
		float y = Input.mousePosition.y;
		float velocity = y - lastPlayerY;
		lastPlayerY = y;
		return velocity * -0.01f;
	}
}
