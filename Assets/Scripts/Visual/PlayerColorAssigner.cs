using System;
using UnityEngine;

public class PlayerColorAssigner : MonoBehaviour
{
    [Serializable]
    private class PlayerColor
    {
        public string playerId;
        public Color color = Color.white;
    }

    [SerializeField] private ServerSettings settings;
    [SerializeField] private Renderer localRenderer;
    [SerializeField] private Renderer remoteRenderer;
    [SerializeField] private PlayerColor[] playerColors;
    [SerializeField] private Color fallbackColor = Color.gray;

    private void Start()
    {
         if (settings == null || localRenderer == null || remoteRenderer == null)
        {
            Debug.LogError("PlayerColorAssigner is missing references in the Inspector.", this);
            enabled = false;
            return;
        }
        localRenderer.material.color = ResolveColor(settings.LocalPlayerId);
        remoteRenderer.material.color = ResolveColor(settings.RemotePlayerId);
    }

    private Color ResolveColor(string playerId)
    {
        foreach (PlayerColor entry in playerColors)
        {
            if (entry.playerId == playerId)
            {
                return entry.color;
            }
        }

        return fallbackColor;
    }
}