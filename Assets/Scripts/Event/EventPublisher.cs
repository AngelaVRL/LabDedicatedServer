using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPublisher : MonoBehaviour
{
    [SerializeField] private ServerSettings settings;

    private readonly Queue<GameEventRequest> pendingEvents = new Queue<GameEventRequest>();
    private bool isSending;

    private void Awake()
    {
        if (settings == null)
        {
            Debug.LogError("EventPublisher is missing ServerSettings.", this);
            enabled = false;
        }
    }

    public void Publish(string type, EventPayload payload)
    {
        if (!enabled)
        {
            return;
        }

        pendingEvents.Enqueue(new GameEventRequest
        {
            PlayerId = settings.LocalPlayerId,
            Type = type,
            Payload = payload
        });

        if (!isSending)
        {
            StartCoroutine(SendPendingEvents());
        }
    }

    private IEnumerator SendPendingEvents()
    {
        isSending = true;
        string url = settings.BuildEventsUrl();

        while (pendingEvents.Count > 0)
        {
            yield return EventApi.Post(url, pendingEvents.Dequeue());
        }

        isSending = false;
    }
}