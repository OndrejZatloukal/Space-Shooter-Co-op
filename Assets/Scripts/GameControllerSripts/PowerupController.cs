using UnityEngine;
using System.Collections;

public class PowerupController : MonoBehaviour 
{
	public int powerupIndex;
	private PlayerController playerController;

	void OnTriggerEnter(Collider other) {

		if (other.tag == "Explosion")
		{
			Destroy (gameObject);
		}

		if (other.tag == "Player") {
			other.GetComponent <PlayerController> ().StartPowerup (powerupIndex);
			Destroy (gameObject);
		}
	}
}
