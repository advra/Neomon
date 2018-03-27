using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]

public class MonsterController : MonoBehaviour
{
    UserController userController;
    public BattleController BC;

    public Monster monster;
    public bool isPlayer;
    public bool isDead;
    public string spriteFile;
    public int currentHealth, maxHealth, attack, defense, level;
    public int currentSpeed;
    public int baseSpeed;
    public int baseAttack, baseDefense;
    public GameObject HealthBarPrefab;
    private RuntimeAnimatorController animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    bool targeted;
    Color lerpedColor;
    //use this to check if targeted then change color

    void Awake ()
    {
        BC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        animator = GetComponent<RuntimeAnimatorController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
        {
            Debug.Log(this.gameObject + " monster animator is null");
        }
            
        if (spriteRenderer == null)
        {
            Debug.Log(this.gameObject + " monster sprite renderer is null");
        }
           
        if (!MonsterInfoDatabase.IsPopulated)
        {
            MonsterInfoDatabase.Populate();
        }
        //create Canvas on top with animated HP bar
        //GameObject playerCanvas = Instantiate(HealthBarPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //playerCanvas.GetComponent<HealthController>().referenceMonster = this.gameObject;
        //playerCanvas.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
    }

    public int CurrentSpeed
    {
        get { return this.currentSpeed; }
        set { this.currentSpeed = value;  }
    }

    //public int BaseSpeed
    //{
    //    get { return baseSpeed; }
    //    set { this.baseSpeed = value; }
    //}

    public bool IsTargeted
    {
        set { targeted = value; }
    }

    public void StartTest()
    {
        StartCoroutine(FlashInput());
    }

    public void Damage(int amount)
    {
        this.currentHealth -= amount;
        //Play damage sprite here
        if (this.currentHealth < 0)
        {
            this.currentHealth = 0;
            this.isDead = true;
        }

        if (isDead)
        {
            //run death animation here
            //Design decision: either fade out and disable now, or allow them to be revived?
        }
    }

    public void Heal(int amount)
    {
        this.currentHealth += amount;
        if (this.currentHealth < 0)
        {
            this.currentHealth = 0;
            this.isDead = true;
        }
    }

    public void PlayTurn()
    {
        if (isPlayer)
        {
            //GameObject.FindGameObjectWithTag("Hand").GetComponent<PlayerHandController>().Draw();
            Debug.Log("Player turn: " + gameObject);
        }
        else
        {
            Debug.Log("Enemy turn" + gameObject);
        }
    }

    void CheckAttack()
    {
        if(currentSpeed > BC.Threshold)
        {
            BC.IsPaused = true;
            Debug.Log(this.gameObject + " Speed at:" + currentSpeed);
        }
    }

    public void ResetTurn(int threshold)
    {
        currentSpeed -= threshold;
    }

    // Use this for initialization
    void Start ()
    {
        //get reference to UserController to check if mouse is on top of this monster
        userController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UserController>();

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
            int randomIndex = Random.Range(0, MonsterInfoDatabase.monsters.Count);   //returns 0 - 1
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

        //battleController.speedTable.Add(baseSpeed);
        //currentSpeed = 0.1f;
        //FindNextTurn();
        Debug.Log(monster.Print);
    }
    
    //bool OnTurn()
    //{
    //    if (speed >= battleController.Threshold)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        speed += baseSpeed;
    //        return false;
    //    }
    //}

    //void FindNextTurn()
    //{
    //    //keep increasing speed until it is our turn
    //    if (!turn)
    //    {
    //        turn = OnTurn();
    //    }
    //    else
    //    {
    //        //do attack stuff here
    //    }
    //}

    IEnumerator FlashInput()
    {
        for (float f = 1f; f >= 0.0f; f -= 0.1f)
        {
            spriteRenderer.color = new Color(1, f, f, 1);
            //yield return null;
        }
        for (float f = 0.0f; f <= 1.0f; f += 0.1f)
        {
            spriteRenderer.color = new Color(1, f, f, 1);
            //yield return null;
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator SimulatePoison()
    {
        monster.ChangeHealth(-1);
        yield return new WaitForSeconds(1f);
    }

    // Update is called once per frame
    void Update () {
        //simulate poison (for testing the hp)
        if (BC.IsBattling & !BC.IsPaused)
        {
            CheckAttack();

            //Used for testing purposes
            //StartCoroutine(SimulatePoison());

            //Need to revisit. Works as intended but mouse hover = multiple calls which conflict the animation.
            if ((targeted))
            {
                lerpedColor = Color.Lerp(Color.white, new Color(1.0f, 0.0f, 0.0f, 1.0f), Mathf.PingPong(Time.time, 0.5f));
                spriteRenderer.color = lerpedColor;
                
            }
        }

    }
}
