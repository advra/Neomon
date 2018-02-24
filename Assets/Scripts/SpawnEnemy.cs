using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour {

    private Monster Monster;

    [SerializeField]
    static Monster EnemyMonster;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private RuntimeAnimatorController animator;

    public Monster GetMonster
    {
        get { return EnemyMonster; }
    }

    // Use this for initialization
    void Start () {

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log("The enemy sprite cannot be null");
        }

        //Generate the monster table
        if (!MonsterDatabase.IsPopulated)
        {
            MonsterDatabase.Populate();
        }

        //for now we will just a random selection within our database
        //We can expand on this later
        int randomIndex = Random.Range(0, 2);   //returns 0 - 1

        //Assign a monster to enemy
        EnemyMonster = MonsterDatabase.monsters[randomIndex];
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + EnemyMonster.SpriteFile);

        Debug.Log(EnemyMonster.Info);
    }
}
