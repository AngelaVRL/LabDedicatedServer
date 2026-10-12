from datetime import datetime, timezone
from threading import Lock


class EventStore:
    def __init__(self):
        self._events_by_game = {}
        self._lock = Lock()

    def append(self, game_id, player_id, event_type, payload):
        with self._lock:
            events = self._events_by_game.setdefault(game_id, [])
            event = {
                "seq": len(events) + 1,
                "player_id": player_id,
                "type": event_type,
                "payload": payload,
                "timestamp": datetime.now(timezone.utc).isoformat(),
            }
            events.append(event)
            return event

    def list_since(self, game_id, since):
        with self._lock:
            events = self._events_by_game.get(game_id, [])
            return events[since:], len(events)