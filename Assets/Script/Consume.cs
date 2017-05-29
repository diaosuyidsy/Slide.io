using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Consume : NetworkBehaviour
{
	public delegate void OnDied ();

	public event OnDied onDied;
	// Use this for initialization
	void Start ()
	{
		GetComponent<SpriteRenderer> ().color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate (Vector3.forward * 100 * Time.deltaTime);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.tag == "Player") {
			if (onDied != null)
				onDied ();
			NetworkServer.Destroy (gameObject);
		}
	}
}
