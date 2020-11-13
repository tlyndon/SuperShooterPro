using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class Player : MonoBehaviour
{
  //public or private reference
  //data types (int, float, bool, string)

  [SerializeField]
  private float _speed = 3.5f;
  private float _speedMultiplier = 2.0f;
  private float _leftShiftKeySpeedMultiplier = 1.5f;
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
  private bool _isLeftShiftKeySpeedBoostActive=false;

  [SerializeField]
  private GameObject _shieldVisualizer;
  private SpriteRenderer _shieldSpriteRenderer;
  private float _shieldAlpha=1f;
  private bool _isShieldsActive=false;
  private float _shieldStrengthDefault=3f;
  private float _shieldStrength;

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
    _shieldStrength=_shieldStrengthDefault;
    _shieldSpriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
    _shieldVisualizer.SetActive(false);

    _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

    _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    _audioSource = GetComponent<AudioSource>();

    if (_spawnManager == null)
    {
      Debug.LogError("The Spawn manager is null.");
    }

    if (_uiManager == null)
    {
      Debug.LogError("UI Manager is null.");
    }

    if (_audioSource==null)
    {
      Debug.Log("The AudioSource component in Player.cs = null");
    }

    if (_shieldSpriteRenderer==null)
    {
      Debug.Log("The Shield Sprite Renderer component in Player.cs = null");
    }

  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))
    {
      _isLeftShiftKeySpeedBoostActive=true;
    }
    else if (Input.GetKeyUp(KeyCode.LeftShift))
    {
      _isLeftShiftKeySpeedBoostActive=false;
    }

    CalculateMovement();

    if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
    {
      FireLaser();
    }
  }

  IEnumerator showShieldStrengthVisually()
  {
    _shieldAlpha = 0.4f + ((_shieldStrength/_shieldStrengthDefault)*0.6f);
    Debug.Log("_shieldAlpha:" + _shieldAlpha);
    _shieldSpriteRenderer.color = new Color(_shieldAlpha,1f,1f,_shieldAlpha);

    yield return new WaitForSeconds(4f);
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

      _shieldStrength=_shieldStrength-1;
      if (_shieldStrength<0)
      {
        _isShieldsActive=false;
        _shieldVisualizer.SetActive(false);
        return;
      }
      else
      {
        StartCoroutine(showShieldStrengthVisually());
      }
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

    if (_isSpeedBoostActive == true)
    {
      transform.Translate(direction * (_speed * _speedMultiplier) * Time.deltaTime);
    }
    else if (_isLeftShiftKeySpeedBoostActive == true)
    {
      transform.Translate(direction * (_speed * _leftShiftKeySpeedMultiplier) * Time.deltaTime);
    }
    else
    {
      transform.Translate(direction * _speed * Time.deltaTime);
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
    _shieldStrength=_shieldStrengthDefault;
    _shieldVisualizer.SetActive(true);
    StartCoroutine(showShieldStrengthVisually());
  }

  public void AddToScore(int points)
  {
    _score = _score + points;
    _uiManager.UpdateScore(_score);
  }
}
