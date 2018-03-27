using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour {

    public TurnState currentState;
    MonsterController player;

    public float tickThreshold;

    public  enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }

	// Use this for initialization
	void Start () {
        player = GetComponent<MonsterController>();
        currentState = TurnState.PROCESSING;
    }

    void UpdateSpeed()
    {
        player.CurrentSpeed = player.CurrentSpeed + player.baseSpeed;
    }
	
	// Update is called once per frame
	void Update () {

        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpdateSpeed();
                break;
            case (TurnState.ADDTOLIST):

                break;

            case (TurnState.WAITING):

                break;

            case (TurnState.SELECTING):

                break;

            case (TurnState.ACTION):

                break;

            case (TurnState.DEAD):

                break;
        }
		
	}
}
