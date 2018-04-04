using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//note need to expand element container as more gameobjects are added

public class DeckViewController : MonoBehaviour {

    public enum DeckList
    {
        DECK,
        GRAVE,
        NONE
    }

    PlayerHandController playerHandController;
    UserController userController;


    public GameObject DeckViewerPrefab;
    public DeckList deckView;
    [SerializeField]
    private List<Card> cards = new List<Card>();
    [SerializeField]
    private GameObject DeckViewer;
    [SerializeField]
    private RectTransform canvasRect;
    [SerializeField]
    private Canvas deckViewerCanvas;
    [SerializeField]
    private Button deckViewerBackButton;
    [SerializeField]
    RectTransform elementContainerRectTransform;
    [SerializeField]
    ScrollRect scrollRect;
    [SerializeField]
    private bool active;
    [SerializeField]
    private List<GameObject> elements = new List<GameObject>();
    [SerializeField]
    GameObject deckContainer;
    [SerializeField]
    private Text[] texts;

    public GameObject blankImagePrefab;

    public bool DeckWindowActive
    {
        get { return active; }
        set
        {
            active = value;
            DisplayCheck();
        }
    }


	// Use this for initialization
	void Start () {
		if(playerHandController == null)
        {
            //this will allow us to reference our deck
            playerHandController = GameObject.FindGameObjectWithTag("Hand").GetComponent<PlayerHandController>();
        }

        if(userController == null)
        {
            userController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UserController>();
        }

        if(canvasRect == null)
        {
            canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        }

        if(DeckViewerPrefab == null)
        {
            DeckViewerPrefab = Resources.Load<GameObject>("Menu/DeckViewWindowPrefab");
        }

        if(blankImagePrefab == null)
        {
            blankImagePrefab = Resources.Load<GameObject>("Menu/BlankImage");
        }
    }

    void DisplayCheck()
    {
        if (active)
        {
            DeckViewer.SetActive(true);
            userController.MenuIsActive = true;
            popoulateTable();

        }
        else
        {
            clearTable();
            DeckViewer.SetActive(false);
            userController.MenuIsActive = false;

        }
    }

    public void ViewCards()
    {
        //this will check if we have our object created and set w/ references
        CheckInitialization();

        active = !active;

        DisplayCheck();
    }


    void CheckInitialization()
    {
        DeckViewer = userController.DeckWindowGameObject;
        if (DeckViewer == null)
        {
            DeckViewer = Instantiate(DeckViewerPrefab, canvasRect.transform);
            userController.DeckWindowGameObject = DeckViewer;

            //assign backbutton reference to this object to acces Trigger backbutton
            DeckViewBackButton deckViewBackButton = DeckViewer.GetComponentInChildren<DeckViewBackButton>();
            deckViewBackButton.deckViewController = this.gameObject.GetComponent<DeckViewController>();
        }

        texts = DeckViewer.GetComponentsInChildren<Text>();

        if (deckContainer == null)
        {
            deckContainer = GameObject.FindGameObjectWithTag("DeckContainer");
        }

        if(scrollRect == null)
        {
            scrollRect = DeckViewer.GetComponentInChildren<ScrollRect>();
        }
    }

    void clearTable()
    {
 
        foreach (GameObject go in elements)
        {
            GameObject.Destroy(go);
        }

        elements = new List<GameObject>();
        elementContainerRectTransform.sizeDelta = new Vector2(676.6f, 312.2f);

    }

    void popoulateTable()
    {

        if (deckContainer == null)
        {
            deckContainer = GameObject.FindGameObjectWithTag("DeckContainer");
        }
        if(elementContainerRectTransform == null)
        {
            elementContainerRectTransform = deckContainer.GetComponent<RectTransform>();
        }


        GameObject cardPrefab = playerHandController.cardPrefab;
        

        if(deckView == DeckList.DECK)
        {
            cards = playerHandController.deck;
            texts[0].text = "Viewing Your Deck";
        }

        if (deckView == DeckList.GRAVE)
        {
            cards = playerHandController.graveyard;
            texts[0].text = "Viewing Your Graveyard";
        }

        int cardNum = cards.Count;

        if(cardNum == 0)
        {
            texts[1].enabled = true;
            texts[1].text = "No Cards";
            texts[2].text = "0 Cards";
            return;
        }
        else
        {
            string s = cardNum + " Cards";
            texts[2].text = s;
            texts[1].enabled = false;
        }
        

        for (int i = 0; i < cards.Count; i++)
        {
            //we add a row every 4 cards

            if (i > 7 && i % 4 == 0)
            {
                //every row increase the size of our element contain otherwise scroll wouldnt work properly
                Debug.Log(i);
                Vector2 currentSize = elementContainerRectTransform.sizeDelta;
                elementContainerRectTransform.sizeDelta = new Vector2(currentSize.x, currentSize.y + 220.00f);

                //once done changing the size set the scrollbar to top
                //scrollbar.value = 1.0f;
                scrollRect.normalizedPosition = new Vector2(0, 1);
            }

            //add a dummy gameobject then attach our card images as child to it otherwise scaling will act up oddly
            GameObject emptyObject = GameObject.Instantiate(blankImagePrefab, deckContainer.transform);
            GameObject cardObj = GameObject.Instantiate(cardPrefab, emptyObject.transform);
            //required for scroll rect
            cardObj.AddComponent<LayoutElement>();
            //need to remove event listeners otherwise our "Click to scroll" wont work and user is forced with scroll wheel only
            Destroy(cardObj.GetComponent<GraphicRaycaster>());
            cardObj.transform.localScale *= 0.5f;
            //add to element so when we close this window it will remove the objects until next time (for refreshing.. will check for drawing or changes in deck rather than destroying)
            elements.Add(emptyObject);

            //Apply the card attributes we need on the GameObject
            CardController cardController = cardObj.GetComponent<CardController>();
            cardController.name = cards[i].name;
            cardController.description = cards[i].description;
            cardController.damageAmount = cards[i].damage;
            cardController.targetArea = cards[i].targetArea;
            cardController.cost = cards[i].cost;
            cardController.chargeTime = cards[i].charge;
            cardController.isCanceling = cards[i].isCanceling;
            //cardController.PlayerHand = this.gameObject;
            //Image cardImage = cardObj.GetComponent<Image>();
            //cardController.enabled = false;
            cardController.SetAsDisplayElement = true;
            
        }

    }


	
}
