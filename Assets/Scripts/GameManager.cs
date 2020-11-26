using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public static bool isGameOver;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && isGameOver == true)
        {
            SceneManager.LoadScene(1);  //0 = "Main Menu", 1="Game"
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESCAPE KEY PRESSED");
            Application.Quit();
        }
    }
}