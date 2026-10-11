using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerSettings : MonoBehaviour
{
    private const string PlayerRoute = "server";

    [Header("Server")]
    [SerializeField] private string baseUrl = "http://localhost:5005";

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
        string escapedGameId = UnityWebRequest.EscapeURL(gameId);
        string escapedPlayerId = UnityWebRequest.EscapeURL(playerId);
        return $"{baseUrl}/{PlayerRoute}/{escapedGameId}/{escapedPlayerId}";
    }

    private void Awake()
    {
        bool hasMissingValue = string.IsNullOrWhiteSpace(baseUrl)
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