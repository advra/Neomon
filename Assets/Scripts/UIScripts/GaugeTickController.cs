using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeTickController : MonoBehaviour {
    BattleController BC;
    MonsterController monsterController;
    PlayerController playerController;
    EnemyStateMachine ESM;
    public bool isPlayer;
    public GaugeState currentState;
    public float tickThreshold = 100;
    public float newPercentage;
    public float zeroBound = 105.0f;    //x position where marker hits middle of wait and charge areas

    public enum GaugeState
    {
        INCREASING,
        STOP,
        CHARGE,
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
    private float differenceBound;
    private float rangeWaitBound;
    private float rangeChargeBound;
    RectTransform rectTransform;
    //private float speedBase;
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

    void Awake ()
    {
        BC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        ESM = GetComponent<EnemyStateMachine>();
        currentState = GaugeState.INCREASING;
        
        
        GameObject tickWindow = GameObject.FindGameObjectWithTag("TickWindow");
        transform.SetParent(tickWindow.transform);
        rectTransform = GetComponent<RectTransform>();
        transform.localPosition = new Vector2 (windowMinBound, 0);
    }

    // Use this for initialization
    void Start()
    {
        differenceBound = windowMaxBound - windowMinBound;
        rangeWaitBound = 100.0f - windowMinBound;

        positionY = transform.localPosition.y;
        if (isPlayer)
        {
            playerController = trackedMonster.GetComponent<PlayerController>();
        }
        else
        {
            monsterController = trackedMonster.GetComponent<MonsterController>();
        }
        if (monsterController == null)
        {
            Debug.Log("monsterController for Tick GUI is null");
        }
        if (playerController == null)
        {
            Debug.Log("playerController for Tick GUI is null");
        }

        //must be called otherwise will not have an initial state to begin with
        //currentState = GaugeState.INCREASING; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Compute();
        if (!done)
        {
            switch (currentState)
            {
                // auto increment
                case (GaugeState.INCREASING):
                    Compute();
                    //IncreaseTicks();
                    //CheckTicks();
                    break;
                //stop tick once speed is at 100% 
                case (GaugeState.STOP):
                    done = true;
                    ReadyUp();
                    break;
                case (GaugeState.CHARGE):

                    break;
                //once we hit max point reset 
                case (GaugeState.RESET):

                    break;
            }
        }
    }

    void Compute()
    {
        //get percentage total assuming out threshold is 100
        newPercentage = (playerController.currentSpeed / BC.threshold) * 100f ;
        //if we are not ready (at 100%) then keep incrementing position of tick
        if(newPercentage <= 100.0f)
        {
            //apply percentage in relation to the area of interest (our wait area)
            positionX = windowMinBound + newPercentage * (rangeWaitBound / 100.0f);
            transform.localPosition = new Vector2(positionX, positionY);
        }
    }



    public void ReadyUp()
    {
        if (trackedMonster == BC.player)
        {
            playerController.currentUserState = PlayerController.PlayerState.READY;
            currentState = GaugeState.INCREASING;
        }
    }

    public void IncreaseTicks()
    {
        positionX = transform.localPosition.x;
    }

}
