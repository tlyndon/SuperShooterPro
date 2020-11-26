using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator anim;

    [SerializeField]
    private float speed = 4.0f;
    [SerializeField]
    private float fireRate = 3.0f;
    [SerializeField]
    private GameObject laserPrefab;

    private float canFire = -1;
    public bool isAlive = true;
    //--------------------------------------------------------------
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null) { Debug.LogError("Animator Component is NULL"); }
    }
    //--------------------------------------------------------------
    void Update()
    {
        CalculateMovement();

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
    void CalculateMovement()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8f, 8f);
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