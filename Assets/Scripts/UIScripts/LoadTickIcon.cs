using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadTickIcon : MonoBehaviour {
    [SerializeField]
    string imageLoaded;

    public void Load(string s )
    {
        imageLoaded = s;
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/bar/tick_" + s);
    }
	

}
