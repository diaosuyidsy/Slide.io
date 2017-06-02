using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderboardManager : NetworkBehaviour
{
	public GameObject playerInfoPrefab;

	public void SpawnPlayerInfo (string name)
	{
		Text[] childernInfo = playerInfoPrefab.GetComponentsInChildren<Text> ();
		childernInfo [0].text = "   " + name;
		childernInfo [1].text = "0   ";
		GameObject newPlayerinfo = (GameObject)Instantiate (playerInfoPrefab, transform.position, Quaternion.identity);
		newPlayerinfo.transform.SetParent (transform);
		NetworkServer.Spawn (newPlayerinfo);
	}
		

}
