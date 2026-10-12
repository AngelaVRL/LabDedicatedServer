# Event Service

Minimal REST service that stores game events in order and returns only the ones a client has not received yet.

## Run

```
docker compose up --build
```

The service is available at `http://localhost:5006`.

## Endpoints

| Method | Route | Description |
|---|---|---|
| POST | `/games/{game_id}/events` | Registers an event. Body: `{"player_id", "type", "payload"}`. Returns 201 with `seq` and `timestamp`, or 422 if data is missing. |
| GET | `/games/{game_id}/events?since={seq}` | Returns events with `seq` greater than `since`, in order, plus `last_seq`. Returns 400 if `since` is invalid. |

Events are kept in memory: restarting the service clears them.