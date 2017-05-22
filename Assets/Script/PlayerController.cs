using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class PlayerController : MonoBehaviour
{
	public float trailSpawnTimeInterval;
	public float speed;
	public float rotateAngle;
	public Transform trailSpawnPoint;

	private Rigidbody2D rb;
	// The variable for Trail logic
	public RaycastHit hit;
	private Vector3[] locs;
	private int head;
	private int tail = 0;

	//Testing new script
	public float acceleration;
	public float steering;
	public float maxSpeed;

	void Update ()
	{
//		if (!isLocalPlayer) {
//			return;
//		}

	}

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
		float v = Input.GetAxis ("Vertical");

		Vector2 speed = transform.up * (v * acceleration);
		rb.AddForce (speed);

		//Limit car velocity
		rb.velocity = Vector2.ClampMagnitude (rb.velocity, maxSpeed);
		if (Input.GetButtonUp ("Jump")) {
			transform.up = rb.velocity;
		}
		if (Input.GetButton ("Jump")) {
			float direction = Vector2.Dot (rb.velocity, rb.GetRelativeVector (Vector2.up));
			if (direction >= 0.0f) {
				rb.rotation += 0.5f * h * steering * (rb.velocity.magnitude / 5.0f);
//				rb.AddTorque (10 * (h * steering) * (rb.velocity.magnitude / 10.0f));
			} else {
				rb.rotation -= 0.5f * h * steering * (rb.velocity.magnitude / 5.0f);
//				rb.AddTorque (10 * (-h * steering) * (rb.velocity.magnitude / 10.0f));
			}
		} else {
			float direction = Vector2.Dot (rb.velocity, rb.GetRelativeVector (Vector2.up));
			if (direction >= 0.0f) {
				rb.rotation += h * steering * (rb.velocity.magnitude / 5.0f);
//				rb.AddTorque ((h * steering) * (rb.velocity.magnitude / 10.0f));
			} else {
				rb.rotation -= h * steering * (rb.velocity.magnitude / 5.0f);
//				rb.AddTorque ((-h * steering) * (rb.velocity.magnitude / 10.0f));
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