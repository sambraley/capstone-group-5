using System;
using UnityEngine;

public class PlayerState
{
    private static PlayerState instance;
    public int coins;
    public int maxHealth;
    public int enemiesKilled;
    public int totalEnemiesKilled;
    public Vector2 pos;
    public bool hitCheckpoint;
    public bool hasSword;
    public int timesDied;

    private PlayerState() {
        coins = 0;
        maxHealth = 3;
        enemiesKilled = 0;
        totalEnemiesKilled = 0;
        pos = default(Vector2);
        hitCheckpoint = false;
        hasSword = false;
        timesDied = 0;
    }

    public static PlayerState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerState();
            }
            return instance;
        }
    }
}