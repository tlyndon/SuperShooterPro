using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
  [SerializeField]
  private float _playerLaserspeed = 8.0f;
  [SerializeField]
  private float _enemyLaserSpeed = 6.0f;
  private bool _isEnemyLaser = false;

  void Update()
  {
    if (_isEnemyLaser == false)
    {
      MoveUp();
    }
    else
    {
      MoveDown();
    }
  }

  void MoveUp()
  {
    transform.Translate(Vector3.up * _playerLaserspeed * Time.deltaTime);

    if (transform.position.y > 8f)
    {
      if (transform.parent != null)
      {
        Destroy(transform.parent.gameObject);
      }
      Destroy(this.gameObject);
    }
  }

  void MoveDown()
  {
    transform.Translate(Vector3.down * _enemyLaserSpeed * Time.deltaTime);

    if (transform.position.y < -8f)
    {
      if (transform.parent != null)
      {
        Destroy(transform.parent.gameObject);
      }
      Destroy(this.gameObject);
    }
  }

  public void AssignEnemyLaser()
  {
    _isEnemyLaser = true;
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Player" && _isEnemyLaser == true)
    {
      Player player=other.GetComponent<Player>();
      if (player != null)
      {
        player.Damage();
      }
    }
  }
}
