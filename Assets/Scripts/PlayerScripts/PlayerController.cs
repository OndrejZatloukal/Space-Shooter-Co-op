using UnityEngine;
using System.Collections;

	[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour 
{
	private Rigidbody rb;
	private AudioSource audioSource;

	public float speed;
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;

	private float nextFire;
	private float fireRateDown;
	private bool fireRatePower;

	void Update ()
	{

		if (Input.GetButton ("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
			audioSource.Play ();
		} 

		if (Input.GetKeyDown (KeyCode.P)) 
		{
			StartCoroutine (Firerate ());
			Debug.Log("p key was pressed");
		}


	}

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		fireRatePower = false;
	}
		

	// player movement 
	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rb.velocity = movement * speed;

	//defines range of players movement
		rb.position = new Vector3
		(
			Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax)
		);

	//ship rotation
		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);
	}	

	// powerup functions
	//---------------------------------------------------------------------------

	public void StartPowerup (int index)
	{
		if (index == 1) 
		{
			StartCoroutine (Firerate ());
		}
	}

	public IEnumerator Firerate ()
	{
		fireRateDown = Time.time + 5;
		Debug.Log (fireRateDown);
		if (fireRatePower == false) {
			fireRatePower = true;
			fireRate = fireRate / 2;
			Debug.Log (Time.time);
			yield return new WaitWhile (() => fireRateDown > Time.time);
			fireRate = fireRate * 2;
			fireRatePower = false;
		}
	}



}
