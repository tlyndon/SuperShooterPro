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

    private string enemyType;
    private string enemyDir;
    private float enemyXgoal;
    private float speedX;
    private float speedY;
    private float enemyMovementRangeX = 12.0f;
    private Vector3 lastEnemyXDirection;
    private Vector3 lastEnemyYDirection;

    private float canFire = -1;
    public bool isAlive = true;
    //--------------------------------------------------------------
    void Start()
    {
        int rnd=Random.Range(0, 2);
        if (rnd==0)
        {
            enemyType = "down";
            speedY = 4.0f;
        }
        else
        {
            enemyType = "leftandright";
            enemyDir = "left";
            enemyXgoal = transform.position.x - (enemyMovementRangeX*0.25f);
            speedY = 1f;
            speedX = 2f;
        }
        anim = GetComponent<Animator>();
        if (anim == null) { Debug.LogError("Animator Component is NULL"); }
    }
    //--------------------------------------------------------------
    void Update()
    {
        CalculateEnemyMovement();

        if (isAlive == true && Time.time > canFire)
        {
            fireRate = Random.Range(3f, 7f);
            canFire = Time.time + fireRate;
            GameObject enemyLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            { lasers[i].AssignEnemyLaser(); }
        }
    }
    //--------------------------------------------------------------
    void CalculateEnemyMovement()
    {
        if (isAlive == false)
        {
            transform.Translate(lastEnemyXDirection);
            transform.Translate(lastEnemyYDirection);
        }
        else if (enemyType == "down")
        {
            lastEnemyYDirection = Vector3.down * speedY * Time.deltaTime;
            transform.Translate(lastEnemyYDirection);
        }
        else  //leftandright
        {
            float deltaX = Mathf.Abs(transform.position.x - enemyXgoal);
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
            float randomX = Random.Range(-8f, 8f);
            if (enemyDir == "leftandright")
            {
                randomX = 7f;
            }
            transform.position = new Vector3(randomX, 7, 0);
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
                player.Damage();
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