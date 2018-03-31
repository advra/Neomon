using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//acting as the battle state manager
public class BattleController : MonoBehaviour {
    MonsterController monsterController;
    GaugeTickController gaugeTickController;
    PlayerTickController playerTickController;

    //used for monster queueing
    public List<HandleTurn> turnList = new List<HandleTurn>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();
    public List<GameObject> FriendliesInBattle = new List<GameObject>();

    public MonsterController monsterControllerA;
    public MonsterController monsterControllerB;
    public MonsterController monsterControllerC;
    //public MonsterController monsterControllerD; //soon to add (will have Boss chance as well)
    public PlayerController playerController;
    //reference player
    public GameObject player;
    //Enemy Objects
    public GameObject enemyA;
    public GameObject enemyB;
    public GameObject enemyC;
    public GameObject tickPrefab;
    public GameObject playerTickPrefab;
    public GameObject enemyTickPrefab;
    public GameObject playerHand;
    public RectTransform canvasRect;
    public GameObject BattleTextPrefab;

    public Camera mainCamera;

    public int numberOfEnemies;
    
    public bool isBattling;
    public bool playerTurn;
    bool paused;
    public float threshold = 1.0f;

    //PerformAction battleState;

    //public enum PerformAction
    //{
    //    WAIT,               //preparing or idling until information recieved
    //    TAKEACTION,         //doing the action
    //    EXECUTEACTION       //wait until action is completed
    //}

    //used to normalize threshold based upon their speeds
    public void ThresholdUpdate(float value)
    {
        if(value > threshold)
        {
            threshold = value * 3;
        }
    }

    public void PauseMonsters()
    {
        foreach (GameObject monster in EnemiesInBattle)
        {
            monster.GetComponent<MonsterController>().pause = true;
        }
    }

    public int NumberOfEnemies
    {
        get { return numberOfEnemies; }
    }

    public float Threshold
    {
        get { return threshold; }
    }

    public bool IsBattling
    {
        get { return isBattling; }
        set { isBattling = value;  }
    }

    public bool IsPaused
    {
        get { return paused; }
        set { paused = value;  }
    }

    public void PauseSpeedsForEnemies(bool value)
    {
        //pause increment of all other monsters
        foreach (GameObject monster in EnemiesInBattle)
        {
            monster.GetComponent<MonsterController>().pause = value;
        }
    }

    void CreateProgressTickFor(GameObject monster)
    {
        if (monster == player)
        {
            GameObject tickObj = Instantiate(playerTickPrefab, this.transform);
            PlayerTickController playerTickController = tickObj.GetComponent<PlayerTickController>();
            playerTickController.TrackedMonster = monster;
        }
        else
        {
            GameObject tickObj = Instantiate(enemyTickPrefab, this.transform);
            EnemyTickController enemyTickController = tickObj.GetComponent<EnemyTickController>();
            enemyTickController.TrackedMonster = monster;
            //add tick object to our monster controller so we can reference it in the future
            MonsterController monsterController = monster.GetComponent<MonsterController>();
            monsterController.trackingTickObject = tickObj;
        }
    }

