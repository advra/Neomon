using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrawEvent {

    public Card card;

    public DrawEvent(Card card)
    {
        this.card = card;
    }
}
