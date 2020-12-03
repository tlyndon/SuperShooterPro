using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float playerLaserspeed = 8.0f;
    [SerializeField]
    public float enemyLaserSpeed = 6.0f;
    public float enemy2LaserSpeed = 2.0f;
    private bool isEnemyLaser = false;
    private bool isEnemy2Laser = false;
    private bool readyToDisappear = false;
    //--------------------------------------------------------------
    void Update()
    {
        if (isEnemy2Laser == true && readyToDisappear == false)
        {
            Debug.Log("---------------------------- ready to remove laser2 ---------------------");
            Destroy(this.gameObject, 5f);
            readyToDisappear = true;
        }

        if (isEnemyLaser == false && isEnemy2Laser == false)
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
    public void AssignEnemy2Laser()
    {
        isEnemy2Laser = true;
    }
    //--------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && (isEnemyLaser == true || isEnemy2Laser == true))
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();

                if (isEnemy2Laser == true)  //killer laser
                {
                    if (player.health == 1 || player.health == 2)
                    {
                        player.Damage();
                    }
                    if (player.health == 1 || player.health == 2)
                    {
                        player.Damage();
                    }
                    Destroy(this.gameObject);
                }
            }
        }
    }
}