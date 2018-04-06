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
    public int cardsInHandLength = 0;

    private ChargeText chargeText;

    public enum State
    {
        IDLE,
        DRAWING,
        SELECTING
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

    //Draws One card from Deck to Hand 
    //Can be changed for more draws later
    public void Draw(int number)
    {
        for(int i = 0; i < number; i++)
        {
            Debug.Log("Drawing card " + i);
            if (hand.Count != maxHand)
            {
                initialDraw = true;

                //draw as normal
                if (deck.Count != 0)
                {
                    //Get the card data from our Deck then pass its attributes to our GameObject in the Hand 
                    hand.Add(deck[0]);
                    //create object and copy attributes
                    GameObject cardObj = GameObject.Instantiate(cardPrefab, this.transform);   //need to apply spacing
                                                                                               //set this to tag with hand
                    cardObj.transform.SetParent(this.transform);
                    //Apply the card attributes we need on the GameObject
                    CardController cardController = cardObj.GetComponent<CardController>();
                    cardController.name = deck[0].name;
                    cardController.description = deck[0].description;
                    cardController.damageAmount = deck[0].damage;
                    cardController.block = deck[0].block;
                    cardController.targetArea = deck[0].targetArea;
                    cardController.cost = deck[0].cost;
                    cardController.chargeTime = deck[0].charge;
                    cardController.isCanceling = deck[0].isCanceling;
                    cardController.isChainCombo = deck[0].chainCombo;
                    cardController.stunNumberOfTurns = deck[0].stunNumberOfTurns;
                    cardController.PlayerHand = this.gameObject;
                    Image cardImage = cardObj.GetComponent<Image>();
                    //visually format image
                    cardInHands.Add(cardObj);
                    cardController.HandIndex = cardsInHandLength;
                    FitAllCards();
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
                        Draw(1); //Need to double Draw otherwise cards will shuffle but not draw this turn
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
        Draw(1);
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

    void Awake()
    {
        if(cardDatabase == null)
        {
            cardDatabase = Resources.Load<CardDatabase>("CardDatabase");
        }
        //if (playerController == null)
        //{
        //    playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<MonsterController>();
        //}
        if(chargeText == null)
        {
            chargeText = UIChargeText.GetComponent<ChargeText>();
        }
        if(userController == null)
        {
            userController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UserController>();
        }
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
        Card firstStrike = cardDatabase.cards[3];
        Card shortCircuit = cardDatabase.cards[4];
        Card armorUp = cardDatabase.cards[5];

        //add cards
        for (int i = 0; i < 5; i++)
        {
            deck.Add(sliceCard);
            deck.Add(thrustCard);
            deck.Add(firstStrike);
            deck.Add(armorUp);
        }
        deck.Add(entangleCard);

        Shuffle(deck);

        updateCount();
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
        if (!initialDraw)
        {
            Draw(3);
            cardCharge = 1;
            maxCardCharge = 1;
            chargeText.UpdateCount(cardCharge, maxCardCharge);
        }
        else
        {
            Draw(1);
            IncrementCostBy(1);
        }
    }
}
