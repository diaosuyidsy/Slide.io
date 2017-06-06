using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectButton : MonoBehaviour
{
	public void disconnectPlayer ()
	{
		GameObject.Find ("Network Manager").GetComponent<SlideioNetworkManager> ().StopClient ();
	}
}
