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

			informationText = GameObject.FindObjectOfType<Text> ();

			if (isLocalPlayer) {
				informationText.text = "Game Over";
			} else {
				informationText.text = "You Won";
			}

			return;
		}
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
	void RpcConsume ()
	{
		GetComponent<TrailRenderer> ().time += 0.5f;
		GetComponent<CrushDetection> ().trailSpawnTimeInterval += 0.015f;
	}

	public void takeConsume ()
	{
		if (!isServer)
			return;
		RpcConsume ();
		return;
	}
}
