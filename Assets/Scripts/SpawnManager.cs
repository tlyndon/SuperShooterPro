using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyContainer;
    [SerializeField]
    private GameObject[] powerupObjects;
    private bool stopSpawning = false;
    [SerializeField]
    private int updateCounter = 0;
    //--------------------------------------------------------------
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }
    //--------------------------------------------------------------
    void Update()
    {
        SpawnEnemyIfNoneRemaining();
    }
    //--------------------------------------------------------------
    void SpawnEnemyIfNoneRemaining()
    {
        updateCounter = updateCounter + 1;
        if (updateCounter > 60)  //once per second (curious, how to change to be time-based)
        {
            //Debug.Log("Update in Spawn Manager");
            updateCounter = 0;
            GameObject[] gameObjects;
            gameObjects = GameObject.FindGameObjectsWithTag("Enemy");
            if (gameObjects.Length == 0)
            {
                Debug.Log("No game objects are tagged with 'Enemy', so we will spawn a new one");
                smallNewEnemy();
            }
        }
    }
    //--------------------------------------------------------------
    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (stopSpawning == false)
        {
            smallNewEnemy();
            yield return new WaitForSeconds(5.0f);
        }
    }
    void smallNewEnemy()
    {
        Vector3 posToSpawn = new Vector3(Random.Range(1, 18) - 8, 7, 0);
        GameObject newEnemy = Instantiate(enemyPrefab, posToSpawn, Quaternion.identity);
        newEnemy.transform.parent = enemyContainer.transform;
    }
    //--------------------------------------------------------------
    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);

            int powerUp = Random.Range(0, 3);  //0, 1 or 2 speed, tripeShot or sheild

            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            Player player = obj.transform.GetComponent<Player>();
            if (player.health < 3)
            {
                powerUp = 4;        //show health powerup
                if (player.ammoCount == 0 && Random.Range(0, 2) == 1)
                { powerUp = 3; }    //show ammo powerup
            }
            else if (player.ammoCount == 0)
            { powerUp = 3; }        //ammo powerup

            Instantiate(powerupObjects[powerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(2, 7));
        }
    }
    //--------------------------------------------------------------
    public void OnPlayerLossOfHealth()
    {
        stopSpawning = true;
    }
}