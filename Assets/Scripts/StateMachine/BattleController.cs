using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//acting as the battle state manager
public class BattleController : MonoBehaviour {
    MonsterController monsterController;
    GaugeTickController gaugeTickController;
    PlayerTickController playerTickController;


    public enum State
    {
        BEGIN,
        RUN,
        ENDCONDITION
    }

    [SerializeField]
    private State battleState;
    [SerializeField]
    bool paused;
    [SerializeField]
    private bool monsterAttacking;

    [SerializeField]
    HandleTurn turnReference;

    //used for monster queueing
    public List<HandleTurn> turnList = new List<HandleTurn>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();
    public List<GameObject> FriendliesInBattle = new List<GameObject>();

    //current turn data

    [SerializeField]
    List<GameObject> targets;
    [SerializeField]
    int damage;
    [SerializeField]
    int newReferenceIndex;
    [SerializeField]
    private bool isPaused;

    public MonsterController monsterControllerA;
    public MonsterController monsterControllerB;
    public MonsterController monsterControllerC;
    //public MonsterController monsterControllerD; //soon to add (will have Boss chance as well)
    public MonsterController playerController;
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
    //End Turn Button
    public GameObject EndTurnButton;
    public EndTurnButtonController endTurnButtonScript;

    public Camera mainCamera;

    public int numberOfEnemies;
    
    public bool isBattling;
    public bool playerTurn;
    public float threshold = 1.0f;

