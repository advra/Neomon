using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour {

    BattleController BC;
    MonsterController enemy;
    public TurnState currentState;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    // Use this for initialization
    void Start()
    {
        BC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        enemy = GetComponent<MonsterController>();
        currentState = TurnState.PROCESSING;
    }

    void UpdateSpeed()
    {
        //enemy.CurrentSpeed = enemy.CurrentSpeed + enemy.baseSpeed;
    }

    //void ChooseAction()
    //{
    //    HandleTurn myAttack = new HandleTurn();
    //    myAttack.Attacker = enemy.name;
    //    myAttack.GameObjectAttacker = this.gameObject;
    //    myAttack.GameObjectTarget = BC.FriendliesInBattle[Random.Range(0, BC.FriendliesInBattle.Count)];
    //    BC.CollectActions(myAttack);
    //}

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpdateSpeed();
                break;
            case (TurnState.CHOOSEACTION):

                break;

            case (TurnState.WAITING):

                break;

            case (TurnState.ACTION):

                break;

            case (TurnState.DEAD):

                break;
        }

    }
}
