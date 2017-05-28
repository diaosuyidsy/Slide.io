using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CrushDetection :  NetworkBehaviour
{
	
	void FixedUpdate ()
	{
		
	}

	void Start ()
	{
		
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		Debug.Log ("Collided");
		PlayerStats stat = null;
		if (coll.gameObject.name == "Body") {
			Debug.Log ("It's Body");
			stat = coll.gameObject.transform.parent.GetComponent<PlayerStats> ();
		} else {
			stat = coll.gameObject.GetComponent<PlayerStats> ();
		}
		if (stat != null) {
			Debug.Log ("Stat is not null");
			if (isServer)
				stat.takeDamage (1);
		}
	}
}
