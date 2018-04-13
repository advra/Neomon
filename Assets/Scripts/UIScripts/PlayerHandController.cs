using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandController : MonoBehaviour {
    Card card;
    MonsterController playerController;
    UserController userController;
    BattleController BC;

    public GameObject cardPrefab;
    //public bool isPlayerTurn;
    public bool isDrawing;
    public bool initialDraw;

    public List<DrawEvent> drawQueue = new List<DrawEvent>();
    [SerializeField]
    public State state;
    public bool drawEvent;

    //contains all the cards to refer to
    public CardDatabase cardDatabase;

    public List<Card> deck;
    public List<Card> hand;
    public List<Card> graveyard;
    public List<GameObject> cardInHands;
    //the number of cards you can play this turn depending on its cost
    [SerializeField]
    private int cardCharge;
    [SerializeField]
    private int maxCardCharge;
    private int cardChargeLimit = 4;

    //the number of charges added per turn
    public int gainChargePerTurn = 1;

    //for UserInterface references especially in CardController
    //Used for spawning BattleTexts
    public GameObject parentCanvas;
    public GameObject UIChargeText;
    public GameObject playerReference;
    public GameObject enemyReferenceA;
    public GameObject enemyReferenceB;
    public GameObject enemyReferenceC;

    //used to cap amount of cards we can have in our hands
    //set to -1 for no limit
    public int maxHand = 6;
    public float cardGap;
    //this is for debugging
    public int deckLength = 0;
    public int handLength = 0;
    public int graveyardLength = 0;
    public int numberOfCardsInHand = 0;

    //if player can viewOrder via a skill then sort their deck to see what they can draw next
    public bool viewOrder; 

    //UI
    public GameObject DeckObject;
    public GameObject GraveyardObject;

    private ChargeText chargeText;

    public enum State
    {
        IDLE,
        DRAWING,
        DRAWING_DONE,
        SELECTING
    }

    //drawing properties on players turn
    //number of cards to draw at the beginning of battle, can change by default is 3
    private int beginningDrawNumberOfCards = 3;
    private int numberCardsToDrawEachTurn = 1;
    private int queuedCardDraw = 0;

    //called if a card is used and player waits until the players next turn
    public int DrawAdditionalCardsNextTurn
    {
        set
        {
            queuedCardDraw = value;
        }
    }

    public void AddCardToDeck()
    {
        deck.Add(cardDatabase.cards[3]);
    }

    public int CardCharge
    {
        get { return cardCharge;  }
    }

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

    public Vector3 NextCardPosition()
    {
        return this.transform.position;
    }

    public void DrawCards(int total)
    {
        if (!isDrawing)
        {
            Debug.Log("Error: DrawCard() not explicitely called from SetupHandForPlayer from MonsterController class.");
            //return;
        }

        //if done drawing then exit
        if (total == 0)
        {
            isDrawing = false;
            return;
        }

        Debug.Log(Time.time + " Drawing " + total + "cards");

        if (hand.Count == maxHand)
        {
            Debug.Log("Max hand limit reached");
        }

        //check if our deck is 0 then force shuffle from our graveyard graveyard
        if(deck.Count == 0 && graveyard.Count != 0)
        {
            ShuffleGraveToDeck();
        }

        //create the card and reorganize how it looks on screen
        CreateCardObject();
        FitAllCards();

        total--;

        //continue until we have no more left
        DrawCards(total);
    }

    public void AddToDrawQueue(int n)
    {
        if(n == 0 )
        {
            return;
        }

        Debug.Log("AddToDrawQueue() index " + n);

        DrawEvent newEvent = new DrawEvent(deck[0]);

        drawQueue.Add(newEvent);

        n--;

        AddToDrawQueue(n);

    }

    void ShuffleGraveToDeck()
    {
        deck = graveyard;
        graveyard = new List<Card>();
        Shuffle(deck);
    }

    public int NumberOfCardsInHand()
    {
        return cardInHands.Count;
    }

    public void DiscardHand()
    {
        //put all Card objects to discard
        //destroy all Game Objects

        for(int i = cardInHands.Count - 1; i >= 0 ; i--)
        {
            PlaceCardIn(cardInHands[i], graveyard);
        }
            
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
        //updateCount();
    }

    //place card via index
    public void PlaceCardIn(GameObject thisCard, List<Card> targetPile)
    {
        if(hand.Count != 0)
        {
            CardController cardController = thisCard.GetComponent<CardController>();
            int i = cardController.HandIndex;
            Debug.Log("Hand index is: " + i);
            targetPile.Add(hand[i]);
            hand.Remove(hand[i]);
            //we MUST remove the gameobject from the List BEFORE destroying it
            //AnimateCard(thisCard, thisCard, GraveyardObject, 1f);
            cardInHands.Remove(thisCard);
            //Note: Can potentially insert some card destroy animation when used here
            StartCoroutine(TimedDeath(thisCard));
            //Recompute Indexes
            RecomputeHandIndexes();
            //Update hand Graphics
            FitAllCards();
        }
        else
        {
            Debug.Log("Hand has zero cards to put into target pile!");
        }

    }

    //loses reference
    public void RecomputeHandIndexes(){
        for(int i = 0; i <= cardInHands.Count-1; i++)
        {
            cardInHands[i].GetComponent<CardController>().HandIndex = i;
        }
    }

    //For "Play" button
    public void Test()
    {
        //Draw(1);
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

    void CreateCardObject()
    {
        //Get the card data from our Deck then pass its attributes to our GameObject in the Hand 
        hand.Add(deck[0]);

        //create object and copy attributes
        //need to apply spacing
        //set this to tag with hand
        GameObject cardObj = GameObject.Instantiate(cardPrefab, this.transform);   
                                                                                   
        //cardObj.transform.SetParent(this.transform);
        //Apply the card attributes we need on the GameObject
        CardController cardController = cardObj.GetComponent<CardController>();
        cardController.InheritPropertiesFrom = deck[0];
        cardController.PlayerHand = this.gameObject;

        Image cardImage = cardObj.GetComponent<Image>();
        //visually format image
        cardInHands.Add(cardObj);
        cardController.HandIndex = cardInHands.Count-1;
        AnimateCard(cardObj, DeckObject, this.gameObject, 1f);
        FitAllCards();
        deck.Remove(deck[0]);
    }

    void Awake()
    {
        if(cardDatabase == null)
        {
            cardDatabase = Resources.Load<CardDatabase>("CardDatabase");
        }

        if(chargeText == null)
        {
            chargeText = UIChargeText.GetComponent<ChargeText>();
        }

        if(userController == null)
        {
            userController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UserController>();
        }
    }

    //user clicks this to end their turn
    public void EndTurn()
    {
        userController.IsUsersTurn = false;
        BC.PauseSpeedsForAllMonsters(false);

        if (playerController == null)
        {
            playerController = BC.playerController;
        }

        //this will ensure that if we have something like a combo card it will execute it 
        if(playerController.actions.Count == 0)
        {
            playerController.ResetAttack();
        }
        else
        {
            playerController.monsterState = MonsterController.State.CHARGING;
            PlayerTickController playerTickController = GameObject.FindGameObjectWithTag("PlayerTick").GetComponent<PlayerTickController>();
            playerTickController.ChangeState(PlayerTickController.GaugeState.CHARGING);
        }

    }

    public bool PlayerHasEnoughCharges(int cost)
    {
        if (cost <= cardCharge)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AdjustCost(int i)
    {
        cardCharge -= i;
        if (cardCharge < 0)
            cardCharge = 0;
        chargeText.UpdateCount(cardCharge, maxCardCharge);
    }

    void IncrementCostBy(int i)
    {
        cardCharge += i;
        maxCardCharge += i;
        if(maxCardCharge > cardChargeLimit)
        {
            maxCardCharge = cardChargeLimit;
        }
        chargeText.UpdateCount(cardCharge, maxCardCharge);
    }

    //draw cards for first time then 1 card each turn after
    public void SetupHand()
    {
        state = State.DRAWING;
    }

    public IEnumerator AnimateCard(GameObject card, GameObject startObj, GameObject endObj, float duration)
    {
        float startTime;
        float totalDistance;
        Vector3 startPos = startObj.transform.position;
        Vector3 endPos = endObj.transform.position;

        startTime = Time.time;
        totalDistance = Vector3.Distance(startPos, endPos);
        yield return DrawCard(card, startPos, endPos, duration);
    }

    IEnumerator DrawCard(GameObject card, Vector3 a, Vector3 b, float time)
    {
        float i = 0.0f;
        float rate = 1.0f / time; //* speed;
        //destination not reached
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            card.transform.position = Vector3.Lerp(a, b, i);
            yield return null;
        }
        cardInHands.Remove(card);
    }

    void OnPlayerTurn()
    {
        IncrementCostBy(1);

        if (!initialDraw)
        {
            //beginning of the battle, draw x cards
            DrawCards(beginningDrawNumberOfCards);
            //setup our energy charge numbers
            maxCardCharge = 1;
            cardCharge = 1;
            chargeText.UpdateCount(cardCharge, maxCardCharge);
            initialDraw = true;
        }
        else
        {
            //draw x cards each turn and an extra or less
            DrawCards(numberCardsToDrawEachTurn + queuedCardDraw);
            //when done reset the number of cards to draw next turn
            queuedCardDraw = 0;
        }
       
        state = State.SELECTING;

        drawEvent = false;
    }

    // Use this for initialization
    void Start()
    {
        if (BC == null)
        {
            BC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        }

        if (parentCanvas == null)
        {
            parentCanvas = GameObject.FindGameObjectWithTag("Canvas");
        }

        if (cardPrefab == null)
        {
            cardPrefab = Resources.Load<GameObject>("CardPrefab");
        }

        cardInHands = new List<GameObject>();
        deck = new List<Card>();
        hand = new List<Card>();
        graveyard = new List<Card>();

        //we can populate more in a seperate class later
        Card sliceCard = cardDatabase.cards[0];
        Card thrustCard = cardDatabase.cards[1];
        Card entangleCard = cardDatabase.cards[2];
        Card firstStrikeCard = cardDatabase.cards[3];
        Card shortCircuitCard = cardDatabase.cards[4];
        Card armorUpCard = cardDatabase.cards[5];
        Card recompileCard = cardDatabase.cards[6];
        Card reloadCard = cardDatabase.cards[7];

        //add cards
        for (int i = 0; i < 3; i++)
        {
            deck.Add(sliceCard);
            deck.Add(thrustCard);
            deck.Add(firstStrikeCard);
            deck.Add(shortCircuitCard);
            deck.Add(armorUpCard);
            deck.Add(recompileCard);
            deck.Add(reloadCard);
        }

        deck.Add(entangleCard);

        //shuffle our deck
        Shuffle(deck);

    }

    void Update()
    {
        switch (state)
        {
            case (State.IDLE):
                break;
            case (State.DRAWING):
                OnPlayerTurn();
                break;
            case (State.SELECTING):
                break;
        }

    }
}
