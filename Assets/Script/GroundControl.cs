using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControl : MonoBehaviour
{
	public enum Ground
	{
		IceGround,
		SandGround,
	};

	public Ground groundType;

	void OnTriggerEnter2D (Collider2D other)
	{
		switch (groundType) {
		case Ground.IceGround:
			if (other.gameObject.tag == "Player" && other.gameObject.name != "Body") {
				other.gameObject.GetComponent<Rigidbody2D> ().angularDrag *= 4;
			}
			break;
		case Ground.SandGround:
			if (other.gameObject.tag == "Player" && other.gameObject.name != "Body") {
				other.gameObject.GetComponent<Rigidbody2D> ().drag *= 4;
			}
			break;
		}

	}

	void OnTriggerExit2D (Collider2D other)
	{
		switch (groundType) {
		case Ground.IceGround:
			if (other.gameObject.tag == "Player" && other.gameObject.name != "Body") {
				other.gameObject.GetComponent<Rigidbody2D> ().angularDrag *= 0.25f;
			}
			break;
		case Ground.SandGround:
			if (other.gameObject.tag == "Player" && other.gameObject.name != "Body") {
				other.gameObject.GetComponent<Rigidbody2D> ().drag *= 0.25f;
			}
			break;
		}
	}
}
