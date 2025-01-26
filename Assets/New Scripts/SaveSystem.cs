using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class HighscoreEntry
{
    public string playerName; // Name des Spielers
    public int score;         // Highscore des Spielers

    public HighscoreEntry(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}

[System.Serializable]
public class HighscoreList
{
    public List<HighscoreEntry> entries = new List<HighscoreEntry>();
}

public static class SaveSystem
{
    public static string highscoreSave = "highscore_list.json";
    public static string gameSettingsSave = "game_settings.json";
    public static string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public static void SaveData<T>(T data, string fileName)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetFilePath(fileName), json);
        Debug.Log($"Data saved to {GetFilePath(fileName)}");
    }

    public static T LoadData<T>(string fileName)
    {
        string path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        Debug.LogWarning($"No save file found at {path}");
        return default;
    }

    public static void UpdateHighscoreList(string fileName, string playerName, int newHighscore)
    {
        // Load the data list
        HighscoreList highscoreList = SaveSystem.LoadData<HighscoreList>(fileName);

        // If there is no data, initialise a new list
        if (highscoreList == null)
        {
            highscoreList = new HighscoreList();
        }

        // Make a new Highscore Entry
        HighscoreEntry newEntry = new HighscoreEntry(playerName, newHighscore);

        // Add a new Entry
        highscoreList.entries.Add(newEntry);

        // Sort the list: Highest Score first
        highscoreList.entries.Sort((a, b) => b.score.CompareTo(a.score));

        // Only include the top 6
        if (highscoreList.entries.Count > 6)
        {
            highscoreList.entries = highscoreList.entries.GetRange(0, 6);
        }

        // Save the updatet list
        SaveSystem.SaveData(highscoreList, fileName);
    }
    public static List<HighscoreEntry> DisplayHighscores(string fileName)
    {
        List<HighscoreEntry> entries = new List<HighscoreEntry>();
        // Lade die Highscore-Liste
        HighscoreList highscoreList = SaveSystem.LoadData<HighscoreList>(fileName);

        // Überprüfe, ob es Einträge gibt
        if (highscoreList == null || highscoreList.entries.Count == 0)
        {
            Debug.Log("Keine Highscores verfügbar.");
            return null;
        }

        // Gib jeden Eintrag aus
        foreach (HighscoreEntry entry in highscoreList.entries)
        {
            entries.Add(entry);
            Debug.Log($"Spieler: {entry.playerName}, Score: {entry.score}");
        }
        return entries;
    }
    public static void ClearHighscores()
    {
        SaveSystem.SaveData(new List<HighscoreEntry>(), "highscore_list.json");
        Debug.Log("Highscore-Liste wurde zurückgesetzt.");
    }
}