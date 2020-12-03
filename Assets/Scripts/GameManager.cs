using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //to access scripts and variables from other scripts,
    //help provided by https://www.youtube.com/watch?v=XeOzp6KJ1j8
    //more help provided by https://www.youtube.com/watch?v=I57PPyA_Dgg

    void Start()
    {
        
    }

    void Update()
    {
        V.frameCounter = V.frameCounter + 1;
        if (V.frameCounter > 60)  //once per second (curious, how to change to be time-based)
        {
            V.seconds = V.seconds + 1;
            V.frameCounter = 0;
        }

        if (V.isGameOver == true && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);  //0 = "Main Menu", 1="Game"

            //are these necessary if the scene is reloaded ?
            V.wave = 1;
            V.isGameOver = false;
            V.frameCounter = 0;
            V.setMode(0);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESCAPE KEY PRESSED");
            Application.Quit();
        }
    }
}