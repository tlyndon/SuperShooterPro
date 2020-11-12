using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  //public or private reference
  //data types (int, float, bool, string)

  [SerializeField]
  private float _speed = 3.5f;
  private float _speedMultiplier = 2;
  [SerializeField]
  private GameObject _laserPrefab;
  [SerializeField]
  private GameObject _tripleShotPrefab;
  [SerializeField]
  private float _fireRate = 0.15f;
  private float _canFire = -1f;
  [SerializeField]
  private int _lives = 3;
  private SpawnManager _spawnManager;
  private bool _isTripleShotActive=false;
  private bool _isSpeedBoostActive=false;
  private bool _isShieldsActive=false;
  [SerializeField]
  private GameObject _shieldVisualizer;

  [SerializeField]
  private GameObject _leftEngine;
  [SerializeField]
  private GameObject _rightEngine;

  [SerializeField]
  private int _score;

  [SerializeField]
  private UIManager _uiManager;

  [SerializeField]
  private AudioSource _audioSource;

  void Start()

  {
    _shieldVisualizer.SetActive(false);

    _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

    transform.position = new Vector3(0, -3.4f, 0);   //move the ship down on the screen a little bit at start
    _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    _audioSource = GetComponent<AudioSource>();
    //_audioSource.SetActive(true);

    if (_spawnManager == null)
    {
      Debug.LogError("The Spawn manager is NULL.");
    }

    if (_uiManager == null)
    {
      Debug.LogError("UI Manager is NULL.");
    }

    if (_audioSource==null)
    {
      Debug.Log("The AudioSource component in Player.cs = NULL");
    }
  }

  void Update()
  {
    CalculateMovement();

    if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
    {
      // Debug.Log("Space Key Pressed");
      FireLaser();
    }
  }

  void FireLaser()
  {
    _canFire = Time.time + _fireRate;

    if (_isTripleShotActive == true)
    {
      Instantiate(_tripleShotPrefab, transform.position + new Vector3(4.7f,0,0), Quaternion.identity);
    }
    else
    {
      Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
    }

    _audioSource.Play();

  }

  public void TripleShotActive()
  {
    _isTripleShotActive=true;
    StartCoroutine(TripleShotPowerDownRoutine());
  }

  IEnumerator TripleShotPowerDownRoutine()
  {
    yield return new WaitForSeconds(5.0f);
    _isTripleShotActive=false;
  }

  public void SpeedBoostActive()
  {
    _isSpeedBoostActive=true;
    _speed *= _speedMultiplier;
    StartCoroutine(SpeedBoostPowerDownRoutine());
  }

  IEnumerator SpeedBoostPowerDownRoutine()
  {
    yield return new WaitForSeconds(5.0f);
    _isSpeedBoostActive=false;
    _speed /= _speedMultiplier;
  }

  public void Damage()
  {
    if (_isShieldsActive == true)
    {
      Debug.Log("Shields Protected me!");
      _isShieldsActive=false;
      _shieldVisualizer.SetActive(false);
      return;
    }
    else
    _lives--;

    if (_lives==2)
    {
      _leftEngine.SetActive(true);
    }
    if (_lives==1)
    {
      _rightEngine.SetActive(true);
    }

    _uiManager.UpdateLives(_lives);

    if (_lives < 1)
    {
      _spawnManager.OnPlayerDeath();  //find the gameObject then get component

      Destroy(this.gameObject);
      Debug.Log("Game Over");
    }
  }

  void CalculateMovement()
  {
    float horizontalInput = Input.GetAxis("Horizontal");
    float verticalInput = Input.GetAxis("Vertical");

    Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

    if (_isSpeedBoostActive == false)
    {
      transform.Translate(direction * _speed * Time.deltaTime);
    }
    else
    {
      transform.Translate(direction * (_speed * _speedMultiplier) * Time.deltaTime);
    }

    transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y,-3.8f,0),0);

    if (transform.position.x > 11f)
    {
      transform.position = new Vector3(-11f, transform.position.y, 0);
    }
    else if (transform.position.x < -11f)
    {
      transform.position = new Vector3(11f, transform.position.y, 0);
    }
  }

  public void ShieldsActive()
  {
    _isShieldsActive=true;
    _shieldVisualizer.SetActive(true);
  }

  public void AddToScore(int points)
  {
    _score = _score + points;
    _uiManager.UpdateScore(_score);
  }
}
