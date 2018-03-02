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


    void Awake ()
    {
        battleController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        animator = GetComponent<RuntimeAnimatorController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
            Debug.Log("Monster animator is null");
        if (spriteRenderer == null)
            Debug.Log("Monster sprite renderer is null");
        if (!MonsterInfoDatabase.IsPopulated)
             MonsterInfoDatabase.Populate();
        //create Canvas on top with animated HP bar
        //GameObject playerCanvas = Instantiate(HealthBarPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //playerCanvas.GetComponent<HealthController>().referenceMonster = this.gameObject;
        //playerCanvas.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
    }

    void CheckAttack()
    {
        if(speed > battleController.Threshold)
        {
            battleController.IsPaused = true;
            Debug.Log("Speed at:" + speed);
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
