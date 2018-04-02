using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeText : MonoBehaviour {
    PlayerHandController playerHandController;
    Text[] texts;
        //title is 0
        //value is 1
    private int charge;
    private int maxCharge;

	void Awake()
    {
        if (playerHandController == null)
        {
            playerHandController = GameObject.FindGameObjectWithTag("Hand").GetComponent<PlayerHandController>();
        }

        if(texts == null)
        {
            texts = GetComponentsInChildren<Text>();
        }
    }

    void Start ()
    {
        texts[0].canvasRenderer.SetAlpha(0.0f);
        texts[1].canvasRenderer.SetAlpha(0.0f);
        //UpdateCount(3);
        StartCoroutine(Birth());
    }

    IEnumerator Birth()
    {
        yield return new WaitForSeconds(2);
        foreach (Text text in texts)
        {
            text.enabled = true;
            text.CrossFadeAlpha(1.0f, 1.0f, false);
        }

    }

    public void UpdateCount(int current, int max)
    {
        //if(current > max)
        //{
        //    maxCharge = current;
        //}
        charge = playerHandController.CardCharge;
        texts[1].text = current + "/" + max;
    }

}
