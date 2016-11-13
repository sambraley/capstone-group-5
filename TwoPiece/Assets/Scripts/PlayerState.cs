using System;

public class PlayerState
{
    private static PlayerState instance;
    private int coins;
    private int health;
    private int maxHealth;

    private PlayerState() {
        coins = 0;
        health = 3;
        maxHealth = 3;
    }
    
    public void setCoins(int coins)
    {
        this.coins = coins;
    }

    public int getCoins()
    {
        return coins;
    }

    public void incrementCoins()
    {
        coins++;
    }

    public int getHealth()
    {
        return health;
    }

    public void giveMaxHealth()
    {
        this.health = maxHealth;
    }

    public int getMaxHealth()
    {
        return maxHealth;
    }

    public void decrementHealth()
    {
        this.health--;
    }

    public void setMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    public void incrementMaxHealth()
    {
        if(maxHealth <= 5)
            maxHealth++;
    }

    public void incrementHealth()
    {
        if(health < maxHealth)
            health++;
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