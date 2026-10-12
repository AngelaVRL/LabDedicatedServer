from flask import Blueprint, jsonify, request

from app.validation import parse_since, validate_event_body


def create_events_blueprint(store):
    blueprint = Blueprint("events", __name__)

    @blueprint.post("/games/<game_id>/events")
    def register_event(game_id):
        body = request.get_json(silent=True)
        error_message = validate_event_body(body)

        if error_message:
            return jsonify({"error": error_message}), 422

        event = store.append(game_id, body["player_id"], body["type"], body["payload"])
        return jsonify({"seq": event["seq"], "timestamp": event["timestamp"]}), 201

    @blueprint.get("/games/<game_id>/events")
    def list_events(game_id):
        since = parse_since(request.args.get("since"))

        if since is None:
            return jsonify({"error": "Parameter 'since' must be a non-negative integer"}), 400

        events, last_seq = store.list_since(game_id, since)
        return jsonify({"events": events, "last_seq": last_seq})

    return blueprint