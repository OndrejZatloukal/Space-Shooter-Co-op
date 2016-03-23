using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour 
{
	public GameObject explosion;
	public GameObject PlayerExplosion;

	//Destroying game objects on entering boundary
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Boundary") 
		{
			return;
		}
	//Asteroid Explosion
		Instantiate(explosion, transform.position, transform.rotation);
	//Player Explosion
		if (other.tag == "Player")
		{
		Instantiate(PlayerExplosion, other.transform.position, other.transform.rotation);
		}
		Destroy(other.gameObject);
		Destroy(gameObject);
	}
}
