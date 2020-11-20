using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//------------------------------
public class Player : MonoBehaviour
{
  [SerializeField]
  private AudioManager myAudioManager;
  [SerializeField]
  private AudioSource audioSource;
  [SerializeField]
  private AudioClip _lasershot;
  [SerializeField]
  private AudioClip _buzz;

  private bool _isSpeedBoostActive=false;
  private float _speed = 3.5f;
  private float _speedMultiplier = 2.0f;
  private float _leftShiftKeySpeedMultiplier = 1.5f;
  private bool _isLeftShiftKeySpeedBoostActive=false;

  [SerializeField]
  private GameObject _laserPrefab;
  private float _fireRate = 0.15f;
  private float _canFire = -1f;

  [SerializeField]
  private GameObject _tripleShotPrefab;
  private bool _isTripleShotActive=false;

  [SerializeField]
  private GameObject _missilesPrefab;
  private bool _areMissilesActive = true;
  private float _canFireMissiles = 1f;
  private float _fireRateOfMissiles = 0.15f;

  [SerializeField]
  private GameObject _explosionPrefab;

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
  public int lives = 4;
  public int health = 3;

  [SerializeField]
  private GameObject _ammoPrefab;
  public int ammoCountDefault=15;
  public int ammoCount;

  private int level=1;

  [SerializeField]
  private GameObject healthGreen;
  [SerializeField]
  private GameObject healthYellow;
  [SerializeField]
  private GameObject healthRed;

