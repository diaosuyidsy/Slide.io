using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConsumableSpawner : NetworkBehaviour
{
	public Consume ConsumablePrefab;
	public GameObject ConsumableHolder;
	public int maxConsumableNum;
	public GameObject RightEdge;
	public GameObject UpEdge;

	public void Spawn ()
	{
		Vector3 randompos = new Vector3 ((Random.value * 2 - 1) * RightEdge.transform.position.x, (Random.value * 2 - 1) * UpEdge.transform.position.y);
		// Spawn the consumable
		Consume newConsumable;
		newConsumable = (Consume)Instantiate (ConsumablePrefab, randompos, Quaternion.identity);
		newConsumable.onDied += OnConsDied;
		newConsumable.gameObject.transform.parent = ConsumableHolder.transform;
		NetworkServer.Spawn (newConsumable.gameObject);
	}
	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < maxConsumableNum; i++) {
			Spawn ();
		}
	}

	public void OnConsDied ()
	{
		Spawn ();
	}

}
