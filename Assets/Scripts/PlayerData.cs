public static class PlayerData 
{
    public static int currentScore = 0;
    public static int currentHealth = 5;
    public static int maxHealth = 6;

    public static void ResetData()
    {
        currentScore = 0;
        currentHealth = maxHealth;
    }
}