using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]

public class MonsterController : MonoBehaviour
{

    UserController userController;
    public BattleController BC;
    public Monster monster;

    public State monsterState;
    public GameObject trackingTickObject;

    public enum State
    {
        WAITING,    //waiting until turn
        READY,      //when monster is ready / drawing cards
        SELECTING,  //is ready now player chooses action
        CHARGING,   //wait time delay needed to perform action
        PERFORMING,
    }

    //used to quickly identify in the queuer the type of monster 
    public enum Type
    {
        PLAYER,
        ENEMY
    }

    [SerializeField]
    public List<Attack> moveSet;
    public bool done;
    public bool pause;
    bool targeted;
    public bool isDead;
    public bool isCharingToAttack;
    public new string name;
    public string description;
    public string spriteFile;
    public int currentHealth, maxHealth, attack, defense, level;
    public int baseAttack, baseDefense;
    public float currentSpeed, baseSpeed;
    public float chargeTimer, chargeDuration;
    public int damage;
    private RuntimeAnimatorController animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    //use this to check if targeted then change color
    Color lerpedColor;

    //used for the info panel
    GameObject panelInfoObject;
    Vector2 canvasPos;
    Vector2 screenPointToTarget;
    Text[] panelInfoText;

    //at the end of every attack after executed (or canceled call this)
    public void ResetAttack()
    {
        trackingTickObject.GetComponent<EnemyTickController>().state = EnemyTickController.GaugeState.RESET;
        chargeTimer = 0.0f;
        currentSpeed = 0.0f;
        isCharingToAttack = false;
        monsterState = State.WAITING;
        done = false;
    }

    public bool IsTargeted
    {
        set { targeted = value; }
    }

    public float CurrentSpeed
    {
        get { return currentSpeed; }
        set { currentSpeed = value;  }
    }

    void ChangeState(State state)
    {
        monsterState = state;
        done = false;
    }

    public void StartTest()
    {
        StartCoroutine(FlashInput());
    }

    public void Damage(int amount)
    {
        this.currentHealth -= amount;
        //Play damage sprite here
        if (this.currentHealth < 0)
        {
            this.currentHealth = 0;
            this.isDead = true;
        }

        if (isDead)
        {
            //run death animation here
            //Design decision: either fade out and disable now, or allow them to be revived?
        }
    }

    public void Heal(int amount)
    {
        this.currentHealth += amount;
        if (this.currentHealth < 0)
        {
            this.currentHealth = 0;
            this.isDead = true;
        }
    }

    void CheckAttack()
    {
        if(currentSpeed > BC.Threshold)
        {
            BC.IsPaused = true;
            Debug.Log(this.gameObject + " Speed at:" + currentSpeed);
        }
    }

    public void ResetTurn(int threshold)
    {
        currentSpeed -= threshold;
    }

    //randomly select an attack in their move list
    void SelectAttack()
    {
        int random = Random.Range(0, moveSet.Count);
        damage = moveSet[random].damage;
        chargeDuration = moveSet[random].chargeTime;
        List<GameObject> target = new List<GameObject>();
        target.Add(BC.player);
        HandleTurn turn = new HandleTurn(this.gameObject, target, moveSet[random].attackType, moveSet[random].damage, chargeDuration);
        BC.AddTurnToQueue(turn);
        //Begin charging
        EnemyTickController enemyTickController = trackingTickObject.GetComponent<EnemyTickController>();
        enemyTickController.ChangeState(EnemyTickController.GaugeState.CHARGING);
        monsterState = State.CHARGING;
    }

