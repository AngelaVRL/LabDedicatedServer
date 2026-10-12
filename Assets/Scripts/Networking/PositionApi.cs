using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class PositionApi
{
    private const string JsonContentType = "application/json";

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
}