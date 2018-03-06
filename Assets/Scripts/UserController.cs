using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour {
    [SerializeField]
    private GameObject selectedObject;
    [SerializeField]
    private GameObject lastSelectedObject;
    private RaycastHit hitInfo;
    private RaycastHit2D hit2D;
    private Ray screenMouseRay;
    
    public GameObject SelectedObject
    {
        get { return selectedObject; }
    }

    public GameObject LastSelectedObject
    {
        get { return lastSelectedObject; }
    }
    // Use this for initialization
    void Start () {
		
	}

    void Update()
    {
        MouseHitsCollider();
    }

    public bool MouseHitsCollider()
    {
        hitInfo = new RaycastHit();
        screenMouseRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        hit2D = Physics2D.GetRayIntersection(screenMouseRay);
        if (hit2D)
        {
            selectedObject = hit2D.transform.gameObject;
            return true;
        }
        else
        {
            selectedObject = null;
            return false;
        }
    }


}
