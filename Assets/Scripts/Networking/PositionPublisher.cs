using System.Collections;
using UnityEngine;

public class PositionPublisher : MonoBehaviour
{
    [SerializeField] private ServerSettings settings;
    [SerializeField] private Transform trackedTransform;

    private void Start()
    {
        if (settings == null || trackedTransform == null)
        {
            Debug.LogError("PositionPublisher is missing references in the Inspector.", this);
            enabled = false;
            return;
        }

        StartCoroutine(PublishLoop());
    }

    private IEnumerator PublishLoop()
    {
        string url = settings.BuildPlayerUrl(settings.LocalPlayerId);

        while (true)
        {
            yield return PositionApi.Post(url, trackedTransform.position);
            yield return new WaitForSecondsRealtime(settings.PollIntervalSeconds);
        }
    }
}