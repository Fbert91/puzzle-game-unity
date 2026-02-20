using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace PuzzleGame.Analytics
{
    /// <summary>
    /// Main analytics manager for sending events to the centralized analytics server
    /// Handles batching, offline queue, and retry logic
    /// </summary>
    public class AnalyticsManager : MonoBehaviour
    {
        [SerializeField] private string serverUrl = "http://localhost:5000/api";
        [SerializeField] private int batchSize = 10;
        [SerializeField] private float flushInterval = 30f; // seconds

        private static AnalyticsManager _instance;
        private AnalyticsEventQueue _eventQueue;
        private AnalyticsNetworkClient _networkClient;
        private float _timeSinceLastFlush = 0f;

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

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize
            _networkClient = new AnalyticsNetworkClient(serverUrl);
            _eventQueue = new AnalyticsEventQueue();
            _eventQueue.LoadFromStorage();
        }

        private void Update()
        {
            _timeSinceLastFlush += Time.deltaTime;

            if (_timeSinceLastFlush >= flushInterval || _eventQueue.Count >= batchSize)
            {
                FlushEvents();
                _timeSinceLastFlush = 0f;
            }
        }

        private void OnApplicationQuit()
        {
            FlushEvents();
        }

        public void LogEvent(string eventType, Dictionary<string, object> properties = null)
        {
            var playerId = GetOrCreatePlayerId();
            var sessionId = GetSessionId();

            var analyticsEvent = new AnalyticsEvent
            {
                player_id = playerId,
                session_id = sessionId,
                event_type = eventType,
                timestamp = DateTime.UtcNow,
                properties = properties ?? new Dictionary<string, object>()
            };

            _eventQueue.AddEvent(analyticsEvent);

            // Auto-flush if batch size reached
            if (_eventQueue.Count >= batchSize)
            {
                FlushEvents();
            }
        }

        public void LogLevelStarted(int levelNumber)
        {
            LogEvent("level_started", new Dictionary<string, object>
            {
                { "level", levelNumber },
                { "timestamp", DateTime.UtcNow }
            });
        }

        public void LogLevelCompleted(int levelNumber, int moves, int stars)
        {
            LogEvent("level_completed", new Dictionary<string, object>
            {
                { "level", levelNumber },
                { "moves", moves },
                { "stars", stars },
                { "timestamp", DateTime.UtcNow }
            });
        }

        public void LogLevelFailed(int levelNumber, int attempts)
        {
            LogEvent("level_failed", new Dictionary<string, object>
            {
                { "level", levelNumber },
                { "attempts", attempts },
                { "timestamp", DateTime.UtcNow }
            });
        }

        public void LogIAPPurchase(string productId, decimal amount, string currency = "USD")
        {
            LogEvent("iap_purchase", new Dictionary<string, object>
            {
                { "product_id", productId },
                { "amount", amount },
                { "currency", currency },
                { "timestamp", DateTime.UtcNow }
            });
        }

        public void LogAdWatch(string adNetwork, string adPlacement)
        {
            LogEvent("ad_watch", new Dictionary<string, object>
            {
                { "ad_network", adNetwork },
                { "ad_placement", adPlacement },
                { "timestamp", DateTime.UtcNow }
            });
        }

        public void LogSessionStart()
        {
            LogEvent("app_launch", new Dictionary<string, object>
            {
                { "app_version", Application.version },
                { "platform", Application.platform.ToString() },
                { "timestamp", DateTime.UtcNow }
            });
        }

        public void LogSessionEnd(int sessionDuration)
        {
            LogEvent("session_end", new Dictionary<string, object>
            {
                { "session_duration_seconds", sessionDuration },
                { "timestamp", DateTime.UtcNow }
            });
        }

        private void FlushEvents()
        {
            if (_eventQueue.Count == 0) return;

            var events = _eventQueue.GetBatch(batchSize);
            StartCoroutine(_networkClient.SendBatchAsync(events, (success) =>
            {
                if (success)
                {
                    _eventQueue.RemoveEvents(events);
                    _eventQueue.SaveToStorage();
                    Debug.Log($"[Analytics] Flushed {events.Count} events to server");
                }
                else
                {
                    Debug.LogWarning("[Analytics] Failed to send events, will retry later");
                    // Events stay in queue for retry
                }
            }));
        }

        private string GetOrCreatePlayerId()
        {
            string key = "analytics_player_id";
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetString(key);
            }

            string playerId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(key, playerId);
            PlayerPrefs.Save();
            return playerId;
        }

        private string GetSessionId()
        {
            string key = "analytics_session_id";
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetString(key);
            }

            string sessionId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(key, sessionId);
            PlayerPrefs.Save();
            return sessionId;
        }
    }
}
