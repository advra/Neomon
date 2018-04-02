using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStartText : MonoBehaviour {

    private Text text;

	// Use this for initialization
	void Start () {
        StartCoroutine(TimedBirth());
        text = this.gameObject.GetComponent<Text>();
        text.enabled = false;
    }

    IEnumerator TimedBirth()
    {
        yield return new WaitForSeconds(2);
        text.canvasRenderer.SetAlpha(0.0f);
        text.enabled = true;
        text.CrossFadeAlpha(1.0f, 1.0f, false);
        yield return new WaitForSeconds(1);
        text.CrossFadeAlpha(0.0f, 1.0f, false);
        BattleController battlecontroller = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        battlecontroller.BattleState = BattleController.State.RUN;
        yield return new WaitForSeconds(2);
        GameObject.Destroy(this.gameObject);
    }
}
