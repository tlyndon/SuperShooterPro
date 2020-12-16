using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyPrefab2;
    [SerializeField]
    private GameObject enemyContainer;
    [SerializeField]
    private GameObject[] powerupObjects;
    [SerializeField]
    private int updateCounter = 0;
    private UIManager uiManager;
    private GameManager gameManager;
    private float nextTimePowerUpCanSpawn = 0.0f;
    private int lastPowerUp = 99;
    //--------------------------------------------------------------
    void Start()
    {

    }
    //--------------------------------------------------------------
    void Update()
    {
        //ConsiderSpawningEnemy();
        ConsiderSpawningPowerUp();
    }

    //--------------------------------------------------------------
    void ConsiderSpawningEnemy()
    {
        if (V.mode == 20)
        {
            nextTimePowerUpCanSpawn = 0;

            int nextEnemyType = 0;
            int ctr = 0;
            V.enemiesToSpawn = (int) V.wave * 3 + 8;
            int howMany = V.enemiesToSpawn;

            while (ctr < howMany)
            {
                SpawnOneEnemy(nextEnemyType, ctr);
                ctr = ctr + 1;
                V.enemiesToSpawn = V.enemiesToSpawn - 1;

                if ((nextEnemyType == 0 && ctr > howMany * 0.65) || (nextEnemyType == 1 && ctr > howMany * .1))
                {
                    nextEnemyType = nextEnemyType + 1;
                    if (nextEnemyType > 2)
                    { nextEnemyType = 0; }
                }
            }
            V.setMode(21);
        }
        if (V.mode == 21)
        {
            V.modeCounter = V.modeCounter + 1;
        }
    }
    //--------------------------------------------------------------
    void SpawnOneEnemy(int typ, int ctr)
    {
        Vector3 posToSpawn = new Vector3(Random.Range(0, 8) - 4, 8 + (ctr * 3), 0);  //typ=0 & 2
        if (typ == 1)
        {
            posToSpawn = new Vector3(-4, 8 + (ctr * 2), 0);  //typ=1
        }
        GameObject obj;
        if (typ != 2)
        {
            obj = enemyPrefab;
        }
        else
        {
            obj = enemyPrefab2;
        }
        GameObject newEnemy = Instantiate(obj, posToSpawn, Quaternion.identity);
        newEnemy.transform.parent = enemyContainer.transform;

        Enemy enemy = newEnemy.GetComponent<Enemy>();
        enemy.enemyType = typ;

        int r = 11 - V.wave;
        if (r<2) { r = 2; }
        if (Random.Range(0, r) == 1)
        {
            enemy.hasShield = true;
        }
    }
    //--------------------------------------------------------------
    void ConsiderSpawningPowerUp()
    {
        if (V.mode == 20 && V.modeCounter > nextTimePowerUpCanSpawn)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int powerUp = Random.Range(0, 3);  //0, 1 or 2 speed, tripleShot or sheild

            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            Player player = obj.transform.GetComponent<Player>();
            if (player.health < 3)
            {
                powerUp = 4;            //show health powerup

                if (player.ammoCount == 0 && Random.Range(0, 2) == 1)
                {
                    powerUp = 3;        //show ammo powerup
                }
            }
            else if (player.ammoCount < player.ammoCountDefault * 0.50)
            {
                powerUp = 3;            //ammo powerup
            }

            if ((powerUp == 3 || powerUp == 4) && powerUp != lastPowerUp)
            {
                nextTimePowerUpCanSpawn = V.modeCounter + 60;
            }
            else
            {
                nextTimePowerUpCanSpawn = V.modeCounter + 240;
            }

            if (powerUp < 3 && Random.Range(0, 10) < 2)
            {
                powerUp = 6;  //mine
            }

            if (powerUp < 3 && Random.Range(0, 10) < 2)
            {
                powerUp = 5;  //butterflybones
            }

            Instantiate(powerupObjects[powerUp], posToSpawn, Quaternion.identity);
            lastPowerUp = powerUp;
        }
    }
    //--------------------------------------------------------------
}