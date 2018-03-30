using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterBase
{
    private int id = 0;
    public string name;
    public string description;
    public string spriteFile;
    public int baseHealth;
    public int baseAttack;
    public int baseDefense;
    public int baseSpeed;
    public BaseType baseType;
    public List<Attack> moveSet = new List<Attack>();    //defines what attacks this monster has

    public enum BaseType
    {
        FUNGI,
        PYRO,
        ACQUA
    }

    //called each constructor even from inherited classes
    public MonsterBase()
    {
        id++;
    }

    //call this after creating the monster to add generic attack not tied to their baseType
    public void AddToMoveSet(Attack attack)
    {
        moveSet.Add(attack);
    }

}