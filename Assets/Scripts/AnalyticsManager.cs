using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// AnalyticsManager - Singleton for tracking game events
/// Sends events to local analytics server for retention analysis
/// 
/// Events tracked:
///   - app_launch: When app starts
///   - level_started: When player starts a level
///   - level_completed: When player wins
///   - level_failed: When player loses
///   - ad_watched: When rewarded ad completes
///   - iap_purchase: When player buys in-app purchase
///   - session_end: When player exits
/// </summary>
public class AnalyticsManager : MonoBehaviour
{
    #region Singleton
    private static AnalyticsManager _instance;
    public static AnalyticsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("AnalyticsManager");
                _instance = obj.AddComponent<AnalyticsManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    #endregion

    #region Configuration
    private string _serverURL = "http://localhost:8888/api/events";
    private string _deviceID = "";
    private string _appVersion = "1.0.0";
    private Dictionary<string, object> _sessionData;
    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        _deviceID = SystemInfo.deviceUniqueIdentifier;
        _appVersion = Application.version;
        _sessionData = new Dictionary<string, object>();
        
        Debug.Log($"[Analytics] Initialized. Device ID: {_deviceID}");
        
        LogEvent("app_launch", new Dictionary<string, object>
        {
            { "device_id", _deviceID },
            { "os", Application.platform.ToString() },
            { "device_model", SystemInfo.deviceModel },
            { "app_version", _appVersion }
        });
    }

    /// <summary>Log generic event to analytics server</summary>
    public void LogEvent(string eventName, Dictionary<string, object> data = null)
    {
        if (data == null) data = new Dictionary<string, object>();
        
        data["event_name"] = eventName;
        data["timestamp"] = System.DateTime.UtcNow.ToString("o");
        data["device_id"] = _deviceID;
        data["session_id"] = GetSessionID();
        
        Debug.Log($"[Analytics] Event: {eventName} - Data: {JsonUtility.ToJson(data)}");
        
        // Save to local file (offline support)
        SaveEventLocally(eventName, data);
        
        // Send to server (async)
        SendEventToServer(data);
    }

    public void LogLevelStarted(int levelNumber)
    {
        LogEvent("level_started", new Dictionary<string, object>
        {
            { "level_number", levelNumber },
            { "timestamp_local", Time.time }
        });
    }

    public void LogLevelCompleted(int levelNumber, int starsEarned, float timeTaken)
    {
        LogEvent("level_completed", new Dictionary<string, object>
        {
            { "level_number", levelNumber },
            { "stars", starsEarned },
            { "time_seconds", timeTaken },
            { "score", starsEarned * 100 }
        });
    }

    public void LogLevelFailed(int levelNumber, int attemptsCount)
    {
        LogEvent("level_failed", new Dictionary<string, object>
        {
            { "level_number", levelNumber },
            { "attempts", attemptsCount }
        });
    }

    public void LogAdWatched(string adType)
    {
        LogEvent("ad_watched", new Dictionary<string, object>
        {
            { "ad_type", adType },
            { "reward", "coins" },
            { "reward_amount", 50 }
        });
    }

    public void LogIAPPurchase(string productID, float price, string currency = "USD")
    {
        LogEvent("iap_purchase", new Dictionary<string, object>
        {
            { "product_id", productID },
            { "price", price },
            { "currency", currency }
        });
    }

    public void LogSessionEnd()
    {
        LogEvent("session_end", new Dictionary<string, object>
        {
            { "session_duration", Time.realtimeSinceStartup },
            { "levels_played", PlayerPrefs.GetInt("TotalLevelsPlayed", 0) }
        });
    }

    #region Local Storage
    private void SaveEventLocally(string eventName, Dictionary<string, object> data)
    {
        string logPath = Path.Combine(Application.persistentDataPath, "analytics_log.json");
        
        try
        {
            string json = JsonUtility.ToJson(new EventData
            {
                event_name = eventName,
                timestamp = System.DateTime.UtcNow.ToString("o"),
                data = data
            });

            File.AppendAllText(logPath, json + "\n");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Analytics] Failed to save event locally: {ex}");
        }
    }

    public string ExportAnalyticsData()
    {
        string logPath = Path.Combine(Application.persistentDataPath, "analytics_log.json");
        if (File.Exists(logPath))
        {
            return File.ReadAllText(logPath);
        }
        return "";
    }
    #endregion

    #region Server Communication
    private void SendEventToServer(Dictionary<string, object> data)
    {
        // In production, send via HTTP to analytics server
        // For now, just log locally (see SaveEventLocally)
        Debug.Log($"[Analytics] Would send to: {_serverURL}");
    }
    #endregion

    #region Session Management
    private string GetSessionID()
    {
        if (!PlayerPrefs.HasKey("SessionID"))
        {
            PlayerPrefs.SetString("SessionID", System.Guid.NewGuid().ToString());
        }
        return PlayerPrefs.GetString("SessionID");
    }
    #endregion

    private void OnApplicationQuit()
    {
        LogSessionEnd();
    }

    [System.Serializable]
    private class EventData
    {
        public string event_name;
        public string timestamp;
        public Dictionary<string, object> data;
    }
}
