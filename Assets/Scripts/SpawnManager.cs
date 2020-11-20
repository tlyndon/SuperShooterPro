using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//------------------------------
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
  private int updateCounter=0;
//------------------------------
  void Start()
  {
    StartCoroutine(SpawnEnemyRoutine());
    StartCoroutine(SpawnPowerupRoutine());
  }
  void Update()
  {
    SpawnEnemyIfNoneRemaining();
  }
//------------------------------
void SpawnEnemyIfNoneRemaining()
{
  updateCounter=updateCounter+1;
  if (updateCounter>60)  //once per second (curious, how to change to be time-based)
  {
    //Debug.Log("Update in Spawn Manager");
    updateCounter=0;
    GameObject[] gameObjects;
    gameObjects = GameObject.FindGameObjectsWithTag("Enemy");
    if (gameObjects.Length == 0)
    {
      Debug.Log("No game objects are tagged with 'Enemy', so we will spawn a new one");
      smallNewEnemy();
    }
  }
}
//------------------------------
  IEnumerator SpawnEnemyRoutine()
  {
    yield return new WaitForSeconds(3.0f);
    while (_stopSpawning == false)
    {
      smallNewEnemy();
      yield return new WaitForSeconds(5.0f);
    }
  }
  void smallNewEnemy()
  {
    Vector3 posToSpawn = new Vector3(Random.Range(1, 18)-8, 7, 0);
    GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
    newEnemy.transform.parent = _enemyContainer.transform;
  }
//------------------------------
  IEnumerator SpawnPowerupRoutine()
  {
    yield return new WaitForSeconds(3.0f);
    while (_stopSpawning == false)
    {
      Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);

      int health = _player.health;
      int ammoCount = _player.ammoCount;

      int powerUp = Random.Range(0,3);  //0,1 or 2 speed, tripeShot or sheild

      if (health < 3)
      {
        powerUp=4;    //show health powerup
        if (ammoCount==0 && Random.Range(0,2)==1)
        {
          powerUp=3;  //show ammo powerup
        }
      }
      else if (ammoCount==0)
      {
        powerUp=3;  //ammo powerup
      }

      Instantiate(_powerups[powerUp], posToSpawn, Quaternion.identity);
      yield return new WaitForSeconds(Random.Range(2,7));
    }
  }
//------------------------------
  public void OnPlayerLossOfHealth()
  {
    _stopSpawning = true;
  }
}
