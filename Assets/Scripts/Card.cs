using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {

    private string name;
    private string description;
    private string sprite;
    private int cardType;

    /*Target Area of Card:
        o   single                          default
        v   Split
        l   line                            penetrating enemies standing in front of another
        a   all enemies                     
        s   self                            Good for self buff cards
        ?   random enemy
    */
    private char targetArea; 

    public string Name {
        get{ return name;}
    }

    public string Sprite
    {
        get { return sprite; }
    }

    public Card(string name, string description, string sprite)
    {
        this.name = name;
        this.description = description;
        this.sprite = sprite;
    }
}
