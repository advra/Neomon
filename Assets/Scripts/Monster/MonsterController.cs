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
    public PlayerController playerController;
    public BattleController BC;
    public PlayerHandController playerHand;

    public Monster monster;

    public State monsterState;
    public GameObject trackingTickObject;
    public PlayerTickController playerTickController;
    public Canvas tickCanvas;
    public Image[] tickImages;
    public Material monsterMat;

    public enum State
    {
        WAITING,    //waiting until turn
        READY,      //when monster is ready / drawing cards
        SELECTING,  //is ready now player chooses action
        CHARGING,   //wait time delay needed to perform action
        PERFORMING,
        DEAD,
        PAUSED
    }

    //used to quickly identify in the queuer the type of monster 
    public enum Team
    {
        UNASSIGNED,
        ENEMY,
        PLAYER
    }

    [SerializeField]
    public List<Attack> moveSet;
    public bool done;
    public bool pause;
    bool targeted;
    public bool isDead;
    public bool isChargingToAttack;
    public Team team;
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
        if (team == Team.ENEMY)
        {
            trackingTickObject.GetComponent<EnemyTickController>().state = EnemyTickController.GaugeState.RESET;
        }
        if (team == Team.PLAYER)
        {
            if (playerTickController == null)
            {
                playerTickController = GameObject.FindGameObjectWithTag("PlayerTick").GetComponent<PlayerTickController>();
            }
            playerTickController.ChangeState(PlayerTickController.GaugeState.INCREASING);
            //playerController.currentUserState = PlayerController.PlayerState.WAITING;
            BC.HideCombatUI();
        }
        chargeTimer = 0.0f;
        currentSpeed = 0.0f;
        isChargingToAttack = false;
        monsterState = State.WAITING;
        done = false;
    }

    public float CurrentSpeed
    {
        get { return currentSpeed; }
        set { currentSpeed = value; }
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

    public void IsTargeted(bool b)
    {
        if (tickCanvas == null)
        {
            tickCanvas = trackingTickObject.GetComponent<Canvas>();
        }

        if (b)
        {
            monsterMat = GetComponent<Renderer>().material;
            monsterMat.color = new Color(1, 0.5f, 0.5f, 1);

            tickImages = trackingTickObject.GetComponentsInChildren<Image>();
            tickImages[0].color = Color.red;
            tickImages[1].color = new Color(1, 0.5f, 0.5f, 1);
            tickCanvas.overrideSorting = true;
            tickCanvas.sortingOrder = 3;
        }
        else
        {
            Material mat = GetComponent<Renderer>().material;
            mat.color = Color.white;

            tickImages = trackingTickObject.GetComponentsInChildren<Image>();
            tickImages[0].color = Color.white;
            tickImages[1].color = Color.white;

            tickCanvas.overrideSorting = false;
        }

    }

    public void Damage(int amount)
    {
        this.currentHealth -= amount;
        //Play damage sprite here
        if (this.currentHealth <= 0)
        {
            this.currentHealth = 0;
            this.isDead = true;
        }

        if (isDead)
        {
            monsterState = State.DEAD;

            //run death animation here
            //Design decision: either fade out and disable now, or allow them to be revived?

            if (team == Team.PLAYER)
            {
                //playerController.currentUserState = PlayerController.PlayerState.DEAD;
                //pause all enemies
                BC.PauseMonsters();
                //display losing text
                BC.PlayerLose();
            }

            if (team == Team.ENEMY)
            {
                if (BC.AllEnemiesDead())
                {
                    BC.PlayerWin();
                }
            }

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

    void DoDeath()
    {
        //Hide Tick
        trackingTickObject.SetActive(false);
        //plays fade animation
        StartCoroutine(DeathFade());
    }

    void CheckAttack()
    {
        if (currentSpeed > BC.Threshold)
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
        BC.PauseSpeedsForAllMonsters(true);
        //randomly select attack from attack set for enemies
        if (team == Team.ENEMY)
        {
            int random = Random.Range(0, moveSet.Count);
            damage = moveSet[random].damage;
            chargeDuration = moveSet[random].chargeTime;
            bool isCanceling = moveSet[random].isCanceling;
            List<GameObject> target = new List<GameObject>();
            target.Add(BC.player);
            HandleTurn turn = new HandleTurn(this.gameObject, target, moveSet[random].attackType, moveSet[random].damage, chargeDuration, isCanceling);
            BC.AddTurnToQueue(turn);
            //Begin charging
            EnemyTickController enemyTickController = trackingTickObject.GetComponent<EnemyTickController>();
            enemyTickController.ChangeState(EnemyTickController.GaugeState.CHARGING);
            monsterState = State.CHARGING;
            BC.PauseSpeedsForAllMonsters(false);
        }
        //wait until card controller changes users state from here
        if (team == Team.PLAYER)
        {
            //Necessary to prevent monsters from increasing their speeds when its players turn
            BC.PauseSpeedsForAllMonsters(true);
            //Debug.Log("Waiting for user to select cards");
        }
    }


    void SpawnEnemy()
    {
        //will replace this with scriptable objects?
        if (!MonsterInfoDatabase.IsPopulated)
        {
            MonsterInfoDatabase.Populate();
        }

        if (team == Team.PLAYER)
        {
            //Set to squidra for now until we expand on graphics
            monster = MonsterInfoDatabase.monsters[0];
            //Set tick icon concatenate "f" for friendly
            spriteFile = monster.MonsterBase.spriteFile + "_f";
            trackingTickObject.GetComponent<PlayerTickController>().SetTickIcon(spriteFile);
        }
        else
        {
            //for now we will just a random selection within our database
            //We can expand on this later
            int randomIndex = Random.Range(0, MonsterInfoDatabase.monsters.Count);   //returns 0 - 1
            //monster = MonsterInfoDatabase.monsters[randomIndex];
            monster = MonsterInfoDatabase.monsters[1];
            //get moves from the "deck" of the enemy
            moveSet = monster.MoveSet;
            //Set tick icon
            spriteFile = monster.MonsterBase.spriteFile;
            trackingTickObject.GetComponent<EnemyTickController>().SetTickIcon(spriteFile);
        }
        name = monster.MonsterBase.name;
        description = monster.MonsterBase.description;
        if (team == Team.PLAYER)
        {
            //We concatenate _b to refer rear sprite images for player sprites
            spriteFile = monster.MonsterBase.spriteFile + "_b";
        }
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/mon/mon_" + spriteFile);
        
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
        //add slight variance so icons are not the same
        float variance = Random.Range(-.5f,.5f);
        baseSpeed = monster.Speed + variance;
        BC.ThresholdUpdate(baseSpeed);
        Debug.Log(monster.Print);
    }

    void IncreaseSpeed()
    {
        //this will prevent speed increasing when still user's turn
        //Note: May need ot make player exclusive
        if (pause || (BC.battleState == BattleController.State.ENDCONDITION))
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
            //if (BC.turnList[0].owner == this.gameObject)
            //{
                BC.ExecuteTurnFor(this.gameObject); 
                ResetAttack();
            //}
        }
        else
        {
            isChargingToAttack = true;
            chargeTimer += Time.deltaTime;
        }
    }

    void OnMouseEnter()
    {
        if (isDead)
            return;

        if(team == Team.PLAYER)
        {
            Material mat = GetComponent<Renderer>().material;
            mat.color = new Color(0.5f, 1, 0.5f, 1); 
            return;
        }
        else
        {
            IsTargeted(true);

            UpdateInfo();
        }
    }

    void OnMouseExit()
    {
        if (isDead)
            return;

        IsTargeted(false);
        RemoveInfo();
    }

    void RemoveInfo()
    {
        if (panelInfoObject == null)
        {
            return;
        }

        if (team == Team.PLAYER)
        {
            return;
        }
        else
        {
            //unhighlight tick
            trackingTickObject.GetComponent<Image>().color = Color.white;
            //move info out of camera frame
            panelInfoObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, 300);
        }

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
        panelInfoObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(canvasPos.x - 100, canvasPos.y -50);
    }

    void SetupHandForPlayer()
    {
        if(team == Team.PLAYER)
        {
            playerHand.SetupHand();
            BC.EndTurnButton.SetActive(true);
        }
    }

    IEnumerator DeathFade()
    {
        for (float f = 1f; f > 0f; f -= 0.005f)
        {
            spriteRenderer.color = new Color(1, 1, 1, f); 
            yield return null;
        }
        //this.gameObject.SetActive(false);
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
    }

    void Start ()
    {
        //get reference to UserController to check if mouse is on top of this monster
        userController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UserController>();
        if (team == Team.PLAYER && playerController == null)
        {
            playerController = GetComponent<PlayerController>();
            playerHand = GameObject.FindGameObjectWithTag("Hand").GetComponent<PlayerHandController>();
        }
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
                SetupHandForPlayer();
                monsterState = State.SELECTING;
                break;
            case (State.SELECTING):
                SelectAttack();
                break;
            case (State.CHARGING):
                ChargeSpeed(chargeDuration);
                break;
            case (State.DEAD):
                DoDeath();
                break;
            case (State.PAUSED):
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
