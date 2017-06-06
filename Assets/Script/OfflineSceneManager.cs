using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfflineSceneManager : MonoBehaviour
{
	public Text inputName;

	public void TryStartGame ()
	{
		GameObject.Find ("Network Manager").GetComponent<SlideioNetworkManager> ().TryStartGame ();
	}

	public void TryJoinGame ()
	{
		GameObject.Find ("Network Manager").GetComponent<SlideioNetworkManager> ().TryJoingame ();
	}
}
