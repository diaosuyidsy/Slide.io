using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;


public class SlideioNetworkManager : NetworkManager
{
	public void TryStartGame ()
	{
		StartServer ();
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
}
