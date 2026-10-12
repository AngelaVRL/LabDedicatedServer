using UnityEngine;

public class EventReactionController : MonoBehaviour
{
    [SerializeField] private EventPoller poller;

    [Header("Lever")]
    [SerializeField] private Renderer leverRenderer;
    [SerializeField] private Color leverOnColor = Color.green;
    [SerializeField] private Color leverOffColor = Color.gray;

    [Header("Door")]
    [SerializeField] private Transform door;
    [SerializeField] private Vector3 doorOpenOffset = new Vector3(0f, 3f, 0f);

    [Header("Key")]
    [SerializeField] private GameObject keyObject;

    private Vector3 doorClosedPosition;

    private void Awake()
    {
        bool hasMissingReference = poller == null
            || leverRenderer == null
            || door == null
            || keyObject == null;

        if (hasMissingReference)
        {
            Debug.LogError("EventReactionController is missing references in the Inspector.", this);
            enabled = false;
            return;
        }

        doorClosedPosition = door.position;
    }

    private void OnEnable()
    {
        if (poller != null)
        {
            poller.EventReceived += HandleRemoteEvent;
        }
    }

    private void OnDisable()
    {
        if (poller != null)
        {
            poller.EventReceived -= HandleRemoteEvent;
        }
    }

    public void Apply(string type, EventPayload payload)
    {
        switch (type)
        {
            case GameEventTypes.LeverToggled:
                SetLever(payload?.IsOn ?? false);
                break;
            case GameEventTypes.DoorOpened:
                OpenDoor();
                break;
            case GameEventTypes.KeyCollected:
                CollectKey();
                break;
            default:
                Debug.LogWarning($"Unknown event type: {type}");
                break;
        }
    }

    private void HandleRemoteEvent(GameEvent gameEvent)
    {
        Apply(gameEvent.Type, gameEvent.Payload);
    }

    private void SetLever(bool isOn)
    {
        leverRenderer.material.color = isOn ? leverOnColor : leverOffColor;
    }

    private void OpenDoor()
    {
        door.position = doorClosedPosition + doorOpenOffset;
    }

    private void CollectKey()
    {
        keyObject.SetActive(false);
    }
}