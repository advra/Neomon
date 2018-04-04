using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckViewBackButton : MonoBehaviour {

    public DeckViewController deckViewController;

	// Use this for initialization
	void Start () {
        //no longer needed as this is referenced once we create it in DeckViewController class
        //deckViewController = GameObject.FindObjectOfType<Button>().GetComponent<DeckViewController>();
    }

    public void BackButton()
    {
        deckViewController.DeckWindowActive = false;
    }
	
}
