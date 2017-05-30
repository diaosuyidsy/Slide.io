using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Consume : NetworkBehaviour
{
	public delegate void OnDied ();

	public event OnDied onDied;

	[SyncVar]
	public Color ConsumeColor;
	// Use this for initialization
	void Start ()
	{
		GetComponent<SpriteRenderer> ().color = ConsumeColor;
		if (isServer) {
			ConsumeColor = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);
			GetComponent<SpriteRenderer> ().color = ConsumeColor;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate (Vector3.forward * 100 * Time.deltaTime);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.tag == "Player" || (other.gameObject.transform.parent != null
		    && other.gameObject.transform.parent.gameObject.tag == "Player")) {
			// Respawn Consumable
			if (onDied != null) {
				onDied ();
				NetworkServer.Destroy (gameObject);
			}

			// Give the player what he gets for consuming
			preConsume (other.gameObject);
		}
	}

	void preConsume (GameObject target)
	{
		PlayerStats stat = null;
		if (target.name == "Body") {
			stat = target.transform.parent.GetComponent<PlayerStats> ();
		} else {
			stat = target.GetComponent<PlayerStats> ();
		}
		if (stat != null) {
			stat.takeConsume ();
		}
	}
}
