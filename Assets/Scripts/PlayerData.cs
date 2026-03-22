public static class PlayerData 
{
    public static int currentScore = 0;
    public static int currentHealth = 100;
    public static int maxHealth = 100;

    public static void ResetData()
    {
        currentScore = 0;
        currentHealth = maxHealth;
    }
}