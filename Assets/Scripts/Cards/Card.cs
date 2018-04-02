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
    public new string name;         //cards name on gameobject
    public string description;      //cards description text on gameobject
    public Sprite artwork;          //the card sprite but will be replaced to single artwork only
    public int damage;              //the amount that affects the HP
    public int cost;                //how much enery it cost to use this attack
    public float charge;            //the amount of time in secs to charge to perform this attack
    public TargetArea targetArea;   
    public bool isCanceling;            //does this card cancel the attacks of enemies?
}
