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

    //bugs to fix
    //missile doesn't kill enemy2

    public static bool musicOn = false;
    public static bool soundOn = false;
    public static int seconds = 0;
    public static int frameCounter = 0;
    public static bool isGameOver = false;
    public static int wave = 1;
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
        string[] arry = { "bossEnergy", "mode", "bossDamage" };
        //string[] arry = { "trace", "mode", "powerup", "error", "enemy", "keys", "damage","raycast","avoid","bossMove","bossCollide","bossDamage","bossEnergy"};
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