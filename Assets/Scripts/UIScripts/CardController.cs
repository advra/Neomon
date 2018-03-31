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
    PlayerController playerController;

    //not to be confused with the Hand Canvas
    Canvas canvas;
    Image cardImage;
    Card card;
    Text[] texts;

    public GameObject PlayerHand;

    public string cardName;
    public string cardSprite;
    public int damageAmount;
    public int cost;
    public float chargeTime;
    public TargetArea targetArea;
    public State state; 
    [SerializeField]
    private int handIndex;
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
        cardImage = GetComponent<Image>();
        if(cardImage == null){
            Debug.Log("No sprite Imgae found!");
        }
        canvas = GetComponent<Canvas>();
        if(canvas == null){
            Debug.Log("This Card does not have a canvas for sorting!");
        }
        playerHandController = GetComponentInParent<PlayerHandController>();
        if(playerHandController == null){
            Debug.Log("playerHandController is null");
        }
        battleController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        if (battleController == null)
        {
            Debug.Log("battleController is null");
        }
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.Log("playerController is null");
        }
        //used to reference all the text gameobjects
        texts = GetComponentsInChildren<Text>();
        //Greater is in front. 10 to ensure it is in front of all other GUI.
        canvas.sortingOrder = 10;
    }

    void Start ()
    {
        BattleTextPrefab = Resources.Load<GameObject>("DamageText");
        if(BattleTextPrefab == null)
        {
            Debug.Log("Cannot find textPrefab for card in Resources folder");
        }

        //texts[0].text = cardName;         //this is for the Cost Text which we wont change
        texts[1].text = cost.ToString();
    }

    public void OnDrag(PointerEventData eventData)
    {
        state = State.DRAG;
        canvas.overrideSorting = true;
        //cardImage.material = Resources.Load<Material>("OutlinedDiffuse");
        transform.position = Input.mousePosition;
        float distanceFactor = this.transform.position.y - originalPosition.y;
        //Debug.Log("Distance factor: " + distanceFactor);
        RescaleCard(distanceFactor);
        VaryTranspaency(distanceFactor);
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!playerHandController.isPlayerTurn)
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

        //If Card dropped over 1 monster
        //Careful: Special case where user cursor drags and drops a card above a monster
        if (IsValidTarget())
        {
            playerHandController.AdjustCost(cost);
            HideCard();
            //this will destroy the card
            playerHandController.PlaceCardIn(this.gameObject, playerHandController.graveyard);
            battleController.PauseSpeedsForEnemies(false);
            playerController.HideCombatUI();
            //if a single target card then apply to one monster
            if (targetArea == TargetArea.single)
            {
                SingleTargetAttack();
            }
            //Otherwise determine which target the card applies to
            else
            {
                DetermineTargets();
            }
        }
        //if mouse not hovering above a monster check for battlefield
        //Check and apply any AoE cards if user drags and drops a card in the battlefield 
        else if (targetArea != TargetArea.single)
        {
            if ((Input.mousePosition.y > Screen.height * 0.45f) )
            {
                //Debug.Log("Played above the hand");
                //Applies card effects within battlefield bounds
                //This does not handle single target cards, check above if statements
                playerHandController.AdjustCost(cost);
                HideCard();
                playerHandController.PlaceCardIn(this.gameObject, playerHandController.graveyard);
                battleController.PauseSpeedsForEnemies(false);
                DetermineTargets();
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

    void FixedUpdate()
    {
        //Some weird bug when dragging and droping really quickly occurs
        switch (state)
        {
            case State.NORMAL:
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

    IEnumerator AnimateForTime(float time)
    {
        yield return new WaitForSeconds(time);
        if(state == State.RESET && state != State.DRAG)
            state = State.NORMAL;
    }

    void ResetCardDisplay()
    {
        //reset card properties back to hand
        canvas.overrideSorting = false;
        //removed because we use lerp instead
        //transform.localPosition = originalPosition;
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
    }

    public void ResetTransparency()
    {
        cardImage.color = new Color (1,1,1,1);
    }

    //This class returns if we hit a monster
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

    //checks if hits a monster
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

    //Called when a card is dragged on top of an enemy
    public void SingleTargetAttack()
    {
        //ensure this card is removed
        state = State.DESTROY;
        //setup and store the event data
        GameObject targetedMonster = ValidTargetDraggedOn();
        List<GameObject> targets = new List<GameObject>();
        targets.Add(targetedMonster);
        HandleTurn turn = new HandleTurn(battleController.player, targets, targetArea, damageAmount, chargeTime);
        battleController.AddTurnToQueue(turn);

        //Begin charging
        playerController.currentUserState = PlayerController.PlayerState.CHARGING;
        playerController.chargeDuration = chargeTime;
        PlayerTickController playerTickController = GameObject.FindGameObjectWithTag("PlayerTick").GetComponent<PlayerTickController>();
        playerTickController.ChangeState(PlayerTickController.GaugeState.CHARGING);
    }

    //Called when a card is dropped on the battlefield
    //Works for all except for single target cards. Call SingleTargetAttack() instead
    public void DetermineTargets()
    {
        //ensure this card is removed
        state = State.DESTROY;
        if (targetArea == TargetArea.all)
        {
            //setup and store the event data
            GameObject targetedMonster = ValidTargetDraggedOn();
            HandleTurn turn = new HandleTurn(battleController.player, battleController.EnemiesInBattle, targetArea, damageAmount, chargeTime);
            battleController.AddTurnToQueue(turn);

            //Begin charging
            playerController.currentUserState = PlayerController.PlayerState.CHARGING;
            playerController.chargeDuration = chargeTime;
            PlayerTickController playerTickController = GameObject.FindGameObjectWithTag("PlayerTick").GetComponent<PlayerTickController>();
            playerTickController.ChangeState(PlayerTickController.GaugeState.CHARGING);

        }
        else if (targetArea == TargetArea.line)
        {
            //logic needed in the future
        }
        else if (targetArea == TargetArea.split)
        {
            //logic needed in the future
        }
        else if (targetArea == TargetArea.random)
        {
            //Random.Range(min, max + 1)
            int randomTarget = Random.Range(0, battleController.numberOfEnemies + 1);
            //ensure this card is removed
            state = State.DESTROY;
            //setup and store the event data
            List<GameObject> targets = new List<GameObject>();
            targets.Add(battleController.EnemiesInBattle[randomTarget]);
            HandleTurn turn = new HandleTurn(battleController.player, targets, targetArea, damageAmount, chargeTime);
            battleController.AddTurnToQueue(turn);

            //Begin charging
            playerController.currentUserState = PlayerController.PlayerState.CHARGING;
            playerController.chargeDuration = chargeTime;
            PlayerTickController playerTickController = GameObject.FindGameObjectWithTag("PlayerTick").GetComponent<PlayerTickController>();
            playerTickController.ChangeState(PlayerTickController.GaugeState.CHARGING);

        }
        else
        {
            Debug.Log("DetermineTargets() Error, target type not found for type: " + targetArea);
        }
    }

}