    public bool PauseGame
    {
        get
        {
            return isPaused;
        }
        set
        {
            isPaused = value;

            if (isPaused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public bool MonsterIsAnimating
    {
        get { return monsterAttacking; }
        set { monsterAttacking = value; }
    }

    public State BattleState
    {
        get { return battleState; }
        set { battleState = value; }
    }

    //used to normalize threshold based upon their speeds
    public void ThresholdUpdate(float value)
    {
        if(value > threshold)
        {
            threshold = value * 3;
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

    public void PauseSpeedsForAllMonsters(bool value)
    {
        foreach (GameObject monster in FriendliesInBattle)
        {
            monster.GetComponent<MonsterController>().isSpeedPaused = value;
        }
        foreach (GameObject monster in EnemiesInBattle)
        {
            monster.GetComponent<MonsterController>().isSpeedPaused = value;
        }

    }

    void CreateProgressTickFor(GameObject monster)
    {
        if (monster == player)
        {
            GameObject tickObj = Instantiate(playerTickPrefab, this.transform);
            PlayerTickController playerTickController = tickObj.GetComponent<PlayerTickController>();
            playerTickController.TrackedMonster = monster;
            //add tick object to our monster controller so we can reference it in the future
            MonsterController monsterController = monster.GetComponent<MonsterController>();
            monsterController.trackingTickObject = tickObj;
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

    public void HideCombatUI()
    {
        EndTurnButton.SetActive(false);
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
        if (EndTurnButton == null)
        {
            EndTurnButton = GameObject.FindGameObjectWithTag("EndTurnButton");
            endTurnButtonScript = EndTurnButton.GetComponent<EndTurnButtonController>();
        }
        //make sure none of the UI stuff is viewed at start
        HideCombatUI();
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
        playerController = player.GetComponent<MonsterController>();
        playerController.team = MonsterController.Team.PLAYER;
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
            monsterControllerB.team = MonsterController.Team.ENEMY;
            EnemiesInBattle.Add(enemyB);
        }
        else if(numberOfEnemies == 2)
        {
            enemyA.SetActive(false);
            CreateProgressTickFor(enemyB);
            CreateProgressTickFor(enemyC);
            monsterControllerB = enemyB.GetComponent<MonsterController>();
            monsterControllerC = enemyC.GetComponent<MonsterController>();
            monsterControllerB.team = MonsterController.Team.ENEMY;
            monsterControllerC.team = MonsterController.Team.ENEMY;
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
            monsterControllerA.team = MonsterController.Team.ENEMY;
            monsterControllerB.team = MonsterController.Team.ENEMY;
            monsterControllerC.team = MonsterController.Team.ENEMY;
            EnemiesInBattle.Add(enemyA);
            EnemiesInBattle.Add(enemyB);
            EnemiesInBattle.Add(enemyC);
        }

        if(monsterControllerA || monsterControllerB || monsterControllerC == null)
        {
            Debug.Log("One or More monsterController for BattleController is null!");
        }
    }

    public bool AllEnemiesDead()
    {
        foreach (GameObject enemy in EnemiesInBattle)
        {
            if (enemy.GetComponent<MonsterController>().isDead)
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    //populates turn list
    public void AddTurnToQueue(HandleTurn turn)
    {
        turnList.Add(turn);
    }

    //compares if current monster (with faster speed) queued up before the one who charged into queue first are same team
    //if theyre on the same team, let faster monster play their attack
    //otherwise play current monsters attack then remove queued monster from list
    public bool MonstersAreDifferenTeams(GameObject monsterA, GameObject monsterB)
    {
        if(monsterA.GetComponent<MonsterController>().team != monsterB.GetComponent<MonsterController>().team)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    void DoCancelAttack(GameObject targetedMonster, MonsterController theirMonsterController)
    {
        Debug.Log("Enemy attack canceld!");
        theirMonsterController.ResetAttack();
        theirMonsterController.Damage(damage);
        SpawnBattleTextAbove(targetedMonster);
        SpawnBattleTextAboveWithString(targetedMonster, "CANCELED");
    }

    void DoNormalAttack(GameObject targetedMonster, MonsterController theirMonsterController)
    {
        theirMonsterController.Damage(damage);
        SpawnBattleTextAbove(targetedMonster);
    }

    IEnumerator BeginAttack(GameObject attacker, GameObject defender, float duration)
    {
        float startTime;
        float totalDistance;
        Vector3 startPos = attacker.transform.position;
        Vector3 endPos = defender.transform.position;

        startTime = Time.time;
        totalDistance = Vector3.Distance(startPos, endPos);
        yield return SingleAttack(attacker, startPos, endPos, duration);
    }

    IEnumerator SingleAttack(GameObject attacker, Vector3 a, Vector3 b, float time)
    {
        Vector3 initialPos = attacker.transform.position;

        float i = 0.0f;
        float rate = 1.0f / time; //* speed;
        //destination not reached
        while( i < 1.0f)
        {
            i += Time.deltaTime * rate;
            attacker.transform.position = Vector3.Lerp(a, b, i);
            yield return null;
        }

        attacker.transform.position = initialPos;

        MonsterIsAnimating = false;
    }

    public void ExecuteTurnFor(GameObject monster)
    {
        if(turnList.Count == 0)
        {
            return;
        }

        newReferenceIndex = -1;
        //PauseSpeedsForAllMonsters(true);

        //Design Decision: check if card can cancel (assume all cards can and add bool later)
        //check if canceling attack targets are on diff team if so do not cancel
        //check if this monster is owner if index 0 otherwise toss out index 0

        //if this monster calling the even owns data then assign
        if (turnList[0].owner == monster)
        {
            turnReference = turnList[0];
            targets = turnReference.targets;
            damage = turnReference.damage;
        }
        //if not owner search for the data to use instead
        else
        {
            for (int i = 0; i < turnList.Count; i++)
            {
                if (turnList[i].owner == monster)
                {
                    //use this turn data instead 
                    //remove the canceled monsters turn at the very last line of this function
                    turnReference = turnList[i];
                    targets = turnReference.targets;
                    damage = turnReference.damage;
                    newReferenceIndex = i;
                    //turnList.RemoveAt(i);
                }
            }
        }

        //from here on out we assume we obtained the correct data for this turn

        if(turnReference.targetArea == TargetArea.SINGLE)
        {
            //lerp monster from their position to their targets position
            MonsterIsAnimating = true;
            StartCoroutine(BeginAttack(turnReference.owner, turnReference.targets[0], 0.5f)) ;
        }

        foreach (GameObject targetedMonster in targets)
        {
            MonsterController targetedMonstersController = targetedMonster.GetComponent<MonsterController>();

            //if canceling
            if (turnReference.isCanceling)
            {
                Debug.Log("Attack Canceling, Diff Team?: " + MonstersAreDifferenTeams(monster, targetedMonster));
                
                if (targetedMonstersController.isChargingToAttack && MonstersAreDifferenTeams(monster, targetedMonster))
                {
                    DoCancelAttack(targetedMonster, targetedMonstersController);
                }

                //if it is an enemy but they arent charing to attack, just do normal attack
                if (!targetedMonstersController.isChargingToAttack && MonstersAreDifferenTeams(monster, targetedMonster))
                {
                    DoNormalAttack(targetedMonster, targetedMonstersController);
                }

                //when done using data that we found for canceling, remove it
                if (newReferenceIndex != -1)
                {
                    turnList.RemoveAt(newReferenceIndex);
                }
            }
            //if not on the same team and not canceling then normal damage
            else
            {
                DoNormalAttack(targetedMonster, targetedMonstersController);
            }
        }

        //remove event for current attacker
        turnList.RemoveAt(0);
        //unpause when action is complete
        //PauseSpeedsForAllMonsters(false);
    }

    void SpawnBattleTextAboveEach(List<GameObject> targets)
    {
        foreach (GameObject targetedMonster in targets)
        {
            GameObject textObj = Instantiate(BattleTextPrefab, canvasRect.transform);
            textObj.transform.localScale = Vector3.one;
            string s = turnList[0].damage.ToString();
            textObj.GetComponent<Text>().text = s;

            Vector2 canvasPos;
            //get target's position relative to canvas screen 
            Vector2 screenPointToTarget = Camera.main.WorldToScreenPoint(targetedMonster.transform.position);
            // Convert screen position to Canvas / RectTransform space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPointToTarget, null, out canvasPos);
            //move text to Game object's position
            textObj.GetComponent<RectTransform>().anchoredPosition = canvasPos;
        }
    }

    void SpawnBattleTextAbove(GameObject monster)
    {

        GameObject textObj = Instantiate(BattleTextPrefab, canvasRect.transform);
        textObj.transform.localScale = Vector3.one;
        string s = turnList[0].damage.ToString();
        textObj.GetComponent<Text>().text = s;

        Vector2 canvasPos;
        //get target's position relative to canvas screen 
        Vector2 screenPointToTarget = Camera.main.WorldToScreenPoint(monster.transform.position);
        // Convert screen position to Canvas / RectTransform space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPointToTarget, null, out canvasPos);
        //move text to Game object's position
        textObj.GetComponent<RectTransform>().anchoredPosition = canvasPos;
    }

    void SpawnBattleTextAboveWithString(GameObject monster, string s)
    {

        GameObject textObj = Instantiate(BattleTextPrefab, canvasRect.transform);
        textObj.transform.localScale = Vector3.one;
        textObj.GetComponent<Text>().text = s;

        Vector2 canvasPos;
        //get target's position relative to canvas screen 
        Vector2 screenPointToTarget = Camera.main.WorldToScreenPoint(monster.transform.position);
        // Convert screen position to Canvas / RectTransform space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPointToTarget, null, out canvasPos);
        //move text to Game object's position
        textObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(canvasPos.x, canvasPos.y + 50);
    }

    void ChangeAllMonsterStatesToPause()
    {
        foreach (GameObject monster in FriendliesInBattle)
        {
            monster.GetComponent<MonsterController>().monsterState = MonsterController.State.PAUSED;
        }
        foreach (GameObject monster in EnemiesInBattle)
        {
            monster.GetComponent<MonsterController>().monsterState = MonsterController.State.PAUSED;
        }
    }

    public void PlayerWin()
    {
        battleState = State.ENDCONDITION;
        isBattling = false;
        GameObject gameResultText = Instantiate(Resources.Load<GameObject>("Menu/BattleResultTextPrefab"), canvasRect.transform);
        gameResultText.GetComponent<BattleResultText>().SetText("Victory!", "All enemies defeated", "Continue");
    }

    public void PlayerLose()
    {
        battleState = State.ENDCONDITION;
        isBattling = false;
        GameObject gameResultText = Instantiate(Resources.Load<GameObject>("Menu/BattleResultTextPrefab"), canvasRect.transform);
        gameResultText.GetComponent<BattleResultText>().SetText("Defeat!", "Heroes are destined to die...", "Retry");
    }
}

