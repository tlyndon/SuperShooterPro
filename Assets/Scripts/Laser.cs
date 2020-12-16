using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float playerLaserspeed = 8.0f;
    [SerializeField]
    public float enemyLaserSpeed = 6.0f;
    [SerializeField]
    public float enemy2LaserSpeed = 2.0f;
    [SerializeField]
    public float enemyLaserUpspeed = 2.5f;
    public bool isEnemyLaser = false;
    public bool isEnemyUpLaser = false;
    public bool isEnemy2Laser = false;  //mega laser
    private bool readyToDisappear = false;
    //--------------------------------------------------------------
    private void Start()
    {
        enemyLaserUpspeed = 2.5f;
    }
    void Update()
    {
        if (isEnemy2Laser == true && readyToDisappear == false)
        {
            Destroy(this.gameObject, 2f);
            readyToDisappear = true;
        }

        if ((isEnemyLaser == false && isEnemy2Laser == false) || isEnemyUpLaser == true)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }
    //--------------------------------------------------------------
    void MoveUp()
    {
        if (isEnemyUpLaser == true)
        {
            transform.Translate(Vector3.up * enemyLaserUpspeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * playerLaserspeed * Time.deltaTime);
        }

        if (transform.position.y > 8f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
    //--------------------------------------------------------------
    void MoveDown()
    {
        if (isEnemyLaser == true)
        {
            transform.Translate(Vector3.down * enemyLaserSpeed * Time.deltaTime);
        }
        else if (isEnemy2Laser == true)
        {
            transform.Translate(Vector3.down * enemy2LaserSpeed * Time.deltaTime);

            //needs to keep the x position of the mega laser with the actual enemy that spawned it
            //transform.position = new Vector3(transform.parent.gameObject.transform.position.x, transform.position.y, 0);
        }

        if (transform.position.y < -8f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
    //--------------------------------------------------------------
    public void AssignEnemyLaser()
    {
        isEnemyLaser = true;
    }
    //--------------------------------------------------------------
    public void AssignEnemyUpLaser()
    {
        isEnemyUpLaser = true;
    }
    //--------------------------------------------------------------
    public void AssignEnemy2Laser()
    {
        isEnemy2Laser = true;
    }
    //--------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        // only handles collissions of enemyLasers with Player
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();  //no need to check if null because other.tag=="Player"
            if (isEnemyLaser == true || isEnemy2Laser == true || (isEnemyUpLaser == true && player.transform.position.y > -2f))
            {
                {
                    player.Damage();

                    if (isEnemy2Laser == true)  //Mega Laser should kill Player
                    {
                        if (player.health == 1 || player.health == 2)
                        {
                            player.Damage();
                        }
                        if (player.health == 1)
                        {
                            player.Damage();
                        }
                    }
                    Destroy(this.gameObject);
                }
            }
        }
        else if (other.tag == "Boss")
        {
            V.zprint("bossCollide", "boss collision");
            if (isEnemyLaser == false && isEnemy2Laser == false && isEnemyUpLaser == false)
            {
                V.zprint("bossCollide", "boss Damage next");
                Boss boss = other.transform.GetComponent<Boss>();
                boss.Damage();
                Destroy(this.gameObject);
            }
        }
    }
}