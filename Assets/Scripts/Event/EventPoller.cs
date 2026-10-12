using System;
using System.Collections;
using UnityEngine;

public class EventPoller : MonoBehaviour
{
    [SerializeField] private ServerSettings settings;

    private long lastSeq;

    public event Action<GameEvent> EventReceived;

    private void Start()
    {
        if (settings == null)
        {
            Debug.LogError("EventPoller is missing ServerSettings.", this);
            enabled = false;
            return;
        }

        StartCoroutine(PollLoop());
    }

    private IEnumerator PollLoop()
    {
        while (true)
        {
            yield return EventApi.Get(settings.BuildEventsUrl(lastSeq), ProcessBatch);
            yield return new WaitForSecondsRealtime(settings.PollIntervalSeconds);
        }
    }

    private void ProcessBatch(EventBatch batch)
    {
        if (batch.Events == null)
        {
            return;
        }

        foreach (GameEvent gameEvent in batch.Events)
        {
            if (gameEvent.Seq <= lastSeq)
            {
                continue;
            }

            lastSeq = gameEvent.Seq;

            if (gameEvent.PlayerId != settings.LocalPlayerId)
            {
                EventReceived?.Invoke(gameEvent);
            }
        }
    }
}