using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeTickController : MonoBehaviour {
    MonsterController monsterController;

    [SerializeField]
    private GameObject trackedMonster;
    [SerializeField]
    private float windowMinBound;
    [SerializeField]
    private float windowMaxBound;
    private float differenceBound;
    RectTransform rectTransform;
    public float percentSpeed;
    [SerializeField]
    public float speed;
    [SerializeField]
    private float speedBase;
    [SerializeField]
    private float positionY;
    [SerializeField]
    private float positionX;

    /// <summary>
    /// Instantiate Tick grahpic used to tracking when enemies are ready to attack
    /// </summary>
    /// <param name="monster">The monster that this GUI will track</param>
    public GaugeTickController(GameObject monster)
    {
        //if we change graphic of window we can set this manually
        this.windowMinBound = -250.0f;
        this.windowMaxBound = 250.0f;
    }

    public GameObject TrackedMonster
    {
        set { trackedMonster = value; }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = value;  }
    }

    public float SpeedBase
    {
        get { return speedBase; }
        set { speedBase = value; }
    }

    public float GetPercentage()
    {
        return speed / differenceBound * 100;
    }

    void Awake ()
    {
        GameObject tickWindow = GameObject.FindGameObjectWithTag("TickWindow");
        transform.SetParent(tickWindow.transform);
        rectTransform = GetComponent<RectTransform>();
        transform.localPosition = new Vector2 (windowMinBound, 0);
    }

    // Use this for initialization
    void Start()
    {
        differenceBound = windowMaxBound - windowMinBound;
        positionY = transform.localPosition.y;
        monsterController = trackedMonster.GetComponent<MonsterController>();
        if (monsterController == null)
        {
            Debug.Log("monsterController for Tick GUI is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseTicks();
        percentSpeed = GetPercentage();
    }

    void IncreaseTicks()
    {
        positionX = transform.localPosition.x;

        if (transform.localPosition.x >= 250)
        {
            transform.localPosition = new Vector2(-250.0f, positionY);
        //if within Charge meter, then go faster
        }else if (transform.localPosition.x < 250.0f && transform.localPosition.x > 100.0f)
        {
            rectTransform.localPosition = new Vector2(positionX + (speedBase * 2.00f), positionY);
        }
        else
        {
            rectTransform.localPosition = new Vector2(positionX + speedBase, positionY);
        }
    }
}
