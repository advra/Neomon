using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelController : MonoBehaviour {

    private Vector2 originalPosition;

    public void ResetPosition()
    {
        this.transform.position = originalPosition;
    }

	// Use this for initialization
	void Start () {
        originalPosition = transform.position;
    }
	
}
