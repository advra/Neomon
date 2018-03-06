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

    //for UserInterface references especially in CardController
    //Used for spawning BattleTexts
    public GameObject parentCanvas;
    public GameObject playerReference;
    public GameObject enemyReferenceA;
    public GameObject enemyReferenceB;
    public GameObject enemyReferenceC;

    //used to cap amount of cards we can have in our hands
    //set to -1 for not limit
    public int maxHand = 6;
    public float cardGap;
    //this is for debugging
    public int deckLength = 0;
    public int handLength = 0;
    public int graveyardLength = 0;
    public int cardsInHandLength = 0;

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

    //graphicaly space out cards
    //Compute the card gap between each cards (from the origin) by how many cards are on screen
    //more = less gap = more overlapping!
    public float GetCardGap() {
        int cards = cardInHands.Count;
        if (cards < 5) {
            return 265.0f;
        }
        else if (cards > 4) {
            return (-26.2f * cards) + 391.81f;
        }
        else if (cards > 9)
        {
            return 45.0f;
        }
        else
        {
            return 45.0f;
        }
    }

    public void FitCardAtIndex(GameObject card, int i)
    {
        card.transform.localPosition = new Vector3((i * GetCardGap()), 100, 0);
        card.GetComponent<CardController>().OriginalPosition = card.transform.localPosition;
    }

    public void FitAllCards()
    {
        for (int i = 0; i < cardInHands.Count; i++)
        {
            GameObject card = cardInHands[i];
            card.transform.localPosition = new Vector3((i * GetCardGap()), 100, 0);
            card.GetComponent<CardController>().OriginalPosition = card.transform.localPosition;
        }
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
                //Get the card data from our Deck then pass its attributes to our GameObject in the Hand 
                hand.Add(deck[0]);
                //create object and copy attributes
                GameObject cardObj = GameObject.Instantiate(cardPrefab, this.transform);   //need to apply spacing
                //set this to tag with hand
                cardObj.transform.SetParent(this.transform);
                CardController cardController = cardObj.GetComponent<CardController>();
                cardController.cardName = deck[0].Name;
                cardController.cardSprite = deck[0].Sprite;
                cardController.damageAmount = deck[0].Damage;
                cardController.targetArea = deck[0].TargetArea;
                Image cardImage = cardObj.GetComponent<Image>();
                cardImage.sprite = Resources.Load<Sprite>("Sprites/" + cardController.cardSprite);
                //visually format image
                cardInHands.Add(cardObj);
                cardController.HandIndex = cardsInHandLength;
                FitAllCards();
                //FitCardAtIndex(cardObj, cardController.HandIndex);
                //cardController.OriginalPosition = cardObj.transform.localPosition;
                deck.Remove(deck[0]);
                FitAllCards();
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
                    Draw(); //Need to double Draw otherwise cards will shuffle but not draw this turn
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
            GameObject.Destroy(cardInHands[0]);
        }
        else
        {
            Debug.Log("Hand has zero cards to put into target pile!");
        }
        updateCount();
    }

        public void PlaceCardIn(GameObject thisCard, List<Card> targetPile)
    {
        if(hand.Count != 0)
        {
            CardController cardController = thisCard.GetComponent<CardController>();
            int index = cardController.HandIndex;
            Debug.Log("Hand index is: " + index);
            targetPile.Add(hand[index]);
            hand.Remove(hand[index]);
            //we MUST remove the gameobject from the List BEFORE destroying it
            cardInHands.Remove(thisCard);
            //Note: Can potentially insert some card destroy animation when used here
            StartCoroutine(TimedDeath(thisCard));
            //Recompute Indexes
            updateCount();
            RecomputeHandIndexes();
            //Update hand Graphics
            FitAllCards();
        }
        else
        {
            Debug.Log("Hand has zero cards to put into target pile!");
        }
        //updateCount();
    }

    //loses reference
    public void RecomputeHandIndexes(){
        for(int i = 0; i < cardsInHandLength; i++)
        {
            cardInHands[i].GetComponent<CardController>().HandIndex = i;
        }
    }

    public void updateCount()
    {
        deckLength = deck.Count;
        handLength = hand.Count;
        graveyardLength = graveyard.Count;
        cardsInHandLength = cardInHands.Count;
    }

    //For "Play" button
    public void Test()
    {
        Draw();
    }
    //For "Draw" button
    public void Test2()
    {
        PlaceCardIn(graveyard);
    }

    //use this to destroy game object delayed by couple seconds to prevent crashing?
    IEnumerator TimedDeath(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        GameObject.Destroy(obj);
    }

    // Use this for initialization
    void Start()
    {
        playerReference = GameObject.FindGameObjectWithTag("PlayerPos");
        enemyReferenceA = GameObject.FindGameObjectWithTag("EnemyPosA");
        enemyReferenceB = GameObject.FindGameObjectWithTag("EnemyPosB");
        enemyReferenceC = GameObject.FindGameObjectWithTag("EnemyPosC");

        parentCanvas = GameObject.FindGameObjectWithTag("Canvas");
        if(parentCanvas == null)
        {
            Debug.Log("Parent canvas is null! GUI Objects may not properly be attached");
        }
        cardPrefab = Resources.Load<GameObject>("CardPrefab");
        if (cardPrefab == null)
        {
            Debug.Log("Card Prefab is Null. Cannot find it in Resources folder.");
        }

        cardInHands = new List<GameObject>();
        deck = new List<Card>();
        hand = new List<Card>();
        graveyard = new List<Card>();

        //we can populate more in a seperate class later
        Card sliceCard = new Card("Slice", "Melee Attack a Single Enemy", 5, TargetArea.single, "card_slice_5");
        Card thrustCard = new Card("Thrust", "Penetrate Enemies within a Line", 2, TargetArea.line, "card_thrust_2");
        Card entangleCard = new Card("Entangle", "Deal Damage to All Enemies", 3, TargetArea.all, "card_entangle_3");

        //add cards
        for (int i = 0; i < 5; i++)
        {
            deck.Add(sliceCard);
            deck.Add(thrustCard);
        }
        deck.Add(entangleCard);
        Shuffle(deck);

        updateCount();
    }

    void SetupHand()
    {
        Draw();
    }
}
