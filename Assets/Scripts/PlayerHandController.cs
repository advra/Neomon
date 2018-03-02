using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandController : MonoBehaviour {
    CardDatabase cardDatabase;
    Card card;
    public GameObject cardPrefab;
    public bool playerTurn;
    public bool draw;

    public List<Card> deck;
    public List<Card> hand;
    public List<Card> graveyard;

    public List<GameObject> cardInHands;
    public int numberOfCards = 0;

    //used to cap amount of cards we can have in our hands
    //set to -1 for not limit
    public int maxHand = 4;
    public float cardGap = 1.0f;
    //this is for debugging
    public int deckLength = 0;
    public int handLength = 0;
    public int graveyardLength = 0;
    public List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7 };

    public void Shuffle(List<int> ints)
    {
        int length = ints.Count;
        for (int i = length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i);
            int temp = ints[i];
            ints[i] = ints[j];
            ints[j] = temp;
        }
    }

    public void Shuffle(List<Card> cards)
    {
        int length = cards.Count;
        for (int i = length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i);
            Card temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    //here we will evenly space
    public void FitCard(GameObject card)
    {
        if (hand.Count == 0)
            return;

        GameObject parentCard = cardInHands[0];
        card.transform.position = parentCard.transform.position;
        card.transform.position += new Vector3(this.transform.position.x, (numberOfCards * cardGap), this.transform.position.z);
    }

    //Draws One card from Deck to Hand 
    //Can be changed for more draws later
    public void Draw()
    {
        if(hand.Count != maxHand)
        {
            //draw as normal
            if (deck.Count != 0)
            {
                //add the card to our hand from the deck 
                hand.Add(deck[0]);

                //create object and copy attributes
                GameObject card = GameObject.Instantiate(cardPrefab, this.transform);   //need to apply spacing
                CardController cardController = card.GetComponent<CardController>();
                cardController.cardName = deck[0].Name;
                cardController.cardSprite = deck[0].Sprite;
                Image cardImage = card.GetComponent<Image>();
                cardImage.sprite = Resources.Load<Sprite>("Sprites/" + cardController.cardSprite);

                //visually format image
                cardInHands.Add(card);
                FitCard(card);
                numberOfCards = handLength;


                //then remove it
                deck.Remove(deck[0]);
            }
            //if deck is 0 then
            else
            {
                //check graveyard
                if (graveyard.Count != 0)
                {
                    //shuffle cards from graveyard to the deck
                    deck = graveyard;
                    graveyard = new List<Card>();   //use this instead of .Clear
                    Shuffle(deck);
                    Debug.Log("Shuffled cards from graveyard back to the deck");
                }
                //otherwise do nothing
                else
                {
                    Debug.Log("Nothing left in Graveyard to shuffle back into the deck");
                }
            }
        }
        else
        {
            Debug.Log("Cannot draw anymore cards! Max hand limit is reached");
        }
        updateCount();
    }

    //called when card is used
    //will move card from hand to graveyard or Deck depending on target
    public void PlaceCardIn(List<Card> targetPile)
    {
        if(hand.Count != 0)
        {
            targetPile.Add(hand[0]);
            hand.Remove(hand[0]);
        }
        else
        {
            Debug.Log("Hand has zero cards to put into target pile!");
        }
        updateCount();
    }

    public void updateCount()
    {
        deckLength = deck.Count;
        handLength = hand.Count;
        graveyardLength = graveyard.Count;
    }

    public void Test()
    {
        Draw();
    }

    public void Test2()
    {
        PlaceCardIn(graveyard);
    }

    // Use this for initialization
    void Start()
    {
        cardInHands = new List<GameObject>();
        deck = new List<Card>();
        hand = new List<Card>();
        graveyard = new List<Card>();

        //we can populate more in a seperate class later
        Card attackCard = new Card("Attack", "Melee Attack a Single Enemy", "card_attack");

        //add 10 cards
        for(int i = 0; i < 10; i++)
        {
            deck.Add(attackCard);
        }
        updateCount();
    }

    void SetupHand()
    {
        Draw();
    }

    void Update()
    {
        
    }

}