    // Use this for initialization
    void Awake ()
    {
        if (tickPrefab == null)
            tickPrefab = Resources.Load<GameObject>("TickPrefab");
        if (playerTickPrefab == null)
            playerTickPrefab = Resources.Load<GameObject>("PlayerTickPrefab");
        if (enemyTickPrefab == null)
            enemyTickPrefab = Resources.Load<GameObject>("EnemyTickPrefab");
        if (BattleTextPrefab == null)
            BattleTextPrefab = Resources.Load<GameObject>("DamageText");
        if (playerHand == null)
        {
            playerHand = GameObject.FindGameObjectWithTag("Hand");
        }
        if (canvasRect == null)
        {
            canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        }

        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        //setup player components
        //monsterControllerPlayer = player.GetComponent<MonsterController>();
        CreateProgressTickFor(player);

        //GameObject tickObj = Instantiate(tickPrefab, this.transform);
        //tickObj.GetComponent<GaugeTickController>().TrackedMonster = player;
        playerController = player.GetComponent<PlayerController>();
        FriendliesInBattle.Add(player);

        //Now setup enemy components and determine number of enemies
        numberOfEnemies = Random.Range(0, 3);
        //numberOfEnemies = 1;
        if (numberOfEnemies == 1)
        {
            enemyA.SetActive(false);
            enemyC.SetActive(false);
            CreateProgressTickFor(enemyB);
            monsterControllerB = enemyB.GetComponent<MonsterController>();
            EnemiesInBattle.Add(enemyB);
        }
        else if(numberOfEnemies == 2)
        {
            enemyA.SetActive(false);
            CreateProgressTickFor(enemyB);
            CreateProgressTickFor(enemyC);
            monsterControllerB = enemyB.GetComponent<MonsterController>();
            monsterControllerC = enemyC.GetComponent<MonsterController>();
            EnemiesInBattle.Add(enemyB);
            EnemiesInBattle.Add(enemyC);
        }
        else
        {
            CreateProgressTickFor(enemyA);
            CreateProgressTickFor(enemyB);
            CreateProgressTickFor(enemyC);
            monsterControllerA = enemyA.GetComponent<MonsterController>();
            monsterControllerB = enemyB.GetComponent<MonsterController>();
            monsterControllerC = enemyC.GetComponent<MonsterController>();
            EnemiesInBattle.Add(enemyA);
            EnemiesInBattle.Add(enemyB);
            EnemiesInBattle.Add(enemyC);
        }

        if(monsterControllerA || monsterControllerB || monsterControllerC == null)
        {
            Debug.Log("One or More monsterController for BattleController is null!");
        }
    }

    //populates turn list
    public void AddTurnToQueue(HandleTurn turn)
    {
        turnList.Add(turn);
    }

    public void ExecuteTurnFor(GameObject monster)
    {
        SpawnBattleTextAbove(turnList[0].target);
        //send damageto targeted GO controller & Reset monsters speed / charge stats 
        if(monster == player)
        {
            //check if targeted enemy is charging for an attack, if so reset their attack
            //if (turnList[0].target.GetComponent<MonsterController>().isCharingToAttack)
            //{
            //    turnList[0].target.GetComponent<MonsterController>().ResetAttack();
            //    //also remove their attack from queue by searching for it
            //    for (int i = 0; i <= turnList.Count; i++)
            //    {
            //        if(turnList[i].owner == turnList[0].target)
            //        {
            //            turnList.RemoveAt(i);
            //            return;
            //        }
            //    }
            //}
            turnList[0].target.GetComponent<MonsterController>().Damage(turnList[0].damage);
        }
        else
        {
            turnList[0].target.GetComponent<PlayerController>().Damage(turnList[0].damage);
        }
        turnList.RemoveAt(0);
    }

    void SpawnBattleTextAbove(GameObject monster)
    {
        GameObject textObj = Instantiate(BattleTextPrefab, canvasRect.transform);
        textObj.transform.localScale = Vector3.one;
        string s = turnList[0].damage.ToString();
        textObj.GetComponent<Text>().text = s;

        Vector2 canvasPos;
        GameObject targetedMonster = turnList[0].target;
        //get target's position relative to canvas screen 
        Vector2 screenPointToTarget = Camera.main.WorldToScreenPoint(targetedMonster.transform.position);
        // Convert screen position to Canvas / RectTransform space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPointToTarget, null, out canvasPos);
        //move text to Game object's position
        textObj.GetComponent<RectTransform>().anchoredPosition = canvasPos;
    }

    public void PlayerWin()
    {
        isBattling = false;
        GameObject gameResultText = Instantiate(Resources.Load<GameObject>("BattleResultTextPrefab"), canvasRect.transform);
        gameResultText.GetComponent<BattleResultText>().SetText("Victory!", "All enemies defeated", "Continue");
    }

    public void PlayerLose()
    {
        isBattling = false;
        GameObject gameResultText = Instantiate(Resources.Load<GameObject>("BattleResultTextPrefab"), canvasRect.transform);
        gameResultText.GetComponent<BattleResultText>().SetText("Defeat!", "Heroes are destined to die...", "Retry");
    }
}

