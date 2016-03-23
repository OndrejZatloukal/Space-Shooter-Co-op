using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour 
{
	//Destroying game objects on entering boundary
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Boundary") 
		{
			return;
		}
		Destroy(other.gameObject);
		Destroy(gameObject);
	}
}
