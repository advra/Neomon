using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Enum is in Card.cs
//public enum TargetArea
//{
//    SINGLE = 0,
//    ALL,
//    SPLIT,
//    LINE,
//    SELF,
//    RANDOM
//}

[System.Serializable]
public class Attack {
    public string name;
    public string description;
    public TargetArea targetArea;
    public int damage;
    public int block;
    public float chargeTime;
    public bool isCanceling;    //will this attack cancel other attacks?
    public bool isRepeating;
    public int repeatTimes;

    public Attack(TargetArea targetArea, string name, string description, int damage, int block, float chargeTime, bool isCanceling)
    {
        this.name = name;
        this.description = description;
        this.damage = damage;
        this.block = block;
        this.chargeTime = chargeTime;
        this.targetArea = targetArea;
        this.isRepeating = false;
        this.isCanceling = isCanceling;
    }

    public Attack(TargetArea targetArea, string name, string description, int damage, int block, float chargeTime, bool isRepeating, int repeatTimes)
    {
        this.name = name;
        this.description = description;
        this.damage = damage;
        this.block = block;
        this.chargeTime = chargeTime;
        this.targetArea = targetArea;
        this.isRepeating = isRepeating;
        this.repeatTimes = repeatTimes;
    }
}
