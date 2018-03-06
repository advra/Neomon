using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTextController : MonoBehaviour
{


    public Text text;
    RectTransform rectTransform;
    public float travelDistance = 50;
    [SerializeField]
    private Vector3 currentPos;
    private int fontScale;

    // Use this for initialization
    void Awake()
    {
        text = GetComponent<Text>();
        if(text == null)
        {
            Debug.Log("Warning: " + this.gameObject + " text is empty.");
        }
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.Log("Warning: " + this.gameObject + " has no rect transform, text animation will not play properly");
        }
    }

    void Start()
    {
        currentPos = transform.position;
        fontScale = text.fontSize;
        text.CrossFadeAlpha(0.0f, 1.0f, false);
        StartCoroutine(SplatText());
        StartCoroutine(TimedDeath());
    }

    void Update()
    {
        FloatText();
    }

    void FloatText()
    {
        currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, currentPos.y + 5);
    }

    IEnumerator SplatText()
    {
        for(int i = fontScale; i>20; i--)
        {
            text.fontSize = i;
            yield return null;
        }
    }

    IEnumerator TimedDeath()
    {
        yield return new WaitForSeconds(3);
        GameObject.Destroy(this.gameObject);
    }
}
