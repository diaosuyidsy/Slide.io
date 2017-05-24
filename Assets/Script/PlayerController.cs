using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class PlayerController : MonoBehaviour
{
	public float trailSpawnTimeInterval;
	public Transform trailSpawnPoint;
	public GameObject[] Wheels;
	public GameObject trailSpawnPrefab;

	private Rigidbody2D rb;
	// The variable for Trail logic
	public RaycastHit hit;
	private Vector3[] locs;
	private int head;
	private int tail = 0;

	//Testing new script
	public float maxAcceleration;
	public float steering;
	public float maxSpeed;
	public float maxBurstTime;

	private float burstTime;
	private float acceleration;
	private bool burstLock = true;
	private GameObject[] spawnedTrails;

	private 

	void FixedUpdate ()
	{
//		if (!isLocalPlayer) {
//			return;
//		}
		RaycastHit2D hit = Hit ();
		if (hit.collider != null) {
			Debug.Log ("I hit: " + hit.collider.name);
			hit.collider.gameObject.SendMessage ("successfulHit");
		}


//		rb.AddForce (transform.up * speed, ForceMode2D.Force);
//
//		if (Input.GetAxis ("Horizontal") > 0f) {
//			transform.Rotate (0f, 0f, -1f * rotateAngle * Time.deltaTime, Space.Self);
//		}
//
//		if (Input.GetAxis ("Horizontal") < 0f) {
//			transform.Rotate (0f, 0f, 1f * rotateAngle * Time.deltaTime, Space.Self);
//		}

		float h = -Input.GetAxis ("Horizontal");

		if (acceleration <= maxAcceleration) {
			acceleration += 5 * Time.deltaTime;
		}

		if (acceleration > maxAcceleration) {
			acceleration -= 5 * Time.deltaTime;
		}

		if (!burstLock) {
			acceleration = 1.5f * maxAcceleration;
			burstTime -= Time.deltaTime;
			if (burstTime <= 0f) {
				burstLock = true;
				burstTime = maxBurstTime;
			}
		}

		Vector2 pullForce = transform.up * (1 * acceleration);
		rb.AddForce (pullForce);

		//Limit car velocity
		rb.velocity = Vector2.ClampMagnitude (rb.velocity, maxSpeed);
		if (Input.GetButton ("Burst")) {
			burstLock = false;
		}
		if (Input.GetButtonDown ("Stop")) {
			for (int i = 0; i < Wheels.Length; i++) {
				spawnedTrails [i] = Instantiate (trailSpawnPrefab);
				spawnedTrails [i].transform.parent = Wheels [i].transform;
				spawnedTrails [i].transform.position = Wheels [i].transform.position;
			}
		}
		if (Input.GetButtonUp ("Stop")) {
			Debug.Log ("Got Button Up!!!!");
			for (int i = 0; i < Wheels.Length; i++) {
				Wheels [i].transform.DetachChildren ();
			}
		}
		if (Input.GetButton ("Stop")) {
			
			float direction = Vector2.Dot (rb.velocity, rb.GetRelativeVector (Vector2.up));
			if (direction >= 0.0f) {
//				rb.rotation += 0.5f * h * steering * (rb.velocity.magnitude / 5.0f);
				rb.AddTorque ((h * 0.008f) * (rb.velocity.magnitude / 10.0f));
			} else {
//				rb.rotation -= 0.5f * h * steering * (rb.velocity.magnitude / 5.0f);
				rb.AddTorque ((-h * 0.008f) * (rb.velocity.magnitude / 10.0f));
			}
		} else {
			if (Input.GetButtonUp ("Stop")) {
				Debug.Log ("Got Button Up!!!!");
				for (int i = 0; i < Wheels.Length; i++) {
					Wheels [i].transform.DetachChildren ();
				}
			}
			float direction = Vector2.Dot (rb.velocity, rb.GetRelativeVector (Vector2.up));
			if (direction >= 0.0f) {
//				rb.rotation += h * steering * (rb.velocity.magnitude / 5.0f);
				rb.AddTorque ((h * 0.005f) * (rb.velocity.magnitude / 10.0f));
			} else {
//				rb.rotation -= h * steering * (rb.velocity.magnitude / 5.0f);
				rb.AddTorque ((-h * 0.005f) * (rb.velocity.magnitude / 10.0f));
			}

			Vector2 forward = new Vector2 (0.0f, 0.5f);
			float steeringRightAngle;
			if (rb.angularVelocity > 0) {
				steeringRightAngle = -90;
			} else {
				steeringRightAngle = 90;
			}

			Vector2 rightAngleFromForward = Quaternion.AngleAxis (steeringRightAngle, Vector3.forward) * forward;

			float driftForce = Vector2.Dot (rb.velocity, rb.GetRelativeVector (rightAngleFromForward.normalized));

			Vector2 relativeForce = (rightAngleFromForward.normalized * -1.0f) * (driftForce * 10.0f);

			rb.AddForce (rb.GetRelativeVector (relativeForce));

		}




	
	}

	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
//		GetComponent<SpriteRenderer> ().color = Color.yellow;
		Camera.main.GetComponent<CameraFollow2D> ().setTarget (gameObject.transform);
		acceleration = 0f;
		burstLock = true;
		burstTime = maxBurstTime;
		spawnedTrails = new GameObject[4];

		// Logic for Trail
		locs = new Vector3[20];
		for (int i = 0; i < locs.Length; i++) {
			locs [i] = trailSpawnPoint.transform.position;
		}
		head = locs.Length - 1;
		StartCoroutine (CollectData ());
	}

	IEnumerator CollectData ()
	{
		while (true) {
			if (trailSpawnPoint.transform.position != locs [head]) {
				head = (head + 1) % locs.Length;
				tail = (tail + 1) % locs.Length;
				locs [head] = trailSpawnPoint.transform.position;
			}
			yield return new WaitForSeconds (trailSpawnTimeInterval);
		}
	}

	private RaycastHit2D Hit ()
	{
		RaycastHit2D hit = new RaycastHit2D ();
		int i = head;
		int j = head - 1;
		if (j < 0)
			j = locs.Length - 1;
		while (j != head) {
			hit = Physics2D.Linecast (locs [i], locs [j]);
			if (hit.collider != null) {
				return hit;
			}

			Debug.DrawLine (locs [i], locs [j], Color.black);

			i = i - 1; 
			if (i < 0)
				i = locs.Length - 1;
			
			j = j - 1;
			if (j < 0)
				j = locs.Length - 1;
		}
		return hit;
	}



	//	public void successfulHit ()
	//	{
	//		Destroy (this.gameObject);
	//	}
}


//	IEnumerator spawnTrailComponents ()
//	{
//		yield return new WaitForSeconds (trailSpawnTimeInterval);
//		CmdSpawn ();
//		StartCoroutine (spawnTrailComponents ());
//	}
//
//	[Command]
//	void CmdSpawn ()
//	{
//		GameObject trailComponent = (GameObject)Instantiate (trailComponentPrefab, trailSpawnPoint.position, Quaternion.identity);
//		NetworkServer.Spawn (trailComponent);
//	}