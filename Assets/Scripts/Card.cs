using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1-sinlge 2-split 3-line 4-self 5-random 6-all 
/// </summary>
public enum TargetArea
{
    single = 0,
    split,
    line,
    self,
    random,
    all                 //attacks all enemies
}

public enum State
{
    NORMAL,
    RESET
}

[System.Serializable]
public class Card {

    private string name;
    private string description;
    private string sprite;
    private int cardType;
    private int damage;
    private int cost;
    private float chargeTime;
    TargetArea targetArea;

    public string Name {
        get{ return name;}
    }

    public string Sprite
    {
        get { return sprite; }
    }

    public float ChargeTime
    {
        get { return chargeTime;  }
        set { chargeTime = value; }
    }

    public int Damage
    {
        get { return damage; }
    }

    public int Cost
    {
        get { return cost; }
    }

    public TargetArea TargetArea
    {
        get { return targetArea; }
        set { targetArea = value; }
    }

    public Card(string name, string description, string sprite)
    {
        this.name = name;
        this.description = description;
        this.sprite = sprite;
    }

    public Card(string name, string description, int damage, TargetArea areaType, string sprite, int cost, float chargeTime)
    {
        this.name = name;
        this.description = description;
        this.damage = damage;
        this.sprite = sprite;
        this.TargetArea = areaType;
        this.cost = cost;
        this.chargeTime = chargeTime;
    }
}
