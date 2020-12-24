using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text ammoText;
    [SerializeField]
    public Image imageLives;
    [SerializeField]
    public Sprite[] spriteLives;
    [SerializeField]
    public GameObject thrusters;
    public float thrustersMaxWidth;
    public float thrustersOriginalX;
    public float thrustersPct = 1.0f;
    public Text flashingText;
    public Text waveText;
    public Text waveTextUI;
    public Text restartText;
    //--------------------------------------------------------------
    void Start()
    {
        thrustersOriginalX = thrusters.transform.position.x;
        thrustersMaxWidth = thrusters.GetComponent<Renderer>().bounds.size.x;
        thrustersPct = 1.0f;
        updateThrustersMeter();

        UpdateScore(0);
        UpdateAmmo(0, 0);
        flashingText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);
    }
    //--------------------------------------------------------------
    void Update()
    {
        SlowlyRegenerateThrusters();
        updateFlashingText();
        updateWaveText();
        NextWaveWhenEnemiesGone();
    }
    //--------------------------------------------------------------
    public void newScaleX(GameObject theGameObject, float newSize)
    {
        float size = theGameObject.GetComponent<Renderer>().bounds.size.x;
        Vector3 rescale = theGameObject.transform.localScale;
        rescale.x = newSize * rescale.x / size;
        theGameObject.transform.localScale = rescale;
    }
    //--------------------------------------------------------------
    public void UpdateScore(int playerScore)
    {
        scoreText.text = "Score: " + playerScore;
    }
    //--------------------------------------------------------------
    public void UpdateAmmo(int playerAmmo, int ammoCountDefault)
    {
        ammoText.text = "Ammo: " + playerAmmo + "/" + ammoCountDefault;
    }
    //--------------------------------------------------------------
    public void UpdateLives(int showRemainingLives)
    {
        imageLives.sprite = spriteLives[showRemainingLives];
    }
    //--------------------------------------------------------------
    public void NextWaveWhenEnemiesGone()
    {
        if (V.mode == 21 && V.modeCounter > 0)
        {
            V.zprint("trace", "NextWaveWhenEnemiesGone()");

            V.modeCounter = V.modeCounter + 1;  //mode 21

            GameObject[] gameObjects;
            gameObjects = GameObject.FindGameObjectsWithTag("Enemy");
            int count = gameObjects.Length;
            gameObjects = GameObject.FindGameObjectsWithTag("Enemy2");
            count = count + gameObjects.Length;

            V.zprint("nextwave", "enemy count:" + count);
            if (count == 0)
            {
                V.zprint("trace", "NextWaveWhenEnemiesGone with enemyCount = 0");
                V.levelAndWave = V.levelAndWave + 1;

                V.wave = V.wave + 1;
                if (V.wave == 4)
                {
                    V.wave = 1;
                    V.level = V.level + 1;
                }

                V.zprint("UIManager", "level:"+V.level+", wave:" + V.wave + ", V.levelAndWave:" + V.levelAndWave);
                
                GetReady();
            }
        }
    }
    //--------------------------------------------------------------
    public void StartWave()
    {
        V.zprint("trace", "StartWave()");
        V.enemiesToSpawn = 0;
        V.setMode(10);
    }
    //--------------------------------------------------------------
    public void GetReady()
    {
        //after finishing a level
        V.flashingText = "Get Ready!";
        if (V.levelAndWave == V.levelEnemy1joins)
        { V.flashingText = "New Enemy Movement!"; }
        else if (V.levelAndWave == V.levelEnemy2joins)
        { V.flashingText = "New Enemy Type!"; }
        else if (V.levelAndWave == V.levelEnemyLaserJoins)
        { V.flashingText = "Enemies Shoot!"; }
        else if (V.levelAndWave == V.levelEnemyAvoidsLasers)
        { V.flashingText = "Enemies See Lasers!"; }
        else if (V.levelAndWave == V.levelGetMissle)
        { V.flashingText = "New Missle!"; }
        else if (V.levelAndWave == V.levelGet3ShotLaser)
        { V.flashingText = "New 3 Shot Laser!"; }
        else if (V.levelAndWave == V.levelGetShields)
        { V.flashingText = "New Shields"; }
        else if (V.levelAndWave == V.levelEnemyGetShields)
        { V.flashingText = "Enemies Get Shields"; }
        else if (V.levelAndWave == V.levelGetMinesToShoot)
        { V.flashingText = "Get Shootable Mines"; }
        else if (V.levelAndWave == V.levelSkullAndCrossBones)
        { V.flashingText = "Get Shootable Mines"; }


        V.setMode(0);
    }
    //--------------------------------------------------------------
    public void GameOverSequence()
    {
        V.zprint("trace", "GameOverSequence()");
        restartText.gameObject.SetActive(true);
        V.flashingText = "Game Over!";
        V.setMode(100);
        V.isGameOver = true;
    }
    //--------------------------------------------------------------
    public void updateWaveText()
    {
        if (V.mode == 10)
        {
            if (V.modeCounter == 0)
            {
                V.zprint("trace", "updateWaveText()");

                waveText.gameObject.SetActive(true);
                waveText.text = "WAVE " + (V.levelAndWave);

                waveTextUI.gameObject.SetActive(true);
                waveTextUI.text = "WAVE: " + (V.levelAndWave);
            }
            else if (V.modeCounter == 90)  //45
            {
                waveText.text = "";
                V.setMode(20);
            }
            V.modeCounter = V.modeCounter + 1;  //mode 10
        }
    }
    //--------------------------------------------------------------
    public void updateFlashingText()
    {
        if (V.mode == 0 || V.mode == 100)
        {
            if (V.modeCounter == 0)
            {
                V.zprint("flashingText", "updateFlashingText = " + V.flashingText);
                flashingText.gameObject.SetActive(true);
            }
            if (V.modeCounter == 0 || V.modeCounter == 60 || V.modeCounter == 120)
            {
                flashingText.text = V.flashingText;
            }
            else if (V.modeCounter == 30 || V.modeCounter == 90 || V.modeCounter == 150)
            {
                flashingText.text = "";
            }

            V.modeCounter = V.modeCounter + 1;  //mode 0 or 100
            //V.zprint("flashingText", "V.modeCounter:"+V.modeCounter);

            if (V.isGameOver == true && V.modeCounter == 120)
            {
                V.modeCounter = 0;
            }
            if (V.modeCounter == 151 && V.isGameOver == false)
            {
                V.zprint("flashingText", "updateFlashingText = " + V.flashingText + ", text finished flashing & not game over, so next wave");
                StartWave();
            }

        }
    }
    //--------------------------------------------------------------
    public void updateThrustersMeter()
    {
        newScaleX(thrusters, thrustersMaxWidth * thrustersPct);
        float xx = thrustersOriginalX + (thrustersMaxWidth * (1 - thrustersPct) * .5f);
        thrusters.transform.position = new Vector3(xx, thrusters.transform.position.y, 0);

        if (thrustersPct >= 1f)
        {
            //make meter green (would only work if sprite image was white)
            //SpriteRenderer thrusterRenderer = thrusters.GetComponent<SpriteRenderer>();
            //if (thrusterRenderer == null) { Debug.Log("The thruster Sprite Renderer component in UIManager.cs = null"); }
            //thrusterRenderer.color = new Color(1f, 1f, 1f, 1);
        }
    }
    //--------------------------------------------------------------
    public bool UseThrusters()  //a normal routine that returns bool
    {
        bool returnValue = true;
        if (thrustersPct > 0)
        {
            float originalValue = thrustersPct;
            thrustersPct = thrustersPct - 0.001f;

            if (originalValue < 0.5f && thrustersPct >= 0.5f)
            {
                //make meter red (would only work if sprite image was white
                //SpriteRenderer thrusterRenderer = thrusters.GetComponent<SpriteRenderer>();
                //if (thrusterRenderer == null) { Debug.Log("The thruster Sprite Renderer component in UIManager.cs = null"); }
                //thrusterRenderer.color = new Color(1f, 0f, 0f, 1);
            }
            updateThrustersMeter();
        }

        if (thrustersPct <= 0)
        {
            thrustersPct = 0f;
            returnValue = false;
        }
        return (returnValue);
    }
    //--------------------------------------------------------------
    void SlowlyRegenerateThrusters()  //a normal routine that returns bool
    {
        if (thrustersPct < 1)
        {
            float originalValue = thrustersPct;
            thrustersPct = thrustersPct + 0.0004f;

            if (originalValue < 0.5f && thrustersPct >= 0.5f)
            {
                //make meter green (would only work if sprite image was white)
                //SpriteRenderer thrusterRenderer = thrusters.GetComponent<SpriteRenderer>();
                //if (thrusterRenderer == null) { Debug.Log("The thruster Sprite Renderer component in UIManager.cs = null"); }
                //thrusterRenderer.color = new Color(1f, 1f, 1f, 1);
            }
            updateThrustersMeter();
        }
        if (thrustersPct > 1)
        {
            thrustersPct = 1;
        }
    }
}