using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRedHealthManager : MonoBehaviour {

    public GameObject greenHealthBar;

    [SerializeField]
    private bool greenCoolingDown = false;
    [SerializeField]
    private float percentage;
    [SerializeField]
    private float percentageTarget;

    // Use this for initialization
    void Start () {
        if(greenHealthBar == null)
        {
            Debug.Log("Green HP Game Object is null");
        }

        //percentageTarget = greenHealthBar.


    }
	
	// Update is called once per frame
	void Update () {

        //update CoolingDown
        //if true then update the red bar

    }
}
