using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//------------------------------
public class Asteroid : MonoBehaviour
{
  [SerializeField]
  private float _speed = 4.0f;
  [SerializeField]
  private GameObject _explosionPrefab;
  private SpawnManager _spawnManager;
//------------------------------
  void Start()
  {
    // _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
  }
//------------------------------
  void Update()
  {
    transform.Rotate(0,0,Time.deltaTime * _speed);
  }
//------------------------------
  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Laser")
    {
      Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
      Destroy(other.gameObject);
      Destroy(this.gameObject,0.25f);

      // We used this when we wanted destroying the asteroid
      // to start the game
      // _spawnManager.startSpawning();
    }
    else if (other.tag == "Player")
    {
      Player player = other.transform.GetComponent<Player>();
      if (player != null)
      {
        other.transform.GetComponent<Player>().Damage();
      }
      Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
      Destroy(this.gameObject,0.25f);
    }
  }
}
