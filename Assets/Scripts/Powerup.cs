using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
  [SerializeField]
  private float _speed = 3.0f;
  //ID for Powerups, 0=Triple Shot, 1=Speed, 2=Sheields, 3=Ammo
  [SerializeField]
  private int powerupID;
  [SerializeField]
  private AudioClip _clip;

  void Start()
  {

  }

  void Update()
  {
    transform.Translate(Vector3.down * _speed * Time.deltaTime);

    if (transform.position.y < -4.5f)
    {
      Destroy(this.gameObject);
    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Player")
    {
      //AudioSource.PlayClipAtPoint(_clip, transform.position);

      Debug.Log("Picked Up Powerup'" + other.tag);
      Destroy(this.gameObject);

      Player player = other.transform.GetComponent<Player>();
      if (player != null)
      {
        switch(powerupID)
        {
          case 0:
          Debug.Log("Got Triple Shot!");
          player.TripleShotActive();
          break;
          case 1:
          Debug.Log("Got Speed!");
          player.SpeedBoostActive();
          break;
          case 2:
          Debug.Log("Got Sheilds!");
          player.ShieldsActive();
          break;
          case 3:
          Debug.Log("Got Ammo!");
          player.SetAmmoToDefaultValue();
          break;
          case 4:
          Debug.Log("Got Health!");
          player.RestoreHealth();
          break;
        }
      }
    }
  }
}
