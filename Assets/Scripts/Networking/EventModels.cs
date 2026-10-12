using System.Collections.Generic;
using Newtonsoft.Json;

public class EventPayload
{
    [JsonProperty("isOn")]
    public bool IsOn { get; set; }

    [JsonProperty("index")]
    public int Index { get; set; }
}

public class GameEventRequest
{
    [JsonProperty("player_id")]
    public string PlayerId { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("payload")]
    public EventPayload Payload { get; set; }
}

public class GameEvent
{
    [JsonProperty("seq")]
    public long Seq { get; set; }

    [JsonProperty("player_id")]
    public string PlayerId { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("payload")]
    public EventPayload Payload { get; set; }

    [JsonProperty("timestamp")]
    public string Timestamp { get; set; }
}

public class EventBatch
{
    [JsonProperty("events")]
    public List<GameEvent> Events { get; set; }

    [JsonProperty("last_seq")]
    public long LastSeq { get; set; }
}