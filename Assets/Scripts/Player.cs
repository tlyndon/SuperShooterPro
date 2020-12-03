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
    private float speedMultiplier = 1.75f;
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
    public bool cameraShake = false;
    //--------------------------------------------------------------
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        { Debug.LogError("audioSource in Player.cs = null."); }

        health = 3;
        transform.position = new Vector3(0, playerStartingYposBelowScreen, 0);

        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (uiManager == null)
        { Debug.LogError("UI Manager is null."); }

        ammoCount = ammoCountDefault;
        uiManager.UpdateAmmo(ammoCount, ammoCountDefault);

        spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (spawnManager == null)
        { Debug.LogError("The Spawn manager is null."); }

        shieldSpriteRenderer = shieldVisualizer.GetComponent<SpriteRenderer>();
        if (shieldSpriteRenderer == null)
        { Debug.Log("The Shield Sprite Renderer component in Player.cs = null"); }

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
                if (V.seconds > timeLastMissileShot + 3)
                {
                    timeLastMissileShot = V.seconds;
                    FireMissiles();
                    ammoCount = ammoCount - 1;
                    uiManager.UpdateAmmo(ammoCount, ammoCountDefault);
                }
                else
                {
                    FireLaser();
                    ammoCount = ammoCount - 1;
                    uiManager.UpdateAmmo(ammoCount, ammoCountDefault);
                }
            }
            else
            {
                if (V.soundOn == true)
                { audioSource.PlayOneShot(snd_buzz, 0.7F); }
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
            if (deltaY < .01)
            { transform.position = new Vector3(0, comeUpToPositionY, 0); }
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

        if (V.soundOn == true)
        { audioSource.PlayOneShot(snd_lasershot, 0.7F); }
    }
    //--------------------------------------------------------------
    void FireMissiles()
    {
        canFireMissiles = Time.time + fireRateOfMissiles;
        if (areMissilesActive == true)
        {
            Instantiate(missilesPrefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
        }

        if (V.soundOn == true)
        { audioSource.PlayOneShot(snd_lasershot, 0.7F); }

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
        NewCameraShake1();
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
            healthGreen.gameObject.SetActive(false);
            healthYellow.gameObject.SetActive(true);
            leftEngine.gameObject.SetActive(true);
        }
        else if (health == 1)
        {
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

            if (uiManager.thrustersPct > 0.02f)
            {
                uiManager.UseThrusters();
                Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
                if (isSpeedBoostActive == true)
                { transform.Translate(direction * (speed * speedMultiplier) * Time.deltaTime); }

                else if (isLeftShiftKeySpeedBoostActive == true)
                { transform.Translate(direction * (speed * leftShiftKeySpeedMultiplier) * Time.deltaTime); }

                else
                { transform.Translate(direction * speed * Time.deltaTime); }

                if (verticalInput > 0 && transform.position.y > 7)
                {
                    transform.position = new Vector3(transform.position.x, 7, 0);
                }

                else if (transform.position.x != 0 && verticalInput < 0 && transform.position.y < lowestYpos)
                {
                    transform.position = new Vector3(transform.position.x, lowestYpos, 0);
                }

                if (transform.position.x > 11f)
                { transform.position = new Vector3(-11f, transform.position.y, 0); }

                else if (transform.position.x < -11f)
                { transform.position = new Vector3(11f, transform.position.y, 0); }
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
        uiManager.UpdateAmmo(ammoCount, ammoCountDefault);
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
    public void NewCameraShake1()
    {
        cameraShake = true;
        StartCoroutine(NewCameraShake2());
    }
    //--------------------------------------------------------------
    public IEnumerator NewCameraShake2()
    {
        GameObject myCamera = GameObject.Find("Main Camera");
        //Debug.Log("myCamera=" + Time.deltaTime);
        if (myCamera == null)
        {
            Debug.LogError("Player: myCamera = Null");
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            //Vector3 origCameraPosition = myCamera.transform.position;
            //Debug.Log("About to Shake Camera:");

            float rdmX = Random.Range(-1f, 1f); rdmX = rdmX * 0.2f;
            float rdmY = Random.Range(-1f, 1f); rdmY = 1 + (rdmY * 0.2f);
            Vector3 newCamPosition = new Vector3(rdmX, rdmY, -10f);
            myCamera.transform.position = newCamPosition;
            yield return new WaitForSeconds(0.15f);

            rdmX = Random.Range(-1f, 1f); rdmX = rdmX * 0.2f;
            rdmY = Random.Range(-1f, 1f); rdmY = 1 + (rdmY * 0.2f);
            newCamPosition = new Vector3(rdmX, rdmY, -10f);
            myCamera.transform.position = newCamPosition;
            yield return new WaitForSeconds(0.15f);

            rdmX = Random.Range(-1f, 1f); rdmX = rdmX * 0.2f;
            rdmY = Random.Range(-1f, 1f); rdmY = 1 + (rdmY * 0.2f);
            newCamPosition = new Vector3(rdmX, rdmY, -10f);
            myCamera.transform.position = newCamPosition;
            yield return new WaitForSeconds(0.15f);

            //Debug.Log("Restore Camera");
            myCamera.transform.position = new Vector3(0, 1, -10);  // origCameraPosition;
            yield return new WaitForSeconds(0.15f);
        }
    }
}