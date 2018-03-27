using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeText : MonoBehaviour {
    PlayerHandController playerHandController;
    Text text;
    private int charge;
    private int maxCharge;

	void Awake()
    {
        if (playerHandController == null)
        {
            playerHandController = GameObject.FindGameObjectWithTag("Hand").GetComponent<PlayerHandController>();
        }

        if(text == null)
        {
            text = GetComponent<Text>();
        }
    }

    void Start ()
    {
        //UpdateCount(3);
    }

    public void UpdateCount(int current, int max)
    {
        //if(current > max)
        //{
        //    maxCharge = current;
        //}
        charge = playerHandController.CardCharge;
        text.text = current + "/" + max;
    }

}
