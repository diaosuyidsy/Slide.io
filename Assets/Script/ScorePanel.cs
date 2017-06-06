using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScorePanel : NetworkBehaviour
{
	[SyncVar]
	public string playerName;

	[SyncVar (hook = "OnChangeScore")]
	public int playerScore;

	public override void OnStartClient ()
	{
		base.OnStartClient ();
		name = playerName + "ScorePanel";
		transform.SetParent (GameObject.Find ("LeaderboardManager").transform);
		Text[] childernInfo = GetComponentsInChildren<Text> ();
		childernInfo [0].text = "   " + playerName;
		childernInfo [1].text = playerScore.ToString ();
	}

	void OnChangeScore (int newScore)
	{
		Text[] childernInfo = GetComponentsInChildren<Text> ();
		childernInfo [1].text = newScore.ToString ();
		if (isServer) {
			if (newScore > playerScore) {
				RpcBubbleUp (newScore);
			}
		}
	}

	[ClientRpc]
	void RpcBubbleUp (int newScore)
	{
		int curPos = transform.GetSiblingIndex ();
		while (curPos != 1 && transform.parent.GetChild (curPos - 1).gameObject.GetComponent<ScorePanel> ().playerScore < newScore) {
			transform.SetSiblingIndex (curPos - 1);
			curPos = transform.GetSiblingIndex ();
		}

	}

	//	void OnApplicationQuit ()
	//	{
	//		Debug.Log ("On Application Quit");
	//		if (!isServer)
	//			return;
	////		NetworkServer.Destroy (gameObject);
	//	}
}
