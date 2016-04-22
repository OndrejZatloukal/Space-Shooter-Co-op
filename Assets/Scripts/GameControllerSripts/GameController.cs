using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{
	public GameObject[] hazards;
	public int hazardCount;
	public int hazardCountInc;
	public float hazardType;
	public float hazardIncrease;
	public Vector3 spawnValues;
	public float spawnWait;
	public float startWait;
	public float waveWait;

	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameOverText;
	public GUIText waveText;

	private bool gameOver;
	private bool restart;
	private int score;
	private int wave;
	//Asteroid spawning
	void Start ()
	{
		SceneManager.LoadScene ("Secondary", mode: LoadSceneMode.Additive);
		gameOver = false;
		restart = false;
		restartText.text = "";
		gameOverText.text = "";
		score = 0;
		wave = 0;
		UpdateScore ();
		UpdateWave ();
		StartCoroutine (SpawnWaves ());
	}

	void Update ()
	{
		if (restart) 
		{
			if (Input.GetKeyDown (KeyCode.R)) 
			{
				SceneManager.LoadScene (SceneManager.GetActiveScene().name);
			}
		}
	}

	IEnumerator SpawnWaves ()
	{
		//Spawn Asteroid in wide range 
		yield return new WaitForSeconds (startWait);
		while (true)
		{
			wave += 1;
			UpdateWave ();
			for (int i = 0; i < hazardCount; i++) 
			{
				GameObject hazard = hazards[Random.Range (0, Mathf.FloorToInt (hazardType + .00001f))];
				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);

			hazardCount += hazardCountInc;
			hazardType = Mathf.Min (hazardType + (1 / hazardIncrease), hazards.Length);

			if (gameOver) 
			{
				restartText.text = "Press 'R' for Restart";
				restart = true;
				break;
			}
		}
	}

	//Score 

	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}
		
	void UpdateScore ()
	{
		scoreText.text = "Score: " + score;
	}

	void UpdateWave ()
	{
		waveText.text = "Wave: " + wave;
	}

	public void GameOver ()
	{
		gameOverText.text = "Game Over";
		gameOver = true;
	}
}
