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


		rb.AddForce (transform.up * speed, ForceMode2D.Force);

		if (Input.GetAxis ("Horizontal") > 0f) {
			transform.Rotate (0f, 0f, -1f * rotateAngle * Time.deltaTime, Space.Self);
		}

		if (Input.GetAxis ("Horizontal") < 0f) {
			transform.Rotate (0f, 0f, 1f * rotateAngle * Time.deltaTime, Space.Self);
		}
	}

	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
		GetComponent<SpriteRenderer> ().color = Color.yellow;
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



	public void successfulHit ()
	{
		Destroy (this.gameObject);
	}
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