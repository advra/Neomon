using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Monster
{
    private string name;
    private int level;
    private int maxHealth;
    private int currentHealth;
    private int speed;
    private int attack;
    private int defense;
    private MonsterInfo monsterInfo;

    public string Print
    {
        get { return "Spawned: " + this.name + " Lv. " + this.level + " HP:" + currentHealth + "/" + maxHealth; }
    }

    // you'll find that you can't access Monster.SpriteFile anymore
    // instead, you would use Monster.MonsterInfo.SpriteFile
    // the same goes for Monster.MonsterInfo.Id
    public MonsterInfo MonsterInfo
    {
        get { return monsterInfo; }
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int Level
    {
        get { return level; }
    }

    public int Health
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    public int Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public int Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    public int Defense
    {
        get { return defense; }
        set { defense = value; }
    }

    public void SetHealth(int h)
    {
        this.currentHealth = h;
    }

    public void RemoveHealth(int h)
    {
        this.currentHealth -= h;
        if (this.currentHealth < 0)
        {
            this.currentHealth = 0;
        }
    }

    // In your previous code, setting level wouldn't immediately affect the monster's stats
    public void LevelUp()
    {
        level += 1;
        ResetMonsterStats();
    }

    public void ResetMonsterStats()
    {
        // Multiply base attributes by x amount depending on level and other factors
        // will revisit this method for game balancing
        this.maxHealth = monsterInfo.BaseHealth * level;
        this.currentHealth = monsterInfo.BaseHealth * level;
        this.speed = monsterInfo.BaseSpeed * level;
        this.attack = monsterInfo.BaseAttack * level;
        this.defense = monsterInfo.BaseDefense * level;
    }

    // each monster is constructed from a monster info and a level.
    // this way you can have as many of the same monster as you want.
    public Monster(MonsterInfo monsterInfo, int level)
    {
        this.monsterInfo = monsterInfo;
        this.level = level;

        // the default monster name is the type from MonsterInfo.
        this.name = monsterInfo.Type;

        ResetMonsterStats();
    }

}