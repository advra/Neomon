using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour {

    private string monsterName;
    private int level;
    private int maxHealth;
    private int currentHealth;
    private int speed;
    private int attack;
    private int defense;
    private Sprite sprite;

    public string GetName()
    {
        return this.monsterName;
    }

    public int GetLevel()
    {
        return this.level;
    }


    public int GetHealth()
    {
        return this.currentHealth;
    }

    public void SetHealth (int h)
    {
        this.currentHealth = h;
    }

    public void RemoveHealth(int h)
    {
        this.currentHealth -= h;
        if(this.currentHealth < 0)
        {
            this.currentHealth = 0;
        }
    }


    public void SetAttributes(int maxHealth, int speed, int attack, int defense)
    {
        if (maxHealth == 0)
        {
            this.maxHealth = 1;         //minimum health can only be 1
            this.currentHealth = 1;
        }

        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
        this.speed = speed;
        this.attack = attack;
        this.defense = defense;

    }

    
    public class Mushroom : Monster
    {
        public Mushroom(string name, int level, string spriteFile)
        {
            this.monsterName = name;
            if(level <= 0)
                Debug.Log("Level must at least equal 1");
            this.level = level;
            if (spriteFile == null)
            {
                Debug.Log("Monster Sprite cannot be null!");
            }
        }
    }

}




