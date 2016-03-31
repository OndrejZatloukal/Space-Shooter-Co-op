using UnityEngine;
using System.Collections;

public class EvasiveManouverMissile : MonoBehaviour {


	//Enemy flying towards movement

	public float dodge;
	public float smoothing;
	public float tilt;
	public Vector2 startWait;
	public Vector2 manouverTime;
	public Vector2 manouverWait;
	public Boundary boundary;

	private Transform playerTransform;

	private float currentSpeed;
	private float targetManouver;
	private Rigidbody rb;

	void Start () 
	{
		rb = GetComponent <Rigidbody> ();
		GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
		if (playerObject != null) {
			playerTransform = playerObject.transform;
		} 
		currentSpeed = rb.velocity.z;
		StartCoroutine (Evade ());
	}

	IEnumerator Evade ()
	{
		yield return new WaitForSeconds (Random.Range (startWait.x, startWait.y));

		while (true) 
		{
	//		targetManouver = Random.Range (1, dodge) * -Mathf.Sign (transform.position.x);
			if (playerTransform != null) {
				targetManouver = playerTransform.position.x;
			}
			yield return new WaitForSeconds (Random.Range (manouverTime.x, manouverTime.y));
			targetManouver = 0;
			yield return new WaitForSeconds (Random.Range (manouverWait.x, manouverWait.y));
		}
	}
	

	void FixedUpdate () 
	{
		float newManouver = Mathf.MoveTowards (rb.velocity.x, targetManouver, Time.deltaTime * smoothing); 
		rb.velocity = new Vector3 (newManouver, 0.0f, currentSpeed);
		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax)
		);

		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);
	}
}
