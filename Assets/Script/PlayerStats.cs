using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{

	public int maxHealth = 1;

	[SyncVar]
	public string playerName;

	Text informationText;

	int health;

	//	private float trailSpawnInterval = 0.05f;
	//	private float trailTime = 1.5f;

	public override void OnStartServer ()
	{
		base.OnStartServer ();
		GetComponentInChildren<Text> ().text = playerName;
	}

	public override void OnStartClient ()
	{
		base.OnStartClient ();
		GetComponentInChildren<Text> ().text = playerName;
	}
		
	// Use this for initialization
	void Start ()
	{
		health = maxHealth;
	}

	[ClientRpc]
	void RpcdamagePlayer (int dmg)
	{
		Debug.Log ("Took Damage");
		if (health <= 0)
			return;
		health -= dmg;

		if (health <= 0) {
			health = 0;

			informationText = GameObject.Find ("FinalText").GetComponent<Text> ();

			if (isLocalPlayer) {
				informationText.text = "Game Over";
				GetComponent<PlayerController> ().locked = true;
				StartCoroutine (respawn (3f));
			} 

			return;
		}
	}

	IEnumerator respawn (float time)
	{
		yield return new WaitForSeconds (time);
		informationText.text = "";
		NetworkStartPosition[] spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
		// Set the spawn point to origin as a default value
		Vector3 spawnPoint = Vector3.zero;

		// If there is a spawn point array and the array is not empty, pick a spawn point at random
		if (spawnPoints != null && spawnPoints.Length > 0) {
			spawnPoint = spawnPoints [Random.Range (0, spawnPoints.Length)].transform.position;
		}

		// Set the player’s position to the chosen spawn point
		transform.position = spawnPoint;

		GetComponent<PlayerController> ().locked = false;

	}

	public void takeDamage (int dmg)
	{
		Debug.Log ("Enter the takeDamage Function");
		if (!isServer) {
			Debug.Log ("It's not server, return");
			return;
		}
			
		RpcdamagePlayer (dmg);
		return;
	}

	[ClientRpc]
	void RpcConsume (bool consumePlayer)
	{
		if (consumePlayer) {
			GetComponent<TrailRenderer> ().time += 0.03f;
			GetComponent<CrushDetection> ().trailSpawnTimeInterval += 0.001f;
		} else {
			GetComponent<TrailRenderer> ().time += 0.003f;
			GetComponent<CrushDetection> ().trailSpawnTimeInterval += 0.0001f;
		}

	}

	public void takeConsume (bool consumePlayer)
	{
		if (isLocalPlayer) {
			if (Camera.main.orthographicSize <= 40) {
				if (consumePlayer) {
					Camera.main.orthographicSize += 1.5f;
				} else {
					Camera.main.orthographicSize += 0.5f;
				}
			}

		}
		if (!isServer) {
			return;
		}
		RpcConsume (consumePlayer);
		return;
	}
}
