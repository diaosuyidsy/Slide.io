using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{

	public int maxHealth = 1;
	public GameObject explosionPrefab;
	[SyncVar]
	public string playerName;

	Text informationText;

	int health;
	//	private PlayerController pc;
	private bool speedyLock;
	private bool ignoreyLock;
	public float mapObjectiveTimer;

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
//		pc = GetComponent<PlayerController> ();
		speedyLock = true;
		ignoreyLock = true;
		mapObjectiveTimer = 0f;
	}

	//Takes care of player when they die
	[ClientRpc]
	void RpcdamagePlayer (int dmg)
	{
		informationText = GameObject.Find ("FinalText").GetComponent<Text> ();
		GameObject.Find (playerName + "ScorePanel").GetComponent<ScorePanel> ().playerScore = 0;
		GetComponent<SpriteRenderer> ().enabled = false;
		GetComponentInChildren<Text> ().enabled = false;
		foreach (BoxCollider2D childcoll in GetComponentsInChildren <BoxCollider2D> ()) {
			childcoll.enabled = false;
		}
		GetComponent<CrushDetection> ().locked = true;
		if (isLocalPlayer) {
			informationText.text = "You Died";
			GetComponent<PlayerController> ().locked = true;
			Camera.main.orthographicSize = 20f;
			StartCoroutine (respawnText (5));
		} 
		StartCoroutine (respawn (5));

	}

	IEnumerator respawnText (int respawnTime)
	{
		yield return new WaitForSeconds (1f);
		StartCoroutine (respawnCountdown (respawnTime));
	}

	IEnumerator respawnCountdown (int respawnTime)
	{
		for (int i = respawnTime - 1; i > 0; i--) {
			informationText.text = i.ToString ();
			yield return new WaitForSeconds (1f);
		}
		GetComponent<PlayerController> ().locked = false;
	}

	IEnumerator respawn (int time)
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
		transform.rotation = Quaternion.identity;
		transform.position = spawnPoint;
		health = maxHealth;

		GetComponent<SpriteRenderer> ().enabled = true;
		GetComponentInChildren<Text> ().enabled = true;
		foreach (BoxCollider2D childcoll in GetComponentsInChildren <BoxCollider2D> ()) {
			childcoll.enabled = true;
		}
		for (int i = 0; i < GetComponent<CrushDetection> ().locs.Length; i++) {
			GetComponent<CrushDetection> ().locs [i] = GetComponent<CrushDetection> ().trailSpawnPoint.position;
		}
		GetComponent<CrushDetection> ().locked = false;
	}

	public void takeDamage (int dmg)
	{
		if (!isServer) {
			return;
		}
		if (health <= 0)
			return;
		health -= dmg;
		if (health <= 0) {
			health = 0;
			GameObject explosion = (GameObject)Instantiate (explosionPrefab, transform.position, Quaternion.identity);
			NetworkServer.Spawn (explosion);

			RpcdamagePlayer (dmg);
		}
		return;
	}

	[ClientRpc]
	void RpcConsume (bool consumePlayer)
	{
		if (consumePlayer) {
			GetComponent<TrailRenderer> ().time += 0.09f;
			GetComponent<CrushDetection> ().trailSpawnTimeInterval += 0.003f;
			GameObject.Find (playerName + "ScorePanel").GetComponent<ScorePanel> ().playerScore += 10;

		} else {
			GetComponent<TrailRenderer> ().time += 0.009f;
			GetComponent<CrushDetection> ().trailSpawnTimeInterval += 0.0003f;
			GameObject.Find (playerName + "ScorePanel").GetComponent<ScorePanel> ().playerScore += 1;
		}

	}

	public void takeConsume (bool consumePlayer)
	{
		// if it's local player, adjust hes camera
		if (isLocalPlayer) {
			if (Camera.main.orthographicSize <= 40) {
				if (consumePlayer) {
					Camera.main.orthographicSize += 0.5f;
				} else {
					Camera.main.orthographicSize += 0.05f;
				}
			}
		}
		if (!isServer) {
			return;
		}
		// if it's server, add ult charge to sync var
		if (consumePlayer)
			GetComponent<PlayerAbilityControl> ().addUltCharge (0.95f);
		else
			GetComponent<PlayerAbilityControl> ().addUltCharge (0.03f);
		// if it's server, do rpc on all clients
		RpcConsume (consumePlayer);
		return;
	}

	public bool isUnstoppable ()
	{
		return !ignoreyLock;
	}

	void FixedUpdate ()
	{
		if (!speedyLock) {
			if (GetComponent<PlayerController> ().getAcc () >= GetComponent<PlayerController> ().maxAcceleration * 1.5f)
				return;
			GetComponent<PlayerController> ().setAcc (GetComponent<PlayerController> ().maxAcceleration * 1.5f);
			mapObjectiveTimer += Time.deltaTime;
			if (mapObjectiveTimer >= 10f) {
				speedyLock = true;
				mapObjectiveTimer = 0f;
				GetComponent<PlayerController> ().setAcc (GetComponent<PlayerController> ().maxAcceleration);
			}
		}

		if (!ignoreyLock) {
			health = 99;
			mapObjectiveTimer += Time.deltaTime;
			if (mapObjectiveTimer >= 10f) {
				ignoreyLock = true;
				mapObjectiveTimer = 0f;
				health = maxHealth;
			}
		}
	}

	public void takeMapObjective (MapObjectiveType mot)
	{
		if (isServer)
			GetComponent<PlayerAbilityControl> ().addUltCharge (0.5f);
	
		switch (mot) {
		case MapObjectiveType.Speedy:
			//Check if there is a stronger effect on player already
			if (GetComponent<PlayerController> ().getAcc () >= GetComponent<PlayerController> ().maxAcceleration * 1.5f)
				return;
			speedyLock = false;
			break;
		case MapObjectiveType.Ignorey:
			ignoreyLock = false;
			break;
		}
	}
}
