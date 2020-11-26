using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float playerLaserspeed = 8.0f;
    [SerializeField]
    private float enemyLaserSpeed = 6.0f;
    private bool isEnemyLaser = false;
    //--------------------------------------------------------------
    void Update()
    {
        if (isEnemyLaser == false)
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
        transform.Translate(Vector3.up * playerLaserspeed * Time.deltaTime);

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
        transform.Translate(Vector3.down * enemyLaserSpeed * Time.deltaTime);

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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && isEnemyLaser == true)
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
        }
    }
}