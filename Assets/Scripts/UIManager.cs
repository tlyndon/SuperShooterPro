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
    public Text restartText;
    //--------------------------------------------------------------
    void Start()
    {
        thrustersOriginalX = thrusters.transform.position.x;
        thrustersMaxWidth = thrusters.GetComponent<Renderer>().bounds.size.x;
        thrustersPct = 1.0f;
        updateThrustersMeter();

        UpdateScore(0);
        UpdateAmmo(0,0);
        flashingText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);
    }
    //--------------------------------------------------------------
    void Update()
    {
        SlowlyRegenerateThrusters();
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
    public void GetReady()
    {
        StartCoroutine(showFlashingTextRoutine("GET READY!", 2, 2));
    }
    //--------------------------------------------------------------
    public void GameOverSequence()
    {
        restartText.gameObject.SetActive(true);
        StartCoroutine(showFlashingTextRoutine("GAME OVER", -1, 0));
        GameManager.isGameOver = true;
    }
    //--------------------------------------------------------------
    IEnumerator showFlashingTextRoutine(string txt, int howMany, int delayFirst)
    {
        flashingText.gameObject.SetActive(true);
        flashingText.text = "";
        yield return new WaitForSeconds(delayFirst);

        while (howMany != 0)
        {
            flashingText.text = txt;
            yield return new WaitForSeconds(0.5f);
            flashingText.text = "";
            yield return new WaitForSeconds(0.5f);
            if (howMany > 0)
            {
                howMany = howMany - 1;
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