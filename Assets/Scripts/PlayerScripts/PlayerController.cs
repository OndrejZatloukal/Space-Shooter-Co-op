using UnityEngine;
using System.Collections;

// adds class boundary to set the bounds in which the player can move around
// data members can be set in Unity editor
	[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour 
{
    private PowerupUI powerupUI;

	private Rigidbody rb;
	private AudioSource audioSource;
	private new MeshCollider collider;
//	private new Camera camera;

	public float speed;
	public float tilt;
	public float powerupTime;
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
//	private GameObject turret;

	void Start ()
	{
        // Find the Game Controller
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            powerupUI = gameControllerObject.GetComponent<PowerupUI>();
        }
        if (powerupUI == null)
        {
            Debug.Log("Cannot find 'PowerupUI' Script");
        }

        // get objects
        rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		collider = GetComponent<MeshCollider>();
//		camera  = GameObject.FindWithTag("MainCamera").GetComponent <Camera> ();

        // set up power up data
		fireRatePower = false;
		shield = GameObject.FindWithTag("Shield");
//		turret = GameObject.FindWithTag("Turret");
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

        // Debug: Active Powerup Firerate
		if (Input.GetKeyDown (KeyCode.P)) 
		{
			StartCoroutine (Firerate ());
			Debug.Log("p key was pressed");
		}

        // Debug: Active Powerup Shield
        if (Input.GetKeyDown (KeyCode.O)) 
		{
			Shield ();
			Debug.Log("o key was pressed");
		}

        // Debug: Active Powerup Fire Double
        if (Input.GetKeyDown (KeyCode.I)) 
		{
			StartCoroutine (FireDouble ());
			Debug.Log("i key was pressed");
		}

        // Debug: Active Powerup Speed
        if (Input.GetKeyDown (KeyCode.U)) 
		{
			StartCoroutine (SpeedPower ());
			Debug.Log("u key was pressed");
		}
	}

	// Player Movement 
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
		fireRateDown = Time.time + powerupTime;
		if (fireRatePower == false)
		{
            // power up
			fireRatePower = true;
			fireRate = fireRate / 2;
            powerupUI.Active(1);

            // wait for timer to run out
            yield return new WaitWhile (() => fireRateDown > Time.time);

            // power down
            fireRate = fireRate * 2;
            fireRatePower = false;
            powerupUI.Deactive(1);
        }
	}

	public void Shield ()
	{
		if (shield.activeSelf) {
			return;
		} else {
            // power up
            shield.SetActive(true);
			collider.enabled = false;
            powerupUI.Active(2);
        }
	}

    public void ShieldDown()
    {
        // power down
        shield.SetActive(false);
        collider.enabled = true;
        powerupUI.Deactive(2);
    } // end function ShieldDown

	public IEnumerator FireDouble ()
	{ 
		fireDoubleDown = Time.time + powerupTime;
		if (shotSpawn.Length < 2)
		{
            // power up
            //fireDouble = true;
            shotSpawn = new int[]{ 1, 2 };
            powerupUI.Active(3);

            // wait for timer to run out
            yield return new WaitWhile (() => fireDoubleDown > Time.time);

            // power down
            shotSpawn = new int[] { 0 };
            //fireDouble = false;
            powerupUI.Deactive(3);
        }
	}

	public IEnumerator SpeedPower ()
	{
		speedDown = Time.time + powerupTime;
		if (speedUpPower == false)
		{
            // power up
            speedUpPower = true;
			speed = speed * 1.5f;
            powerupUI.Active(4);

            // wait for timer to run out
            yield return new WaitWhile (() => speedDown > Time.time);

            // power down
            speed = speed / 1.5f;
			speedUpPower = false;
            powerupUI.Deactive(4);
        }
	}


}
