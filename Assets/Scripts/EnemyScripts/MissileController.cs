using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {


	private Transform playerTransform;

	private Animator anim;

	public GameObject missileExplosion;
	public float triggerRadius;
	public float flashSpeed;
	public float flashIncrement;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		anim.speed = 1.0f;
		GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
		if (playerObject != null)
		{
			playerTransform = playerObject.transform;
		}

	}
	
	// Update is called once per frame
	void FixedUpdate () { 
		if (playerTransform != null)
		{
			anim.speed = Mathf.Max (1, flashSpeed - (Vector3.Distance (transform.position, playerTransform.position) / flashIncrement));

			if (Vector3.Distance (transform.position, playerTransform.position) < triggerRadius)
			{ 
				Instantiate (missileExplosion, transform.position, transform.rotation);
				Destroy (gameObject);
			}
		} else {
			anim.speed = 1.0f;
		}
	}
}
