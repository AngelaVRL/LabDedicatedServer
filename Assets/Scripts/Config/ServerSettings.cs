using UnityEngine;
using UnityEngine.Networking;

public class ServerSettings : MonoBehaviour
{
    private const string PlayerRoute = "server";
    private const string GamesRoute = "games";
    private const string EventsRoute = "events";

    [Header("Position service")]
    [SerializeField] private string baseUrl = "http://localhost:5005";

    [Header("Event service")]
    [SerializeField] private string eventServiceBaseUrl = "http://localhost:5006";

    [Header("Session")]
    [SerializeField] private string gameId = "game1";
    [SerializeField] private string localPlayerId = "playerA";
    [SerializeField] private string remotePlayerId = "playerB";

    [Header("Polling")]
    [Min(10)]
    [SerializeField] private int pollIntervalMs = 200;

    public string LocalPlayerId => localPlayerId;
    public string RemotePlayerId => remotePlayerId;
    public float PollIntervalSeconds => pollIntervalMs / 1000f;

    public string BuildPlayerUrl(string playerId)
    {
        string escapedPlayerId = UnityWebRequest.EscapeURL(playerId);
        return $"{baseUrl}/{PlayerRoute}/{EscapedGameId()}/{escapedPlayerId}";
    }

    public string BuildEventsUrl()
    {
        return $"{eventServiceBaseUrl}/{GamesRoute}/{EscapedGameId()}/{EventsRoute}";
    }

    public string BuildEventsUrl(long since)
    {
        return $"{BuildEventsUrl()}?since={since}";
    }

    private string EscapedGameId()
    {
        return UnityWebRequest.EscapeURL(gameId);
    }

    private void Awake()
    {
        bool hasMissingValue = string.IsNullOrWhiteSpace(baseUrl)
            || string.IsNullOrWhiteSpace(eventServiceBaseUrl)
            || string.IsNullOrWhiteSpace(gameId)
            || string.IsNullOrWhiteSpace(localPlayerId)
            || string.IsNullOrWhiteSpace(remotePlayerId);

        if (hasMissingValue)
        {
            Debug.LogError("ServerSettings has empty fields. Fill them in the Inspector.", this);
            enabled = false;
        }
    }
}