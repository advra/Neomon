using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TargetArea
{
    single = 0,
    split,
    line,
    self,
    random,
    all             
}

//public enum State
//{
//    NORMAL,
//    RESET
//}

[CreateAssetMenu(fileName ="New Card", menuName = "Game/Card")]
public class Card : ScriptableObject {
    public new string name;
    public string description;
    public Sprite artwork;
    public int damage;
    public int cost;
    public float charge;
    public TargetArea targetArea;
}
