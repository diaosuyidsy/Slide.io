using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;


public class SlideioNetworkManager : NetworkManager
{
	public Text inputName;
	public Button JoinGame;

	public void TryStartGame ()
	{
		StartHost ();
	}

	void Start ()
	{
		if (IsHeadless ()) {
			StartServer ();
		}
	}

	public void TryJoingame ()
	{
		StartClient ();
	}

	// detect headless mode (which has graphicsDeviceType Null)
	bool IsHeadless ()
	{
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
	}

	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
	{
		Debug.Log ("On Server Add Player");
		string customParameters = "";
		if (extraMessageReader != null) {
			customParameters = extraMessageReader.ReadString ();
		}
		playerPrefab.GetComponentInChildren<PlayerStats> ().playerName = customParameters;
		var player = (GameObject)GameObject.Instantiate (playerPrefab, GetStartPosition ().position, Quaternion.identity);
		NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
	}

	public override void OnStartClient (NetworkClient client)
	{
		Debug.Log ("On Start Client");
	}

	public override void OnClientConnect (NetworkConnection conn)
	{
		Debug.Log ("On Client Connect");
		Debug.Log ("The name is : " + inputName.text);
		StringMessage msg = new StringMessage (inputName.text);
		ClientScene.AddPlayer (conn, 0, msg);

	}

	// Override to disable default action
	public override void OnClientSceneChanged (NetworkConnection conn)
	{
//		if (GameObject.Find ("LeaderboardManager") == null) {
//			Debug.Log ("Leaderboard Manager is null");
//		} else {
//			GameObject.Find ("LeaderboardManager").GetComponent<LeaderboardManager> ().SpawnPlayerInfo (playerPrefab.GetComponentInChildren<PlayerStats> ().playerName);
//		}
	}

}
