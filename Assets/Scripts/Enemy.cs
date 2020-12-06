using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator anim;

    [SerializeField]
    private float fireRate = 3.0f;
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private GameObject laserPrefab2;
    [SerializeField]
    private GameObject explosionPrefab;
    public GameObject newShield;
    [SerializeField]
    private GameObject shieldPrefab;

    public int enemyType = 999;
    private int fireWhenCounterEquals;
    private string enemyDir;
    private float enemyXgoal;
    private float speedX = 4f;
    private float speedY = 4f;
    private float deltaX;
    private float enemyMovementRangeX = 12.0f;
    private Vector3 lastEnemyXDirection;
    private Vector3 lastEnemyYDirection;

    private float canFire = -1;
    public bool isAlive = true;
    public bool hasShield = false;
    //--------------------------------------------------------------
    void Start()
    {

    }
    //--------------------------------------------------------------
    void Update()
    {
        // ready to initialize enemy?
        if (enemyType < 999)
        {
            if (enemyDir == null)
            {
                if (enemyType == 0 || enemyType == 2)
                {
                    enemyDir = "down";
                    speedY = 4.0f;
                    if (enemyType == 2)
                    {
                        speedY = 2.0f;
                    }
                }
                else if (enemyType == 1)
                {
                    enemyDir = "left";
                    enemyXgoal = transform.position.x - (enemyMovementRangeX * 0.25f);
                    speedY = 1f;
                    speedX = 2f;
                }
                if (enemyType != 2)
                {
                    anim = GetComponent<Animator>();
                    if (anim == null)
                    { V.zprint("trace", "enemy.cs Update() anim=null"); }
                }

                if (hasShield == true)
                {
                    newShield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
                }

                fireWhenCounterEquals = Random.Range(300, 450);
            }
            else
            {
                CalculateEnemyMovement();
                SpawnEnemyFire();
                MoveShieldWithEnemy();
            }
        }
    }
    //--------------------------------------------------------------
    void MoveShieldWithEnemy()
    {
        if (hasShield == true)
        {
            newShield.transform.position = this.transform.position;
        }
    }
    //--------------------------------------------------------------
    void SpawnEnemyFire()
    {
        //generate enemy fire
        if (isAlive == true && enemyType != 2 && Time.time > canFire && V.modeCounter > fireWhenCounterEquals)
        {
            fireWhenCounterEquals = V.modeCounter + Random.Range(300, 660);

            fireRate = Random.Range(3f, 7f);
            canFire = Time.time + fireRate;
            GameObject enemyLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            { lasers[i].AssignEnemyLaser(); }
        }
        else if (isAlive == true && enemyType == 2 && V.modeCounter > fireWhenCounterEquals)
        {
            fireWhenCounterEquals = V.modeCounter + 180;
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y - 5, 0);
            GameObject enemyLaser = Instantiate(laserPrefab2, newPos, Quaternion.identity);
            Laser[] lasers2 = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers2.Length; i++)
            { lasers2[i].AssignEnemy2Laser(); }
        }
    }
    //--------------------------------------------------------------
    void CalculateEnemyMovement()
    {
        if (isAlive == false)
        {
            lastEnemyYDirection = Vector3.down * speedY * 0.5f * Time.deltaTime;
            transform.Translate(lastEnemyYDirection);

            if (enemyType == 1)
            {
                if (enemyDir == "left")
                {
                    lastEnemyXDirection = Vector3.left * speedX * 0.25f * deltaX * Time.deltaTime;
                }
                else if (enemyDir == "right")
                {
                    lastEnemyXDirection = Vector3.right * speedX * 0.25f * deltaX * Time.deltaTime;
                }
            }
            transform.Translate(lastEnemyXDirection);
        }
        else if (enemyType == 0 || enemyType == 2)  //down
        {
            lastEnemyYDirection = Vector3.down * speedY * Time.deltaTime;  //regular down
            transform.Translate(lastEnemyYDirection);

            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null && enemyType == 2)
            {
                Player player = obj.transform.GetComponent<Player>();

                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < 7)
                {

                    lastEnemyXDirection = Vector3.left * 1.15f * Time.deltaTime;
                    deltaX = transform.position.x - player.transform.position.x;
                    if (deltaX < 0)
                    {
                        lastEnemyXDirection = Vector3.right * 1.15f * Time.deltaTime;
                    }
                    transform.Translate(lastEnemyXDirection);
                }
            }
        }
        else if (enemyType == 1)  //leftandright
        {
            deltaX = Mathf.Abs(transform.position.x - enemyXgoal);
            if (deltaX < 0.5f)
                if (enemyDir == "left")
                {
                    enemyDir = "right";
                    enemyXgoal = transform.position.x + enemyMovementRangeX;
                }
                else
                {
                    enemyDir = "left";
                    enemyXgoal = transform.position.x - enemyMovementRangeX;
                }

            if (enemyDir == "left")
            {
                lastEnemyXDirection = Vector3.left * speedX * deltaX * Time.deltaTime;
            }
            else
            {
                lastEnemyXDirection = Vector3.right * speedX * deltaX * Time.deltaTime;

            }
            transform.Translate(lastEnemyXDirection);

            lastEnemyYDirection = Vector3.down * speedY * Time.deltaTime;
            transform.Translate(lastEnemyYDirection);
        }

        // check if reached bottom of screen and bring back to the top
        if (transform.position.y < -7f)
        {
            //float newX = transform.position.x;
            //transform.position = new Vector3(newX, 7, 0);
            if (hasShield == true)
            {
                hasShield = false;
                Destroy(newShield.gameObject, 0f);
            }
            Destroy(this.gameObject);
        }
    }
    //--------------------------------------------------------------
    private void ThisEnemyDiesFromCollisionWith(string collider)
    {
        isAlive = false;
        if (enemyType != 2)
        {
            anim.SetTrigger("OnEnemyDeath");
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);  //destroy projectile
        }
        else
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(GetComponent<Collider2D>());  //remove enemy collider
            Destroy(this.gameObject, 0.5f);  //destroy projectile
        }
        if (hasShield == true)
        {
            hasShield = false;
            Destroy(newShield.gameObject, 0f);
        }
        V.zprint("trace", "ThisEnemyDiesFromCollisionWith()");
        V.zprint("enemy", "Enemy Collided with " + collider + "!");
    }
    //--------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null && isAlive == true)
        {
            Player player = obj.transform.GetComponent<Player>();
            if (other.tag == "Player")
            {
                ThisEnemyDiesFromCollisionWith("Player");
                if (V.mode == 21)
                {
                    player.Damage();
                }
            }
            else if (other.tag == "Laser" || other.tag == "Missile")
            {
                if (hasShield == true)
                {
                    hasShield = false;
                    Destroy(newShield.gameObject, 0f);
                    Destroy(other.gameObject, 0f);
                }
                else
                {
                    ThisEnemyDiesFromCollisionWith(other.tag);
                    Destroy(other.gameObject);
                    player.AddToScore(10);
                }
            }
        }
    }
}