using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private float speed = 2f;
    private int bossMode = 0;
    private int nextModeCounter = 0;
    private float maxSpeedX = 5f;
    [SerializeField]
    private float stopY = 2.4f;
    [SerializeField]
    private float stopLeft = -7.36f;
    [SerializeField]
    private float stopRight = 7.36f;
    [SerializeField]
    private Transform energyBar;
    private float scaleX = 300f;
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private SpriteRenderer energyBarSpriteRenderer;
    //--------------------------------------------------------------
    void Start()
    {
        transform.position = new Vector3(0, 9.2f, 0);
        bossMode = 0;   //0=move down, 1=move left, 2=move right
        energyBar = GameObject.FindGameObjectWithTag("bossEnergy").transform;
    }
    //--------------------------------------------------------------
    void Update()
    {
        if (V.mode == 20)
        {
            V.modeCounter = V.modeCounter + 1;

            calculateBossMovement();
            displayAndPositionBossEnergyBar();
        }
    }
    //--------------------------------------------------------------
    void displayAndPositionBossEnergyBar()
    {
        if (scaleX >= 0 && scaleX <=300)
        {
            V.zprint("bossEnergy", "scaleX:" + scaleX);
            V.zprint("bossEnergy", "energyBar.localScale.x:" + energyBar.localScale.x);
            float delta = energyBar.localScale.x - scaleX;
            V.zprint("bossEnergy", "delta:" + delta);
            energyBar.localScale -= new Vector3(delta, 0, 0);
        }
        else if (scaleX<0)
        {
            V.zprint("bossEnergy", "scaleX:" + scaleX);
            energyBar.localScale += new Vector3(-scaleX, 0, 0);
            V.zprint("bossEnergy", "energyBar.localScale.x:" + energyBar.localScale.x);
            V.zprint("bossEnergy", "Zero scaleX");
            scaleX = 0;            
        }

        float xOffset = -((300 - scaleX) * 0.5f) * 0.01f;
        //float xOffset = -(300 - scaleX) * 0.05f;
        Vector3 offset = new Vector3(xOffset, 2.77f, 0);
        energyBar.position = transform.position + offset;

    }
    //--------------------------------------------------------------
    void calculateBossMovement()
    {
        transform.Rotate(0, 0, 3);

        if (bossMode == 0)
        {
            float y = transform.position.y;
            V.zprint("bossMove", "y:" + y + ", stopY:" + stopY);
            if (y > stopY)
            {
                speed = (y - stopY) * 0.5f;
                V.zprint("bossMove", "speed:" + speed);
                if (speed < .25f)
                {
                    speed = .25f;
                }
                transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
                V.zprint("bossMove", "transform.position.y:" + transform.position.y);
            }
            else
            {
                nextModeCounter = V.modeCounter + 60;  //a method of waiting
                bossMode = 1;
            }
        }
        else if (bossMode == 1 && V.modeCounter > nextModeCounter)
        {
            float x = transform.position.x;
            V.zprint("bossMove", "x:" + x + ", stopLeft:" + stopLeft);
            if (x > stopLeft)
            {
                speed = (x - stopLeft) * 0.5f;
                V.zprint("bossMove", "left speed:" + speed);
                if (speed > maxSpeedX)
                {
                    speed = maxSpeedX;
                }
                else if (speed < .25f)
                {
                    speed = .25f;
                }
                transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
                V.zprint("bossMove", "x:" + x);
            }
            else
            {
                nextModeCounter = V.modeCounter + 90;  //a method of waiting
                bossMode = 2;
            }
        }
        else if (bossMode == 2 && V.modeCounter > nextModeCounter)
        {
            float x = transform.position.x;
            V.zprint("bossMove", "x:" + x + ", stopRight:" + stopRight);
            if (x < stopRight)
            {
                speed = (stopRight - x) * 0.5f;
                V.zprint("bossMove", "right speed:" + speed);
                if (speed > maxSpeedX)
                {
                    speed = maxSpeedX;
                }
                else if (speed < .25f)
                {
                    speed = .25f;
                }
                transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
                V.zprint("bossMove", "x:" + x);
            }
            else
            {
                nextModeCounter = V.modeCounter + 90;  //a method of waiting
                bossMode = 1;
            }
        }
    }
    //--------------------------------------------------------------
    public void Damage()
    {
        scaleX = scaleX - 10f;
        V.zprint("bossDamage", "subtracted 10 from scaleX, which is now:"+ scaleX);
        // set color of boss energyBar
        if (scaleX > 200)
        {
            energyBarSpriteRenderer.color = Color.green;
        }
        else if (scaleX > 100)
        {
            energyBarSpriteRenderer.color = Color.yellow;
        }
        else
        {
            energyBarSpriteRenderer.color = Color.red;
        }

        // do damage
        if (scaleX < 0)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            for (int i = 1; i < 5; i++)
            {
                Vector3 offset = new Vector3(Random.Range(0, 4f) - 2f, Random.Range(0, 4f) - 2f, 0);
                Vector3 newPosition = transform.position + offset;
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(this.gameObject, 0.2f);
            Destroy(energyBar.gameObject, 0.2f);
        }
        else
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
    }
}
