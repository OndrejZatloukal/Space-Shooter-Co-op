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
    //private GameController gameController;
    private PowerupUI powerupUI;
    private SecondaryController secondaryController;

	private Rigidbody rb;
	public AudioSource shotPlayer;
    public AudioSource shotTurret;
    private new MeshCollider collider;
	private new Camera camera;

	public float speed;
	public float tilt;
	public float powerupTime;
	public Boundary boundary;

	public GameObject shot;
    public GameObject turretShot;
    public Transform[] shotSpawns;
	public float fireRate;

	private float nextFire;
	private float fireRateDown;
	private float speedDown;
	private bool fireRatePower;
	private bool speedUpPower;
	private float fireDoubleDown;
	public bool startShield = false;

    // variables for turret
    private bool turretActive = false;
    private float turretDown;
    private float turretFireRate;
    private float nextTurretFire;

	private int[] shotSpawn;
	private GameObject shield;
	private GameObject turret;
    public Transform turretShotSpawn;

    private Vector3 mouseVector;

    void Start ()
	{
        // Find the Game Controller
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            //gameController = gameControllerObject.GetComponent<GameController>();
            powerupUI = gameControllerObject.GetComponent<PowerupUI>();
        }
        //if (gameController == null)
        //{
        //    Debug.Log("Cannot find 'GameController' Script");
        //}
        if (powerupUI == null)
        {
            Debug.Log("Cannot find 'PowerupUI' Script");
        }

        // Find the Secondary Controller
        GameObject secondaryControllerObject = GameObject.FindWithTag("SecondaryController");
        if (secondaryControllerObject != null)
        {
            secondaryController = secondaryControllerObject.GetComponent<SecondaryController>();
        }
        if (secondaryController == null)
        {
            Debug.Log("Cannot find 'SecondaryController' Script");
        }

        // get objects
        rb = GetComponent<Rigidbody>();
		//audioSource = GetComponent<AudioSource>();
		collider = GetComponent<MeshCollider>();
		camera  = GameObject.FindWithTag("MainCamera").GetComponent <Camera>();

        // set up power up data
		fireRatePower = false;
		shield = GameObject.FindWithTag("Shield");
		shotSpawn = new int[]{0};
        turretFireRate = fireRate;
        turret = GameObject.FindWithTag("Turret");

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
		if (Input.GetButton ("Fire1") && Time.time > nextFire)
        {
			nextFire = Time.time + fireRate;
			for (int i = 0; i < shotSpawn.Length; i++)
            { 
				Instantiate (shot, shotSpawns [shotSpawn [i]].position, shotSpawns [shotSpawn [i]].rotation);
			}
            shotPlayer.Play ();
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

        // Debug: Active Turret
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartCoroutine(Turret());
            Debug.Log("y key was pressed");
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

        // if turret powerup active
        if (turretActive)
        {
            //turret rotation
            mouseVector = camera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 relative = mouseVector - turret.transform.position;

            turret.transform.rotation = Quaternion.Euler(
                0.0f,
                Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg,
                180.0f
                );

            //shoot turret
            if (Input.GetMouseButton(0) && Time.time > nextTurretFire)
            {
                nextTurretFire = Time.time + turretFireRate;
                Instantiate(turretShot, turretShotSpawn.position, turretShotSpawn.rotation);

                shotTurret.Play();
            }
        }
    }	

	// powerup functions
	//---------------------------------------------------------------------------

	public void StartPowerup (int index)
	{
		if (index == 1)
        {
			StartCoroutine (Firerate ());
		}
        else if (index == 2)
        {
			Shield ();
		}
        else if (index == 3)
        {
			StartCoroutine (FireDouble());
		}
        else if (index == 4)
        {
			StartCoroutine (SpeedPower());
        }
        else if (index == 5)
        {
            StartCoroutine(Turret());
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

    public IEnumerator Turret()
    {
        turretDown = Time.time + powerupTime * 2;
        if (turretActive == false)
        {
            // power up
            secondaryController.DeactivateMouse();
            turretActive = true;
            powerupUI.Active(5);

            // wait for timer to run out
            yield return new WaitWhile(() => turretDown > Time.time);

            // power down
            turretActive = false;
            secondaryController.ReactivateMouse();
            turret.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
            powerupUI.Deactive(5);
        }
    }
}
