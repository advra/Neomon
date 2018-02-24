using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Button : MonoBehaviour {
    public Button[] Buttons;    //array that stores all out buttons made for referencing

    private string text;
    private Sprite sprite;
    private bool active = false;
    private string spritePath = "Sprites/";


    public Button(string s, string spriteFile)
    {
        if (s == null)
        {
            Debug.Log("Button instance name cannot be null");
        }
        if (spriteFile == null)
        {
            Debug.Log("Button instance name cannot be null");
        }

        this.text = s;
        //Sprite x = Resources.Load(spritePath + spriteFile, typeof(Sprite)) as Sprite;

        //Sprite newButton = Instantiate(this.sprite);

    }

    public Sprite Sprite
    {
        get;
        private set; 
            
    }

    public string Text
    {
        get { return text; }
    }

}
