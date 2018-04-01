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
        RUN,
        ENDCONDITION
    }

    public State battleState;

    //used for monster queueing
    public List<HandleTurn> turnList = new List<HandleTurn>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();
    public List<GameObject> FriendliesInBattle = new List<GameObject>();

    //current turn data
    [SerializeField]
    HandleTurn turnReference;
    [SerializeField]
    List<GameObject> targets;
    [SerializeField]
    int damage;
    [SerializeField]
    int newReferenceIndex;

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

    public void PauseSpeedsForAllMonsters(bool value)
    {
        foreach (GameObject monster in FriendliesInBattle)
        {
            monster.GetComponent<MonsterController>().pause = value;
        }
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

    //public void ExecuteTurnFor(GameObject monster)
    //{
    //    List<GameObject> targets = turnList[0].targets;

    //    //Design Decision: check if card can cancel (assume all cards can and add bool later)
    //    //check if canceling attack targets are on diff team if so do not cancel
    //    //check if this monster is owner if index 0 otherwise toss out index 0

    //    //create ui damage text above targeted monsters
    //    SpawnBattleTextAboveEach(targets);
    //    //send damage to targeted GO controller & Reset monsters speed / charge stats 

    //    foreach (GameObject targetedMonster in targets)
    //    {
    //        //MonsterController targetedMonstersController = targetedMonster.GetComponent<MonsterController>();
    //        //if (MonstersAreSameTeam(monster, targetedMonster))
    //        //{
    //        //    //swap places and play index 0 then dont cancel
    //        //}
    //        //else
    //        //{
    //        //    if (targetedMonstersController.isCharingToAttack)
    //        //    {
    //        //        //Cancel and reset the enemies attack
    //        //        targetedMonstersController.ResetAttack();  
    //        //    }
    //        //    //swap places and play index 0 then cancel
    //        //}
    //        targetedMonster.GetComponent<MonsterController>().Damage(turnList[0].damage);
    //    }

    //    turnList.RemoveAt(0);
    //}

    public void ExecuteTurnFor(GameObject monster)
    {
        Debug.Log("Counter: " + turnList.Count);
        if(turnList.Count == 0)
        {
            return;
        }

        newReferenceIndex = -1;
        PauseSpeedsForAllMonsters(true);

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
        //if i am queued late but charge quicker to cancel
        //we should find this monsters attack event and use that data instead
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

        foreach (GameObject targetedMonster in targets)
        {
            MonsterController targetedMonstersController = targetedMonster.GetComponent<MonsterController>();

            //if canceling
            if (turnReference.isCanceling)
            {
                Debug.Log("Attack Canceling, Same Team?: " + MonstersAreDifferenTeams(monster, targetedMonster));
                if (targetedMonstersController.isChargingToAttack && MonstersAreDifferenTeams(monster, targetedMonster))
                {
                    Debug.Log("Enemy attack canceld!");
                    targetedMonstersController.ResetAttack();
                    targetedMonstersController.Damage(damage);
                    SpawnBattleTextAbove(targetedMonster);
                    SpawnBattleTextAboveWithString(targetedMonster, "CANCELED");
                }
                ////do normal damage if theyre not charging 
                //else
                //{
                //    targetedMonstersController.Damage(damage);
                //    SpawnBattleTextAbove(targetedMonster);
                //}

                //when done using data for canceling, remove it
                if (newReferenceIndex != -1)
                {
                    turnList.RemoveAt(newReferenceIndex);
                }
            }
            //if not on the same team and not canceling then do damage as normal
            else
            {
                targetedMonstersController.Damage(damage);
                SpawnBattleTextAbove(targetedMonster);
            }
        }
        //remove event for current attacker
        
        //if we found a different monster from earlier, make sure to remove that data as well

        turnList.RemoveAt(0);
        //unpause when action is complete
        PauseSpeedsForAllMonsters(false);
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
            //GameObject targetedMonster = turnList[0].targets;
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
        //GameObject targetedMonster = turnList[0].targets;
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
        //GameObject targetedMonster = turnList[0].targets;
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
        GameObject gameResultText = Instantiate(Resources.Load<GameObject>("BattleResultTextPrefab"), canvasRect.transform);
        gameResultText.GetComponent<BattleResultText>().SetText("Victory!", "All enemies defeated", "Continue");
    }

    public void PlayerLose()
    {
        battleState = State.ENDCONDITION;
        isBattling = false;
        GameObject gameResultText = Instantiate(Resources.Load<GameObject>("BattleResultTextPrefab"), canvasRect.transform);
        gameResultText.GetComponent<BattleResultText>().SetText("Defeat!", "Heroes are destined to die...", "Retry");
    }
}

