using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderboardManager : NetworkBehaviour
{
	public GameObject playerInfoPrefab;

	public override void OnStartClient ()
	{
		base.OnStartClient ();
		transform.localPosition = new Vector3 (0, 100);
	}

	public void SpawnPlayerInfo (string name)
	{
		if (!isServer)
			return;
		playerInfoPrefab.GetComponent<ScorePanel> ().playerName = name;
		playerInfoPrefab.GetComponent<ScorePanel> ().playerScore = 0;
		GameObject newPlayerinfo = (GameObject)Instantiate (playerInfoPrefab, transform.position, Quaternion.identity);
		NetworkServer.Spawn (newPlayerinfo);
	}
		

}
