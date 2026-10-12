REQUIRED_TEXT_FIELDS = ("player_id", "type")


def validate_event_body(body):
    if not isinstance(body, dict):
        return "Request body must be a JSON object"

    for field in REQUIRED_TEXT_FIELDS:
        value = body.get(field)
        if not isinstance(value, str) or not value.strip():
            return f"Field '{field}' is required and must be a non-empty string"

    if not isinstance(body.get("payload"), dict):
        return "Field 'payload' is required and must be a JSON object"

    return None


def parse_since(raw_value):
    if raw_value is None:
        return 0

    try:
        since = int(raw_value)
    except ValueError:
        return None

    return since if since >= 0 else None