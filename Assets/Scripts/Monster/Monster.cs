using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Monster
{
    private MonsterBase monsterBase;
    private string name;
    private int level;
    private int maxHealth;
    private int currentHealth;
    private float speed;
    private float attack;
    private float defense;
    private bool dead;
    private int exp;
    private List<Attack> moveSet;
    
    public string Print
    {
        get { return "Spawned: " + this.name + " Lv. " + this.level + " HP:" + currentHealth + "/" + maxHealth; }
    }

    // you'll find that you can't access Monster.SpriteFile anymore
    // instead, you would use Monster.monsterBase.SpriteFile
    // the same goes for Monster.monsterBase.Id
    public MonsterBase MonsterBase
    {
        get { return monsterBase; }
    }

    public List <Attack> MoveSet
    {
        get
        {
            return moveSet;
        }
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
        get
        {
            if(currentHealth <= 0)
            {
                dead = true;
                return 0;
            }
            return currentHealth;
        }
        set { currentHealth = value; }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public float Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    public float Defense
    {
        get { return defense; }
        set { defense = value; }
    }

    public bool IsDead
    {
        get { return dead;  }
    }

    public void SetHealth(int h)
    {
        this.currentHealth = h;
    }

    public void ChangeHealth(int h)
    {
        this.currentHealth += h;
        if (this.currentHealth < 0)
        {
            this.currentHealth = 0;
            this.dead = true;
        }
    }

    public float DoAttack(float currentSpeed, float attackCost)
    {
        return currentSpeed - attackCost;
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
        this.maxHealth = monsterBase.baseHealth * level;
        this.currentHealth = monsterBase.baseHealth * level;
        this.speed = monsterBase.baseSpeed * level;
        this.attack = monsterBase.baseAttack * level;
        this.defense = monsterBase.baseDefense * level;
    }

    // each monster is constructed from a monster info and a level.
    // this way you can have as many of the same monster as you want.

    public Monster(MonsterBase monsterBase, int level)
    {
        this.monsterBase = monsterBase;
        this.level = level;
        this.name = monsterBase.name;
        this.moveSet = monsterBase.moveSet;

        ResetMonsterStats();
    }

}