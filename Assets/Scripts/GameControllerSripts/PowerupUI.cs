using UnityEngine;
using System.Collections;

public class PowerupUI : MonoBehaviour {

    public GUIText[] activeTexts;
    public AudioSource activateSound;
    public AudioSource deactivateSound;

    // Use this for initialization
    void Start () {
	    foreach (GUIText text in activeTexts)
        {
            text.text = "";
        }
	}
	
	// Update is called once per frame
	//void Update () {
	
	//}

    public void Active(int index)
    {
        activeTexts[index - 1].text = "Active";
        activateSound.Play();
    }

    public void Deactive(int index)
    {
        activeTexts[index - 1].text = "";
        deactivateSound.Play();
    }
}
