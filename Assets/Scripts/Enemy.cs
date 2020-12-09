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
    private bool ramingPlayerNow = false;
    private bool avoidingPlayerLaserNow = false;
    private bool shootingLaserNow = false;
    private Vector3 avoidToVector3;
    private float avoidToX = 0f;
    private float avoidToY = 0f;
    //--------------------------------------------------------------
    void Start()
    {
        isAlive = true;
    }
    //--------------------------------------------------------------
    void Update()
    {
        // ready to initialize enemy?
        if (enemyType < 999)
        {
            //initialize this enemy based upon type
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
                SpawnEnemyFire();
                MoveShieldWithEnemy();
                CalculateEnemyMovement();
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
        shootingLaserNow = false;
        if (isAlive == true && enemyType != 2 && Time.time > canFire && V.modeCounter > fireWhenCounterEquals)
        {
            //regular enemy laser fire down
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
            //mega blaster
            fireWhenCounterEquals = V.modeCounter + 180;
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y - 5, 0);
            GameObject enemyLaser = Instantiate(laserPrefab2, newPos, Quaternion.identity);
            Laser[] lasers2 = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers2.Length; i++)
            { lasers2[i].AssignEnemy2Laser(); }
        }
        else if (isAlive == true & V.modeCounter > fireWhenCounterEquals)
        {
            //find if powerup is below us, and if so, fire laser at it.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down * 10);
            if (hit.collider != null)
            {
                //Draw Line between this object and the object collided with, of the color red, and display that line for 0.01 seconds
                Debug.DrawLine(transform.position, hit.collider.transform.position, Color.red, 0.01f);
                V.zprint("raycast", "Object Detected " + hit.collider.name); //name of object holding the collider we hit
                V.zprint("raycast", "Distance from Origin to Object = " + hit.distance);  //distance between the RayCast and the Collider of the object we hit

                if (hit.collider.tag == "powerup")
                {
                    shootingLaserNow = true;
                    fireWhenCounterEquals = V.modeCounter + Random.Range(300, 660);
                    fireRate = Random.Range(3f, 7f);
                    canFire = Time.time + fireRate;

                    GameObject enemyLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
                    Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                    for (int i = 0; i < lasers.Length; i++)
                    { lasers[i].AssignEnemyLaser(); }
                }
            }
        }
    }
    //--------------------------------------------------------------
    void CalculateEnemyMovement()
    {
        ramingPlayerNow = false;
        avoidingPlayerLaserNow = false;
        if (isAlive == false)
        {
            // This code keeps the direction of the enemy moving when they die,
            // because the explosion is connected to their position
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

                // if the enemy is getting close to the player, then have it try and ram the player
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < 7)
                {
                    ramingPlayerNow = true;
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
        if (transform.position.y < -4f)
        {
            //just before an enemy dies, it shoots backwards
            GameObject enemyLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            { lasers[i].AssignEnemyUpLaser(); }

            //if the enemy has a shield, we must destroy it, too
            if (hasShield == true)
            {
                hasShield = false;
                Destroy(newShield.gameObject, 0f);
            }

            Destroy(this.gameObject);
        }
        else if (enemyType == 0 && isAlive == true)  // && ramingPlayerNow == false && shootingLaserNow == false)
        {
            //if (avoidingPlayerLaserNow == false)
            //{
            //    RaycastHit2D boxResult;
            //    boxResult = Physics2D.BoxCast(transform.position, new Vector2(2, 10), 0f, new Vector2(0, -10), 0);
            //    if (boxResult.collider != null)
            //    {
            //        //Draw Line between this object and the object collided with, of the color red, and display that line for 0.01 seconds
            //        Debug.DrawLine(transform.position, boxResult.collider.transform.position, Color.red, 0.01f);
            //        V.zprint("avoid", "Object Detected " + boxResult.collider.name); //name of object holding the collider we hit
            //        V.zprint("avoid", "Distance from Origin to Object = " + boxResult.distance);  //distance between the RayCast and the Collider of the object we hit

            //        if (boxResult.collider.tag == "Laser")
            //        {
            //            avoidingPlayerLaserNow = true;
            //            avoidToY = transform.position.y - 3;
            //            avoidToX = transform.position.x + 3;
            //            if (boxResult.collider.transform.position.x > transform.position.x)
            //            {
            //                avoidToX = transform.position.x - 3;
            //            }
            //            avoidToVector3 = new Vector3(avoidToX, avoidToY, 0);
            //            V.zprint("avoid", "Initilizing Avoid logic");
            //        }
            //    }
            //}

            if (avoidingPlayerLaserNow == true)
            {

                if (transform.position.x == avoidToX && transform.position.y == avoidToY)
                {
                    V.zprint("avoid", "Avoid complete");
                    avoidingPlayerLaserNow = false;
                }
                else
                {
                    V.zprint("avoid", "Moving to Avoid Now");
                    Vector3 direction = avoidToVector3;
                    int sp = 5;
                    transform.Translate(direction * sp * Time.deltaTime);
                }
            }
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
    private void damageOrDestroyThisEnemy(Collider2D other)
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
        }
    }
    //--------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null && isAlive == true)  //both player and enemy are alive
        {
            Player player = obj.transform.GetComponent<Player>();  //put here once, needed for "Player" or "Missile"
            if (other.tag == "Laser")
            {
                //we need to make sure this is a player laser
                Laser laser = other.GetComponent<Laser>();
                if (laser.isEnemyLaser == false && laser.isEnemyUpLaser == false && laser.isEnemy2Laser == false)
                {
                    damageOrDestroyThisEnemy(other);
                }
            }

            else if (other.tag == "Player")
            {
                ThisEnemyDiesFromCollisionWith("Player");
                if (V.mode == 21)
                {
                    player.Damage();
                }
            }
            else if (other.tag == "Missile")
            {
                damageOrDestroyThisEnemy(other);
                player.AddToScore(10);
            }
        }
    }
}