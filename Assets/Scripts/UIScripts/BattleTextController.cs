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

    Vector3 startPos;
    Vector3 endPos;
    private float fraction = 0;

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
        startPos = currentPos;
        endPos = new Vector2(startPos.x, startPos.y + 250);
        fontScale = text.fontSize;
        text.CrossFadeAlpha(0.0f, 1.0f, false);
        StartCoroutine(SplatText());
        StartCoroutine(TimedDeath());
    }

    void Update()
    {
        FloatText(1.0f);
    }

    void FloatText()
    {
        currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, currentPos.y + 5);
    }

    void FloatText(float speed)
    {
        if (fraction < 1)
        {
            fraction += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, endPos, fraction);
        }
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
