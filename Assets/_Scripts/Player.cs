using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int Money;
    public float Level;
}
public static class Player
{
    public static int Money;
    public static float Level;

    private static string savePath = Application.persistentDataPath + "/playerData.json";

    public static void Save()
    {
        PlayerData playerData = new PlayerData
        {
            Money = Money,
            Level = Level
        };
        var json = JsonUtility.ToJson(playerData);
        Debug.Log(savePath);
        System.IO.File.WriteAllText(savePath, json);
    }

    public static void Load()
    {
        if (System.IO.File.Exists(savePath))
        {
            string json = System.IO.File.ReadAllText(savePath);

            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            Money = playerData.Money;
            Level = playerData.Level;
        }
        else
        {
            Debug.LogWarning("Save file not found " + savePath);
        }
    }
}
