using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount; 
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public int hazardType;

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
		gameOver = false;
		restart = false;
		restartText.text = "";
		gameOverText.text = "";
		waveText.text = "";
		score = 0;
		wave = 0;
		UpdateScore ();
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
			UpdateWave ();
			for (int i = 0; i < hazardCount; i++) 
			{
				GameObject hazard = hazards[Random.Range (0, hazardType)];
				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);

			hazardCount += 2;
			hazardType += 1;

			if(hazardType > hazards.Length) {
				hazardType = hazards.Length;
			}

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
		wave += 1;
		waveText.text = "Wave: " + wave;
	}

	public void GameOver ()
	{
		gameOverText.text = "Game Over";
		gameOver = true;
	}
}
