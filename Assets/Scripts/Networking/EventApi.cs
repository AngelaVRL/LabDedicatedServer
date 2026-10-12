using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static class EventApi
{
    public static IEnumerator Post(string url, GameEventRequest gameEvent)
    {
        string body = JsonConvert.SerializeObject(gameEvent);

        using UnityWebRequest request = JsonRequest.CreatePost(url, body);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"Event publish failed: {request.error} (HTTP {request.responseCode})");
        }
    }

    public static IEnumerator Get(string url, Action<EventBatch> onBatchReceived)
    {
        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"Event fetch failed: {request.error} (HTTP {request.responseCode})");
            yield break;
        }

        EventBatch batch = ParseBatch(request.downloadHandler.text);

        if (batch == null)
        {
            yield break;
        }

        onBatchReceived(batch);
    }

    private static EventBatch ParseBatch(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<EventBatch>(json);
        }
        catch (JsonException exception)
        {
            Debug.LogWarning($"Event response could not be parsed: {exception.Message}");
            return null;
        }
    }
}