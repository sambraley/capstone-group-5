using System;

public class PlayerState
{
    private static PlayerState instance;
    public int coins;
    public int maxHealth;

    private PlayerState() {
        coins = 0;
        maxHealth = 3;
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