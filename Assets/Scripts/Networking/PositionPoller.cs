using System.Collections;
using UnityEngine;

public class PositionPoller : MonoBehaviour
{
    [SerializeField] private ServerSettings settings;
    [SerializeField] private Transform remoteTransform;

    private void Start()
    {
        if (settings == null || remoteTransform == null)
        {
            Debug.LogError("PositionPoller is missing references in the Inspector.", this);
            enabled = false;
            return;
        }

        StartCoroutine(PollLoop());
    }

    private IEnumerator PollLoop()
    {
        string url = settings.BuildPlayerUrl(settings.RemotePlayerId);

        while (true)
        {
            yield return PositionApi.Get(url, position => remoteTransform.position = position.ToVector());
            yield return new WaitForSecondsRealtime(settings.PollIntervalSeconds);
        }
    }
}