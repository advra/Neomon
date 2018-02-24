using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Monster
{

    private int monID = 0;
    private string monName;
    private int monLevel;
    private int monMaxHealth;
    private int monCurrentHealth;
    private int monSpeed;
    private int monAttack;
    private int monDefense;
    private string monSpriteFile;

    public int ID
    {
        get { return monID; }
        set { monID = value; }
    }

    public string Info
    {
        get
        {
            return "Spawned a " + this.Name + " lv. " + this.Level + " (HP:" + this.Health + ")";

        }
    }

    public string Name
    {
        get { return monName; }
    }

    public int Level
    {
        get { return monLevel; }
        set { monLevel = value; }
    }


    public int Health
    {
        get { return monCurrentHealth; }
        set { monCurrentHealth = value; }
    }

    public int Speed
    {
        get { return monSpeed; }
        set { monSpeed = value; }
    }

    public int Attack
    {
        get { return monAttack; }
        set { monAttack = value; }
    }

    public int Defense
    {
        get { return monDefense; }
        set { monDefense = value;  }
    }

    public string SpriteFile
    {
        get { return monSpriteFile; }
    }

    public void SetHealth(int h)
    {
        this.monCurrentHealth = h;
    }

    public void RemoveHealth(int h)
    {
        this.monCurrentHealth -= h;
        if (this.monCurrentHealth < 0)
        {
            this.monCurrentHealth = 0;
        }
    }

    //Increase Base attributes by x amount depending on level and other factors
    //will revisit this method for game balancing
    public void LevelMultiplier(int level)
    {
        this.monMaxHealth *= level;
        this.monAttack *= level;
        this.monSpeed *= level;
        this.monDefense *= level;
    }


    public void SetBaseAttributes(int maxHealth, int speed, int attack, int defense)
    {
        if (maxHealth <= 0)
        {
            //The lowest health allowed is 1
            this.monMaxHealth = 1;        
            this.monCurrentHealth = 1;
        }

        this.monMaxHealth = maxHealth;
        this.monCurrentHealth = maxHealth;
        this.monSpeed = speed;
        this.monAttack = attack;
        this.monDefense = defense;

    }

    //Create a monster checking Strings are Arropriate before assigning it to an actual Game Object
    public Monster (string name, string fileName)
    {
        this.monName = name;
        this.monSpriteFile = fileName;
        this.monLevel = 1;
    }

}




