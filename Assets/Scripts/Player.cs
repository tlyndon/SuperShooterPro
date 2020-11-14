using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  private bool _isSpeedBoostActive=false;
  private float _speed = 3.5f;
  private float _speedMultiplier = 2.0f;
  private float _leftShiftKeySpeedMultiplier = 1.5f;
  private bool _isLeftShiftKeySpeedBoostActive=false;

  [SerializeField]
  private GameObject _laserPrefab;

  [SerializeField]
  private GameObject _tripleShotPrefab;
  private bool _isTripleShotActive=false;

  [SerializeField]
  private SpawnManager _spawnManager;
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
  private UIManager _uiManager;
  private int _score;
  private int _lives = 3;

  [SerializeField]
  private GameObject _ammoPrefab;
  private float _fireRate = 0.15f;
  private float _canFire = -1f;
  private int ammoCountDefault=15;
  public int ammoCount;

  [SerializeField]
  private AudioSource _audioSource;
  [SerializeField]
  private AudioClip _lasershot;
  [SerializeField]
  private AudioClip _buzz;

  void Start()
  {
    _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    if (_uiManager == null)
    { Debug.LogError("UI Manager is null."); }

    ammoCount=ammoCountDefault;
    _uiManager.UpdateAmmo(ammoCount);

    _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    if (_spawnManager == null)
    { Debug.LogError("The Spawn manager is null."); }

    _shieldSpriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
    if (_shieldSpriteRenderer==null)
    { Debug.Log("The Shield Sprite Renderer component in Player.cs = null"); }
    _shieldStrength=_shieldStrengthDefault;
    _shieldVisualizer.SetActive(false);

    _audioSource = GetComponent<AudioSource>();
    if (_audioSource==null)
    {
      Debug.Log("The _audioSource component in Player.cs = null");
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
      if (ammoCount>0)
      {
        FireLaser(); ammoCount=ammoCount-1;
        _uiManager.UpdateAmmo(ammoCount);
      }
      else
      {
        Debug.Log("Play Buzz Sound");
        _audioSource.PlayOneShot(_buzz, 0.7F);
      }
    }
  }

  IEnumerator showShieldStrengthVisually()
  {
    _shieldAlpha = 0.4f + ((_shieldStrength/_shieldStrengthDefault)*0.6f);
    Debug.Log("_shieldAlpha:" + _shieldAlpha);
    _shieldSpriteRenderer.color = new Color(1f,1f,1f,_shieldAlpha);
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

    Debug.Log("Play Laser Sound");
    _audioSource.PlayOneShot(_lasershot, 0.7F);
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
    _isSpeedBoostActive=true; _speed *= _speedMultiplier;
    StartCoroutine(SpeedBoostPowerDownRoutine());
  }

  IEnumerator SpeedBoostPowerDownRoutine()
  {
    yield return new WaitForSeconds(5.0f);
    _isSpeedBoostActive=false; _speed /= _speedMultiplier;
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
    else if (_lives==1)
    {
      _rightEngine.SetActive(true);
    }

    _uiManager.UpdateLives(_lives);

    if (_lives < 1)
    {
      _spawnManager.OnPlayerDeath();
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

  public void SetAmmoToDefaultValue()
  {
    Debug.Log("Reset ammoCount to default");
    ammoCount=ammoCountDefault;
    _uiManager.UpdateAmmo(ammoCount);
  }
}
