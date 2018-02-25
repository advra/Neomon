using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]

public class MonsterController : MonoBehaviour
{
    public Monster monster;
    public bool isPlayer = false;
    public string spriteFile;
    public int currentHealth, maxHealth, speed, attack, defense, level;
    public GameObject HealthBarPrefab;
    private RuntimeAnimatorController animator;
    private SpriteRenderer spriteRenderer;

    void Awake ()
    {
        animator = GetComponent<RuntimeAnimatorController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
            Debug.Log("Monster animator is null");
        if (spriteRenderer == null)
            Debug.Log("Monster sprite renderer is null");

        if (!MonsterInfoDatabase.IsPopulated)
             MonsterInfoDatabase.Populate();

        //create Canvas on top
        //GameObject playerCanvas = Instantiate(HealthBarPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //playerCanvas.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
    }

    // Use this for initialization
    void Start () {
        
        if (isPlayer)
        {
            //Set to squidra for now until we expand on graphics
            monster = MonsterInfoDatabase.monsters[0];
            //We concatenate _b to refer rear sprite images
            spriteFile = monster.MonsterInfo.SpriteFile + "_b";
        }
        else
        {
            //for now we will just a random selection within our database
            //We can expand on this later
            int randomIndex = Random.Range(0, 2);   //returns 0 - 1
            monster = MonsterInfoDatabase.monsters[randomIndex];
            spriteFile = monster.MonsterInfo.SpriteFile;
        }
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + spriteFile);

        currentHealth = monster.Health;
        maxHealth = currentHealth;

        Debug.Log(monster.Print);
    }

	
	// Update is called once per frame
	void Update () {
		
	}
}
