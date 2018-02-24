using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerMonster : MonoBehaviour {

    //reference our Monster class
    Monster Monster;
    public Monster PlayerMonster;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private RuntimeAnimatorController animator;

    public Monster GetMonster
    {
        get { return PlayerMonster; }
    }

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log("The player sprite cannot be null");
        }

        //Generate the monster table
        if (!MonsterDatabase.IsPopulated)
        {
            MonsterDatabase.Populate();
        }

        //Assign a monster to a player
        PlayerMonster = MonsterDatabase.monsters[0];

        //Since this class is spawning the player we will concatenate string + "_b"
        //because we want to load the back end of the sprite
        string spriteFile = PlayerMonster.SpriteFile+"_b";
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + spriteFile);

        Debug.Log(PlayerMonster.Info);
    }

}
