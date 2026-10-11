using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PositionPoller : MonoBehaviour
{
    private const long NotFoundStatusCode = 404;

    [SerializeField] private ServerSettings settings;
    [SerializeField] private Transform remoteTransform;

    private void Start()
    {
        StartCoroutine(PollLoop());
    }

    private IEnumerator PollLoop()
    {
        while (true)
        {
            yield return FetchRemotePosition();
            yield return new WaitForSecondsRealtime(settings.PollIntervalSeconds);
        }
    }

    private IEnumerator FetchRemotePosition()
    {
        string url = settings.BuildPlayerUrl(settings.RemotePlayerId);

        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.responseCode == NotFoundStatusCode)
        {
            yield break;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"Position fetch failed: {request.error} (HTTP {request.responseCode})");
            yield break;
        }

        ApplyPosition(request.downloadHandler.text);
    }

    private void ApplyPosition(string json)
    {
        PositionData position = JsonUtility.FromJson<PositionData>(json);

        if (position == null)
        {
            Debug.LogWarning("Position response could not be parsed.");
            return;
        }

        remoteTransform.position = position.ToVector();
    }
}