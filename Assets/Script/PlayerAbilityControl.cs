using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerAbilityControl : NetworkBehaviour
{
	public float maxMainAbilityCD;
	public float maxUltLastTime;

	private float UltLastTime;
	private bool UltLock;
	private bool useUltNow;
	private float MainAbilityCD;
	private Rigidbody2D rb;
	private bool startMACD;
	private bool MALock;
	//	private PlayerController pc;

	public Image UltCoverImage;
	public Image[] MainAbCoverImages;

	[SyncVar (hook = "OnChangeUltCharge")]
	public float ultCharge;

	// Use this for initialization
	void Start ()
	{
		UltLock = true;
		useUltNow = false;
		UltLastTime = maxUltLastTime;
		MainAbilityCD = maxMainAbilityCD;
		rb = GetComponent<Rigidbody2D> ();
		startMACD = false;
		MALock = false;
//		pc = GetComponent<PlayerController> ();
	}

	void FixedUpdate ()
	{
		if (useUltNow) {
			// If there is a stronger effect on it, ignore this
			if (GetComponent<PlayerController> ().getAcc () >= GetComponent<PlayerController> ().maxAcceleration * 3f)
				return;
			GetComponent<PlayerController> ().setAcc (GetComponent<PlayerController> ().maxAcceleration * 3f);
			UltLastTime -= Time.deltaTime;
			// If Ult is used up, reset ult;
			if (UltLastTime <= 0f) {
				useUltNow = false;
				UltLastTime = maxUltLastTime;
				if (isServer) {
					addUltCharge (-ultCharge);
				} else {
					CmdAddUltCharge (-ultCharge);
				}

				GetComponent<PlayerController> ().setAcc (GetComponent<PlayerController> ().maxAcceleration);
			}
		}
	}

	void Update ()
	{
		if (startMACD) {
			MainAbilityCD -= Time.deltaTime;
			foreach (Image coverImage in MainAbCoverImages) {
				coverImage.fillAmount -= (Time.deltaTime / maxMainAbilityCD);
			}
			if (MainAbilityCD <= 0f) {
				startMACD = false;
				MALock = false;
				MainAbilityCD = maxMainAbilityCD;
				foreach (Image coverImage in MainAbCoverImages) {
					coverImage.fillAmount = 0f;
				}
			}
		}
	}

	public void useMainAbility (bool toLeft)
	{
		if (!MALock) {
			foreach (Image coverImage in MainAbCoverImages) {
				coverImage.fillAmount = 1f;
			}
			MALock = true;
			if (toLeft) {
				transform.Rotate (0, 0, 90);
			} else {
				transform.Rotate (0, 0, -90);
			}
			//Also rotate the velocity
			rb.velocity = transform.up * rb.velocity.magnitude;
			startMACD = true;
		}
	}

	public void useUlt ()
	{
		if (UltLock) {
			return;
		} else {
			UltLock = true;
			useUltNow = true;
		}

	}

	[Command]
	void CmdAddUltCharge (float addAmount)
	{
		ultCharge += addAmount;
	}

	public void addUltCharge (float addAmount)
	{
		if (!isServer)
			return;
		ultCharge += addAmount;
	}

	void OnChangeUltCharge (float newUltCharge)
	{
		if (isLocalPlayer) {
			UltCoverImage.fillAmount -= (newUltCharge - ultCharge);
		}
		if (newUltCharge > 1f) {
			UltLock = false;
		}
		ultCharge = newUltCharge;
	}
}