    void SpawnEnemy()
    {
        //for now we will just a random selection within our database
        //We can expand on this later
        int randomIndex = Random.Range(0, MonsterInfoDatabase.monsters.Count);   //returns 0 - 1
        monster = MonsterInfoDatabase.monsters[randomIndex];
        name = monster.MonsterBase.name;
        description = monster.MonsterBase.description;
        spriteFile = monster.MonsterBase.spriteFile;
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/mon/mon_" + spriteFile);
        trackingTickObject.GetComponent<EnemyTickController>().SetTickIcon(spriteFile);
        //We add box collider later because it inherits the sprites dimensions otherwise 
        //the box collider would not generate the appropriate size for the monster
        gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D == null)
        {
            Debug.Log(this.gameObject + " does not have a BoxCollider2d");
        }
        currentHealth = monster.Health;
        maxHealth = monster.MaxHealth;
        baseSpeed = monster.Speed;
        BC.ThresholdUpdate(baseSpeed);
        moveSet = monster.MoveSet;
        Debug.Log(monster.Print);
    }

    void IncreaseSpeed()
    {
        //this will prevent speed increasing when still user's turn
        if (pause)
        {
            return;
        }

        if (currentSpeed >= BC.Threshold)
        {
            monsterState = State.READY;
        }
        else
        {
            currentSpeed += baseSpeed * Time.deltaTime;
        }
    }

    void ChargeSpeed(float durationInSeconds)
    {
        //this will prevent speed increasing when still user's turn
        if (pause)
        {
            return;
        }

        if (chargeTimer >= durationInSeconds) //change back to BC.Threshold for testing
        {
            //this will ensure it will not reset without trigger if it is not their turn
            if (BC.turnList[0].owner == this.gameObject)
            {
                BC.ExecuteTurnFor(this.gameObject);
                ResetAttack();
                monsterState = State.WAITING;
            }
        }
        else
        {
            isCharingToAttack = true;
            chargeTimer += Time.deltaTime;
        }
    }

    void OnMouseEnter()
    {
        //Material mat = GetComponent<Renderer>().material;
        //mat.color = Color.red;
        UpdateInfo();
    }

    void OnMouseExit()
    {
        RemoveInfo();
    }

    void RemoveInfo()
    {
        //move it out of camera frame
        panelInfoObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, 300);
    }

    void UpdateInfo()
    {
        if (!BC.isBattling)
        {
            return;
        }

        //obtain panel gameobject
        if (panelInfoObject == null)
        {
            panelInfoObject = GameObject.FindGameObjectWithTag("InfoPanel");
        }
        //obtain panel text
        if (panelInfoText == null)
        {
            panelInfoText = panelInfoObject.GetComponentsInChildren<Text>();
        }
        //set text
        panelInfoText[0].text = this.name;
        panelInfoText[1].text = this.description;
        //capture screen Pos in 2d space from 3d space
        screenPointToTarget = Camera.main.WorldToScreenPoint(this.transform.position);
        // Convert screen position to Canvas / RectTransform space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(BC.canvasRect, screenPointToTarget, null, out canvasPos);
        //move text to Game object's position
        panelInfoObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(canvasPos.x - 100, canvasPos.y);
    }

    IEnumerator FlashInput()
    {
        for (float f = 1f; f >= 0.0f; f -= 0.1f)
        {
            spriteRenderer.color = new Color(1, f, f, 1);
            //yield return null;
        }
        for (float f = 0.0f; f <= 1.0f; f += 0.1f)
        {
            spriteRenderer.color = new Color(1, f, f, 1);
            //yield return null;
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator SimulatePoison()
    {
        monster.ChangeHealth(-1);
        yield return new WaitForSeconds(1f);
    }

    void Awake()
    {
        BC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        animator = GetComponent<RuntimeAnimatorController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
        {
            Debug.Log(this.gameObject + " monster animator is null");
        }

        if (spriteRenderer == null)
        {
            Debug.Log(this.gameObject + " monster sprite renderer is null");
        }

        if (!MonsterInfoDatabase.IsPopulated)
        {
            MonsterInfoDatabase.Populate();
        }

    }

    void Start ()
    {
        //get reference to UserController to check if mouse is on top of this monster
        userController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UserController>();

        SpawnEnemy();
    }

    void Update ()
    {
        switch (monsterState)
        {
            case (State.WAITING):
                IncreaseSpeed();
                break;
            case (State.READY):
                monsterState = State.SELECTING;
                break;
            case (State.SELECTING):
                SelectAttack();
                break;
            case (State.CHARGING):
                ChargeSpeed(chargeDuration);
                break;
            default:
                break;
        }

        ////simulate poison (for testing the hp)
        //if (BC.IsBattling & !BC.IsPaused)
        //{
        //    CheckAttack();

        //    //Used for testing purposes
        //    //StartCoroutine(SimulatePoison());

        //    //Need to revisit. Works as intended but mouse hover = multiple calls which conflict the animation.
        //    if ((targeted))
        //    {
        //        lerpedColor = Color.Lerp(Color.white, new Color(1.0f, 0.0f, 0.0f, 1.0f), Mathf.PingPong(Time.time, 0.5f));
        //        spriteRenderer.color = lerpedColor;
        //    }
        //}

    }
}
