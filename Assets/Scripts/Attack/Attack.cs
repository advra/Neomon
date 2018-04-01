using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    SINGLE,     //one enemy target
    LINE,       //enemy target in a line
    SPLIT,      //in a V pattern        
    SELF,       //buffs or heals to caster
    ALL,        //every enemy
    ALL_MON,    //all monsters in the map
    RANDOM      //random enemy (includes bouncing attacks)
}

[System.Serializable]
public class Attack {
    public string name;
    public string description;
    public AttackType attackType;
    public int damage;
    public float chargeTime;
    public bool isCanceling;    //will this attack cancel other attacks?
    public bool isRepeating;
    public int repeatTimes;

    public Attack(AttackType attackType, string name, string description, int damage, float chargeTime, bool isCanceling)
    {
        this.name = name;
        this.description = description;
        this.damage = damage;
        this.chargeTime = chargeTime;
        this.attackType = attackType;
        this.isRepeating = false;
        this.isCanceling = isCanceling;
    }

    public Attack(AttackType attackType, string name, string description, int damage, float chargeTime, bool isRepeating, int repeatTimes)
    {
        this.name = name;
        this.description = description;
        this.damage = damage;
        this.chargeTime = chargeTime;
        this.attackType = attackType;
        this.isRepeating = isRepeating;
        this.repeatTimes = repeatTimes;
    }
}
