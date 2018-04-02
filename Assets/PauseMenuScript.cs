using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuScript : MonoBehaviour {

    UserController userController;

    public void GenerateLevel()
    {
        Time.timeScale = 1;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        if(userController == null)
        {
            userController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UserController>();
        }
        userController.ContinueGame();
    }
}
