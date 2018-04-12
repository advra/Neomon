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
    //public string attackName;
    //public string attackDescription;
    public int damage;
    public int block;
    public float chargeTime;
    public bool isCanceling;
    public bool stun;
    public int stunNumberOfTurns;
    public DrawType drawType;

    public HandleTurn(GameObject owner, List<GameObject> target, TargetArea targetArea, int damage, int block, float chargeTime, bool isCanceling, int stunNumberOfTurns, DrawType drawType)
    {
        this.owner = owner;
        this.targets = target;
        this.targetArea = targetArea;
        this.damage = damage;
        this.block = block;
        this.chargeTime = chargeTime;
        this.isCanceling = isCanceling;
        this.stunNumberOfTurns = stunNumberOfTurns;
        if (stunNumberOfTurns <= 0)
        {
            stun = false;
        }

        //these only affects the player
        this.drawType = drawType;
    }

    public void Clear()
    {
        this.owner = null;
        this.targets = null;
        this.targetArea = TargetArea.NOT_SET;
        this.damage = 0;
        this.block = 0;
        this.chargeTime = 0;
        this.isCanceling = false;
        this.stunNumberOfTurns = 0;
        this.stun = false;
        this.drawType = DrawType.NONE;
    }

}
