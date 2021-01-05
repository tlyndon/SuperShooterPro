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
        if (V.mode == 21)
        {
            V.modeCounter = V.modeCounter + 1;  //mode 21
            if (V.levelAndWave < V.bossLevel)
            {
                NextWaveWhenEnemiesGone();
            }
        }
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

        V.zprint("trace", "NextWaveWhenEnemiesGone()");

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

            V.zprint("UIManager", "level:" + V.level + ", wave:" + V.wave + ", V.levelAndWave:" + V.levelAndWave);

            GetReady();
        }
    }
    //--------------------------------------------------------------
    public void StartWave()
    {
        V.zprint("trace", "StartWave()");
        V.setMode(10);
    }
    //--------------------------------------------------------------
    public void GetReady()
    {
        //after finishing a level
        V.flashingText = "Get Ready!";
        if (V.levelAndWave == V.levelEnemy1joins)
        { V.flashingText = "Get Ready!"; }
        else if (V.levelAndWave == V.levelEnemy2joins)
        { V.flashingText = "New Enemy types will be added!"; }
        else if (V.levelAndWave == V.levelEnemyLaserJoins)
        { V.flashingText = "Some Enemies will shoot at you!"; }
        else if (V.levelAndWave == V.levelEnemyAvoidsLasers)
        { V.flashingText = "Enemies can see your Lasers and move out of the way!"; }
        else if (V.levelAndWave == V.levelGetMissle)
        { V.flashingText = "Every 5 shots you will shoot a new random focused Missle!"; }
        else if (V.levelAndWave == V.levelGet3ShotLaser)
        { V.flashingText = "You can now pickup a new 3-Shot Laser powerup!"; }
        else if (V.levelAndWave == V.levelGetShields)
        { V.flashingText = "You can now pickup a Shield Powerup to protect your ship!"; }
        else if (V.levelAndWave == V.levelEnemyGetShields)
        { V.flashingText = "Some Enemies will have a Shield"; }
        else if (V.levelAndWave == V.levelGetMinesToShoot)
        { V.flashingText = "You can now pickup a Mine powerup to shoot homing mines at enemies!"; }
        else if (V.levelAndWave == V.levelSkullAndCrossBones)
        { V.flashingText = "Avoid skull & bone Powerup to avoid damage your ship"; }
        else if (V.levelAndWave == V.ShiftKeyForMoreThrusters)
        { V.flashingText = "Press Shift key for faster thrusters"; }
        else if (V.levelAndWave == V.PkeyToBringPowerupsToYou)
        { V.flashingText = "Holding down P key will bring Powerups to You"; }
        else if (V.levelAndWave == V.bossLevel)
        { V.flashingText = "Destroy the Boss and Win!"; }

        V.setMode(0);
    }
    //--------------------------------------------------------------
    public void GameOverSequence(string txt)
    {
        V.zprint("trace", "GameOverSequence()");
        restartText.gameObject.SetActive(true);
        V.flashingText = txt;  // "Game Over!";
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
                waveTextUI.gameObject.SetActive(true);
                if (V.levelAndWave < V.bossLevel)
                {
                    waveText.text = "WAVE " + (V.levelAndWave);
                    waveTextUI.text = "WAVE: " + (V.levelAndWave);
                }
                else
                {
                    waveText.text = "FINAL WAVE";
                    waveTextUI.text = "FINAL WAVE";
                }
            }
            else if (V.modeCounter == 90)
            {
                waveText.text = "";
                V.setMode(20);

                //don't need this is health and ammo powerups keep working
                //if (V.levelAndWave == V.bossLevel)
                //{
                //    Player player = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Player>();
                //    if (player == null)
                //    {
                //        V.zprint("error", "player = null in Powerup.cs");
                //    }
                //    else
                //    {
                //        player.RestoreHealth();
                //        player.SetAmmoToDefaultValue();
                //    }
                //}
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
                V.zprint("flashingText", "updateFlashingText = " + V.flashingText + ", V.mode:"+V.mode+", V.modeCounter:" + V.modeCounter);
                flashingText.gameObject.SetActive(true);
            }

            if (V.modeCounter == 0 || V.modeCounter == 60 || V.modeCounter == 120 || V.modeCounter == 180 || V.modeCounter == 240 || V.modeCounter == 300)
            {
                flashingText.text = V.flashingText;
            }
            else if (V.modeCounter == 45 || V.modeCounter == 105 || V.modeCounter == 165 || V.modeCounter == 225 || V.modeCounter == 285 || V.modeCounter == 345)
            {
                flashingText.text = "";
            }

            V.modeCounter = V.modeCounter + 1;  //mode 0 or 100
            V.zprint("flashingText", "V.mode:"+V.mode+", V.modeCounter:"+V.modeCounter);

            if (V.isGameOver == true && V.modeCounter == 120)
            {
                V.modeCounter = 0;
            }

            if (V.modeCounter == 346 && V.isGameOver == false)
            {
                V.zprint("flashingText", "FLASHING TEXT OVER as updateFlashingText = " + V.flashingText + ", text finished flashing & not game over, so next wave");
                //if (V.levelAndWave < V.bossLevel)
                //{
                    StartWave();
                //}
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