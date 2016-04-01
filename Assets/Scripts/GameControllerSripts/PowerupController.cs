using UnityEngine;
using System.Collections;

public class PowerupController : MonoBehaviour 
{
	
	private PlayerController playerController;

	void OnTriggerEnter(Collider other) {


		if (other.tag == "Player") {
			StartCoroutine (other.GetComponent <PlayerController> ().Firerate ());



			Destroy (gameObject);
		}
	}
}
