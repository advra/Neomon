using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler
{
    //well just use a global game object

    string tooltiptext;
    Text text;

    public void Awake()
    {
        if(text == null)
        {

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

}
