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
    private int lastPowerUp = 99;
    //--------------------------------------------------------------
    void Start()
    {

    }
    //--------------------------------------------------------------
    void Update()
    {
        ConsiderSpawningPowerUp();

        if (V.mode == 20)
        {
            if (V.levelAndWave < V.bossLevel)
            {
                ConsiderSpawningEnemy();
            }

            V.setMode(21);
        }
    }
    //--------------------------------------------------------------
    void ConsiderSpawningEnemy()
    {
        V.nextTimePowerUpCanSpawn = 0;

        // spawn all enemies for each wave at the start of the wave

        int ctr = 0;
        int howMany = (int)(V.levelAndWave * 1.5f) + 2;

        while (ctr < howMany)
        {
            int spawnEnemyType = 0;

            ctr = ctr + 1;
            howMany = howMany - 1;

            if (V.levelAndWave >= V.levelEnemy1joins && Random.Range(0, 5f) >= 4)
            {
                spawnEnemyType = 1;
                if (V.levelAndWave >= V.levelEnemy2joins && Random.Range(0, 5f) >= 4)
                {
                    spawnEnemyType = 2;
                }
            }
            SpawnOneEnemy(spawnEnemyType, ctr);

        }
    }
    //--------------------------------------------------------------
    void SpawnOneEnemy(int typ, int ctr)
    {
        Vector3 posToSpawn = new Vector3(Random.Range(0, 12) - 6, 8 + (ctr * 3), 0);  //typ=0 & 2
        if (typ == 1)
        {
            float rnd = Random.Range(-8, 8f);
            posToSpawn = new Vector3(-4, 8 + rnd, 0);
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

        if (typ == 1)
        {
            if (V.levelAndWave < 2)
            { enemy.speedY = 4.0f; }
            else if (V.levelAndWave == 2)
            { enemy.speedY = 4.5f; }
            else if (V.levelAndWave == 3)
            { enemy.speedY = 5.0f; }
            else if (V.levelAndWave == 4)
            { enemy.speedY = 5.5f; }
            else if (V.levelAndWave == 5)
            { enemy.speedY = 6.0f; }
            else
            { enemy.speedY = 6.5f; }
        }

        if (V.levelAndWave >= V.levelEnemyGetShields)
        {
            //int r = 11 - V.wave;
            //if (r < 2) { r = 2; }

            int r = 4;
            if (Random.Range(0, r) == 1)
            {
                enemy.hasShield = true;
            }
        }
    }
    //--------------------------------------------------------------
    void ConsiderSpawningPowerUp()
    {
        // 1 = tripleshot
        // 0 = speed
        // 2 = sheilds
        // 4 = health
        // 5 = skull & crossbones
        // 6 = mine
        if (V.mode >= 20 && V.modeCounter > V.nextTimePowerUpCanSpawn)
        {
            V.zprint("spawn", "SPAWNING POWERUP?");
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            Player player = obj.transform.GetComponent<Player>();

            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);

            int[] pups = { 0 };

            if (V.levelAndWave >= V.levelGet3ShotLaser)
            { pups[pups.Length - 1] = 1; }
            if (V.levelAndWave >= V.levelGetShields && (player.isShieldsActive == false || player.shieldStrength < player.shieldStrengthDefault))
            { pups[pups.Length - 1] = 2; }

            V.zprint("spawnPowerup", "pups.Length:" + pups.Length);

            int r = Random.Range(0, pups.Length);
            V.zprint("spawnPowerup", "r:" + r);

            int powerUp = pups[r];
            V.zprint("spawnPowerup", "powerUp:" + powerUp);

            if (player.health < 3)
            {
                powerUp = 4;            //show health powerup

                if (player.ammoCount == 0 && Random.Range(0, 2) == 1)
                {
                    powerUp = 3;        //show ammo powerup
                }
            }
            else if (player.ammoCount < player.ammoCountDefault * .25f)
            {
                powerUp = 3;            //ammo powerup
            }

            if ((powerUp == 3 || powerUp == 4) && powerUp != lastPowerUp)
            {
                V.nextTimePowerUpCanSpawn = V.modeCounter + 120;
            }
            else
            {
                int tim = 640;
                if (V.levelAndWave > 9)
                { tim = 1280; }
                V.nextTimePowerUpCanSpawn = V.modeCounter + tim;
            }

            if (V.levelAndWave >= V.levelGetMinesToShoot && powerUp < 3 && Random.Range(0, 10) < 2)
            {
                powerUp = 6;  //mine
            }

            if (V.levelAndWave >= V.levelSkullAndCrossBones && powerUp < 3 && Random.Range(0, 10) < 2)
            {
                powerUp = 5;  //skull & crossbones
            }

            Instantiate(powerupObjects[powerUp], posToSpawn, Quaternion.identity);
            lastPowerUp = powerUp;
        }
        //else if (V.modeCounter % 60 == 0)
        //{
        //    V.zprint("spawnPowerup", "V.mode:" + V.mode + ", V.modeCounter:" + V.modeCounter + ", V.nextTimePowerUpCanSpawn:" + V.nextTimePowerUpCanSpawn);
        //}
    }
    //--------------------------------------------------------------
}