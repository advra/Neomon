using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterNameText : MonoBehaviour {
    public GameObject referenceMonster;
    MonsterController monsterController;
    CameraBehavior cameraBehavior;
    Text text;
    string monsterName;

    void Start () {
        if (referenceMonster.activeInHierarchy == true)
        {
            text = GetComponent<Text>();
            monsterController = referenceMonster.GetComponent<MonsterController>();
            cameraBehavior = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraBehavior>();
            text.text = "";
        }
        else
        {
            Debug.Log("Disabled!");
            this.gameObject.SetActive(false);
        }
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        // fade from transparent to opaque
        for (float i = 0; i <= 2; i += Time.deltaTime)
        {
            text.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update () {
        if(cameraBehavior.cameraIntroIsDone == true)
        {
            //do fade in
            monsterName = monsterController.monster.Name;
            text.text = monsterName;
            FadeIn();
        }
    }


}
