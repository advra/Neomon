using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour {

    Monster Monster;

    //get our components
    [SerializeField]
    Text text;
    [SerializeField]
    private Image image;
    [SerializeField]
    private int health;
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private bool coolingDown = false;
    [SerializeField]
    private float percentage;
    [SerializeField]
    private float percentageTarget;
    [SerializeField]
    public bool isPlayerHP;
    public bool isEnemyHP;

    private bool _isSetup;

    public GameObject referenceMonster;
    public MonsterController monsterController;

    public bool CoolingDown
    {
        get { return coolingDown; }
    }

    public bool IsSetup
    {
        get { return _isSetup; }
        set { _isSetup = value; }
    }

    public int Health
    {
        get { return health; }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    public float PercentageTarget
    {
        get { return percentageTarget; }
    }

    //uset health to current HP and force animation update 
    public void SetHealth(int currentHealth, int maximumHealth)
    {
        maxHealth = maximumHealth;
        health = currentHealth;
        UpdateHealthBar();
    }

    ////Function to apply damage to the player 
    //public bool ApplyDamage(int value)
    //{
    //    //Apply damage to the player 
    //    health -= value;
    //    //Check if the player has still health and update the Health Bar 
    //    if (health > 0)
    //    {
    //        UpdateHealthBar();
    //        return false;
    //    //If health is a negative value, set it to 0
    //    }else if (health < 0){
    //        health = 0;
    //        return true;
    //    }
    //    else
    //    {
    //        //In case the player has no health left, set health to zero and //return true
    //        health = 0;
    //        UpdateHealthBar();
    //        return true;
    //    }
    //}

    //Function to update the Health Bar Graphic 
    void UpdateHealthBar()
    {
        //Calculate the percentage (from 0% to 100%) of the current amount of health of the player
        percentage = health * 1f / maxHealth;
        //Assign the target percentage as reference when to stop in Update
        percentageTarget = percentage;
        coolingDown = true;
    }

    void Awake()
    {
        text = GetComponentInChildren<Text>();
        if (text == null)
            Debug.Log("HP Text component (should be child) cannot be null");
        image = GetComponent<Image>();
        if (image == null)
            Debug.Log("HP Image component cannot be null");
    }

    void Start()
    {
        //altenatively can use this if pre-defined object using string find (but is slower) 
        //monsterController = GameObject.Find("Player Monster").GetComponent<MonsterController>();
        monsterController = referenceMonster.GetComponent<MonsterController>();
        if (monsterController == null)
            Debug.Log("parentMonster monster script not found");


        //need to manually apply canvas to each enemy using this
        //Vector3 monsterPos = Camera.main.WorldToViewportPoint(referenceMonster.transform.position);
        //transform.position = monsterPos;
    }

    // Update is called once per frame
    void Update () {

        if (coolingDown)
        {
            //As long as we are above the percent we want, keep decreasing the HP
            if(image.fillAmount >= percentageTarget)
            {
                image.fillAmount -= 1.0f / 5.0f * Time.deltaTime;
            }
            //Once we go below the HP we want, stop
            else
            {
                coolingDown = false;
            }
        }
        health = monsterController.currentHealth;
        maxHealth = monsterController.maxHealth;
        SetHealth(health, maxHealth);

        if (monsterController.monster.IsDead)
        {
            if(image.fillAmount == 0)
            {
                //#A32323C8
                //text.color = Color.red;
            }
            else
            {
                text.text = "HP " + health + " / " + maxHealth;
            }
        }
        else
        {
            text.text = "HP " + health + " / " + maxHealth;
        }
        
    }
}
