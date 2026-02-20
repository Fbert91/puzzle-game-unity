using System;
using System.Collections.Generic;

namespace PuzzleGame.Analytics
{
    [System.Serializable]
    public class AnalyticsEvent
    {
        public string player_id;
        public string session_id;
        public string event_type;
        public System.DateTime timestamp;
        public Dictionary<string, object> properties;
    }

    [System.Serializable]
    public class BatchEventPayload
    {
        public List<AnalyticsEvent> events;
        public string client_version;
    }
}
