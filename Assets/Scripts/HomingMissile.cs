using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// help with this provided by https://www.youtube.com/watch?v=jWUz5J-vfGA

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab;

    public float speed = 3;
    public float rotatingSpeed = 200;
    private GameObject target;
    Rigidbody2D rigidBody;
    private bool isAlive = true;
    //--------------------------------------------------------------
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Enemy");
        if (target != null)
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy.isAlive == false)
            { target = null; }
        }
        //else
        //{
        //    Vector3 direction = new Vector3(7, 10, 0);
        //    transform.Translate(direction * Time.deltaTime);
        //}
    }
    //--------------------------------------------------------------
    void FixedUpdate()
    {
        if (isAlive)
        {
            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Enemy");
                if (target != null)
                {
                    Enemy enemy = target.GetComponent<Enemy>();
                    if (enemy.isAlive == false) { target = null; }
                }
                else
                {
                    Vector3 direction = new Vector3(7, 10, 0);
                    transform.Translate(direction * Time.deltaTime);
                }
            }
            else
            {
                // move missile toward target
                Vector2 point2Target = (Vector2)transform.position - (Vector2)target.transform.position;
                point2Target.Normalize();
                float value = Vector3.Cross(point2Target, transform.up).z;
                rigidBody.angularVelocity = rotatingSpeed * value;
                rigidBody.velocity = transform.up * speed;
            }
        }
    }
    //--------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAlive)
        {
            if (other.tag == "Enemy")
            {
                Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
                Enemy enemy = other.GetComponent<Enemy>();
                enemy.isAlive = false;
                Destroy(other.gameObject, 0.02f);                                       //modify so that we can add the direction of missile to the explosion
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

                GameObject obj = GameObject.FindGameObjectWithTag("Player");
                if (obj != null)
                {
                    Player player = obj.transform.GetComponent<Player>();
                    player.AddToScore(25);
                }
                Destroy(this.gameObject, 0.02f);
            }
        }
    }
}