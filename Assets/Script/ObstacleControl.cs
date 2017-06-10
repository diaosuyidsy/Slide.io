using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

public class ObstacleControl : NetworkBehaviour
{
	public enum Obstacle
	{
		Edges,
		Wood,
		Metal,
		Vortex,
	};

	public Obstacle ObstacleType;

	public float vortexForce;
	public float vortexDestroyDistance;

	void OnTriggerStay2D (Collider2D other)
	{
		if (ObstacleType == Obstacle.Vortex && other.gameObject.tag == "Player") {
			float circleRadius = GetComponent<CircleCollider2D> ().radius;
			float distance = Vector2.Distance (transform.position, other.gameObject.transform.position);
			float extra = circleRadius + 2f - distance;
			other.attachedRigidbody.AddForce ((transform.position - other.gameObject.transform.position).normalized * vortexForce * Time.deltaTime * extra);
			//If player enters center area, destroy player
			if (distance < vortexDestroyDistance) {
				other.gameObject.SendMessageUpwards ("takeDamage", 1);
			}
		}
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		switch (ObstacleType) {
		case Obstacle.Edges:
			if (coll.collider.gameObject.tag == "Player") {
				coll.gameObject.SendMessageUpwards ("takeDamage", 100);
			}
			break;
		case Obstacle.Metal:
			if (coll.collider.gameObject.tag == "Player") {
				coll.gameObject.SendMessageUpwards ("takeDamage", 1);
			}
			break;
		case Obstacle.Wood:
			if (coll.collider.gameObject.tag == "Player") {
				coll.gameObject.SendMessageUpwards ("takeDamage", 1);
			}
			if (isServer) {
				RpcDestroyAndRevive (1);
			} else {
				//Hide Rigidebody, SpriteRenderer, Play break animation
				foreach (Collider2D c in GetComponents<Collider2D> ()) {
					c.enabled = false;
				}
				GetComponent<SpriteRenderer> ().enabled = false;
				StartCoroutine (DestroyAndRevive (1));
			}
			break;
		}
	}

	[ClientRpc]
	void RpcDestroyAndRevive (int reviveTime)
	{
		//Hide Rigidebody, SpriteRenderer, Play break animation
		foreach (Collider2D c in GetComponents<Collider2D> ()) {
			c.enabled = false;
		}
		GetComponent<SpriteRenderer> ().enabled = false;

		StartCoroutine (DestroyAndRevive (reviveTime));
	}

	IEnumerator DestroyAndRevive (int reviveTime)
	{
		yield return new WaitForSeconds (reviveTime);

		foreach (Collider2D c in GetComponents<Collider2D> ()) {
			c.enabled = true;
		}
		GetComponent<SpriteRenderer> ().enabled = true;
	}
}

[CustomEditor (typeof(ObstacleControl))]
public class ObstacleControlEditor : Editor
{
	//	SerializedProperty ObstacleType;
	//
	//	void OnEnable ()
	//	{
	//		ObstacleType = serializedObject.FindProperty ("ObstacleType");
	//	}
	//
	//	public override void OnInspectorGUI ()
	//	{
	//		serializedObject.Update ();
	//		EditorGUILayout.PropertyField (ObstacleType);
	//		serializedObject.ApplyModifiedProperties ();
	////		if (ObstacleType == (target as ObstacleControl).Ob) {
	////
	////		}
	//	}
	public override void OnInspectorGUI ()
	{
		ObstacleControl script = (ObstacleControl)target;

		script.ObstacleType = (ObstacleControl.Obstacle)EditorGUILayout.EnumPopup ("Obstacle Type", script.ObstacleType);
		if (script.ObstacleType == ObstacleControl.Obstacle.Vortex) {
			script.vortexForce = EditorGUILayout.FloatField ("Vortex Force", script.vortexForce);
			script.vortexDestroyDistance = EditorGUILayout.FloatField ("Vortex Destroy Distance", script.vortexDestroyDistance);
		}
	}
}