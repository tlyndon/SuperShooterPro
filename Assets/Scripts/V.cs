using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class V
{
    public static bool musicOn = false;
    public static bool soundOn = false;
    public static int seconds = 0;
    public static int frameCounter = 0;
    public static bool isGameOver = false;
    public static int wave = 1;
    public static int enemiesToSpawn = 0;
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

    public static void setMode(int newMode)
    {
        Debug.Log("Set New Mode from " + V.mode + " to " + newMode);
        V.mode = newMode;
        V.modeCounter = 0;
    }
}