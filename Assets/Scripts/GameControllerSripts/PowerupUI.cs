using UnityEngine;
using System.Collections;

public class PowerupUI : MonoBehaviour {

    public GUIText[] activeTexts;

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
    }

    public void Deactive(int index)
    {
        activeTexts[index - 1].text = "";
    }
}
