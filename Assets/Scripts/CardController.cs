using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class CardController : MonoBehaviour, IDragHandler, IEndDragHandler {
    PlayerHandController playerHandController;
    //not to be confused with the Hand Canvas
    Canvas canvas;
    Image cardImage;
    Card card;
    public string cardName;
    public string cardSprite;
    public enum cardType {attack, defense}
    public enum targetArea{single, penetrate,split,all}
    [SerializeField]
    private int handIndex;
    [SerializeField]
    private Vector3 originalPosition;
    RaycastHit hitInfo;
    RaycastHit2D hit;
    Ray ray;
    Ray screenRay;

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
        //Greater is in front. 10 to ensure it is in front of all other GUI.
        canvas.sortingOrder = 10;
    }

    public void OnDrag(PointerEventData eventData)
    {
        canvas.overrideSorting = true;
        //cardImage.material = Resources.Load<Material>("OutlinedDiffuse");
        transform.position = Input.mousePosition;
        float distanceFactor = this.transform.position.y - originalPosition.y;
        Debug.Log("Distance factor: " + distanceFactor);
        RescaleCard(distanceFactor);
        VaryTranspaency(distanceFactor);
        ValidTargetDraggedOn();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //destroy cardobject and update our arrays
        if(IsValidTarget())
        {
            playerHandController.PlaceCardIn(this.gameObject, playerHandController.graveyard);
            Debug.Log("Card played!");
            
        }
        //reset card properties back to hand
        canvas.overrideSorting = false;
        transform.localPosition = originalPosition;
        this.transform.localScale = new Vector3 (1,1,1);
        ResetTransparency();
    }

    /// <summary>
    /// Called every frame while the mouse is over the GUIElement or Collider.
    /// </summary>
    // void OnMouseOver()
    // {
    //     Debug.Log("Mouse over!");
    //     //renderer.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
    // }

    // void OnMouseDown()
    // {
    //     Debug.Log("Pressed");
    //     playerHandController.PlaceCardIn(this.gameObject, playerHandController.graveyard);
    // }

    public void RescaleCard(float distance)
    {
        //0.8f is the target scaling
        if(distance > 0 && distance < 350)//originally 250
        {
            //float slope = 0.002f;
            //y = mx + b
            float newScale = (-0.0013f * distance) + 1.0f;
            Debug.Log("Scaling to " + newScale);
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

    public bool IsValidTarget(){
        hitInfo = new RaycastHit();
        screenRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        hit = Physics2D.GetRayIntersection(screenRay);
        if (hit)
        {
            Debug.Log("hit something: " + hit.transform.gameObject);
            return true;
        }else{
            return false;
        }
    }
    
	
	// Update is called once per frame
	// void Update () {
    //     //		if (Input.GetMouseButton(0)){
    //     if(Input.GetMouseButtonUp(0)){
    //         Debug.Log("Mouse down");
    //         //playerHandController.PlaceCardIn(this.gameObject, playerHandController.graveyard);
    //         playerHandController.PlaceCardIn(playerHandController.graveyard);
    //         GameObject.Destroy(this.gameObject);
    //     }
	// }

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
    {
        //as image position in y moves from 150 to 1000 decrease the scale
        //calculate distance from original position, further = smaller

    }

    
}
