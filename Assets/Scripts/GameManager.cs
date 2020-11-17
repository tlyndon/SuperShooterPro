using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  [SerializeField]
  private bool _isGameOver;

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.R) && _isGameOver==true)
    {
      SceneManager.LoadScene(1);  //0 = "Main Menu", 1="Game"
    }
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Debug.Log("ESCAPE KEY PRESSED");
        Application.Quit();
    }
  }

  public void GameOver()
  {
    _isGameOver = true;
  }
}
