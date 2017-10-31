using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapObjective : NetworkBehaviour
{
	public MapObjectiveType thisMapObjective;

	// Use this for initialization
	void Start ()
	{
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.tag == "Player" || (other.gameObject.transform.parent != null
		    && other.gameObject.transform.parent.gameObject.tag == "Player")) {
			// Respawn Consumable
			OnObjectiveTook ();
			// Give the player what he gets for consuming
			preTook (other.gameObject);
		}
	}

	void preTook (GameObject target)
	{
		PlayerStats stat = null;
		if (target.name == "Body") {
			stat = target.transform.parent.GetComponent<PlayerStats> ();
		} else {
			stat = target.GetComponent<PlayerStats> ();
		}
		if (stat != null && Mathf.Approximately (stat.mapObjectiveTimer, 0f)) {
			stat.takeMapObjective (thisMapObjective);
		}
	}

	void OnObjectiveTook ()
	{
		if (isServer) {
			RpcTook ();	
		} else {
			StartCoroutine (startRespawn (3f));
		}
	}

	[ClientRpc]
	void RpcTook ()
	{
		StartCoroutine (startRespawn (3f));
	}

	IEnumerator startRespawn (float time)
	{
		tivate (false);
		yield return new WaitForSeconds (time);
		tivate (true);
	}

	void tivate (bool activate)
	{
		GetComponent<CircleCollider2D> ().enabled = activate;
		GetComponent<SpriteRenderer> ().enabled = activate;
	}
}

public enum MapObjectiveType
{
	Speedy,
	Ignorey,
}
