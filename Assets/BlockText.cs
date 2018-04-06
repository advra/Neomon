using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockText : MonoBehaviour
{
    public GameObject referenceMonster;
    private MonsterController monsterController;
    CameraBehavior cameraBehavior;
    public RectTransform canvasRect;
    Vector2 canvasPos;
    Text text;
    private int block;

    // Use this for initialization
    void Start()
    {
        if (referenceMonster.activeInHierarchy == true)
        {
            text = GetComponent<Text>();
            monsterController = referenceMonster.GetComponent<MonsterController>();
            monsterController.SetBlockUIText = this.gameObject;
            cameraBehavior = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraBehavior>();
            text.text = "";

            canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
            //get reference relative position of the monster
            Vector2 screenPointToTarget = Camera.main.WorldToScreenPoint(referenceMonster.transform.position);
            //Convert screen position to Canvas / RectTransform space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPointToTarget, null, out canvasPos);
            //move text to Game object's position
            GetComponent<RectTransform>().anchoredPosition = new Vector2(canvasPos.x - 50, canvasPos.y + 100);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator DeathFade()
    {
        for (float f = 1f; f > 0f; f -= 0.05f)
        {
            text.color = new Color(1, 1, 1, f);
            yield return null;
        }
    }

    IEnumerator FadeInRoutine()
    {
        // fade from transparent to opaque
        for (float i = 0; i <= 2; i += Time.deltaTime)
        {
            text.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraBehavior.cameraIntroIsDone == true)
        {
            if (monsterController.monsterState == MonsterController.State.DEAD)
            {
                StartCoroutine(DeathFade());
            }

            if (monsterController.monster.IsDead)
            {
                text.text = "";
            }
            else
            {
                block = monsterController.block;
                if(block > 0)
                {
                    text.text = block.ToString();
                }
                else
                {
                    text.text = "";
                }
                
                StartCoroutine(FadeInRoutine());
            }
        }

    }
}

