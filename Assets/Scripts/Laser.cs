using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private GameObject bossObject;
    private GameObject playerObject;
    [SerializeField]
    private float playerLaserspeed = 8.0f;
    [SerializeField]
    public float enemyLaserSpeed = 2.25f;
    [SerializeField]
    public float enemy2LaserSpeed = 2.0f;
    [SerializeField]
    public float enemyLaserUpspeed = 2.5f;
    [SerializeField]
    public float bossLaserSpeed = 0.1f;
    public bool isEnemyLaser = false;
    public bool isEnemyUpLaser = false;
    public bool isEnemy2Laser = false;  //mega laser
    public bool isBossLaser = false;
    public int modeCounterAtInstantiate;
    private bool readyToDisappear = false;
    private Vector3 bossLaserDirection;
    //--------------------------------------------------------------
    private void Start()
    {
        enemyLaserUpspeed = 2f;
        playerLaserspeed = 8f;
        enemyLaserSpeed = 6f;
        enemy2LaserSpeed = 2f;
        bossLaserSpeed = 0.5f;
    }
    //--------------------------------------------------------------
    void Update()
    {
        if (isEnemy2Laser == true && readyToDisappear == false)
        {
            Destroy(this.gameObject, 2f);
            readyToDisappear = true;
        }
        else if (isBossLaser == true)
        {
            if (bossObject != null)
            { MoveBossLaser(); }
        }
        else if ((isEnemyLaser == false && isEnemy2Laser == false) || isEnemyUpLaser == true)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }
    //--------------------------------------------------------------
    void MoveBossLaser()
    {
        if (V.modeCounter > modeCounterAtInstantiate + 180)
        {
            Destroy(this.gameObject);
        }
        else
        {
            transform.position += bossLaserDirection * bossLaserSpeed * Time.deltaTime;
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
    public void AssignBossLaser()
    {
        isBossLaser = true;
        
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        { V.zprint("bossLaser", "NO PLAYER OBJECT in Laser.cs"); }
        else
            bossObject = GameObject.FindGameObjectWithTag("Boss");
        if (bossObject == null)
        { V.zprint("bossLaser", "NO BOSS OBJECT in Laser.cs"); }
        else
        {   // since player and boss objects found, initialize boss laser shot
            modeCounterAtInstantiate = V.modeCounter;

            // calculate the direction from where the laser starts at the boss and move toward the player
            bossLaserDirection = playerObject.transform.position - transform.position;

            // calculate the proper angle to display the laser so that it points toward the player
            float angle = 180f - Vector3.Angle(playerObject.transform.position - transform.position, Vector3.down);
            if (playerObject.transform.position.x>transform.position.x) { angle = 180f - angle; }

            // zero out the rotation, but use angle to set the proper facing of the player
            Vector3 eulerRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, angle);
        }
    }
    //--------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        // only handles collissions of enemyLasers with Player
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();  //no need to check if null because other.tag=="Player"
            if (isEnemyLaser == true || isEnemy2Laser == true || isBossLaser == true || (isEnemyUpLaser == true && player.transform.position.y > -2f))
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
        else if (other.tag == "Boss" && isBossLaser == false)
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