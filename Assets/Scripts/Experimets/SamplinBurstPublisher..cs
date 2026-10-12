using System.Collections;
using UnityEngine;

public class SamplingBurstPublisher : MonoBehaviour
{
    private const float MillisecondsPerSecond = 1000f;

    [SerializeField] private ServerSettings settings;
    [SerializeField] private KeyCode triggerKey = KeyCode.P;
    [Min(1)]
    [SerializeField] private int valueCount = 20;
    [Min(1)]
    [SerializeField] private int publishIntervalMs = 20;

    private bool isPublishing;

    private void Start()
    {
        if (settings == null)
        {
            Debug.LogError("SamplingBurstPublisher is missing ServerSettings.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(triggerKey) && !isPublishing)
        {
            StartCoroutine(PublishBurst());
        }
    }

    private IEnumerator PublishBurst()
    {
        isPublishing = true;
        string url = settings.BuildPlayerUrl(settings.LocalPlayerId);
        float intervalSeconds = publishIntervalMs / MillisecondsPerSecond;

        for (int value = 1; value <= valueCount; value++)
        {
            float start = Time.realtimeSinceStartup;
            yield return PositionApi.Post(url, new Vector3(value, 0f, 0f));

            float remainingSeconds = intervalSeconds - (Time.realtimeSinceStartup - start);
            if (remainingSeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(remainingSeconds);
            }
        }

        Debug.Log($"Burst of {valueCount} values published.");
        isPublishing = false;
    }
}