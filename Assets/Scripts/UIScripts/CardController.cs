using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading;

[System.Serializable]
public class CardController : MonoBehaviour, IDragHandler, IEndDragHandler {

    public enum State
    {
        NORMAL,
        DRAG,
        RESET,
        DESTROY
    }

    BattleController battleController;
    PlayerHandController playerHandController;
    MonsterController playerController;
    MonsterController monsterController;
    UserController userController;

    //not to be confused with the Hand Canvas
    Canvas canvas;
    Image cardImage;
    [SerializeField]
    private int handIndex;
    [SerializeField]
    Card card;
    Text[] texts;
    Image[] images;

    public GameObject PlayerHand;

    public new string name;
    public string description;
    public Sprite sprite;
    public TargetArea targetArea;
    public int damageAmount;
    public int block;
    public int cost;
    public float chargeTime;
    public bool isCanceling;
    public bool isChainCombo;
    public int stunNumberOfTurns;
    public DrawType drawType;
    public int drawNumber;
    public bool onNextTurn;

    public State state; 
    [SerializeField]
    private Vector3 originalPosition;
    public GameObject BattleTextPrefab;
    RaycastHit hitInfo;
    RaycastHit2D hit;
    Ray ray;
    Ray screenRay;
    public GameObject selectedTarget;
    [SerializeField]
    bool resetDisplay;
    [SerializeField]
    bool displayElement;

    public Card InheritPropertiesFrom
    {
        set
        {
            this.card = value;

            //inherit all properties to the card we just set
            //setup the card properties
            this.name = card.name;
            this.description = card.description;
            this.damageAmount = card.damage;
            this.block = card.block;
            this.targetArea = card.targetArea;
            this.cost = card.cost;
            this.chargeTime = card.charge;
            this.isCanceling = card.isCanceling;
            this.isChainCombo = card.chainCombo;
            this.stunNumberOfTurns = card.stunNumberOfTurns;
            this.drawType = card.drawType;
            this.drawNumber = card.drawNumber;
            this.onNextTurn = card.onNextTurn;
        }
    }

    //used if we use this gameobject as a visiual menu display we do not want to have it interact any battle sequence, only display its data
    public bool SetAsDisplayElement
    {
        set { displayElement = value; }
    }

    public Vector3 OriginalPosition{
        get{return originalPosition;}
        set{originalPosition = value;}
    }

    public int HandIndex{
        get {return handIndex; }
        set { handIndex = value; }
    }

