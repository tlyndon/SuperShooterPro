using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip snd_lasershot;                      //snd_xxxx throughout code should indicate sound
    [SerializeField]
    private AudioClip snd_buzz;

    private bool isSpeedBoostActive = false;
    private float speed = 3.5f;
    private float speedMultiplier = 2.0f;
    private float leftShiftKeySpeedMultiplier = 1.5f;
    private bool isLeftShiftKeySpeedBoostActive = false;

    [SerializeField]
    private GameObject laserPrefab;
    private float fireRate = 0.15f;
    private float canFire = -1f;

    [SerializeField]
    private GameObject tripleShotPrefab;
    private bool isTripleShotActive = false;

    [SerializeField]
    private GameObject missilesPrefab;
    private bool areMissilesActive = true;
    private float canFireMissiles = 1f;
    private float fireRateOfMissiles = 0.15f;

    [SerializeField]
    private GameObject explosionPrefab;

    [SerializeField]
    private SpawnManager spawnManager;
    [SerializeField]
    public GameObject shieldVisualizer;
    public SpriteRenderer shieldSpriteRenderer;
    public float shieldAlpha = 1f;
    public bool isShieldsActive = false;
    public float shieldStrengthDefault = 3f;
    public float shieldStrength;

    [SerializeField]
    private GameObject leftEngine;
    [SerializeField]
    private GameObject rightEngine;

    [SerializeField]
    private UIManager uiManager;

    public int score;
    public int lives = 4;
    public int health = 3;

    [SerializeField]
    private GameObject ammoPrefab;
    public int ammoCountDefault = 15;
    public int ammoCount;

    [SerializeField]
    private GameObject healthGreen;
    [SerializeField]
    private GameObject healthYellow;
    [SerializeField]
    private GameObject healthRed;

    private float timeLastMissileShot = 0;
    private float lowestYpos = -3.8f;
    private float playerStartingYposBelowScreen = -10f;
    //--------------------------------------------------------------
    void Start()
    {      
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { Debug.LogError("audioSource in AudioManager.cs = null."); }

        health = 3;
        transform.position = new Vector3(0, playerStartingYposBelowScreen, 0);

        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (uiManager == null) { Debug.LogError("UI Manager is null."); }

        ammoCount = ammoCountDefault;
        uiManager.UpdateAmmo(ammoCount);

        spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (spawnManager == null) { Debug.LogError("The Spawn manager is null."); }

        shieldSpriteRenderer = shieldVisualizer.GetComponent<SpriteRenderer>();
        if (shieldSpriteRenderer == null) { Debug.Log("The Shield Sprite Renderer component in Player.cs = null"); }

        shieldStrength = shieldStrengthDefault;
        shieldVisualizer.SetActive(false);

        uiManager.GetReady();
    }
    //--------------------------------------------------------------
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))
        {
            isLeftShiftKeySpeedBoostActive = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isLeftShiftKeySpeedBoostActive = false;
        }

        movePlayerUpFromOffscreenAtStartOfNewLife();
        CalculateMovement();
        managePlayerFiring();
    }
    //--------------------------------------------------------------
    void managePlayerFiring()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > canFire)
        {
            if (ammoCount > 0)
            {
                if (Time.time > timeLastMissileShot + 5)
                {
                    timeLastMissileShot = Time.time;
                    FireMissiles();
                    ammoCount = ammoCount - 1;
                    uiManager.UpdateAmmo(ammoCount);
                }
                else
                {
                    FireLaser();
                    ammoCount = ammoCount - 1;
                    uiManager.UpdateAmmo(ammoCount);
                }
            }
            else
            {
                if (AudioManager.soundOn == true) { audioSource.PlayOneShot(snd_buzz, 0.7F); }
            }
        }
    }
    //--------------------------------------------------------------
    void movePlayerUpFromOffscreenAtStartOfNewLife()
    {
        int comeUpToPositionY = -3;
        if (transform.position.y < comeUpToPositionY)
        {
            float deltaY = comeUpToPositionY - transform.position.y;
            if (deltaY < .01) { transform.position = new Vector3(0, comeUpToPositionY, 0); }
            else
            {
                if (deltaY < 0.20f) { deltaY = 0.20f; }

                Vector3 direction = new Vector3(0, deltaY, 0);
                transform.Translate(direction * Time.deltaTime);
            }
        }
    }
    //--------------------------------------------------------------
    IEnumerator showShieldStrengthVisually()
    {
        shieldAlpha = 0.4f + ((shieldStrength / shieldStrengthDefault) * 0.6f);
        shieldSpriteRenderer.color = new Color(1f, 1f, 1f, shieldAlpha);
        yield return new WaitForSeconds(5.0f);
    }
    //--------------------------------------------------------------
    void FireLaser()
    {
        canFire = Time.time + fireRate;
        if (isTripleShotActive == true)
        {
            Instantiate(tripleShotPrefab, transform.position + new Vector3(4.7f, 0, 0), Quaternion.identity);
        }
        else
        {
            Instantiate(laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        if (AudioManager.soundOn == true) { audioSource.PlayOneShot(snd_lasershot, 0.7F); }
    }
    //--------------------------------------------------------------
    void FireMissiles()
    {
        canFireMissiles = Time.time + fireRateOfMissiles;
        if (areMissilesActive == true)
        {
            Instantiate(missilesPrefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
        }

        if (AudioManager.soundOn == true) { audioSource.PlayOneShot(snd_lasershot, 0.7F); }

    }
    //--------------------------------------------------------------
    public void TripleShotActive()
    {
        isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }
    //--------------------------------------------------------------
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        isTripleShotActive = false;
    }
    //--------------------------------------------------------------
    public void SpeedBoostActive()
    {
        isSpeedBoostActive = true; speed *= speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    //--------------------------------------------------------------
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        isSpeedBoostActive = false; speed /= speedMultiplier;
    }
    //--------------------------------------------------------------
    public void Damage()
    {
        if (isShieldsActive == true)
        {
            Debug.Log("Shields Protected me!");
            shieldStrength = shieldStrength - 1;

            if (shieldStrength < 0)
            {
                isShieldsActive = false;
                shieldVisualizer.SetActive(false);
                return;
            }
            else
            {
                StartCoroutine(showShieldStrengthVisually());
            }
        }

        else
            health = health - 1;

        if (health == 2)
        {
            //Debug.Log("healthGreen about to be changed to Yellow");
            healthGreen.gameObject.SetActive(false);
            healthYellow.gameObject.SetActive(true);
            leftEngine.gameObject.SetActive(true);
        }
        else if (health == 1)
        {
            //Debug.Log("healthYellow about to be changed to Red");
            healthYellow.SetActive(false);
            healthRed.gameObject.SetActive(true);
            rightEngine.SetActive(true);
        }

        if (health < 1)
        {
            healthRed.gameObject.SetActive(false);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            if (lives == 0)
            {
                //yes, we are out of more lives, so it's time to die
                spawnManager.OnPlayerLossOfHealth();
                Destroy(this.gameObject);
                Debug.Log("Game Over");
                uiManager.GameOverSequence();

            }
            else
            {
                lives = lives - 1;
                uiManager.UpdateLives(lives);
                
                //put player below the screen, so he'll come back
                transform.position = new Vector3(0, playerStartingYposBelowScreen, 0);
                uiManager.GetReady();

                //still have another life, so...
                RestoreHealth();
                SetAmmoToDefaultValue();
            }

        }
    }
    //--------------------------------------------------------------
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (verticalInput != 0 || horizontalInput != 0)
        {
            uiManager.UseThrusters();
            {

                    Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
                    if (isSpeedBoostActive == true) { transform.Translate(direction * (speed * speedMultiplier) * Time.deltaTime); }
                    else if (isLeftShiftKeySpeedBoostActive == true) { transform.Translate(direction * (speed * leftShiftKeySpeedMultiplier) * Time.deltaTime); }
                    else { transform.Translate(direction * speed * Time.deltaTime); }

                    if (verticalInput > 0 && transform.position.y > 7)
                    {
                    transform.position = new Vector3(transform.position.x, 7, 0);
                    }
                    else if (transform.position.x != 0 && verticalInput < 0 && transform.position.y < lowestYpos)
                    {
                        transform.position = new Vector3(transform.position.x, lowestYpos, 0);
                    }

                    if (transform.position.x > 11f) { transform.position = new Vector3(-11f, transform.position.y, 0); }
                    else if (transform.position.x < -11f) { transform.position = new Vector3(11f, transform.position.y, 0); }
            }
        }
    }
    //--------------------------------------------------------------
    public void ShieldsActive()
    {
        isShieldsActive = true;
        shieldStrength = shieldStrengthDefault;
        shieldVisualizer.SetActive(true);
        StartCoroutine(showShieldStrengthVisually());
    }
    //--------------------------------------------------------------
    public void AddToScore(int points)
    {
        score = score + points;
        uiManager.UpdateScore(score);
    }
    //--------------------------------------------------------------
    public void SetAmmoToDefaultValue()
    {
        Debug.Log("Reset ammoCount to default");
        ammoCount = ammoCountDefault;
        uiManager.UpdateAmmo(ammoCount);
    }
    //--------------------------------------------------------------
    public void RestoreHealth()
    {
        leftEngine.gameObject.SetActive(false);
        rightEngine.gameObject.SetActive(false);
        uiManager.thrustersPct = 1.0f;
        uiManager.updateThrustersMeter();
        health = 3;
        healthGreen.gameObject.SetActive(true);
        healthYellow.gameObject.SetActive(false);
        healthRed.gameObject.SetActive(false);
    }
    //--------------------------------------------------------------
    // IEnumerator NewCameraShake2()
    //     {
    //         _isCameraShaking = true;
    //         // get orig camera position
    //         GameObject myCamera = GameObject.Find("Main Camera");
    //         Debug.Log("myCamera=" + myCamera);
    //         if (myCamera == null)
    //         {
    //             Debug.LogError("Player: myCamera = Null");
    //         }
    //     //    Vector3 origCameraPosition = myCamera.transform.position;
    //         Vector3 origCameraPosition = new Vector3(0f, 0f, -10f);
    //         while (true)
    //         {
    //             Debug.Log("In New Camera Shake loop");
    //             // generate new camera position
    //             Vector3 newCamPosition = new Vector3(1f, 1f, -10f);
    //             // Set new camera position
    //             Debug.Log("Set New Cam position");
    //             myCamera.transform.position = newCamPosition;
    //             // wait for .5 sec
    //             yield return new WaitForSeconds(0.5f);
    //             // Back to original position
    //             Debug.Log("Back to orig position");
    //             myCamera.transform.position = origCameraPosition;
    //         }
    //     }
}