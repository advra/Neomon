using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {
    SpriteRenderer spriteRenderer;

    Card card;
    public string cardName;
    public string cardSprite;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + cardSprite);
        if (spriteRenderer == null)
            Debug.Log("Card sprite renderer is null");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
