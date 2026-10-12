using System.Collections;
using UnityEngine;

public class EventBurstPublisher : MonoBehaviour
{
    private const float MillisecondsPerSecond = 1000f;

    [SerializeField] private ServerSettings settings;
    [SerializeField] private KeyCode triggerKey = KeyCode.B;
    [Min(1)]
    [SerializeField] private int eventCount = 20;
    [Min(1)]
    [SerializeField] private int publishIntervalMs = 20;

    private bool isPublishing;

    private void Start()
    {
        if (settings == null)
        {
            Debug.LogError("EventBurstPublisher is missing ServerSettings.", this);
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
        string url = settings.BuildEventsUrl();
        float intervalSeconds = publishIntervalMs / MillisecondsPerSecond;

        for (int index = 1; index <= eventCount; index++)
        {
            GameEventRequest gameEvent = new GameEventRequest
            {
                PlayerId = settings.LocalPlayerId,
                Type = GameEventTypes.LeverToggled,
                Payload = new EventPayload { IsOn = index % 2 == 1, Index = index }
            };

            float start = Time.realtimeSinceStartup;
            yield return EventApi.Post(url, gameEvent);

            float remainingSeconds = intervalSeconds - (Time.realtimeSinceStartup - start);
            if (remainingSeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(remainingSeconds);
            }
        }

        Debug.Log($"Burst of {eventCount} events published.");
        isPublishing = false;
    }
}