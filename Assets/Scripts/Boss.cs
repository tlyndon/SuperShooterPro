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
    void Start()
    {
        transform.position = new Vector3(0, 9.2f, 0);
        bossMode = 0;   //0=move down, 1=move left, 2=move right
        energyBar = GameObject.FindGameObjectWithTag("bossEnergy").transform;
    }

    void Update()
    {
        if (V.mode == 20)
        {
            V.modeCounter = V.modeCounter + 1;

            calculateBossMovement();
            displayBossEnergyBar();
        }
    }
    void displayBossEnergyBar()
    {
        float scaleX = energyBar.localScale.x;

        if (scaleX >= 0)
        {
            energyBar.localScale -= new Vector3(0.25f, 0, 0);
        }
        else
        {
            energyBar.localScale += new Vector3(0.25f, 0, 0);
            scaleX = 0;
        }

        float xOffset = -((300 - scaleX) * 0.5f)*0.01f;
        Vector3 offset = new Vector3(xOffset, 2.77f, 0);
        energyBar.position = transform.position + offset;
        
    }
    void calculateBossMovement()
    {
        transform.Rotate(0, 0, 3);

            if (bossMode == 0)
            {
                float y = transform.position.y;
                //V.zprint("boss", "y:" + y + ", stopY:" + stopY);
                if (y > stopY)
                {
                    speed = (y - stopY) * 0.5f;
                    //V.zprint("boss", "speed:" + speed);
                    if (speed < .25f)
                    {
                        speed = .25f;
                    }
                    transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
                    //V.zprint("boss", "transform.position.y:" + transform.position.y);
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
                V.zprint("boss", "x:" + x + ", stopLeft:" + stopLeft);
                if (x > stopLeft)
                {
                    speed = (x - stopLeft) * 0.5f;
                    V.zprint("boss", "left speed:" + speed);
                    if (speed > maxSpeedX)
                    {
                        speed = maxSpeedX;
                    }
                    else if (speed < .25f)
                    {
                        speed = .25f;
                    }
                    transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
                    V.zprint("boss", "x:" + x);
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
                V.zprint("boss", "x:" + x + ", stopRight:" + stopRight);
                if (x < stopRight)
                {
                    speed = (stopRight - x) * 0.5f;
                    V.zprint("boss", "right speed:" + speed);
                    if (speed > maxSpeedX)
                    {
                        speed = maxSpeedX;
                    }
                    else if (speed < .25f)
                    {
                        speed = .25f;
                    }
                    transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
                    V.zprint("boss", "x:" + x);
                }
                else
                {
                    nextModeCounter = V.modeCounter + 90;  //a method of waiting
                    bossMode = 1;
                }
            }
    }
}
