using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {


	private Transform playerTransform;

	public GameObject missileExplosion; 

	// Use this for initialization
	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if ( Vector3.Distance (transform.position, playerTransform.position) < 5) { 
			Instantiate (missileExplosion, transform.position, transform.rotation);
			Destroy (gameObject);
		}
	}
}
