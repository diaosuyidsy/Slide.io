using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;


public class SlideioNetworkManager : NetworkManager
{
	public Text inputName;
	public Button JoinGame;

	void OnEnable ()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

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
		string customParameters = "";
		if (extraMessageReader != null) {
			customParameters = extraMessageReader.ReadString ();
		}
		playerPrefab.GetComponentInChildren<PlayerStats> ().playerName = customParameters;
		var player = (GameObject)GameObject.Instantiate (playerPrefab, GetStartPosition ().position, Quaternion.identity);
		NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
		if (GameObject.Find ("LeaderboardManager") != null) {
			Debug.Log ("On Server Add Player called: " + customParameters);
			GameObject.Find ("LeaderboardManager").GetComponent<LeaderboardManager> ().SpawnPlayerInfo (customParameters);
		}

	}

	public override void OnStartClient (NetworkClient client)
	{
	}

	public override void OnClientConnect (NetworkConnection conn)
	{
		StringMessage msg = new StringMessage (inputName.text);
		ClientScene.AddPlayer (conn, 0, msg);
	}

	public override void OnServerSceneChanged (string sceneName)
	{
		if (GameObject.Find ("InputName") != null) {
			inputName = GameObject.Find ("InputName").GetComponent<Text> ();
		}
		if (sceneName == "StartScene")
			GameObject.Find ("LeaderboardManager").GetComponent<LeaderboardManager> ().SpawnPlayerInfo (inputName.text);
	}

	// Override to disable default action
	public override void OnClientSceneChanged (NetworkConnection conn)
	{
		if (GameObject.Find ("InputName") != null) {
			inputName = GameObject.Find ("InputName").GetComponent<Text> ();
		}
	}

	public override void OnStopClient ()
	{
		base.OnStopClient ();

		Debug.Log ("On Stop Client");
	}

	public override void OnClientDisconnect (NetworkConnection conn)
	{
		base.OnClientDisconnect (conn);
		Debug.Log ("On client Disconnect");
	}

	void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "OfflineScene") {
			inputName = GameObject.Find ("InputName").GetComponent<Text> ();
		}
	}

	public override void OnServerConnect (NetworkConnection conn)
	{
		base.OnServerConnect (conn);
	}

	public override void OnServerDisconnect (NetworkConnection conn)
	{
		string playerName = conn.playerControllers [0].gameObject.GetComponent<PlayerStats> ().playerName;
		NetworkServer.Destroy (GameObject.Find (playerName + "ScorePanel"));
		base.OnServerDisconnect (conn);
		Debug.Log ("On Server Disconnect");
	}

}
