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
	private MeshCollider collider;

	public float speed;
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform[] shotSpawns;
	public float fireRate;

	private float nextFire;
	private float fireRateDown;
	private float speedDown;
	private bool fireRatePower;
	private bool speedUpPower;
	private float fireDoubleDown;
	public bool startShield = false;

	private int[] shotSpawn;
	private GameObject shield;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		collider = GetComponent<MeshCollider>();
		fireRatePower = false;
		shield = GameObject.FindWithTag("Shield");
		shotSpawn = new int[]{0};

		if (!startShield) 
		{
			shield.SetActive (false);
		} else 
		{
			collider.enabled = false;
		}

	}

	void Update ()
	{
		if (Input.GetButton ("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			for (int i = 0; i < shotSpawn.Length; i++) { 
				Instantiate (shot, shotSpawns [shotSpawn [i]].position, shotSpawns [shotSpawn [i]].rotation);
			}
			audioSource.Play ();
		} 

		if (Input.GetKeyDown (KeyCode.P)) 
		{
			StartCoroutine (Firerate ());
			Debug.Log("p key was pressed");
		}

		if (Input.GetKeyDown (KeyCode.O)) 
		{
			Shield ();
			Debug.Log("o key was pressed");
		}

		if (Input.GetKeyDown (KeyCode.I)) 
		{
			StartCoroutine (FireDouble ());
			Debug.Log("i key was pressed");
		}

		if (Input.GetKeyDown (KeyCode.U)) 
		{
			StartCoroutine (SpeedPower ());
			Debug.Log("u key was pressed");
		}

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
		if (index == 1) {
			StartCoroutine (Firerate ());
		} else if (index == 2) {
			Shield ();
		} else if (index == 3) {
			StartCoroutine (FireDouble());
		} else if (index == 4) {
			StartCoroutine (SpeedPower());
		}
	}

	public IEnumerator Firerate ()
	{
		fireRateDown = Time.time + 5;
		if (fireRatePower == false)
		{
			fireRatePower = true;
			fireRate = fireRate / 2;
			yield return new WaitWhile (() => fireRateDown > Time.time);
			fireRate = fireRate * 2;
			fireRatePower = false;
		}
	}

	public void Shield ()
	{
		if (shield.activeSelf) {
			return;
		} else {
			shield.SetActive(true);
			collider.enabled = false;
		}
	}

	public IEnumerator FireDouble ()
	{ 
		fireDoubleDown = Time.time + 5;
		if (shotSpawn.Length < 2)
		{
			//fireDouble = true;
			shotSpawn = new int[]{ 1, 2 };
			yield return new WaitWhile (() => fireDoubleDown > Time.time);
			//fireDouble = false;
			shotSpawn = new int[]{0};
		}
	}

	public IEnumerator SpeedPower ()
	{
		speedDown = Time.time + 5;
		if (speedUpPower == false)
		{
			speedUpPower = true;
			speed = speed * 1.5f;
			yield return new WaitWhile (() => speedDown > Time.time);
			speed = speed / 1.5f;
			speedUpPower = false;
		}
	}


}
