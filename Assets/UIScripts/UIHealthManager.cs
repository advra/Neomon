using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthManager : MonoBehaviour {

    //get our components

    [SerializeField]
    Text text;
    [SerializeField]
    private Image image;

    [SerializeField]
    private int health;         //current health
    [SerializeField]
    private int maxHealth = 1;   //max health monster possesses
    [SerializeField]
    private bool coolingDown = false;
    [SerializeField]
    private float percentage;
    [SerializeField]
    private float percentageTarget;

    public bool CoolingDown
    {
        get { return coolingDown; }
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
        get { return percentageTarget;  }
    }

    //for testing 
    public void SetHealth(int maximumHealth, int currentHealth)
    {
        maxHealth = maximumHealth;
        health = currentHealth;
        UpdateHealthBar();
    }
    public void SetHealth(int maximumHealth)
    {
        maxHealth = maximumHealth;
        health = maximumHealth;

        //UpdateHealthBar();    //max HP so we do not need to update healthbar here
    }

    //Function to apply damage to the player 
    public bool ApplyDamage(int value)
    {
        //Apply damage to the player 
        health -= value;

        //Check if the player has still health and update the Health Bar 
        if (health > 0)
        {
            UpdateHealthBar();
            return false;
        //If health is a negative value, set it to 0
        }else if (health < 0){
            health = 0;
            return true;
        }
        else
        {
            //In case the player has no health left, set health to zero and //return true
            health = 0;
            UpdateHealthBar();
            return true;
        }
    }

    //Function to update the Health Bar Graphic 
    void UpdateHealthBar()
    {
        //Calculate the percentage (from 0% to 100%) of the current amount of health of the player
        percentage = health * 1f / maxHealth;
        //Assign the target percentage as reference when to stop in Update
        percentageTarget = percentage;
        coolingDown = true;
    }

    // Use this for initialization
    void Start () {
        text = GetComponentInChildren<Text>();
        if (text == null)
            Debug.Log("HP Text component (should be child) cannot be null");
        image = GetComponent<Image>();
        if (image == null)
            Debug.Log("HP Image component cannot be null");

        //We will use this for setting the health until our Monster class is finished to call this method
        //SetHealth(1000, 630);

        text.text = "HP " + Health + " / " + MaxHealth;
        
        //used for testing
        //ApplyDamage(15);
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
    }
}
