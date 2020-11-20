//------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//------------------------------
public class Enemy : MonoBehaviour
{
  [SerializeField]
  private AudioManager myAudioManager;
  [SerializeField]
  private AudioSource audioSource;

  [SerializeField]
  private float _speed = 4.0f;
  [SerializeField]
  private GameObject _laserPrefab;
  private float _laserSpeed = 2.65f;
  private bool _stopSpawningEnemyFire = false;

  private Player _player;
  private Animator anim;

  [SerializeField]
  private float _fireRate = 3.0f;
  private float _canFire = -1;

  public bool isAlive=true;
//--------------------------------------------------------------
  void Start()
  {
    myAudioManager= GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
    if (myAudioManager == null) { Debug.LogError("myAudioManager is null."); }

    audioSource = GetComponent<AudioSource>();
    if (audioSource==null) { Debug.LogError("The AudioSource component in Player.cs is NULL"); }

    _player = GameObject.Find("Player").GetComponent<Player>();
    if (_player == null) { Debug.LogError("Player Component is NULL"); }

    anim = GetComponent<Animator>();
    if (anim == null) { Debug.LogError("Animator Component is NULL"); }
  }
//--------------------------------------------------------------
  void Update()
  {
    CalculateMovement();

    if (isAlive==true && Time.time>_canFire)
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
//--------------------------------------------------------------
  void CalculateMovement()
  {
    transform.Translate(Vector3.down * _speed * Time.deltaTime);
    if (transform.position.y < -5f)
    {
      float randomX = Random.Range(-8f, 8f);
      transform.position = new Vector3(randomX, 7, 0);
    }
  }
//--------------------------------------------------------------
  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Player" && isAlive==true)
    {
      Debug.Log("Player Collided with Enemy! '" + other.tag);

      isAlive=false;
      anim.SetTrigger("OnEnemyDeath");
      if (myAudioManager.SoundStatus()==true) { audioSource.Play(); }
      Destroy(this.gameObject,2.8f);

      Player player = other.transform.GetComponent<Player>();
      if (player != null) { other.transform.GetComponent<Player>().Damage(); }
    }
    else if (other.tag == "Laser")
    {
      Debug.Log("Player Shot Enemy with Laser!" + other.tag);
      Destroy(other.gameObject);

      if (_player != null) { _player.AddToScore(10); }

      isAlive=false;
      anim.SetTrigger("OnEnemyDeath");

      if (myAudioManager.SoundStatus()==true) { audioSource.Play(); }

      Destroy(GetComponent<Collider2D>());
      Destroy(this.gameObject,2.8f);
    }
  }
}