  private float timeLastMissileShot=0;
  private float _lowestYpos=-3.8f;
  private float _playerStartingYposBelowScreen=-20f;
  //------------------------------
  void Start()
  {
    if (myAudioManager == null) { Debug.LogError("myAudioManager is null."); }
    // audioSource = GetComponent<AudioSource>();
    if (audioSource==null) { Debug.Log("The audioSource component in Player.cs = null"); }

    health=3;
    transform.position = new Vector3(0, _playerStartingYposBelowScreen, 0);

    _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    if (_uiManager == null) { Debug.LogError("UI Manager is null."); }

    ammoCount=ammoCountDefault;
    _uiManager.UpdateAmmo(ammoCount);

    _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    if (_spawnManager == null) { Debug.LogError("The Spawn manager is null."); }

    _shieldSpriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
    if (_shieldSpriteRenderer==null) { Debug.Log("The Shield Sprite Renderer component in Player.cs = null"); }

    _shieldStrength=_shieldStrengthDefault;
    _shieldVisualizer.SetActive(false);

    _uiManager.GetReady();
  }
  //------------------------------
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
        if (myAudioManager.SoundStatus()==true) { audioSource.PlayOneShot(_buzz, 0.7F); }
      }
    }

    if (Input.GetKeyDown(KeyCode.M) && Time.time > _canFireMissiles)
    {
      // Debug.Log("Pressed M Key");
      if (Time.time>timeLastMissileShot+5)
      {
        timeLastMissileShot=Time.time;
        FireMissiles();
      }
      else
      {
        if (myAudioManager.SoundStatus()==true) { audioSource.PlayOneShot(_buzz, 0.7F); }
      }
    }
    //move player up if he's below the screen at the start of a new life
    movePlayerUpFromOffscreenAtStartOfNewLife();
  }
  //------------------------------
  void movePlayerUpFromOffscreenAtStartOfNewLife()
  {
    int comeUpToPositionY=-3;
    if (transform.position.x==0 && transform.position.y<comeUpToPositionY)
    {
      float deltaY=comeUpToPositionY-transform.position.y;
      if (deltaY<.01)
      {
        transform.position = new Vector3(0,comeUpToPositionY,0);
      }
      else
      {
        if (deltaY<0.20f) {deltaY=0.20f;}
        Vector3 direction = new Vector3(0,deltaY,0);
        transform.Translate(direction * Time.deltaTime);
      }
    }
  }
  //------------------------------
  IEnumerator showShieldStrengthVisually()
  {
    _shieldAlpha = 0.4f + ((_shieldStrength/_shieldStrengthDefault)*0.6f);
    Debug.Log("_shieldAlpha:" + _shieldAlpha);
    _shieldSpriteRenderer.color = new Color(1f,1f,1f,_shieldAlpha);
    yield return new WaitForSeconds(5.0f);
  }
  //------------------------------
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

    if (myAudioManager.SoundStatus()==true) { audioSource.PlayOneShot(_lasershot, 0.7F); }
  }
  //------------------------------
  void FireMissiles()
  {
    _canFireMissiles = Time.time + _fireRateOfMissiles;
    if (_areMissilesActive == true)
    {
      Instantiate(_missilesPrefab, transform.position + new Vector3(0,2f,0), Quaternion.identity);
    }

    if (myAudioManager.SoundStatus()==true) { audioSource.PlayOneShot(_lasershot, 0.7F); }

  }
  //------------------------------
  public void TripleShotActive()
  {
    _isTripleShotActive=true;
    StartCoroutine(TripleShotPowerDownRoutine());
  }
  //------------------------------
  IEnumerator TripleShotPowerDownRoutine()
  {
    yield return new WaitForSeconds(5.0f);
    _isTripleShotActive=false;
  }
  //------------------------------
  public void SpeedBoostActive()
  {
    _isSpeedBoostActive=true; _speed *= _speedMultiplier;
    StartCoroutine(SpeedBoostPowerDownRoutine());
  }
  //------------------------------
  IEnumerator SpeedBoostPowerDownRoutine()
  {
    yield return new WaitForSeconds(5.0f);
    _isSpeedBoostActive=false; _speed /= _speedMultiplier;
  }
  //------------------------------
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
    Debug.Log("healthGreen about to be changed");
    health = health-1;

    if (health==2)
    {
      Debug.Log("healthGreen about to be changed");
      healthGreen.gameObject.SetActive(false);
      healthYellow.gameObject.SetActive(true);
      _leftEngine.gameObject.SetActive(true);
    }
    else if (health==1)
    {
      Debug.Log("healthYellow about to be changed");
      healthYellow.SetActive(false);
      healthRed.gameObject.SetActive(true);
      _rightEngine.SetActive(true);
    }

    if (health < 1)
    {
      healthRed.SetActive(false);
      Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

      //are we out of more lives?
      if (lives == 0)
      {
        //yes, we are out of more lives, so it's time to die
        _spawnManager.OnPlayerLossOfHealth();
        Destroy(this.gameObject);
        Debug.Log("Game Over");
        _uiManager.GameOverSequence();

      }
      else
      {
        lives = lives-1;
        _uiManager.UpdateLives(lives);
        //still have another life, so...
        RestoreHealth();
        SetAmmoToDefaultValue();
        //put player below the screen, so he'll come back
        transform.position = new Vector3(0, _playerStartingYposBelowScreen, 0);
        _uiManager.GetReady();
      }

    }
  }
  //------------------------------
  void CalculateMovement()
  {
    if (transform.position.x!=0 || transform.position.y >= _lowestYpos)
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

      // only clamp the minimum y position after the player has moved the x position
      // if (transform.position.x!=0)
      // {
      transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y,_lowestYpos,7),0);
      // }

      if (transform.position.x > 11f)
      {
        transform.position = new Vector3(-11f, transform.position.y, 0);
      }
      else if (transform.position.x < -11f)
      {
        transform.position = new Vector3(11f, transform.position.y, 0);
      }
    }
  }
  //------------------------------
  public void ShieldsActive()
  {
    _isShieldsActive=true;
    _shieldStrength=_shieldStrengthDefault;
    _shieldVisualizer.SetActive(true);
    StartCoroutine(showShieldStrengthVisually());
  }
  //------------------------------
  public void AddToScore(int points)
  {
    _score = _score + points;
    _uiManager.UpdateScore(_score);
  }
  //------------------------------
  public void SetAmmoToDefaultValue()
  {
    Debug.Log("Reset ammoCount to default");
    ammoCount=ammoCountDefault;
    _uiManager.UpdateAmmo(ammoCount);
  }
  //------------------------------
  public void RestoreHealth()
  {
    _leftEngine.SetActive(false);
    _rightEngine.SetActive(false);
    health=3;
    healthGreen.gameObject.SetActive(true);
    healthYellow.gameObject.SetActive(false);
    healthRed.gameObject.SetActive(false);
  }
}
