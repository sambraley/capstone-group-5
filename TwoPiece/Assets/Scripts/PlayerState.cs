using System;

public class PlayerState
{
    private static PlayerState instance;
    private int coins;

    private PlayerState() {
        coins = 0;
    }
    
    public void setCoins(int coins)
    {
        this.coins = coins;
    }

    public int getCoins()
    {
        return coins;
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