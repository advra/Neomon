using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Database", menuName = "Game/Card Database")]
public class CardDatabase : ScriptableObject {
    public Card[] cards;
}