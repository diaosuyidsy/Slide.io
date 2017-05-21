using UnityEngine;
using System.Collections;

public class dumnAI : MonoBehaviour
{
	private Rigidbody2D rb;
	public float speed;
	public float rotateAngle;

	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		rb.AddForce (transform.up * speed, ForceMode2D.Force);
		transform.Rotate (0f, 0f, -1f * rotateAngle * Time.deltaTime, Space.Self);
	}

	public void successfulHit ()
	{
		Destroy (gameObject);
	}
}
