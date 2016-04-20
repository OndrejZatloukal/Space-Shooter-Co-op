using UnityEngine;
using System.Collections;

public class MissileExplosion : MonoBehaviour 
{
	public GameObject explosion;
	public GameObject PlayerExplosion;
	public float smoothing;
	[HideInInspector]
	public float blastRadius;

	private GameController gameController;
	private new SphereCollider collider;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) 
		{
			gameController = gameControllerObject.GetComponent <GameController> ();
		}
		if (gameController == null) 
		{
			Debug.Log ("Cannot find 'GameController' Script");
		}

		collider = GetComponent<SphereCollider> ();

		Instantiate(explosion, transform.position, transform.rotation);
	}

	void FixedUpdate () 
	{
		float newCollider = Mathf.MoveTowards (collider.radius, blastRadius, Time.deltaTime * smoothing);
		collider.radius = newCollider;
	}
		
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Boundary")) 
		{
			return;
		}

	//Player Explosion
		if (other.CompareTag ("Shield")) 
		{
			other.gameObject.SetActive(false);
			MeshCollider playerCollider = GameObject.FindWithTag("Player").GetComponent<MeshCollider> ();
			playerCollider.enabled = true;
			Destroy (gameObject);
		}
		else if (other.CompareTag ("Player"))
		{
			Instantiate (PlayerExplosion, other.transform.position, other.transform.rotation);
			gameController.GameOver ();
			Destroy (other.gameObject);
		}
	}
}
