using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class PositionApi
{
    private const string JsonContentType = "application/json";
    private const long NotFoundStatusCode = 404;

    public static IEnumerator Post(string url, Vector3 position)
    {
        string body = JsonUtility.ToJson(PositionData.FromVector(position));

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

    public static IEnumerator Get(string url, Action<PositionData> onPositionReceived)
    {
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

        PositionData position = JsonUtility.FromJson<PositionData>(request.downloadHandler.text);

        if (position == null)
        {
            Debug.LogWarning("Position response could not be parsed.");
            yield break;
        }

        onPositionReceived(position);
    }
}