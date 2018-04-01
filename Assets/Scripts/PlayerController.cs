using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    UserController userController;
    public BattleController BC;
    public MonsterController monsterController;
    public PlayerHandController playerHand;
    public PlayerTickController playerTickController;

    public bool draw; 
    bool count;
    public PlayerState currentUserState;





    public enum PlayerState
    {
        WAITING,    //waiting until turn
        READY,      //when monster is ready / drawing cards
        SELECTING,  //is ready now player chooses action
        CHARGING,   //wait time delay needed to perform action
        PERFORMING,
        DEAD
    }


    ////at the end of every attack after executed (or canceled call this)
    //public void ResetAttack()
    //{
    //    Debug.Log("Resetting");
    //    if (playerTickController == null)
    //    {
    //        playerTickController = GameObject.FindGameObjectWithTag("PlayerTick").GetComponent<PlayerTickController>();
    //    }
    //    //playerTickController.state = PlayerTickController.GaugeState.RESET;
    //    playerTickController.ChangeState(PlayerTickController.GaugeState.INCREASING);
    //    chargeTimer = 0.0f;
    //    currentSpeed = 0.0f;
    //    isCharingToAttack = false;
    //    currentUserState = PlayerState.WAITING;
    //    EndTurnButton.SetActive(false);
    //}

    //public void HideCombatUI()
    //{
    //    EndTurnButton.SetActive(false);
    //}

    //public void Damage(int amount)
    //{
    //    this.currentHealth -= amount;
    //    //Play damage sprite here
    //    if (this.currentHealth <= 0)
    //    {
    //        this.currentHealth = 0;
    //        this.isDead = true;
    //    }

    //    if (isDead)
    //    {
    //        //run death animation here
    //        //Design decision: either fade out and disable now, or allow them to be revived?

    //        //end battle scene
    //        currentUserState = PlayerState.DEAD;
    //        //pause all enemies
    //        BC.PauseMonsters();
    //        //display losing text
    //        BC.PlayerLose();
    //    }
    //}

    void Awake()
    {
        currentUserState = PlayerState.WAITING;
        BC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();


    }

    // Use this for initialization
    void Start()
    {
        //if (EndTurnButton == null)
        //{
        //    EndTurnButton = GameObject.FindGameObjectWithTag("EndTurnButton");
        //    endTurnButtonScript = EndTurnButton.GetComponent<EndTurnButtonController>();
        //}
        //EndTurnButton.SetActive(false);
        ////get reference to UserController to check if mouse is on top of this monster
        //userController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UserController>();
        ////get reference to hand to draw cards
        //playerHand = GameObject.FindGameObjectWithTag("Hand").GetComponent<PlayerHandController>();

    }

    //void IncreaseSpeed()
    //{
    //    if (currentSpeed >= BC.Threshold) 
    //    {
    //        currentUserState = PlayerState.READY;
    //    }
    //    else
    //    {
    //        currentSpeed += baseSpeed * Time.deltaTime;
    //    }
    //}

    //void ChargeSpeed(float durationInSeconds)
    //{
    //    if (chargeTimer >= durationInSeconds) //change back to BC.Threshold for testing
    //    {
    //        //this will ensure it will not reset without trigger if it is not their turn
    //        if (BC.turnList[0].owner == this.gameObject)
    //        {
    //            BC.ExecuteTurnFor(this.gameObject);
    //            ResetAttack();
    //        }
    //    }
    //    else
    //    {
    //        isCharingToAttack = true;
    //        chargeTimer += Time.fixedDeltaTime;
    //    }
    //}

    void Update()
    {
        switch (currentUserState)
        {
            case (PlayerState.WAITING):
                break;
            case (PlayerState.READY):
               // playerHand.SetupHand();
                //EndTurnButton.SetActive(true);
                //currentUserState = PlayerState.SELECTING;
                break;
            case (PlayerState.SELECTING):
                //Necessary to prevent monsters from increasing their speeds when its players turn
                //BC.PauseSpeedsForEnemies(true);
                //Debug.Log("Waiting for user to select cards");
                break;
            case (PlayerState.CHARGING):
                break;
            case (PlayerState.DEAD):
                //HideCombatUI();
                break;
            default:
                break;
        }
    }

}