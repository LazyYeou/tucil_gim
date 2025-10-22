using UnityEngine;


public static class SaveData
{
    public static void SaveLevel(int level)
    {
        PlayerPrefs.SetInt("Level", level);
    }


    public static int LoadLevel()
    {
        return PlayerPrefs.GetInt("Level", 1);
    }
}