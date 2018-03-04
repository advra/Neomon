using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    public GameObject selected;
    //public bool hit;
    RaycastHit hitInfo;
    CardController cardController;
    RaycastHit2D hit;
     Ray ray;
     Ray screenRay;
	// Use this for initialization
	void Start () {
        
    }
    
    /// Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {
        // if(Input.GetMouseButtonDown(0)){
        //     hitInfo = new RaycastHit();
        //     hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        //     if(hit){
        //         Debug.Log("Target Hit is" + hitInfo.transform.gameObject);
        //     }
        // }
        
        hitInfo = new RaycastHit();
        screenRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        hit = Physics2D.GetRayIntersection(screenRay);
        if (hit)
        {
            Debug.Log("hit something: " + hit.transform.gameObject);
        }

        
    }
	
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    void FixedUpdate()
    {
        //Check for User clicks
        // if(Input.GetMouseButtonDown(0))
        // {
        //     Debug.Log("Mouse clicked");
        //     RaycastHit hitInfo = new RaycastHit();
        //     hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        //     if(hit){
        //         Debug.Log("Hit is true!");
        //         if(hitInfo.transform.gameObject.tag == "Card")
        //         {
        //             Debug.Log("Card found");
        //             cardController.DragCardTo(hitInfo.transform.gameObject);
        //         }
        //     }
        // }
    }
}
