using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultText : MonoBehaviour {
    Text[] text;

	// Use this for initialization
	void Awake () {
        //get component parent (0) and its children since GetComponentsInChildren also returns parent
        //with same component
        text = GetComponentsInChildren<Text>();
    }
	
    public void SetText(string mainText, string descriptionText, string buttonText)
    {
        text[0].text = mainText;
        text[1].text = descriptionText;
        text[2].text = buttonText;
    }
}
