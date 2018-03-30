using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase {
    Card card;

    public Card attackCard;

    public void Populate()
    {
        attackCard = new Card("Attack", "Melee Attack a Single Enemy", "card_attack");
    }
}
