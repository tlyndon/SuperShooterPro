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

    public int enemyType;
    private int fireWhenCounterEquals;
    private string enemyDir;
    private float enemyXgoal;
    private float speedX;
    private float speedY;
    private float deltaX;
    private float enemyMovementRangeX = 12.0f;
    private Vector3 lastEnemyXDirection;
    private Vector3 lastEnemyYDirection;
    
    private float canFire = -1;
    public bool isAlive = true;
    //--------------------------------------------------------------
    void Start()
    {
        
    }
    //--------------------------------------------------------------
    void Update()
    {
        // ready to initialize enemy?
        if (enemyType != null && enemyDir == null)
        {
            if (enemyType == 0)
            {
                enemyDir = "down";
                speedY = 4.0f;
            }
            else if (enemyType == 1)
            {
                enemyDir = "left";
                enemyXgoal = transform.position.x - (enemyMovementRangeX * 0.25f);
                speedY = 1f;
                speedX = 2f;
            }
            anim = GetComponent<Animator>();
            if (anim == null) { Debug.LogError("Animator Component is NULL"); }

            fireWhenCounterEquals = Random.Range(300,660);
        }
        else
        {
            CalculateEnemyMovement();

            if (isAlive == true && Time.time > canFire && V.modeCounter > fireWhenCounterEquals)
            {
                fireWhenCounterEquals = fireWhenCounterEquals + Random.Range(300, 660);

                fireRate = Random.Range(3f, 7f);
                canFire = Time.time + fireRate;
                GameObject enemyLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                { lasers[i].AssignEnemyLaser(); }
            }
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
        else if (enemyType == 0)  //down
        {
            lastEnemyYDirection = Vector3.down * speedY * Time.deltaTime;
            transform.Translate(lastEnemyYDirection);
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
        if (transform.position.y < -5f)
        {
            float newX = transform.position.x;
            //float newX = Random.Range(-8f, 8f);
            //if (enemyType == 1)
            //{
            //    newX = 7f;
            //}
            transform.position = new Vector3(newX, 7, 0);
        }
    }
    //--------------------------------------------------------------
    private void ThisEnemyDiesFromCollisionWith(string collider)
    {
        isAlive = false;
        anim.SetTrigger("OnEnemyDeath");
        Destroy(this.gameObject, 2.8f);
        Debug.Log("Player Collided with " + collider + "!");
    }
    //--------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            Player player = obj.transform.GetComponent<Player>();
            if (other.tag == "Player" && isAlive == true)
            {
                ThisEnemyDiesFromCollisionWith("Player");
                if (V.mode == 21)
                {
                    player.Damage();
                }
            }
            else if (other.tag == "Laser")
            {
                ThisEnemyDiesFromCollisionWith("Laser");
                Destroy(other.gameObject);
                Destroy(GetComponent<Collider2D>());
                player.AddToScore(10);
            }
        }
    }
}