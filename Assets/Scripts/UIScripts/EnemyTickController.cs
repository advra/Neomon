using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;

public class EnemyTickController : MonoBehaviour
{
    BattleController BC;
    MonsterController monsterController;
    public GaugeState state;
    public GameObject TickIcon;
    public string spriteIcon;
    public float tickThreshold;
    [SerializeField]
    private float idlePercentage;
    [SerializeField]
    private float chargePercentage;
    public float zeroBound;             //x position where marker hits middle of wait and charge areas
    public Image[] images;

    public enum GaugeState
    {
        INCREASING,
        STOP,
        CHARGING,
        RESET
    }

    [SerializeField]
    bool done;
    bool init;
    private float range;

    [SerializeField]
    private GameObject trackedMonster;
    [SerializeField]
    private float windowMinBound;
    [SerializeField]
    private float windowMaxBound;
    [SerializeField]
    private float differenceBound;
    [SerializeField]
    private float rangeWaitBound;
    [SerializeField]
    private float rangeChargeBound;
    RectTransform rectTransform;
    //private float speedBase;
    [SerializeField]
    private float positionY;
    [SerializeField]
    private float positionX;

    public EnemyTickController(GameObject monster)
    {
        //if we change graphic of window we can set this manually
        this.windowMinBound = -250.0f;
        this.windowMaxBound = 250.0f;
        this.zeroBound = 105.0f;
        this.tickThreshold = 100f;
    }

    public GameObject TrackedMonster
    {
        set { trackedMonster = value; }
    }

    public void Reset()
    {
        done = true;
        idlePercentage = 0;
        chargePercentage = 0;
        ChangeState(GaugeState.INCREASING);
    }

    public void ReadyUp()
    {
        monsterController.monsterState = MonsterController.State.READY;
        //state = GaugeState.INCREASING;
    }

    public void ChangeState(GaugeState state)
    {
        done = false;
        this.state = state;
    }

    public void ComputeCharge()
    {
        chargePercentage = (monsterController.chargeTimer / monsterController.chargeDuration) * 100f;
        //update position over time
        if (chargePercentage <= 100.0f)
        {
            float perIncrement = rangeChargeBound / 100.0f;
            positionX = 100.0f + chargePercentage * perIncrement;
            transform.localPosition = new Vector2(positionX, positionY);
        }
        //if value is over then execute
        else
        {
            //insert pause here in the future to allow for things like animation etc.
            //execute event in queue
            if (BC.turnList[0].owner == trackedMonster)
            {
                //BC.ExecuteTurnFor(trackedMonster);
                //done = true;
                ChangeState(GaugeState.RESET);
            }
            //idlePercentage = 0;
            //chargePercentage = 0;
            //ChangeState(GaugeState.INCREASING);
        }
    }

    void ComputeIdle()
    {
        //get percentage total assuming our threshold is 100
        idlePercentage = (monsterController.currentSpeed / BC.threshold) * 100f;
        //if we are not ready (at 100%) then keep incrementing position of tick
        if (idlePercentage <= 100.0f)
        {
            //apply percentage in relation to the area of interest (our wait area from -250 to 250)
            positionX = windowMinBound + idlePercentage * (rangeWaitBound / 100.0f);
            transform.localPosition = new Vector2(positionX, positionY);
        }
        else
        {
            state = GaugeState.STOP;
            return;
        }
    }

    void Awake()
    {
        BC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        state = GaugeState.INCREASING;
        GameObject tickWindow = GameObject.FindGameObjectWithTag("TickWindow");
        transform.SetParent(tickWindow.transform);
        rectTransform = GetComponent<RectTransform>();
        transform.localPosition = new Vector2(windowMinBound, 0);
    }

    // Use this for initialization
    void Start()
    {
        if (monsterController == null)
        {
            monsterController = trackedMonster.GetComponent<MonsterController>();
        }
            
        differenceBound = windowMaxBound - windowMinBound;
        rangeWaitBound = 100.0f - windowMinBound;
        rangeChargeBound = windowMaxBound - 100.0f;
        positionY = transform.localPosition.y;
        //SetTickIcon();
    }

    public void SetTickIcon(string s)
    {
        //this.spriteIcon = s;
        //GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/bar/tick_" + s);
        GetComponentInChildren<LoadTickIcon>().Load(s);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            // auto increment
            case (GaugeState.INCREASING):
                ComputeIdle();
                break;
            //stop tick once speed is at 100% 
            case (GaugeState.STOP):
                done = true;
                ReadyUp();
                break;
            case (GaugeState.CHARGING):
                ComputeCharge();
                break;
            //once we hit max point reset 
            case (GaugeState.RESET):
                Reset();
                break;
        }

    }

}
