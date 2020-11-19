using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//------------------------------
public class UIManager : MonoBehaviour
{
  [SerializeField]
  private GameManager _gameManager;
  [SerializeField]
  private Text _scoreText;
  [SerializeField]
  private Text _ammoText;
  public Text flashingText;
  [SerializeField]
  private Text _restartText;
  [SerializeField]
  private Image _LivesImg;
  [SerializeField]
  private Sprite[] _liveSprites;
  //------------------------------
  void Start()
  {
    _gameManager=GameObject.Find("Game_Manager").GetComponent<GameManager>();
    if (_gameManager==null)
    {
      Debug.LogError("GameManager is NULL");
    }
    _scoreText.text= "Score: " + 0;
    _ammoText.text="Ammo: " + 0;
    flashingText.gameObject.SetActive(false);
    _restartText.gameObject.SetActive(false);
  }
//------------------------------
  public void UpdateScore(int playerScore)
  {
    _scoreText.text = "Score: " + playerScore;
  }
//------------------------------
  public void UpdateAmmo(int playerAmmo)
  {
    _ammoText.text = "Ammo: " + playerAmmo;
  }
//------------------------------
  public void UpdateLives(int showRemainingLives)
  {
        _LivesImg.sprite=_liveSprites[showRemainingLives];
  }
//------------------------------
public void GetReady()
{
  StartCoroutine(showFlashingTextRoutine("GET READY!",2,2));
  // StartCoroutine(waitForXseconds(2));
}
//------------------------------
// IEnumerator waitForXseconds(float secs)
// {
//   Debug.Log("Wait for " + secs + " seconds");
//   yield return new WaitForSeconds(secs);
// }
//------------------------------
  public void GameOverSequence()
  {
    _restartText.gameObject.SetActive(true);
    StartCoroutine(showFlashingTextRoutine("GAME OVER",-1,0));
    _gameManager.GameOver();
  }
//------------------------------
  IEnumerator showFlashingTextRoutine(string txt, int howMany, int delayFirst)
  {
    flashingText.gameObject.SetActive(true);
    flashingText.text="";
    yield return new WaitForSeconds(delayFirst);

    while(howMany!=0)
    {
      flashingText.text = txt;
      yield return new WaitForSeconds(0.5f);
      flashingText.text="";
      yield return new WaitForSeconds(0.5f);
      if (howMany>0)
      {
        howMany=howMany-1;
      }
    }
  }
}
