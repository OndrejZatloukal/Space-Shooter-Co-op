using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour 
{
	public GameObject explosion;
	public GameObject PlayerExplosion;
	public int scoreValue;
	private GameController gameController;

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
	}
		
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Boundary") || other.CompareTag ("Enemy") || other.CompareTag ("Powerup") || this.tag == "Dead") 
		{
			return;
		}
	//Asteroid Explosion
		if (explosion != null)
		{
			if (explosion.tag == "Explosion")
			{
				GameObject Explosion = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
				Explosion.GetComponent<MissileExplosion>().blastRadius = GetComponent<MissileController>().triggerRadius;
			} else {
				Instantiate (explosion, transform.position, transform.rotation);
			}
		}
	//Player Explosion
		if (other.tag == "Player")
		{
			Instantiate(PlayerExplosion, other.transform.position, other.transform.rotation);
			gameController.GameOver ();
		}
		gameController.AddScore (scoreValue);

		if (other.tag != "Explosion" && other.tag != "Shield") 
		{
			Destroy(other.gameObject);
		}

		if (other.CompareTag ("Shield")) 
		{
            other.GetComponentInParent<PlayerController>().ShieldDown();
        }

		Destroy(gameObject);
	}
}
