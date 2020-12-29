using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// help from https://answers.unity.com/questions/323195/how-can-i-have-a-static-class-i-can-access-from-an.html

public class V : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    //sound and music
    //change skull & crossbones

    public static int levelEnemy1joins = 3;
    public static int levelGet3ShotLaser = 4;
    public static int levelGetMissle = 5;
    public static int levelGetShields = 6;
    public static int levelEnemy2joins = 7;
    public static int levelGetMinesToShoot = 8;
    public static int levelEnemyGetShields = 9;
    public static int levelEnemyLaserJoins = 10;
    public static int levelEnemyAvoidsLasers = 11;
    public static int levelSkullAndCrossBones = 12;
    public static int bossLevel = 15;

    public static bool musicOn = true;
    public static bool soundOn = true;
    public static int seconds = 0;
    public static int frameCounter = 0;
    public static bool isGameOver = false;
    public static int wave = 1; //3;
    public static int level = 1; //5;
    public static int levelAndWave = 1; //15;
    public static int enemiesToSpawn = 0;
    public static int mineCount = 0;
    public static string flashingText = "";
    public static int lastPowerUpSpawned;
    public static int modeCounter = 0;
    public static int mode = 999;
    //Mode 0: Get Ready(3 seconds)
    //Mode 0: Ship Comes up from the Bottom
    //Mode 10: Display "Wave #1"
    //Mode 20: Every 3–5 seconds, I'll spawn a new enemy, perhaps 7 times for wave 1.
    //Mode 21: After 7 enemies are spawned, I then wait for the player to destroy them and then:
    //Mode 10: Display "WAVE #2"
    //Mode 100: GameOver
    //--------------------------------------------------------------
    public static void setMode(int newMode)
    {
        zprint("mode", "V.mode = " + newMode + " (was: " + V.mode + ")");
        V.mode = newMode;
        V.modeCounter = 0;
    }
    //--------------------------------------------------------------
    public static string repeatString(string str, int count)
    {
        for (int i = 0; i < count; i++)
        {
            str = str + " ";
        }
        return str;
    }
    //--------------------------------------------------------------
    // V.zprint("trace", "scaleX");
    public static void zprint(string searchFor, string txt)
    {
        string[] arry = { "avoid", "mode"};
        //string[] arry = { "trace", "mode", "powerup", "error", "enemy", "keys", "damage","raycast","avoid","bossMove","bossCollide","bossDamage","bossEnergy", "spawnPowerup","flashingText","UIManager","enemyShield"};
        int f = System.Array.IndexOf(arry, searchFor);
        if (f > -1)
        {
            string str = arry[f];
            int spaces = 10 - str.Length;
            string newstring = str + V.repeatString(" ", spaces);
            if (str != "error")
            {
                Debug.Log("------------------------------------------->" + newstring + "|" + txt);
            }
            else
            {
                Debug.LogError("------------------------------------------->" + newstring + "|" + txt);
            }
        }
    }
}