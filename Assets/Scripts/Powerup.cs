using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private int powerupID;    //ID for Powerups, 0=Triple Shot, 1=Speed, 2=Sheields, 3=Ammo, 4=health

    [SerializeField]
    private AudioClip powerUpClip;

    [SerializeField]
    private GameObject explosionPrefab;

    private SpriteRenderer powerUpRenderer;

    private float speed = 3.0f;
    private bool isAlive = true;
    //--------------------------------------------------------------
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        { V.zprint("error", "The audioSource component in Powerup.cs = null"); }

        powerUpRenderer = this.GetComponent<SpriteRenderer>();
        if (powerUpRenderer == null)
        { V.zprint("error", "The powerUpRenderer component in Powerup.cs = null"); }
    }
    //--------------------------------------------------------------
    void Update()
    {
        if (isAlive)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            if (transform.position.y < -4.5f)
            { Destroy(this.gameObject, 2f); }
        }
    }
    //--------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAlive)
        {
            if (other.tag == "Laser" && powerupID == 5)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                Destroy(this.gameObject, 0.2f);
                powerUpRenderer.color = Color.clear;
                Destroy(other.gameObject, 0.1f);
            }
            else if (other.tag == "Player")
            {
                V.zprint("powerup", "Picked Up Powerup " + other.tag);
                //only use if want to be able to blow-up the Butterfly Bomb
                //if (powerupID == 5)
                //{
                //    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                //}
                
                Destroy(this.gameObject, 2f);
                powerUpRenderer.color = Color.clear;

                V.zprint("powerup", "powerupID:" + powerupID);

                Player player = other.transform.GetComponent<Player>();
                if (player == null)
                {
                    V.zprint("error","player = null in Powerup.cs");
                }

                else           
                {   
                    if (V.soundOn == true)
                    { audioSource.PlayOneShot(powerUpClip, 0.7F); }

                    isAlive = false;

                    switch (powerupID)
                    {
                        case 0:
                            V.zprint("powerup", "Got Triple Shot PowerUp!");
                            player.TripleShotActive();
                            break;
                        case 1:
                            V.zprint("powerup", "Got Speed PowerUp!");
                            player.SpeedBoostActive();
                            break;
                        case 2:
                            V.zprint("powerup", "Got Sheilds PowerUp!");
                            player.ShieldsActive();
                            break;
                        case 3:
                            V.zprint("powerup", "Got Ammo PowerUp!");
                            player.SetAmmoToDefaultValue();
                            break;
                        case 4:
                            V.zprint("powerup", "Got Health PowerUp!");
                            player.RestoreHealth();
                            break;
                        case 5:
                            V.zprint("powerup", "Got Butterflybones PowerUp!");
                            player.Damage();
                            break;
                    }
                }
            }
        }
    }
}