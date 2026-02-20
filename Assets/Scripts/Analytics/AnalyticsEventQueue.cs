using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace PuzzleGame.Analytics
{
    public class AnalyticsEventQueue
    {
        private List<AnalyticsEvent> _queue = new List<AnalyticsEvent>();
        private string _storagePath;
        private const string QUEUE_FILE = "analytics_queue.json";

        public int Count => _queue.Count;

        public AnalyticsEventQueue()
        {
            _storagePath = Path.Combine(Application.persistentDataPath, QUEUE_FILE);
        }

        public void AddEvent(AnalyticsEvent analyticsEvent)
        {
            _queue.Add(analyticsEvent);
        }

        public List<AnalyticsEvent> GetBatch(int batchSize)
        {
            int count = Mathf.Min(batchSize, _queue.Count);
            return _queue.GetRange(0, count);
        }

        public void RemoveEvents(List<AnalyticsEvent> events)
        {
            foreach (var evt in events)
            {
                _queue.Remove(evt);
            }
        }

        public void SaveToStorage()
        {
            try
            {
                string json = JsonUtility.ToJson(new EventQueueData { events = _queue });
                File.WriteAllText(_storagePath, json);
                Debug.Log($"[Analytics] Queue saved to storage: {_queue.Count} events");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Analytics] Failed to save queue: {ex.Message}");
            }
        }

        public void LoadFromStorage()
        {
            try
            {
                if (File.Exists(_storagePath))
                {
                    string json = File.ReadAllText(_storagePath);
                    var data = JsonUtility.FromJson<EventQueueData>(json);
                    _queue = data.events ?? new List<AnalyticsEvent>();
                    Debug.Log($"[Analytics] Queue loaded from storage: {_queue.Count} events");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Analytics] Failed to load queue: {ex.Message}");
                _queue = new List<AnalyticsEvent>();
            }
        }

        public void Clear()
        {
            _queue.Clear();
        }

        [System.Serializable]
        private class EventQueueData
        {
            public List<AnalyticsEvent> events;
        }
    }
}
