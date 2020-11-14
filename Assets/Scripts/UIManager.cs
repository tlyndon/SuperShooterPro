using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  [SerializeField]
  private GameManager _gameManager;
  [SerializeField]
  private Text _scoreText;
  [SerializeField]
  private Text _ammoText;
  [SerializeField]
  private Text _gameOverText;
  [SerializeField]
  private Text _restartText;
  [SerializeField]
  private Image _LivesImg;
  [SerializeField]
  private Sprite[] _liveSprites;
  void Start()
  {
    _gameManager=GameObject.Find("Game_Manager").GetComponent<GameManager>();
    if (_gameManager==null)
    {
      Debug.LogError("GameManager is NULL");
    }
    _scoreText.text= "Score: " + 0;
    _ammoText.text="Ammo: " + 0;
    _gameOverText.gameObject.SetActive(false);
    _restartText.gameObject.SetActive(false);
  }

  public void UpdateScore(int playerScore)
  {
    _scoreText.text = "Score: " + playerScore;
  }

  public void UpdateAmmo(int playerAmmo)
  {
    _ammoText.text = "Ammo: " + playerAmmo;
  }

  public void UpdateLives(int currentLives)
  {
    _LivesImg.sprite=_liveSprites[currentLives];
    if (currentLives==0)
    {
      GameOverSequence();
    }
  }

  void GameOverSequence()
  {
    _gameOverText.gameObject.SetActive(true);
    _restartText.gameObject.SetActive(true);
    StartCoroutine(GameOverFlickerRoutine());
    _gameManager.GameOver();
  }

  IEnumerator GameOverFlickerRoutine()
  {
    while(true)
    {
      _gameOverText.text = "GAME OVER";
      yield return new WaitForSeconds(0.5f);
      _gameOverText.text="";
      yield return new WaitForSeconds(0.5f);
    }
  }
}
