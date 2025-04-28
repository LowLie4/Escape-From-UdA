using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Analytics;

[Serializable]
public class EventRecord
{
    public string eventType;   // "pickup" o "puzzle"
    public string eventID;     // ex. "USB1", "Puzzle4"
    public float timestamp;    // Time.time quan va passar
    public float duration;     // només per puzzles, temps des de l'inici del puzzle
}

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    private Dictionary<string, float> puzzleStartTimes = new Dictionary<string, float>();
    private List<EventRecord> records = new List<EventRecord>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RecordPickup(string objectID)
    {
        records.Add(new EventRecord
        {
            eventType = "pickup",
            eventID = objectID,
            timestamp = Time.time,
            duration = 0f
        });
        Debug.Log($"[Stats] Pickup {objectID} at {Time.time}");

        Analytics.CustomEvent("pickup", new Dictionary<string, object>{
        { "id", objectID },
        { "time", Time.time }
    });
    }

    public void StartPuzzle(string puzzleID)
    {
        puzzleStartTimes[puzzleID] = Time.time;
        Debug.Log($"[Stats] Start puzzle {puzzleID} at {Time.time}");
    }

    public void CompletePuzzle(string puzzleID)
    {
        if (!puzzleStartTimes.TryGetValue(puzzleID, out float start)) return;
        float now = Time.time;
        records.Add(new EventRecord
        {
            eventType = "puzzle",
            eventID = puzzleID,
            timestamp = now,
            duration = now - start
        });
        puzzleStartTimes.Remove(puzzleID);
        Debug.Log($"[Stats] Complete puzzle {puzzleID} in {now - start:F2}s");

        // enviar a Analytics
        Analytics.CustomEvent("puzzle_complete", new Dictionary<string, object> {
        { "id", puzzleID },
        { "time", now },
        { "duration", now - start }
    });
    }

    public void ExportStats()
    {
        string json = JsonUtility.ToJson(new { events = records }, prettyPrint: true);
        string path = Path.Combine(Application.persistentDataPath, "game_stats.json");
        File.WriteAllText(path, json);
        Debug.Log($"[Stats] Exported to {path}");
    }
}
