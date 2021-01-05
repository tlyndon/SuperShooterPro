using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float speed = 4.0f;

    [SerializeField]
    private GameObject explosionPrefab;

    private GameObject onFireObj;
    private bool isAlive = true;
    //--------------------------------------------------------------
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (isAlive)
        {
            transform.Rotate(0, 0, Time.deltaTime * speed);
        }
        else
        {
            onFireObj.transform.position = new Vector3(onFireObj.transform.position.x, transform.position.y, 0);
        }
    }
    //--------------------------------------------------------------
    private void changeAsteroidToInvisibleObjectOnFire()
    {
        isAlive = false;
        onFireObj = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        Destroy(this.gameObject, 2f);
    }
    //--------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAlive)
        {
            if (other.tag == "Laser" || other.tag == "Missile")
            {
                changeAsteroidToInvisibleObjectOnFire();

                GameObject obj = GameObject.FindGameObjectWithTag("Player");
                Player player = obj.transform.GetComponent<Player>();
                if (player == null)
                { V.zprint("powerup", "player is null"); }
                else
                { player.AddToScore(100); }

                Destroy(other.gameObject);
            }
            else if (other.tag == "Player")
            {
                changeAsteroidToInvisibleObjectOnFire();
                Player player = other.transform.GetComponent<Player>();
                if (player != null)
                {
                    player.Damage();
                }
            }
        }
    }
}