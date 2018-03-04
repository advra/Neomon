using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]

public class MonsterController : MonoBehaviour
{
    public BattleController battleController;
    public Monster monster;
    public bool isPlayer = false;
    public bool isDead;
    public string spriteFile;
    public int currentHealth, maxHealth, attack, defense, level;
    public float speed, nextSpeed;
    public int baseSpeed, baseAttack, baseDefense;
    public bool turn;
    public GameObject HealthBarPrefab;
    private RuntimeAnimatorController animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    //use this to check if targeted then change color
    public bool targeted;

    void Awake ()
    {
        battleController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        animator = GetComponent<RuntimeAnimatorController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
            Debug.Log(this.gameObject + " monster animator is null");
        if (spriteRenderer == null)
            Debug.Log(this.gameObject + " monster sprite renderer is null");
        if (!MonsterInfoDatabase.IsPopulated)
             MonsterInfoDatabase.Populate();
        //create Canvas on top with animated HP bar
        //GameObject playerCanvas = Instantiate(HealthBarPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //playerCanvas.GetComponent<HealthController>().referenceMonster = this.gameObject;
        //playerCanvas.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
    }

    public void IsTarget(bool value){
        if(value){
            StartCoroutine(FlashSprite());
        }else{
            StopCoroutine(FlashSprite());
        }
    }

    void CheckAttack()
    {
        if(speed > battleController.Threshold)
        {
            battleController.IsPaused = true;
            Debug.Log(this.gameObject + " Speed at:" + speed);
        }
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
            int randomIndex = Random.Range(0, 3);   //returns 0 - 1
            monster = MonsterInfoDatabase.monsters[randomIndex];
            spriteFile = monster.MonsterInfo.SpriteFile;
        }
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + spriteFile);

        //We add box collider later because it inherits the sprites dimensions otherwise 
        //the box collider would not generate the appropriate size for the monster
        gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        if(boxCollider2D == null){
            Debug.Log(this.gameObject + " does not have a BoxCollider2d");
        }
        currentHealth = monster.Health;
        maxHealth = monster.MaxHealth;
        baseSpeed = monster.Speed;
        FindNextTurn();
        Debug.Log(monster.Print);
    }
    
    bool OnTurn()
    {
        if (speed >= battleController.Threshold)
        {
            return true;
        }
        else
        {
            speed += baseSpeed;
            return false;
        }
    }

    void FindNextTurn()
    {
        //keep increasing speed until it is our turn
        if (!turn)
        {
            turn = OnTurn();
        }
        else
        {
            //do attack stuff here
        }
    }

    private IEnumerator FlashSprite()
    {
        //1,1,1 to 255,80,80
        float duration = 1f;

        while(duration > 0)
        {
            duration -= Time.deltaTime;
            spriteRenderer.color = new Color(1, 1, 1, duration);
            return null;
        }

        return null;
    }

    // Update is called once per frame
    void Update () {
        //simulate poison (for testing the hp)
        //monster.ChangeHealth(-1);

        if (battleController.IsBattling & !battleController.IsPaused)
        {
            currentHealth = monster.Health;
            maxHealth = monster.MaxHealth;
            CheckAttack();
        }
    }
}
