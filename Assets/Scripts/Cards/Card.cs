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
    RANDOM             
}


[CreateAssetMenu(fileName ="New Card", menuName = "Game/Card")]
public class Card : ScriptableObject {
    //cards name on gameobject
    public new string name;
    //cards description text on gameobject
    public string description;
    //the card sprite but will be replaced to single artwork only
    public Sprite artwork;
    //the amount that affects the HP
    public int damage;
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
}
             
