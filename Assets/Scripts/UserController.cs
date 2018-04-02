using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour {

    [SerializeField]
    private GameObject PauseMenu;
    BattleController battleController;
    [SerializeField]
    private bool menuActive;
    [SerializeField]
    private bool userturn;

    public bool IsUsersTurn
    {
        get { return userturn; }
        set { userturn = value;  }
    }

    public bool MenuIsActive
    {
        get { return menuActive; }
        set { menuActive = value; }
    }

    public void Start()
    {
        if(battleController == null)
        {
            battleController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleController>();
        }

        PauseMenu = Instantiate(Resources.Load<GameObject>("Menu/PauseMenuPrefab"), battleController.canvasRect.transform);
        Canvas canvas = PauseMenu.GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 20;
        PauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (PauseMenu.activeInHierarchy)
            {
                MenuIsActive = false;
                ContinueGame();
            }
            else
            {
                MenuIsActive = true;
                PauseGame();
            }
        }
    }

    public void ContinueGame()
    {
        PauseMenu.SetActive(false);
        battleController.PauseGame = false;
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        battleController.PauseGame = true;
    }
}
