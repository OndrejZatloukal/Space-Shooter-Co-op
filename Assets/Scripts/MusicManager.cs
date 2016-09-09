using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour 
{

	public AudioClip[] levelMusicChangeArray;

	private AudioSource audioSource;


	void Awake () 
	{
		DontDestroyOnLoad (gameObject);
		Debug.Log ("Don't destroy on load " + name);
	}

	// Use this for initialization
	void Start () 
	{
		audioSource = GetComponent < AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnLevelWasLoaded (int level)
	{
        if (level < 2)
        {
            AudioClip thisLevelMusic = levelMusicChangeArray[level];
            Debug.Log("Playing clip: " + thisLevelMusic);

            if (thisLevelMusic)
            {
                audioSource.clip = thisLevelMusic;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
	}

	public void SetVolume (float volume)
	{
		audioSource.volume = volume;
	}
}
