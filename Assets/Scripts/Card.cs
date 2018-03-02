using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {

    private string name;
    private string description;
    private string sprite;
    private TypeOfCard CardType;

    enum TypeOfCard
    {
        attack = 0,
        defense = 1
    }

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
