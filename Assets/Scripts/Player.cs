using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    private GameObject minePrefab;

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
    public bool playerMoving = false;
    //--------------------------------------------------------------
    void Start()
    {
        health = 3;
        transform.position = new Vector3(0, playerStartingYposBelowScreen, 0);

        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (uiManager == null)
        { V.zprint("error", "UI Manager is null."); }

        ammoCountDefault = 15;
        SetAmmoToDefaultValue();

        spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (spawnManager == null)
        { V.zprint("error", "The Spawn manager is null."); }

        shieldSpriteRenderer = shieldVisualizer.GetComponent<SpriteRenderer>();
        if (shieldSpriteRenderer == null)
        { V.zprint("error", "The Shield Sprite Renderer component in Player.cs = null"); }

        shieldStrength = shieldStrengthDefault;
        shieldVisualizer.SetActive(false);

        uiManager.GetReady();
    }
    //--------------------------------------------------------------
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            V.zprint("keys", "LEFT SHIFT KEY PRESSED");
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
            V.zprint("keys", "SPACE KEY PRESSED");
            if (ammoCount > 0)  //-1000 for testing
            {
                string weapon = "laser";  //default
                if (V.levelAndWave >= V.levelGetMissle && V.seconds > timeLastMissileShot + 5)  //1000 for testing
                {
                    GameObject[] gameObjects;
                    gameObjects = GameObject.FindGameObjectsWithTag("Enemy");
                    int count = gameObjects.Length;
                    gameObjects = GameObject.FindGameObjectsWithTag("Enemy2");
                    count = count + gameObjects.Length;
                    if (count > 0)
                    {
                        weapon = "missile";
                        timeLastMissileShot = V.seconds;
                        FireMissilesAndMines();
                        ammoCount = ammoCount - 1;
                        uiManager.UpdateAmmo(ammoCount, ammoCountDefault);
                    }
                    else
                    { timeLastMissileShot = V.seconds - 2; }
                }
                if (weapon == "laser")
                {
                    FireLaser();
                    ammoCount = ammoCount - 1;
                    uiManager.UpdateAmmo(ammoCount, ammoCountDefault);
                }
            }
            else
            {
                GameObject.Find("manageAudio").GetComponent<ManageEazySoundManager>().playBuzzSound();
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
            { transform.position = new Vector3(transform.position.x, comeUpToPositionY, 0); }
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

        GameObject.Find("manageAudio").GetComponent<ManageEazySoundManager>().playLaserSound();
    }
    //--------------------------------------------------------------
    void FireMissilesAndMines()
    {
        canFireMissiles = Time.time + fireRateOfMissiles;
        if (areMissilesActive == true)
        {
            if (V.mineCount == 0)
            {
                Instantiate(missilesPrefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
            }
            else
            {
                float bestDistance = 0;
                GameObject chosenEnemy = null;
                GameObject[] gameObjects;
                gameObjects = GameObject.FindGameObjectsWithTag("Enemy");
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    if (gameObjects[i].transform.position.y - 2 > transform.position.y)
                    {
                        float distance = Vector3.Distance(transform.position, gameObjects[i].transform.position);
                        if (distance > bestDistance)
                        {
                            bestDistance = i;
                            chosenEnemy = gameObjects[i];
                        }
                    }
                }
                GameObject[] gameObjects2;
                gameObjects2 = GameObject.FindGameObjectsWithTag("Enemy2");
                for (int i = 0; i < gameObjects2.Length; i++)
                {
                    if (gameObjects[i].transform.position.y - 3 > transform.position.y)
                    {
                        float distance = Vector3.Distance(transform.position, gameObjects[i].transform.position);
                        if (distance > bestDistance)
                        {
                            bestDistance = i;
                            chosenEnemy = gameObjects2[i];
                        }
                    }
                }
                if (V.levelAndWave>=V.levelGetMinesToShoot && chosenEnemy != null)
                {
                    V.mineCount = V.mineCount - 1;

                    Instantiate(minePrefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
                }
                else
                {
                    Instantiate(missilesPrefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
                }
            }
        }
        GameObject.Find("manageAudio").GetComponent<ManageEazySoundManager>().playLaserSound();
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
        V.zprint("damage", "Player.Damage()");
        NewCameraShake1();
        if (isShieldsActive == true)
        {
            V.zprint("damage", "Player.Damage() Shields Protected me!");
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
                V.zprint("damage", "Player.Damage() lies = 0 > Game Over");
                uiManager.GameOverSequence("Game Over!");

            }
            else
            {
                lives = lives - 1;
                uiManager.UpdateLives(lives);

                //put player below the screen, so he'll come back
                transform.position = new Vector3(transform.position.x, playerStartingYposBelowScreen, 0);
                if (V.levelAndWave < V.bossLevel)
                {
                    uiManager.GetReady();
                }
                //still have another life, so...
                RestoreHealth();
                SetAmmoToDefaultValue();
            }

        }
    }
    //--------------------------------------------------------------
    void CalculateMovement()
    {
        bool moving = false;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (verticalInput != 0 || horizontalInput != 0)
        {
            moving = true;
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
        playerMoving = moving;
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
            V.zprint("error", "Player: myCamera = Null");
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            //Vector3 origCameraPosition = myCamera.transform.position;
            //Debug.Log("About to Shake Camera:");

            float factor = 0.4f;
            float rdmX = Random.Range(-1f, 1f); rdmX = rdmX * factor;
            float rdmY = Random.Range(-1f, 1f); rdmY = 1 + (rdmY * factor);
            Vector3 newCamPosition = new Vector3(rdmX, rdmY, -10f);
            myCamera.transform.position = newCamPosition;
            yield return new WaitForSeconds(0.15f);

            rdmX = Random.Range(-1f, 1f); rdmX = rdmX * factor;
            rdmY = Random.Range(-1f, 1f); rdmY = 1 + (rdmY * factor);
            newCamPosition = new Vector3(rdmX, rdmY, -10f);
            myCamera.transform.position = newCamPosition;
            yield return new WaitForSeconds(0.15f);

            rdmX = Random.Range(-1f, 1f); rdmX = rdmX * factor;
            rdmY = Random.Range(-1f, 1f); rdmY = 1 + (rdmY * factor);
            newCamPosition = new Vector3(rdmX, rdmY, -10f);
            myCamera.transform.position = newCamPosition;
            yield return new WaitForSeconds(0.15f);

            //Debug.Log("Restore Camera");
            myCamera.transform.position = new Vector3(0, 1, -10);  // origCameraPosition;
            yield return new WaitForSeconds(0.15f);
        }
    }
}