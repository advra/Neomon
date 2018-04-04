using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour {

    BattleController battleController;

    [SerializeField]
    private GameObject PauseMenu;
    [SerializeField]
    private GameObject DeckWindow;
    [SerializeField]
    private bool menuActive;
    [SerializeField]
    private bool userturn;

    public GameObject DeckWindowGameObject
    {
        get { return DeckWindow; }
        set { DeckWindow = value; }
    }

    public bool IsUsersTurn
    {
        get { return userturn; }
        set { userturn = value;  }
    }

    public bool SetPauseMenuVisible
    {
        set { PauseMenu.SetActive(value); }
    }

    public bool MenuIsActive
    {
        get { return menuActive; }
        set
        {
            menuActive = value;

            if (MenuIsActive)
            {
                PauseGame();
            }
            else
            {
                ContinueGame();
            }
        }
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
                PauseMenu.SetActive(false);
                ContinueGame();
            }
            else
            {
                MenuIsActive = true;
                PauseMenu.SetActive(true);
                PauseGame();
            }
        }
    }

    public void ContinueGame()
    {
        battleController.PauseGame = false;
    }

    public void PauseGame()
    {
        battleController.PauseGame = true;
    }

    //used for things like a settings button
    public void PauseToggle()
    {
        MenuIsActive = true;
        PauseMenu.SetActive(true);
        PauseGame();

    }
}
