﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CrushDetection :  NetworkBehaviour
{
	// The variable for Trail logic
	public float trailSpawnTimeInterval;
	public Transform trailSpawnPoint;

	private RaycastHit hit;
	private Vector3[] locs;
	private int head;
	private int tail = 0;
	private int numOfTrails;



	void FixedUpdate ()
	{
		RaycastHit2D hit = Hit ();
		if (hit.collider != null) {
			if (hit.collider.gameObject != this.gameObject &&
			    hit.collider.gameObject.transform.parent != null &&
			    hit.collider.gameObject.transform.parent.gameObject != this.gameObject) {
				preDealDamage (hit.collider.gameObject, 1);
			}
		}
	}

	void Start ()
	{
		numOfTrails = 20;
		// Logic for Trail
		locs = new Vector3[numOfTrails];
		for (int i = 0; i < locs.Length; i++) {
			locs [i] = trailSpawnPoint.transform.position;
		}
		head = locs.Length - 1;

		StartCoroutine (CollectData ());
//		StartCoroutine (TestAdd ());
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		Debug.Log ("Collided");
		preDealDamage (coll.gameObject, 1);
	}

	void preDealDamage (GameObject target, int damage)
	{
		PlayerStats stat = null;
		if (target.name == "Body") {
			Debug.Log ("It's Body");
			stat = target.transform.parent.GetComponent<PlayerStats> ();
		} else {
			stat = target.GetComponent<PlayerStats> ();
		}
		if (stat != null) {
			Debug.Log ("Stat is not null");
			if (isServer)
				stat.takeDamage (damage);
		}
	}

	IEnumerator TestAdd ()
	{
		yield return new WaitForSeconds (3f);
		int oldNum = numOfTrails;
		numOfTrails += 20;
		Vector3[] newLocs = new Vector3[numOfTrails];
		for (int i = 0; i < newLocs.Length; i++) {
			if (i < oldNum) {
				newLocs [i] = locs [i];
			} else {
				newLocs [i] = locs [oldNum - 1];
			}
		}

		locs = newLocs;
		StartCoroutine (TestAdd ());
	}

	IEnumerator CollectData ()
	{
		while (true) {
			if (trailSpawnPoint.transform.position != locs [head]) {
				head = (head + 1) % locs.Length;
				tail = (tail + 1) % locs.Length;
				locs [head] = trailSpawnPoint.transform.position;
			}
			yield return new WaitForSeconds (trailSpawnTimeInterval);
		}
	}

	private RaycastHit2D Hit ()
	{
		RaycastHit2D hit = new RaycastHit2D ();
		int i = head;
		int j = head - 1;
		if (j < 0)
			j = locs.Length - 1;
		while (j != head) {
			hit = Physics2D.Linecast (locs [i], locs [j]);
			if (hit.collider != null) {
				return hit;
			}

			Debug.DrawLine (locs [i], locs [j], Color.black);

			i = i - 1; 
			if (i < 0)
				i = locs.Length - 1;

			j = j - 1;
			if (j < 0)
				j = locs.Length - 1;
		}
		return hit;
	}
}
