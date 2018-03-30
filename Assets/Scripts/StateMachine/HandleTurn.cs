using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Serializable allows this class to be viewable in the Inspector
[System.Serializable]
public class HandleTurn {
    //unit who is performing this action
    public GameObject owner;
    public GameObject target;
    //area of effrct, single, self, all, random
    public TargetArea targetArea;
    public AttackType attackType;
    public int damage;
    public float chargeTime;

    //player event constructor
    //constructor if not specific single attacker
    public HandleTurn(GameObject owner, GameObject target, TargetArea targetArea, int damage, float chargeTime)
    {
        this.owner = owner;
        if(targetArea == TargetArea.single)
        {
            this.target = target;
        }
        else
        {
            this.target = null;
        }
        this.targetArea = targetArea;
        this.damage = damage;
        this.chargeTime = chargeTime;
    }

    //constructor for enemy
    public HandleTurn(GameObject owner, GameObject target, AttackType attackType, int damage, float chargeTime)
    {
        this.owner = owner;
        this.target = target;
        this.attackType = attackType;
        this.damage = damage;
        this.chargeTime = chargeTime;
    }

}
