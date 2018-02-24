using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour {

    [SerializeField]
    private Monster Monster;
    [SerializeField]
    private SpriteRenderer SpriteRenderer;
    [SerializeField]
    private RuntimeAnimatorController animator;
    

	// Use this for initialization
	void Start () {

        
        SpriteRenderer = GetComponent<SpriteRenderer>();
        if(SpriteRenderer == null)
        {
            Debug.Log("Enemy sprite cannot be null");
        }

        //Create the monster and assign it to this object
        GenerateMonsters();

    }

    void GenerateMonsters()
    {
        
        //Construct a Mushroom named Mush Kid level 5
        Monster.Mushroom Mushkid = new Monster.Mushroom("Mush-kid", 5, "mon_mush");
        SpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/mon_mush");
        //Give it attributes (Health, speed, attack, defense)
        Mushkid.SetAttributes(25, 10, 5, 3);

        //animator = GetComponent<RuntimeAnimatorController>();
        

        //Add it to monster dictionary to load next time
        //monsters[0] = Mushkid;

        Debug.Log("Name: " + Mushkid.GetName());
        Debug.Log("Level: " + Mushkid.GetLevel());
        Debug.Log("Health: " + Mushkid.GetHealth());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
