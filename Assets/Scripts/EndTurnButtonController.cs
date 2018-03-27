using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonController : MonoBehaviour {

    Image buttonImage;

	// Use this for initialization
	void Start () {
        buttonImage = GetComponent<Image>();
    }
	
	// Update is called once per frame
	public void ShowButton (bool value) {
        if (value)
        {
            buttonImage.enabled = true;
        }
        else
        {
            buttonImage.enabled = false;
        }
	}

    public void TestButtn()
    {

        buttonImage.enabled = true;
    }
}
