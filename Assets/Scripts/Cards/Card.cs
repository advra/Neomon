using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TargetArea
{
    SINGLE = 0,
    ALL,
    SPLIT,
    LINE,
    SELF,
    RANDOM,
    NOT_SET
}

public enum DrawType
{
    NONE,
    DRAW_NORMAL,
    DISCARD_NORMAL,
    RESHUFFLE_HAND
}


[CreateAssetMenu(fileName ="New Card", menuName = "Game/Card")]
public class Card : ScriptableObject {
    //cards name on gameobject
    public new string name;
    //cards description text on gameobject
    public string description;
    //the card sprite but will be replaced to single artwork only
    public Sprite artwork;
    // the amount that affects the HP
    public int damage;
    // amount that blocks damage
    public int block;
    //how much enery it cost to use this attack
    public int cost;
    //the amount of time in secs to charge to perform this attack
    public float charge;           
    public TargetArea targetArea;
    //does this card cancel the attacks of enemies?
    public bool isCanceling;
    //does this card combo with other cards?
    public bool chainCombo;
    //does this card stun if so how many turns?            
    public int stunNumberOfTurns;
    //does this card draw, discard or reshuffle the entire hand?
    public DrawType drawType;
    //how many cards does this card draw?
    public int drawNumber;
    //is this instant or wait until next turn? for drawing cards or other stuff
    public bool onNextTurn;

}
             
