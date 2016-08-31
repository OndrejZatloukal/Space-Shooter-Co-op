using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecondaryController : MonoBehaviour {

	public const int gridX = 8;
	public const int gridY = 12;

	[HideInInspector]
	public int ySpawn;

	public GUIText mouseText;
	public GUIText redScore;
	public GUIText blueScore;
	public GUIText yellowScore;
	public GUIText greenScore;
    public GUIText whiteScore;

    private int scoreRed;
	private int scoreBlue;
	private int scoreYellow;
	private int scoreGreen;
    private int scoreWhite;

    public float zOffset;
	public GameObject[] powerups;

	public int targetRed;
	public int targetBlue;
	public int targetYellow;
	public int targetGreen;
    public int baseTargetWhite;
    public int targetWhite;

	private GameObject[][] grid = new GameObject[gridX][];
	private List<GameObject> Match = new List<GameObject>();
	private List<int> Selected = new List<int>();
	private GameObject Swap;

    private PlayerController player;

    private new Camera camera;
	private bool initializing = true;

    private bool mouseDeactive = false;
    private bool mouseActive = false;
	private Vector3 mouseVector;
	private int mousePositionX;
	private int mousePositionY;

    public GameObject banner;

	// Use this for initialization
	void Start () {
        // Set spawn point for match objects
		ySpawn = gridY + 1;

		camera  = GameObject.FindWithTag("SecondaryCamera").GetComponent <Camera> ();

        // Try to find the player
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }
        catch (System.NullReferenceException)
        {
            player = null;
        }

        // Initialize score and score text
        scoreRed = 0;
		scoreBlue = 0;
		scoreYellow = 0;
		scoreGreen = 0;
        scoreWhite = 0;
        baseTargetWhite = targetWhite;

        banner.SetActive(false);

        redScore.text = "Fire Rate: " + scoreRed + " / " + targetRed;
        blueScore.text = "Shield Up: " + scoreBlue + " / " + targetBlue;
        yellowScore.text = "Double Fire: " + scoreYellow + " / " + targetYellow;
        greenScore.text = "Speed Up: " + scoreGreen + " / " + targetGreen;
        whiteScore.text = "Turret On: " + scoreWhite + " / " + targetWhite;

        // Create 2D grid
        for (int i = 0; i < grid.Length; i++)
		{
			grid[i] = new GameObject[gridY];
		}

        // Fill empty grid with match objects
		StartCoroutine (InitSpawnPowerups ());
	} // end function start
	
	// Update is called once per frame
	void Update () {
		mouseVector = camera.ScreenToWorldPoint(Input.mousePosition);
		mousePositionX = Mathf.RoundToInt(mouseVector.x);
		mousePositionY = Mathf.RoundToInt(mouseVector.y);

		if (Input.GetMouseButtonDown(0) && mousePositionX >= 0 && mousePositionX < gridX && mousePositionY >= 0 && mousePositionY < gridY && mouseActive)
		{
			Debug.Log("Selected (grid[" + mousePositionX +"]["+ mousePositionY+ "]");
			if (Selected.Count == 0)
			{
				Selected.Add(mousePositionX);
				Selected.Add(mousePositionY);
			} else if (Selected[0] == mousePositionX && (Selected[1] == (mousePositionY - 1) || Selected[1] == (mousePositionY + 1))
				|| Selected[1] == mousePositionY && (Selected[0] == (mousePositionX - 1) || Selected[0] == (mousePositionX + 1)))
			{
				mouseActive = false;
				Swap = grid[mousePositionX][mousePositionY];

				grid[mousePositionX][mousePositionY] = grid[Selected[0]][Selected[1]];
				grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().xPos = mousePositionX;
				grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().yPos = mousePositionY;
				grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().swap = true;

				grid[Selected[0]][Selected[1]] = Swap;
				grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController> ().xPos = Selected[0];
				grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController> ().yPos = Selected[1];
				grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController> ().swap = true;

				if (checkMatch())
				{
					Debug.Log("Matches found.");
					StartCoroutine (checkCycle ());
				} else {
					Debug.Log("No matches found.");
					StartCoroutine (revertSwap(Selected[0], Selected[1], mousePositionX, mousePositionY, Swap));
				}

				Swap = null;

				Selected.Clear();
				Selected.TrimExcess();

			} else {
				Selected.Clear();
				Selected.TrimExcess();
			}
		}
			
		if (Input.GetKeyDown (KeyCode.X)) 
		{
			StartCoroutine (spawnPowerups ());
		}

		// Print list of logged powerups
		if (Input.GetKeyDown (KeyCode.Z)) 
		{
			foreach (GameObject i in grid[0])
			{
				if (i != null)
				{
					Debug.Log (i.transform.position + " " + i.tag);
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.C)) 
		{
			checkMatch ();
		}

		if (Input.GetKeyDown (KeyCode.V)) 
		{
			deleteMatches();
		}

		if (Input.GetKeyDown (KeyCode.B)) 
		{
			StartCoroutine (checkCycle ());
		}
			
		mouseText.text = "Mouse position: (" + mousePositionX + ", " + mousePositionY + ")";

		// Make mouseText red when mouse inactive for debugging
		if (mouseActive)
		{
			mouseText.color = Color.white;
		}
		else
		{
			mouseText.color = new Color(1,0,0,1);
		}

	} // end function Update
		
	// Spawn initial playfield
	IEnumerator InitSpawnPowerups ()
	{
		for (int i = 0; i < gridX; i++)
		{

			for (int ii = 0; ii < gridY; ii++)
			{
				GameObject powerup = powerups [Random.Range (0, powerups.Length)];
				Vector3 spawnPosition = new Vector3 (i, ii, zOffset);
				Quaternion spawnRotation = Quaternion.identity;
				GameObject spawned = Instantiate (powerup, spawnPosition, spawnRotation) as GameObject;
				spawned.GetComponent<MatchObjectController> ().xPos = i;
				spawned.GetComponent<MatchObjectController> ().yPos = ii;
				grid[i][ii] = spawned;
			}
		}

		bool run = checkMatch();
		Debug.Log(run);
		while (run)
		{
			Debug.Log("InitLoop");
			deleteMatches();
			yield return new WaitForSeconds (0.05f);

			for (int i = 0; i < gridX; i++)
			{
				bool spawnNew = false;
				for (int ii = 0; ii < gridY; ii++)
				{
					if (!spawnNew && grid[i][ii] == null)
					{
						spawnNew = true;
						for (int iii = ii + 1; iii < gridY; iii++)
						{
							if (grid[i][iii] != null)
							{
								grid[i][ii] = grid[i][iii];
								grid[i][ii].GetComponent<MatchObjectController> ().yPos = ii;
								grid[i][ii].transform.position = new Vector3(i, ii, zOffset);
								grid[i][iii] = null;
								spawnNew = false;
								break;
							}
						} // end for
					}

					if (spawnNew)
					{
						GameObject powerup = powerups [Random.Range (0, powerups.Length)];
						Vector3 spawnPosition = new Vector3 (i, ii, zOffset);
						Quaternion spawnRotation = Quaternion.identity;
						GameObject spawned = Instantiate (powerup, spawnPosition, spawnRotation) as GameObject;
						spawned.GetComponent<MatchObjectController> ().xPos = i;
						spawned.GetComponent<MatchObjectController> ().yPos = ii;
						grid[i][ii] = spawned;
					}
				} // end inner for
			} // end outer for

			run = checkMatch();
		} // end while

		initializing = false;
		mouseActive = true;
	} // end co-routine InitSpawnPowerups

	// Powerup Spawner
	IEnumerator spawnPowerups ()
	{
		for (int i = 0; i < gridX; i++)
		{
			bool spawnNew = false;
			for (int ii = 0; ii < gridY; ii++)
			{
				if (!spawnNew && grid[i][ii] == null)
				{
					spawnNew = true;
					for (int iii = ii + 1; iii < gridY; iii++)
					{
						if (grid[i][iii] != null)
						{
							grid[i][ii] = grid[i][iii];
							grid[i][ii].GetComponent<MatchObjectController> ().yPos = ii;
							grid[i][iii] = null;
							spawnNew = false;
							break;
						}
					}
				}

				if (spawnNew)
				{
					GameObject powerup = powerups [Random.Range (0, powerups.Length)];
					Vector3 spawnPosition = new Vector3 (i, gridY, zOffset);
					Quaternion spawnRotation = Quaternion.identity;
					GameObject spawned = Instantiate (powerup, spawnPosition, spawnRotation) as GameObject;
					spawned.GetComponent<MatchObjectController> ().xPos = i;
					spawned.GetComponent<MatchObjectController> ().yPos = ii;
					grid[i][ii] = spawned;
					yield return new WaitForSeconds (0.025f);
				}
			}
		}
	} // end co-routine spawnPowerups

	// Check for matches
	bool checkMatch()
	{
		bool ret = false;
		// Check for horizontal matches
		for (int i = 0; i < gridY; i++)
		{
			for (int ii = 0; ii < gridX; ii++)
			{
				//Debug.Log("grid["+i+"]["+ii+"]");
				List<GameObject> tempMatch = new List<GameObject>();
				tempMatch.Add (grid[ii][i]);
				int iii;
				for (iii = ii + 1; iii < gridX; iii++)
				{
					if (grid[ii][i].tag == grid[iii][i].tag)
					{
						tempMatch.Add (grid[iii][i]);
					} else {
						break;
					}
				}

				if (tempMatch.Count >= 3)
				{
					ret = true;
					Debug.Log("Horizontal match on object y."+i+" x."+ii+"; counted: "+tempMatch.Count+"; of type: "+grid[ii][i].tag);
					foreach (GameObject x in tempMatch)
					{
						Match.Add(x);
					}
					addScore(tempMatch[0].tag, tempMatch.Count);
				}

				ii = iii - 1;
			}
		}

		// Check for vertical matches
		for (int i = 0; i < gridX; i++)
		{
			for (int ii = 0; ii < gridY; ii++)
			{
				List<GameObject> tempMatch = new List<GameObject>();
				tempMatch.Add (grid[i][ii]);
				int iii;
				for (iii = ii + 1; iii < gridY; iii++)
				{
					if (grid[i][ii].tag == grid[i][iii].tag)
					{
						tempMatch.Add (grid[i][iii]);
					} else {
						break;
					}
				}

				if (tempMatch.Count >= 3)
				{
					ret = true;
					Debug.Log("Vertical match on object x."+i+" y."+ii+"; counted: "+tempMatch.Count+"; of type: "+grid[i][ii].tag);
					foreach (GameObject x in tempMatch)
					{
						Match.Add(x);
					}
					addScore(tempMatch[0].tag, tempMatch.Count);
				}

				ii = iii - 1;
			}
		}
		return ret;
	} // end function checkMatch

	// Delete matches
	void deleteMatches()
	{
		foreach (GameObject i in Match)
		{
			Destroy(i);
		}
		Match.Clear();
		Match.TrimExcess();
	} // end function deleteMatches

	IEnumerator checkCycle ()
	{
		bool run = true;
        yield return new WaitForSeconds (0.2f);
		while (run)
		{
			yield return new WaitForSeconds (0.05f);
			deleteMatches();
			yield return new WaitForSeconds (0.05f);
			yield return StartCoroutine (spawnPowerups ());
			yield return new WaitForSeconds (0.05f);
			Debug.Log("Finsihed checkCycle");
			run = checkMatch();
        }

        if (!mouseDeactive)
        {
            mouseActive = true;
        }
    } // end co-routine checkCycle

	IEnumerator revertSwap(int x, int y, int mousePositionX, int mousePositionY, GameObject Swap)
	{
		yield return new WaitForSeconds (0.25f);
		grid[x][y] = grid[mousePositionX][mousePositionY];
		grid[x][y].GetComponent<MatchObjectController> ().xPos = x;
		grid[x][y].GetComponent<MatchObjectController> ().yPos = y;
		grid[x][y].GetComponent<MatchObjectController> ().swap = true;

		grid[mousePositionX][mousePositionY] = Swap;
		grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().xPos = mousePositionX;
		grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().yPos = mousePositionY;
		grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().swap = true;

        if (!mouseDeactive)
        {
            mouseActive = true;
        }
	} // end co-routine revertSwap

	void addScore(string tag, int count)
	{
		if (!(initializing))
		{

			if (tag == "PowerupFireRate")
			{
				scoreRed += 10 * (count -2);

				if (scoreRed >= targetRed && player != null)
				{
                    player.StartPowerup(1);
					scoreRed -= targetRed;
				}

				redScore.text = "Fire Rate: " + scoreRed + " / " + targetRed;
			}
			else if (tag == "PowerupShield")
			{
				scoreBlue += 10 * (count -2);

				if (scoreBlue >= targetBlue && player != null)
				{
                    player.StartPowerup(2);
					scoreBlue -= targetBlue;
				}

				blueScore.text = "Shield Up: " + scoreBlue + " / " + targetBlue;
			}
			else if (tag == "PowerupFireDouble")
			{
				scoreYellow += 10 * (count -2);

				if (scoreYellow >= targetYellow && player != null)
				{
                    player.StartPowerup(3);
					scoreYellow -= targetYellow;
				}

				yellowScore.text = "Double Fire: " + scoreYellow + " / " + targetYellow;
			}
			else if (tag == "PowerupSpeed")
			{
				scoreGreen += 10 * (count -2);

				if (scoreGreen >= targetGreen && player != null)
				{
                    player.StartPowerup(4);
					scoreGreen -= targetGreen;
				}

				greenScore.text = "Speed Up: " + scoreGreen + " / " + targetGreen;
			}

            // count total score in white
            scoreWhite += 10 * (count - 2);

            if (scoreWhite >= targetWhite && player != null)
            {
                player.StartPowerup(5);
                //scoreWhite -= targetWhite;
                //targetWhite = Mathf.FloorToInt(targetWhite * 2.2f);
                targetWhite = targetWhite + baseTargetWhite + Mathf.FloorToInt(baseTargetWhite * (0.2f * (targetWhite / baseTargetWhite)));
            }

            whiteScore.text = "Turret On: " + scoreWhite + " / " + targetWhite;
		}
	} // end function addScore

    public void DeactivateMouse()
    {
        mouseDeactive = true;
        mouseActive = false;

        banner.SetActive(true);
    }

    public void ReactivateMouse()
    {
        mouseDeactive = false;
        mouseActive = true;

        banner.SetActive(false);
    }
} // end class SecondaryController
