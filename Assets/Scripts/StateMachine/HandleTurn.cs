using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Serializable allows this class to be viewable in the Inspector
[System.Serializable]
public class HandleTurn {
    //unit who is performing this action
    public GameObject owner;
    public List<GameObject> targets = new List<GameObject>();
    //area of effrct, single, self, all, random
    public TargetArea targetArea;
    public AttackType attackType;
    //public string attackName;
    //public string attackDescription;
    public int damage;
    public float chargeTime;
    public bool isCanceling;

    //player event constructor
    //constructor if not specific single attacker
    //public HandleTurn(GameObject owner, List<GameObject> target, TargetArea targetArea, int damage, float chargeTime)
    //{
    //    this.owner = owner;
    //    this.targets = target;
    //    this.targetArea = targetArea;
    //    this.damage = damage;
    //    this.chargeTime = chargeTime;
    //}
    //for canceling cards and others of special type
    public HandleTurn(GameObject owner, List<GameObject> target, TargetArea targetArea, int damage, float chargeTime, bool isCanceling)
    {
        this.owner = owner;
        this.targets = target;
        this.targetArea = targetArea;
        this.damage = damage;
        this.chargeTime = chargeTime;
        this.isCanceling = isCanceling;
    }

    //constructor for enemy
    public HandleTurn(GameObject owner, List<GameObject> target, AttackType attackType, int damage, float chargeTime, bool isCanceling)
    {
        this.owner = owner;
        this.targets = target;
        this.attackType = attackType;
        this.damage = damage;
        this.chargeTime = chargeTime;
        this.isCanceling = isCanceling;
    }

}
