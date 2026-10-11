using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PositionPublisher : MonoBehaviour
{
    private const string JsonContentType = "application/json";

    [SerializeField] private ServerSettings settings;
    [SerializeField] private Transform trackedTransform;

    private void Start()
    {
        StartCoroutine(PublishLoop());
    }

    private IEnumerator PublishLoop()
    {
        while (true)
        {
            yield return PublishPosition();
            yield return new WaitForSecondsRealtime(settings.PollIntervalSeconds);
        }
    }

    private IEnumerator PublishPosition()
    {
        string body = JsonUtility.ToJson(PositionData.FromVector(trackedTransform.position));
        string url = settings.BuildPlayerUrl(settings.LocalPlayerId);

        using var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", JsonContentType);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"Position publish failed: {request.error} (HTTP {request.responseCode})");
        }
    }
}