    void Awake()
    {
        if(cardImage == null){
            cardImage = GetComponent<Image>();
        }
        
        if(canvas == null){
            canvas = GetComponent<Canvas>();
        }
        
        if(playerHandController == null){
            playerHandController = GetComponentInParent<PlayerHandController>();
        }
        
        if (battleController == null)
        {
            battleController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        }
        
        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<MonsterController>();
        }
        if(userController == null)
        {
            userController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UserController>();
        }

    }

    void SetupCard()
    {

        //used to reference all the text gameobjects
        texts = GetComponentsInChildren<Text>();
        images = GetComponentsInChildren<Image>();
        //Greater is in front. 10 to ensure it is in front of all other GUI.
        canvas.sortingOrder = 2;

        //texts[0].text = cardName;         //this is for the Cost Text which we wont change
        texts[1].text = cost.ToString();
        texts[2].text = name;

        if (name.Length > 10)
        {
            texts[2].fontSize = 30;
        }

        if (description.Length > 30)
        {
            texts[3].fontSize = 16;
        }

        if (description.Length > 20)
        {
            texts[3].fontSize = 20;
        }

        texts[3].text = description;
        //texts[3].text = description;
        if (targetArea == TargetArea.SINGLE)
        {
            images[1].sprite = Resources.Load<Sprite>("Sprites/type_single");
        }
        if (targetArea == TargetArea.ALL)
        {
            images[1].sprite = Resources.Load<Sprite>("Sprites/type_all");
        }
    }

    void Start ()
    {
        BattleTextPrefab = Resources.Load<GameObject>("DamageText");
        if(BattleTextPrefab == null)
        {
            Debug.Log("Cannot find textPrefab for card in Resources folder");
        }

        SetupCard();
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        // is a menu window is open do nothing
        if (displayElement)
        {
            return;
        }

        state = State.DRAG;
        canvas.overrideSorting = true;
        //cardImage.material = Resources.Load<Material>("OutlinedDiffuse");
        transform.position = Input.mousePosition;
        float distanceFactor = this.transform.position.y - originalPosition.y;
        //Debug.Log("Distance factor: " + distanceFactor);
        RescaleCard(distanceFactor);
        VaryTranspaency(distanceFactor);

        //do target highlights if above screen threshold otherwise clear
        if ((Input.mousePosition.y > Screen.height * 0.45f))
        {
            //display as targeted for each type
            if (targetArea == TargetArea.ALL)
            {
                foreach (GameObject monster in battleController.EnemiesInBattle)
                {
                    monster.GetComponent<MonsterController>().IsTargeted(true);
                }
            }
            else if (targetArea == TargetArea.SELF)
            {
                battleController.FriendliesInBattle[0].GetComponent<MonsterController>().IsPlayerTargeted(true);
            }
            else
            {
                // other attack types not listed
            }
        }
        else
        {
            // reset colors
            foreach (GameObject monster in battleController.AllInBattle)
            {
                monster.GetComponent<MonsterController>().ResetTargetedToClear();
            }
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // is a menu window is open do nothing
        if (displayElement)
        {
            return;
        }

        // reset colors
        foreach (GameObject monster in battleController.AllInBattle)
        {
            monster.GetComponent<MonsterController>().ResetTargetedToClear();
        }

        if(!(battleController.player.GetComponent<MonsterController>().monsterState == MonsterController.State.SELECTING))
        {
            //provide player feedback it is not their turn
            //or discard cards completely and draw a new hand every time
            state = State.RESET;
            return;
        }

        if (!playerHandController.PlayerHasEnoughCharges(cost))
        {
            state = State.RESET;
            return;
        }


        // prevent player from attacking himself unless it is a "self or heal card"
        if(targetArea == TargetArea.SINGLE)
        {
            if (ValidGameObjectTarget() == battleController.player)
            {
                state = State.RESET;
                return;
            }
        }

        // If Card dropped over 1 monster
        // Careful: Special case where user cursor drags and drops a card above a monster
        if (IsValidTarget())
        {
            selectedTarget = ValidGameObjectTarget();
            playerHandController.AdjustCost(cost);
            // hide transparency of the card while we determine the attack
            HideCard();
            // Note: this will destroy the card within this method
            playerHandController.PlaceCardIn(this.gameObject, playerHandController.graveyard);

            // if a single target card then apply to one monster
            if (targetArea == TargetArea.SINGLE)
            {
                SingleTargetAttack();
            }
            // Otherwise determine which target the card applies to
            else
            {
                DetermineTargets();
            }

            //Does this card draw? If so wait next turn or instantly?
            CheckInstantDraw();
            

            // Check if we are combing, if we are not then end the turn and setup status to charging
            CheckComboChaining();
            

        }
        // if mouse not hovering above a monster check for battlefield
        // Check and apply any AoE cards if user drags and drops a card in the battlefield 
        else if (targetArea != TargetArea.SINGLE)
        {
            if ((Input.mousePosition.y > Screen.height * 0.45f) )
            {
                // Debug.Log("Played above the hand");
                // Applies card effects within battlefield bounds
                // This does not handle single target cards, check above if statements
                playerHandController.AdjustCost(cost);
                HideCard();
                // Note: this will destroy the card within this method
                playerHandController.PlaceCardIn(this.gameObject, playerHandController.graveyard);

                // determine the targets to add to our monsters action queue
                DetermineTargets();

                //Does this card draw? If so execute it now
                CheckInstantDraw();

                // check if we finish chaining cards or continue waiting
                CheckComboChaining();               
            }
            else
            {
                if (state == State.DRAG || state == State.NORMAL)
                {
                    state = State.RESET;
                }
                
            }
        }
        else
        {
            if (state == State.DRAG || state == State.NORMAL)
            {
                state = State.RESET;
            }
        }
    }

    private void AnimatePos(Vector3 start, Vector3 end, float speed)
    {
        float startTime = Time.time;
        float distance = Vector3.Distance(start, end);
        Debug.Log(distance);

        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / distance;
        transform.localPosition = Vector3.Lerp(start, end, fracJourney);
    }

    IEnumerator AnimateForTime(float time)
    {
        yield return new WaitForSeconds(time);
        if(state == State.RESET && state != State.DRAG)
            state = State.NORMAL;
    }

    void ResetCardDisplay()
    {
        // reset card properties back to hand
        canvas.overrideSorting = false;
        // removed because we use lerp instead
        // transform.localPosition = originalPosition;
        transform.localScale = new Vector3(1, 1, 1);
        ResetTransparency();
    }

    public void RescaleCard(float distance)
    {
        //0.8f is the target scaling
        if(distance > 100 && distance < 350)//originally 100 and 350
        {
            //float slope = 0.002f;
            //y = mx + b
            float newScale = (-0.0013f * distance) + 1.0f;
            //Debug.Log("Scaling to " + newScale + " with distance: " + distance);
            transform.localScale = new Vector3 (newScale, newScale, 0);
        }
    }

    public void VaryTranspaency(float distance)
    {
        if(distance > 100 && distance < 250)
        {   
            float value = (-0.0033f*distance) + 1.0f;
            cardImage.color = new Color (1,1,1,value);
        }
        if (distance <= 100)
        {
            ResetTransparency();
        }

    }

    public void HideCard()
    {
        cardImage.color = Color.clear;
        foreach(Text t in texts)
        {
            t.color = Color.clear;
        }

        foreach(Image i in images)
        {
            i.color = Color.clear;
        }
        
    }

    public void ResetTransparency()
    {
        cardImage.color = new Color (1,1,1,1);
    }

    // This class returns if we hit a monster
    public GameObject ValidTargetDraggedOn(){
        hitInfo = new RaycastHit();
        screenRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        hit = Physics2D.GetRayIntersection(screenRay);
        if (hit)
        {
            Debug.Log("hit something: " + hit.transform.gameObject);
            return hit.transform.gameObject;
        }else{
            return null;
        }
    }

    // checks if hits a monster
    public bool IsValidTarget()
    {
        hitInfo = new RaycastHit();
        screenRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        hit = Physics2D.GetRayIntersection(screenRay);
        if (hit)
        {
            Debug.Log("IsTargetValid: " + hit.transform.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    // checks if hits player
    public GameObject ValidGameObjectTarget()
    {
        hitInfo = new RaycastHit();
        screenRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        hit = Physics2D.GetRayIntersection(screenRay);
        if (hit)
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    public void SpawnBattleText()
    {
        GameObject textObj = Instantiate(BattleTextPrefab, this.transform);
        textObj.transform.SetParent(playerHandController.parentCanvas.transform);
        textObj.transform.localScale = Vector3.one;
        string s = damageAmount.ToString();
        textObj.GetComponent<Text>().text = s; 
    }

    public void SpawnBattleTextAbove(GameObject monster)
    {
        GameObject textObj = Instantiate(BattleTextPrefab, monster.transform);
        if(monster == battleController.enemyA)
        {
            textObj.transform.SetParent(playerHandController.enemyReferenceA.transform);
        }
        else if (monster == battleController.enemyB)
        {
            textObj.transform.SetParent(playerHandController.enemyReferenceB.transform);
        }
        else if (monster == battleController.enemyC)
        {
            textObj.transform.SetParent(playerHandController.enemyReferenceC.transform);
        }
        else if (monster == battleController.player)
        {
            textObj.transform.SetParent(playerHandController.enemyReferenceC.transform);
        }
        else
        {
            Debug.Log("SpawnBattleText(): Did not find if condition for" + monster);
        }
        //textObj.transform.SetParent(playerHandController.parentCanvas.transform);
        textObj.transform.localScale = Vector3.one;
        string s = damageAmount.ToString();
        textObj.GetComponent<Text>().text = s;
    }
    // Called when a card is dragged on top of an enemy
    public void SingleTargetAttack()
    {
        // ensure this card is removed
        state = State.DESTROY;
        // setup and store the event data
        GameObject targetedMonster = ValidTargetDraggedOn();
        List<GameObject> targets = new List<GameObject>();
        targets.Add(targetedMonster);
        HandleTurn turn = new HandleTurn(battleController.player, targets, targetArea, damageAmount, block, chargeTime, isCanceling, stunNumberOfTurns, drawType);
        //battleController.AddTurnToQueue(turn);
        playerController.actions.Add(turn);
    }

    // Called when a card is dropped on the battlefield
    // Works for all except for single target cards. Call SingleTargetAttack() instead
    public void DetermineTargets()
    {
        // ensure this card is removed
        state = State.DESTROY;
        if (targetArea == TargetArea.ALL)
        {
            // setup and store the event data
            GameObject targetedMonster = ValidTargetDraggedOn();
            HandleTurn turn = new HandleTurn(battleController.player, battleController.EnemiesInBattle, targetArea, damageAmount, block, chargeTime, isCanceling, stunNumberOfTurns, drawType);
            //battleController.AddTurnToQueue(turn);
            playerController.actions.Add(turn);
        }
        else if (targetArea == TargetArea.SELF)
        {
            HandleTurn turn = new HandleTurn(battleController.player, battleController.FriendliesInBattle, targetArea, damageAmount, block, chargeTime, isCanceling, stunNumberOfTurns, drawType);
            playerController.actions.Add(turn);
        }
        else if (targetArea == TargetArea.RANDOM)
        {
            // logic needed in the future
        }
        else
        {
            Debug.Log("DetermineTargets() Error, target type not found for type: " + targetArea);
        }
    }

    // check to ensure if we can chain another card and wait for user to play a card that doesnt chain or end their turn
    void CheckComboChaining()
    {
        // Well add the chain card's charge duration
        if (isChainCombo)
        {
            playerController.AddDuration = chargeTime;
            return;
        }
        // end the users turn and begin charging up for the combo attack
        else
        {
            // hiding the Combat UI is last (otherwise it wont catch our early return when using comboes in Single and Determine Attack methods
            battleController.HideCombatUI();
            userController.IsUsersTurn = false;
            battleController.PauseSpeedsForAllMonsters(false);

            // Begin charging for player
            //playerController.monsterState = MonsterController.State.CHARGING;
            playerController.ChangeState(MonsterController.State.CHARGING);
            playerController.chargeDuration = chargeTime;
            PlayerTickController playerTickController = GameObject.FindGameObjectWithTag("PlayerTick").GetComponent<PlayerTickController>();
            playerTickController.ChangeState(PlayerTickController.GaugeState.CHARGING);
        }
    }

    void CheckInstantDraw()
    {
        //do we wait until next turn?
        if(onNextTurn && drawNumber != 0)
        {
            playerHandController.DrawAdditionalCardsNextTurn = drawNumber;
            return;
        }

        if (drawType == DrawType.DRAW_NORMAL)
        {
            playerHandController.DrawCards(1);
        }
        else if (drawType == DrawType.RESHUFFLE_HAND)
        {
            //Get current number of cards in hand
            int currentCardsInHand = playerHandController.NumberOfCardsInHand();
            Debug.Log("currentCardsInHand: " + currentCardsInHand);
            playerHandController.DiscardHand();
            playerHandController.DrawCards(currentCardsInHand);
        }
        else
        {

        }

        //since we organize by an index value we must recompute every time
        playerHandController.RecomputeHandIndexes();
    }

    void FixedUpdate()
    {
        // Some weird bug when dragging and droping really quickly occurs
        switch (state)
        {
            case State.NORMAL:
                break;
            case State.RESET:
                StartCoroutine(AnimateForTime(0.5f));
                transform.localPosition = Vector2.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 20);
                ResetCardDisplay();
                break;
            //remove this card
            case State.DESTROY:

                break;
        }

    }

}