using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace PuzzleGame.Analytics
{
    public class AnalyticsNetworkClient
    {
        private string _serverUrl;

        public AnalyticsNetworkClient(string serverUrl)
        {
            _serverUrl = serverUrl;
        }

        public IEnumerator SendBatchAsync(List<AnalyticsEvent> events, System.Action<bool> callback)
        {
            if (events.Count == 0)
            {
                callback?.Invoke(true);
                yield break;
            }

            var batchPayload = new BatchEventPayload
            {
                events = events,
                client_version = Application.version
            };

            string json = JsonUtility.ToJson(batchPayload);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest($"{_serverUrl}/events/batch", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = 10;

                yield return request.SendWebRequest();

                bool success = request.result == UnityWebRequest.Result.Success;

                if (success)
                {
                    Debug.Log($"[Analytics] Successfully sent {events.Count} events to server");
                }
                else
                {
                    Debug.LogError($"[Analytics] Failed to send events: {request.error}");
                }

                callback?.Invoke(success);
            }
        }
    }
}
