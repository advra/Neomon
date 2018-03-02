using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {
    MonsterController monsterController;
    public MonsterController monsterControllerA;
    public MonsterController monsterControllerB;
    public MonsterController monsterControllerC;
    public MonsterController monsterControllerPlayer;
    //reference player
    public GameObject player;
    //Enemy Objects
    public GameObject enemyA;
    public GameObject enemyB;
    public GameObject enemyC;
    int numberOfEnemies;
    
    float tick = 0.05f;
    public bool isBattling;
    public bool playerTurn;
    bool paused;
    float threshold = 100.0f;

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




    // Use this for initialization
    void Awake () {

        //setup player components
        monsterControllerPlayer = player.GetComponent<MonsterController>();

        //Now setup enemy components
        //determine number of enemies
        //this can be changed later to be more dynamic
        //numberOfEnemies = Random.Range(0,3)+1; //returns 1-3 enemies
        numberOfEnemies = Random.Range(0, 3);
        if (numberOfEnemies == 1)
        {
            enemyA.SetActive(false);
            enemyC.SetActive(false);
            //get the stats of the active monster
            monsterControllerB = enemyB.GetComponent<MonsterController>();
        }
        else if(numberOfEnemies == 2)
        {
            enemyA.SetActive(false);
            monsterControllerB = enemyB.GetComponent<MonsterController>();
            monsterControllerC = enemyC.GetComponent<MonsterController>();
        }
        else
        {
            monsterControllerA = enemyA.GetComponent<MonsterController>();
            monsterControllerB = enemyB.GetComponent<MonsterController>();
            monsterControllerC = enemyC.GetComponent<MonsterController>();
        }

        if(monsterControllerA || monsterControllerB || monsterControllerC == null)
        {
            Debug.Log("One or More monsterController for BattleController is null!");
        }
    }

    //void OnTick(tick)
    //{
    //    if(tick = next)
    //    {
    //        TakeTurn 
    //        FindNextTurn();
    //    }
    //}

    //void FindNextTurn()
    //{
    //    FindNextTurn = ciel(500/speed) + remainder
    //}

    void IncreaseSpeed(MonsterController monsterController)
    {
        monsterController.speed = monsterController.speed + (monsterController.baseSpeed * tick);
    }

    void Update()
    {
        //increment speed value every tick
        //when over 500 it is unit's turn
        if (isBattling && !paused)
        {
            //count speed for each monster to determine who goes first
            IncreaseSpeed(monsterControllerPlayer);
            if (numberOfEnemies == 1)
            {
                IncreaseSpeed(monsterControllerB);
            }
            else if(numberOfEnemies == 2)
            {
                IncreaseSpeed(monsterControllerB);
                IncreaseSpeed(monsterControllerC);
            }
            else
            {
                IncreaseSpeed(monsterControllerA);
                IncreaseSpeed(monsterControllerB);
                IncreaseSpeed(monsterControllerC);
            }
        }
    }
}
