using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthText : MonoBehaviour {
    public GameObject referenceMonster;
    MonsterController MC;
    PlayerController PC;
    CameraBehavior cameraBehavior;
    public RectTransform canvasRect;
    Vector2 canvasPos;
    Text text;
    public bool isPlayer;
    public int health;
    public int maxHealth;

    // Use this for initialization
    void Start () {  
        if (referenceMonster.activeInHierarchy == true)
        {
            text = GetComponent<Text>();
            if (isPlayer)
            {
                PC = referenceMonster.GetComponent<PlayerController>();
            }
            else
            {
                MC = referenceMonster.GetComponent<MonsterController>();
            }

            cameraBehavior = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraBehavior>();
            text.text = "";

            canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
            //get reference relative position of the monster
            Vector2 screenPointToTarget = Camera.main.WorldToScreenPoint(referenceMonster.transform.position);
            //Convert screen position to Canvas / RectTransform space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPointToTarget, null, out canvasPos);
            //move text to Game object's position
            GetComponent<RectTransform>().anchoredPosition = new Vector2(canvasPos.x, canvasPos.y + 100);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    public void FadeOut()
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

    private IEnumerator FadeOutRoutine()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            text.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update () {
        if (cameraBehavior.cameraIntroIsDone == true)
        {
            if (PC)
            {
                if (PC.monster.IsDead)
                {
                    text.text = "HP: 0/" + maxHealth;
                    FadeOut();
                }
                else
                {
                    health = PC.currentHealth;
                    maxHealth = PC.maxHealth;
                    text.text = "HP: " + health + " / " + maxHealth;
                    FadeIn();
                }
            }
            else
            {
                if (MC.monster.IsDead)
                {
                    text.text = "HP: 0/" + maxHealth;
                    FadeOut();
                }
                else
                {
                    health = MC.currentHealth;
                    maxHealth = MC.maxHealth;
                    text.text = "HP: " + health + " / " + maxHealth;
                    FadeIn();
                }
            }
        }

    }
}
