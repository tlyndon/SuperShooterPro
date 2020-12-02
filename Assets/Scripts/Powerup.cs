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

    private SpriteRenderer powerUpRenderer;

    private float speed = 3.0f;
    private bool isAlive = true;
    //--------------------------------------------------------------
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        { Debug.LogError("The audioSource component in Powerup.cs = null"); }

        powerUpRenderer = this.GetComponent<SpriteRenderer>();
        if (powerUpRenderer == null)
        { Debug.Log("The powerUpRenderer component in Powerup.cs = null"); }
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
            if (other.tag == "Player")
            {
                Debug.Log("Picked Up Powerup " + other.tag);
                Destroy(this.gameObject, 2f);
                powerUpRenderer.color = Color.clear;

                Debug.Log("powerupID:" + powerupID);
                Player player = other.transform.GetComponent<Player>();
                if (player == null)
                { Debug.LogError("player = null in Powerup.cs"); }

                else           
                {   
                    if (V.soundOn == true)
                    { audioSource.PlayOneShot(powerUpClip, 0.7F); }

                    isAlive = false;

                    switch (powerupID)
                    {
                        case 0:
                            Debug.Log("Got Triple Shot PowerUp!");
                            player.TripleShotActive();
                            break;
                        case 1:
                            Debug.Log("Got Speed PowerUp!");
                            player.SpeedBoostActive();
                            break;
                        case 2:
                            Debug.Log("Got Sheilds PowerUp!");
                            player.ShieldsActive();
                            break;
                        case 3:
                            Debug.Log("Got Ammo PowerUp!");
                            player.SetAmmoToDefaultValue();
                            break;
                        case 4:
                            Debug.Log("Got Health PowerUp!");
                            player.RestoreHealth();
                            break;
                    }
                }
            }
        }
    }
}