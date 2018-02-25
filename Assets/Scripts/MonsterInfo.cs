using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo
{
    private int monId;
    private string monType;
    private int monBaseHealth;
    private int monBaseSpeed;
    private int monBaseAttack;
    private int monBaseDefense;
    private string monSpriteFile;

    public MonsterInfo(int id, string type, string spriteFile, int baseHealth, int baseSpeed, int baseAttack, int baseDefense)
    {
        // you may want to introduce restrictions here.
        // for example, here you can check that all of your base stats are greater than 0
        monId = id;

        // note that in MonsterInfo, monsters get a type rather than a name
        monType = type;
        monSpriteFile = spriteFile;
        monBaseHealth = baseHealth;
        monBaseSpeed = baseSpeed;
        monBaseAttack = baseAttack;
        monBaseDefense = baseDefense;
    }


    public int Id
    {
        get { return monId; }
    }

    public string Type
    {
        get { return monType; }
    }

    public int BaseHealth
    {
        get { return monBaseHealth; }
    }

    public int BaseAttack
    {
        get { return monBaseAttack; }
    }

    public int BaseDefense
    {
        get { return monBaseDefense; }
    }

    public int BaseSpeed
    {
        get { return monBaseSpeed; }
    }

    public string SpriteFile
    {
        get { return monSpriteFile; }
    }
}