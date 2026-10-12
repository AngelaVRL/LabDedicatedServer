using UnityEngine;

public class LocalActionInput : MonoBehaviour
{
    [SerializeField] private EventPublisher publisher;
    [SerializeField] private EventReactionController reactions;
    [SerializeField] private KeyCode toggleLeverInput = KeyCode.L;
    [SerializeField] private KeyCode openDoorInput = KeyCode.O;
    [SerializeField] private KeyCode collectKeyInput = KeyCode.K;

    private bool isLeverOn;

    private void Awake()
    {
        if (publisher == null || reactions == null)
        {
            Debug.LogError("LocalActionInput is missing references in the Inspector.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleLeverInput))
        {
            isLeverOn = !isLeverOn;
            Trigger(GameEventTypes.LeverToggled, new EventPayload { IsOn = isLeverOn });
        }

        if (Input.GetKeyDown(openDoorInput))
        {
            Trigger(GameEventTypes.DoorOpened, new EventPayload());
        }

        if (Input.GetKeyDown(collectKeyInput))
        {
            Trigger(GameEventTypes.KeyCollected, new EventPayload());
        }
    }

    private void Trigger(string type, EventPayload payload)
    {
        reactions.Apply(type, payload);
        publisher.Publish(type, payload);
    }
}