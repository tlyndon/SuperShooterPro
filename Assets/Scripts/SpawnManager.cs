using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
  [SerializeField]
  private GameObject _enemyPrefab;
  [SerializeField]
  private GameObject _tripleShotPowerupPrefab;
  [SerializeField]
  private GameObject _enemyContainer;
  [SerializeField]
  private GameObject[] _powerups;
  private bool _stopSpawning = false;
  [SerializeField]
  private Player _player;

  public void startSpawning()
  {
    StartCoroutine(SpawnEnemyRoutine());
    StartCoroutine(SpawnPowerupRoutine());
  }
  
  IEnumerator SpawnEnemyRoutine()
  {
    yield return new WaitForSeconds(3.0f);
    while (_stopSpawning == false)
    {
      Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
      GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
      newEnemy.transform.parent = _enemyContainer.transform;
      yield return new WaitForSeconds(5.0f);
    }
  }

  IEnumerator SpawnPowerupRoutine()
  {
    yield return new WaitForSeconds(3.0f);
    while (_stopSpawning == false)
    {
      Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
      int powerUp = Random.Range(0,3);

      int ammoCount = _player.ammoCount;
      if (ammoCount==0)
      {
        powerUp=3;  //ammo powerup
      }

      Instantiate(_powerups[powerUp], posToSpawn, Quaternion.identity);
      yield return new WaitForSeconds(Random.Range(3,8));
    }
  }

  public void OnPlayerDeath()
  {
    _stopSpawning = true;
  }
}
