using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  [SerializeField]
  private float _speed = 4.0f;
  [SerializeField]
  private GameObject _laserPrefab;
  private float _laserSpeed = 4.0f;
  private bool _stopSpawningEnemyFire = false;

  private Player _player;
  private Animator _anim;
  private AudioSource _audioSource;
  [SerializeField]
  private float _fireRate = 3.0f;
  private float _canFire = -1;


  void Start()
  {
    _player = GameObject.Find("Player").GetComponent<Player>();
    if (_player == null)
    {
      Debug.LogError("Player Component is NULL");
    }
    _anim = GetComponent<Animator>();
    if (_anim == null)
    {
      Debug.LogError("Animator Component is NULL");
    }
    _audioSource = GetComponent<AudioSource>();
    if (_audioSource==null)
    {
      Debug.Log("The AudioSource component in Player.cs = NULL");
    }
  }

  void Update()
  {
    CalculateMovement();

    if (Time.time > _canFire)
    {
      _fireRate = Random.Range(3f, 7f);
      _canFire = Time.time + _fireRate;
      GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
      Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

      for (int i = 0; i < lasers.Length; i++)
      {
        lasers[i].AssignEnemyLaser();
      }
    }
  }

  void CalculateMovement()
  {
    transform.Translate(Vector3.down * _speed * Time.deltaTime);

    if (transform.position.y < -5f)
    {
      float randomX = Random.Range(-8f, 8f);
      transform.position = new Vector3(randomX, 7, 0);
    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Player")
    {
      Debug.Log("Player Collided with Enemy! '" + other.tag);

      Player player = other.transform.GetComponent<Player>();
      if (player != null)
      {
        other.transform.GetComponent<Player>().Damage();
      }

      _anim.SetTrigger("OnEnemyDeath");
      _speed=0;
      _audioSource.Play();
      Destroy(this.gameObject,2.8f);
    }
    else if (other.tag == "Laser")
    {
      Debug.Log("Player Shot Enemy with Laser!" + other.tag);
      Destroy(other.gameObject);

      if (_player != null)
      {
        _player.AddToScore(10);
      }

      _anim.SetTrigger("OnEnemyDeath");
      _speed=0;
      _audioSource.Play();
      Destroy(GetComponent<Collider2D>());
      Destroy(this.gameObject,2.8f);
    }
  }
}
